using System.IO;

using Microsoft.AspNetCore.Mvc;

namespace MESystem.Controllers;

public partial class UploadController : ControllerBase
{
    [HttpPost("[action]")]
    public ActionResult UploadFile(IFormFile myFile)
    {
        try
        {
            var path = GetOrCreateUploadFolder();
            using FileStream? fileStream = System.IO.File.Create(Path.Combine(path, myFile.FileName));
            myFile.CopyTo(fileStream);
        }
        catch
        {
            Response.StatusCode=400;
        }
        return new EmptyResult();
    }
}