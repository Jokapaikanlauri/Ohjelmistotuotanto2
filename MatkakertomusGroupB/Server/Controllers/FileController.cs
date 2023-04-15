using MatkakertomusGroupB.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.IO;
using Microsoft.EntityFrameworkCore;
using System.Drawing;

namespace MatkakertomusGroupB.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        public FileController(IWebHostEnvironment env) 
        {
            _env = env;
        }

        [HttpGet("story/{id}")]
        public async Task<ActionResult<IEnumerable<Picture>>> GetStoryPicture(int id)
        {
            // using the path where story pictures for id are uploaded
            var path = Path.Combine(_env.ContentRootPath + "Uploaded/story/" + id);
            List<String>? pictures = null;

            foreach (string file in Directory.GetFiles(path))
            {
                using (Image image = Image.FromFile(file))
                {
                    using (MemoryStream m = new MemoryStream())
                    {
                        image.Save(m, image.RawFormat);
                        byte[] imageBytes = m.ToArray();

                        // Convert byte[] to Base64 String
                        string base64String = Convert.ToBase64String(imageBytes);
                        pictures.Add(base64String);
                    }
                }
            }

            if (pictures == null) return NotFound();

            return Ok(pictures);
        }


        // Post an image to a story
        [HttpPost("story/{id}")]
        public async Task<ActionResult<List<UploadResult>>> UploadStoryPicture(int id, List<IFormFile> files)
        {
            // This part is for filename validation and setting
            
            List<UploadResult> uploadResults = new List<UploadResult>();
            foreach (var file in files)
            {
                var uploadResult = new UploadResult();
                // We set a random string for storing the file locally
                string trustedFileNameForFileStorage = Path.GetRandomFileName(); 
                // We set the original filename for filename display
                var untrustedFileName = file.FileName;
                uploadResult.FileName = untrustedFileName;
                var trustedFileNameForDisplay = WebUtility.HtmlEncode(untrustedFileName);
                
                // Here we set the path in where we will upload pictures and in this case they will be
                // uploaded to the solution server root path in a folder named Uploaded
                var path = Path.Combine(_env.ContentRootPath, "Uploaded/story/" + id, trustedFileNameForFileStorage);
                
                // Create a path for specific id story
                Directory.CreateDirectory(_env.ContentRootPath + "Uploaded/story/" + id);

                // These lines will create the files and upload them to the previously set path
                await using FileStream fs = new(path, FileMode.Create);
                await file.CopyToAsync(fs);

                // This will add the storedfilename to uploadresults for each file, for testing purposes
                uploadResult.StoredFileName = trustedFileNameForFileStorage;
                uploadResults.Add(uploadResult);
            }

            return Ok(uploadResults);
        }

        // Post an image to a traveller
        [HttpPost("traveller/{id}")]
        public async Task<ActionResult<List<UploadResult>>> UploadTravellerPicture(string id, List<IFormFile> files)
        {
            // This part is for filename validation and setting

            List<UploadResult> uploadResults = new List<UploadResult>();
            foreach (var file in files)
            {
                var uploadResult = new UploadResult();
                // We set a random string for storing the file locally
                string trustedFileNameForFileStorage = Path.GetRandomFileName();
                // We set the original filename for filename display
                var untrustedFileName = file.FileName;
                uploadResult.FileName = untrustedFileName;
                var trustedFileNameForDisplay = WebUtility.HtmlEncode(untrustedFileName);

                // Here we set the path in where we will upload pictures and in this case they will be
                // uploaded to the solution server root path in a folder named Uploaded
                var path = Path.Combine(_env.ContentRootPath, "Uploaded/traveller/" + id, trustedFileNameForFileStorage);
                
                // Create a path for specific traveller
                Directory.CreateDirectory(_env.ContentRootPath + "Uploaded/traveller/" + id);

                // These lines will create the files and upload them to the previously set path
                await using FileStream fs = new(path, FileMode.Create);
                await file.CopyToAsync(fs);

                // This will add the storedfilename to uploadresults for each file, for testing purposes
                uploadResult.StoredFileName = trustedFileNameForFileStorage;
                uploadResults.Add(uploadResult);
            }

            return Ok(uploadResults);
        }

        // Post an image to a destination
        [HttpPost("destination/{id}")]
        public async Task<ActionResult<List<UploadResult>>> UploadDestinationPicture(int id, List<IFormFile> files)
        {
            // This part is for filename validation and setting

            List<UploadResult> uploadResults = new List<UploadResult>();
            foreach (var file in files)
            {
                var uploadResult = new UploadResult();
                // We set a random string for storing the file locally
                string trustedFileNameForFileStorage = Path.GetRandomFileName();
                // We set the original filename for filename display
                var untrustedFileName = file.FileName;
                uploadResult.FileName = untrustedFileName;
                var trustedFileNameForDisplay = WebUtility.HtmlEncode(untrustedFileName);

                // Here we set the path in where we will upload pictures and in this case they will be
                // uploaded to the solution server root path in a folder named Uploaded
                var path = Path.Combine(_env.ContentRootPath, "Uploaded/destination/" + id, trustedFileNameForFileStorage);
                
                // Create a specific path for destination id
                Directory.CreateDirectory(_env.ContentRootPath + "Uploaded/destination/" + id);

                // These lines will create the files and upload them to the previously set path
                await using FileStream fs = new(path, FileMode.Create);
                await file.CopyToAsync(fs);

                // This will add the storedfilename to uploadresults for each file, for testing purposes
                uploadResult.StoredFileName = trustedFileNameForFileStorage;
                uploadResults.Add(uploadResult);
            }

            return Ok(uploadResults);
        }
    }
}
