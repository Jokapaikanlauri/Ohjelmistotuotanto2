using MatkakertomusGroupB.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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

        [HttpPost]
        public async Task<ActionResult<List<UploadResult>>> UploadFile(List<IFormFile> files)
        {
            List<UploadResult> uploadResults = new List<UploadResult>();
            foreach (var file in files)
            {
                // This part is just a bunch of jargon for filename validation and setting
                var uploadResult = new UploadResult();
                string trustedFileNameForFileStorage;
                var untrustedFileName = file.Name;
                uploadResult.FileName = untrustedFileName;
                var trustedFileNameForDisplay = WebUtility.HtmlEncode(untrustedFileName);
                trustedFileNameForFileStorage = Path.GetRandomFileName();
                
                // Here we set the path in where we will upload pictures and in this case they will be
                // uploaded to the solution server root path in a folder named Uploaded
                var path = Path.Combine(_env.ContentRootPath, "Uploaded", trustedFileNameForFileStorage);

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
