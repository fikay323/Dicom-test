using FellowOakDicom;
using FellowOakDicom.Imaging;
using server.Contracts;
using SixLabors.ImageSharp;

namespace server.Services
{
    public class DicomService: IDicomService
    {
        private readonly IBlobStorageService _blob;

        public DicomService(IBlobStorageService blobStorageService)
        {
            _blob = blobStorageService;
        }

        public async Task<string> UploadDicomFileAsync(IFormFile? file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("No file uploaded.");

            var fileName = $"{Path.GetFileNameWithoutExtension(file.FileName)}_{DateTime.UtcNow:yyyyMMdd_HHmmss}{Path.GetExtension(file.FileName)}";
            using var stream = file.OpenReadStream();
            await _blob.UploadAsync(stream, fileName, file.ContentType);
            return fileName;
        }

        public async Task<Stream> RenderDicomAsPngAsync(string fileName)
        {
            using var stream = await _blob.DownloadAsync(fileName);
            var dicomFile = await DicomFile.OpenAsync(stream);

            if (!dicomFile.Dataset.Contains(DicomTag.PixelData))
                throw new InvalidOperationException("DICOM file does not contain image data.");

            var image = new DicomImage(dicomFile.Dataset, frame: 0);
            var rendered = image.RenderImage();
            var sharpImage = rendered.As<SixLabors.ImageSharp.Image>();

            var pngStream = new MemoryStream();
            await sharpImage.SaveAsPngAsync(pngStream);
            pngStream.Position = 0;

            return pngStream;
        }

        public async Task<string> GetHeaderValueAsync(string fileName, string tag)
        {
            using var stream = await _blob.DownloadAsync(fileName);
            var dicomFile = await DicomFile.OpenAsync(stream);

            DicomTag? dicomTagObject = null;

            try { dicomTagObject = DicomTag.Parse(tag); }
            catch {
                throw new ArgumentException($"'{tag}' is not a valid DICOM tag or known keyword.");
            }

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
