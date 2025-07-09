using FellowOakDicom;
using FellowOakDicom.Imaging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;

namespace server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DicomController : ControllerBase
    {
        private string _uploadFolderPath = "UploadedFiles";

        [HttpPost("upload")]
        public async Task<IActionResult> UploadDicom(IFormFile file)
        {
            if (file is null || file.Length == 0) return BadRequest("No file uploaded");

            string originalFileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.FileName);
            string originalFileExtension = Path.GetExtension(file.FileName);
            string timeStamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
            string uniqueFileName = $"{originalFileNameWithoutExtension}_{timeStamp}{originalFileExtension}";

            var filePath = Path.Combine(_uploadFolderPath, uniqueFileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return Ok(new { filename = uniqueFileName });
        }

        [HttpGet("header")]
        public IActionResult GetHeaderValue(string fileName, string tag)
        {
            if (string.IsNullOrWhiteSpace(fileName) || string.IsNullOrWhiteSpace(tag))
            {
                return BadRequest("Filename and tag parameters are required.");
            }

            var filePath = Path.Combine(_uploadFolderPath, fileName);
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound($"DICOM file '{fileName}' not found.");
            }

            try
            {
                var dicomFile = DicomFile.Open(filePath);

                // --- MODIFIED LOGIC HERE ---
                DicomTag dicomTagObject = null;

                // 1. Try parsing as a group and element (e.g., "0010,0010")
                try
                {
                    dicomTagObject = DicomTag.Parse(tag);
                }
                catch (DicomDataException) // Catches errors during parsing like invalid format
                {
                    // Ignore and try the next method
                }

                // 2. If not parsed or not found, try looking up by keyword (e.g., "PatientName")
                if (dicomTagObject == null)
                {
                    var knownTagEntry = DicomDictionary.Default[tag];
                    if (knownTagEntry != null)
                    {
                        dicomTagObject = new DicomTag(knownTagEntry.Group, knownTagEntry.Element);
                    }
                }

                if (dicomTagObject == null)
                {
                    return BadRequest($"Invalid or unknown DICOM tag: {tag}. Please use format like '0010,0010' or a known tag name like 'PatientName'.");
                }
                // --- END MODIFIED LOGIC ---


                if (dicomFile.Dataset.Contains(dicomTagObject))
                {
                    var value = dicomFile.Dataset.GetString(dicomTagObject);
                    return Ok(new { tag = dicomTagObject.ToString(), value });
                }
                else
                {
                    return NotFound($"Tag '{tag}' not found in DICOM file '{fileName}'.");
                }
            }
            catch (DicomFileException dex)
            {
                Console.WriteLine($"Error opening or parsing DICOM file '{fileName}': {dex.Message}");
                return StatusCode(500, $"Error processing DICOM file: {dex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("render")]
        public async Task<IActionResult> RenderToPng(string filename)
        {
            var filePath = Path.Combine(_uploadFolderPath, filename);
            if (!System.IO.File.Exists(filePath))
                return NotFound("DICOM file not found.");

            try
            {
                var dicomFile = await DicomFile.OpenAsync(filePath);

                if (!dicomFile.Dataset.Contains(DicomTag.PixelData))
                    return BadRequest("DICOM file does not contain image data.");

                var image = new DicomImage(dicomFile.Dataset, frame: 0);

                if (image.NumberOfFrames == 0)
                    return BadRequest("DICOM file contains no renderable frames.");

                var rendered = image.RenderImage();
                var sharpImage = rendered.As<SixLabors.ImageSharp.Image>();

                var stream = new MemoryStream();

                await sharpImage.SaveAsPngAsync(stream);
                stream.Position = 0;

                return File(stream, "image/png");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[RenderToPng ERROR] {ex}");
                return StatusCode(500, $"An error occurred while processing the DICOM file: {ex.Message}");
            }
        }
    }
}
