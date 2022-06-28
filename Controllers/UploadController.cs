using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;

namespace MESystem.Controllers;


[Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
[ApiController]
public partial class UploadController : ControllerBase
{
    protected string ContentRootPath { get; set; }
    public UploadController(IWebHostEnvironment hostingEnvironment)
    {
        ContentRootPath = hostingEnvironment.ContentRootPath;
    }
    public string GetOrCreateUploadFolder()
    {
        var path = Path.Combine(ContentRootPath, "wwwroot", "uploads");
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        return path;
    }
}
