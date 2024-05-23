using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;
using docker_dotnet_three_js.DataAccess.Contracts;
using docker_dotnet_three_js.DataAccess.Implementations.Entities;

namespace docker_dotnet_three_js.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileElementsController : ControllerBase
    {
        private readonly IFileElementRepository _repository;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<FileElementsController> _logger;

        public FileElementsController(IFileElementRepository repository, IWebHostEnvironment environment, ILogger<FileElementsController> logger)
        {
            _repository = repository;
            _environment = environment;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetFiles()
        {
            var files = _repository.GetAll();
            return Ok(files);
        }

        [HttpGet("{id}")]
        public IActionResult GetFile(int id)
        {
            var file = _repository.GetById(id);
            if (file == null)
            {
                return NotFound();
            }
            return Ok(file);
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile([FromForm] IFormFile file, [FromForm] string description)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var extension = Path.GetExtension(file.FileName).ToLower();
            if (extension != ".gltf" && extension != ".obj" && extension != ".glb")
            {
                return BadRequest("Only .gltf, .obj and .glb files are allowed.");
            }

            var request = HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";

            var uploads = Path.Combine(_environment.ContentRootPath, "uploads");
            _logger.LogInformation($"Uploads path: {uploads}");

            if (!Directory.Exists(uploads))
            {
                _logger.LogInformation("Directory does not exist, creating...");
                Directory.CreateDirectory(uploads);
            }
            else
            {
                _logger.LogInformation("Directory already exists.");
            }

            var fileName = Path.GetFileName(file.FileName);
            var filePath = Path.Combine(uploads, fileName);
            _logger.LogInformation($"File path: {filePath}");

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
                _logger.LogInformation("File copied to stream successfully.");
            }

            var fileUrl = $"{baseUrl}/uploads/{fileName}";
            var fileElement = new FileElement(fileUrl, description);
            _repository.Add(fileElement);
            _repository.SaveChanges();

            return CreatedAtAction(nameof(GetFile), new { id = fileElement.Id }, fileElement);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFile(int id, [FromForm] IFormFile file, [FromForm] string description)
        {
            var fileElement = _repository.GetById(id);
            if (fileElement == null)
            {
                return NotFound();
            }

            if (file != null && file.Length > 0)
            {
                var extension = Path.GetExtension(file.FileName).ToLower();
                if (extension != ".gltf" && extension != ".obj" && extension != ".glb")
                {
                    return BadRequest("Only .gltf, .obj, and .glb files are allowed.");
                }

                var existingFilePath = Path.Combine(_environment.ContentRootPath, fileElement.file_path.TrimStart('/'));
                if (System.IO.File.Exists(existingFilePath))
                {
                    System.IO.File.Delete(existingFilePath);
                }

                var uploads = Path.Combine(_environment.ContentRootPath, "uploads");
                if (!Directory.Exists(uploads))
                {
                    Directory.CreateDirectory(uploads);
                }

                var fileName = Path.GetFileName(file.FileName);
                var filePath = Path.Combine(uploads, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var request = HttpContext.Request;
                var baseUrl = $"{request.Scheme}://{request.Host}";

                fileElement.file_path = $"{baseUrl}/uploads/{fileName}";
            }

            if (!string.IsNullOrEmpty(description))
            {
                fileElement.description = description;
            }

            _repository.Update(fileElement);
            _repository.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteFile(int id)
        {
            var fileElement = _repository.GetById(id);
            if (fileElement == null)
            {
                return NotFound();
            }

            _repository.Remove(fileElement);
            _repository.SaveChanges();

            var existingFilePath = Path.Combine(_environment.ContentRootPath, fileElement.file_path.TrimStart('/'));
            if (System.IO.File.Exists(existingFilePath))
            {
                System.IO.File.Delete(existingFilePath);
            }

            return NoContent();
        }

        [HttpGet("download/{id}")]
        public IActionResult DownloadFile(int id)
        {
            var fileElement = _repository.GetById(id);
            if (fileElement == null)
            {
                return NotFound();
            }

            var filePath = Path.Combine(_environment.ContentRootPath, "uploads", Path.GetFileName(fileElement.file_path));
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var contentType = "application/octet-stream";
            return PhysicalFile(filePath, contentType);
        }
    }
}
