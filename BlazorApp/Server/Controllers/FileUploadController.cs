using BlazorApp.WASM.Shared;
using BlazorApp.WASM.Shared.Contracts;
using BlazorApp.WASM.Shared.Entities;
using Microsoft.AspNetCore.Mvc;

namespace BlazorApp.WASM.Server.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class FileUploadController : ControllerBase
  {
    private readonly IExcelReader<Article> _excelReader;

    public FileUploadController(IExcelReader<Article> excelReader)
    {
      _excelReader = excelReader;
    }

    [HttpPost]
    public async Task<IActionResult> Post(UploadedFile uploadedFile)
    {
      var fileBytes = uploadedFile.FileContent;
      IList<Article> result = new List<Article>();

      using (var reader = new MemoryStream(fileBytes))
      {
        result = await _excelReader.Read(reader);
      }

      return Ok(result);
    }
  }
}
