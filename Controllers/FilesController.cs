// using Microsoft.AspNetCore.Http;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Mvc;
// using System.Threading.Tasks;
using WebApiS3Integration.Services;

namespace WebApiS3Integration.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly S3FileService _s3Service;

        public FilesController(S3FileService s3Service)
        {
            _s3Service = s3Service;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            await _s3Service.UploadFileAsync("hlm-demo-s3", file.FileName, file.OpenReadStream());
            return Ok("File uploaded successfully.");
        }

        [HttpGet("download/{fileName}")]
        public async Task<IActionResult> DownloadFile(string fileName)
        {
            var stream = await _s3Service.DownloadFileAsync("hlm-demo-s3", fileName);

            if (stream == null)
                return NotFound();

            return File(stream, "application/octet-stream", fileName);
        }

        [HttpGet("download-url/{fileName}")]
        public IActionResult GetDownloadUrl(string fileName)
        {
            // Generate a signed URL valid for 60 minutes
            var url = _s3Service.GetPreSignedURL("hlm-demo-s3", fileName, 60);
            return Ok(url);
        }

        [HttpDelete("delete/{fileName}")]
        public async Task<IActionResult> DeleteFile(string fileName)
        {
            await _s3Service.DeleteFileAsync("hlm-demo-s3", fileName);
            return Ok("File deleted successfully.");
        }
    }

}
