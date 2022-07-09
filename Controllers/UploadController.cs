using System.IO;

using Microsoft.AspNetCore.Mvc;

namespace MESystem.Controllers;


[Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
[ApiController]
public partial class UploadController : ControllerBase
{
    protected string ContentRootPath { get; set; }
    public UploadController(IWebHostEnvironment hostingEnvironment)
    {
        ContentRootPath=hostingEnvironment.ContentRootPath;
    }
    public string GetOrCreateUploadFolder()
    {
        var path = Path.Combine(ContentRootPath, "wwwroot", "uploads");
        if(!Directory.Exists(path))
        {
            _=Directory.CreateDirectory(path);
        }

        return path;
    }
}
