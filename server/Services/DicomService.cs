using FellowOakDicom;
using FellowOakDicom.Imaging;
using server.Contracts;
using SixLabors.ImageSharp;

namespace server.Services
{
    public class DicomService: IDicomService
    {
        private readonly string _uploadFolderPath = "UploadedFiles";

        public async Task<string> UploadDicomFileAsync(IFormFile? file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("No file uploaded.");

            Directory.CreateDirectory(_uploadFolderPath);

            string baseName = Path.GetFileNameWithoutExtension(file.FileName);
            string extension = Path.GetExtension(file.FileName);
            string timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
            string uniqueFileName = $"{baseName}_{timestamp}{extension}";
            string fullPath = Path.Combine(_uploadFolderPath, uniqueFileName);

            using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);

            return uniqueFileName;
        }

        public async Task<Stream> RenderDicomAsPngAsync(string fileName)
        {
            var filePath = Path.Combine(_uploadFolderPath, fileName);
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"DICOM file not found: {fileName}");

            var dicomFile = await DicomFile.OpenAsync(filePath);

            if (!dicomFile.Dataset.Contains(DicomTag.PixelData))
                throw new InvalidOperationException("DICOM file does not contain image data.");

            var image = new DicomImage(dicomFile.Dataset, frame: 0);
            if (image.NumberOfFrames == 0)
                throw new InvalidOperationException("DICOM file contains no renderable frames.");

            var rendered = image.RenderImage();
            var sharpImage = rendered.As<SixLabors.ImageSharp.Image>();

            var stream = new MemoryStream();
            await sharpImage.SaveAsPngAsync(stream);
            stream.Position = 0;
            return stream;
        }

        public async Task<string> GetHeaderValueAsync(string fileName, string tag)
        {
            var filePath = Path.Combine(_uploadFolderPath, fileName);
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"DICOM file not found: {fileName}");

            var dicomFile = await DicomFile.OpenAsync(filePath);
            DicomTag? dicomTagObject = null;

            try { dicomTagObject = DicomTag.Parse(tag); }
            catch { }

            if (dicomTagObject == null)
            {
                var entry = DicomDictionary.Default[tag];
                if (entry != null)
                    dicomTagObject = new DicomTag(entry.Group, entry.Element);
            }

            if (dicomTagObject == null)
                throw new ArgumentException($"Invalid or unknown DICOM tag: {tag}");

            if (!dicomFile.Dataset.Contains(dicomTagObject))
                throw new KeyNotFoundException($"Tag '{tag}' not found in DICOM file '{fileName}'.");

            return dicomFile.Dataset.GetString(dicomTagObject);
        }
    }
}
