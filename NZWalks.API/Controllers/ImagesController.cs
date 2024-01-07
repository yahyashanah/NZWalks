using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IImageRepository ImageRepository;
        public ImagesController(IImageRepository imageRepository)
        {
            ImageRepository = imageRepository;
        }

        

        // POST: /api/Images/Upload
        [HttpPost]
        [Route("Upload")]
        public async Task<IActionResult> Upload([FromForm] ImageUploadRequestDto request)
        {
            ValidateFileUpload(request);

            if(ModelState.IsValid)
            {
                // Convert DTO to Domain Model
                var imageDomainModel = new Image
                {
                    File = request.File,
                    FileExtention = Path.GetExtension(request.File.FileName),
                    FileSizeInBytes = request.File.Length,
                    FileName = request.FileName,
                    FileDescription = request.FileDescription,
                };


                // User repository to upload image
                await ImageRepository.Upload(imageDomainModel);
               
                return Ok(imageDomainModel);
            }

            return BadRequest(ModelState);
        }

        private void ValidateFileUpload(ImageUploadRequestDto request)
        {
            var allowExtentions = new string[] { ".jpg", ".jpeg", ".png" };

            if (!allowExtentions.Contains(Path.GetExtension(request.File.FileName)))
            {
                ModelState.AddModelError("file","Unsupported file extension");
            }

            if(request.File.Length > 10485760)
            {
                ModelState.AddModelError("file", "File size more than 10MB, please upload a smaller size file");
            }
        }
    }
}
