using FellowOakDicom;
using FellowOakDicom.Imaging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using server.Contracts;
using SixLabors.ImageSharp;

namespace server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DicomController(IDicomService _dicomService) : ControllerBase
    {
        [HttpPost("upload")]
        public async Task<IActionResult> UploadDicom(IFormFile file)
        {
            try
            {
                var filename = await _dicomService.UploadDicomFileAsync(file);
                return Ok(new { filename });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("header")]
        public async Task<IActionResult> GetHeaderValue(string fileName, string tag)
        {
            try
            {
                var value = await _dicomService.GetHeaderValueAsync(fileName, tag);
                return Ok(new { tag, value });
            }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
            catch (FileNotFoundException ex) { return NotFound(ex.Message); }
            catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [HttpGet("render")]
        public async Task<IActionResult> RenderToPng(string filename)
        {
            try
            {
                var pngStream = await _dicomService.RenderDicomAsPngAsync(filename);
                return File(pngStream, "image/png");
            }
            catch (FileNotFoundException ex) { return NotFound(ex.Message); }
            catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
