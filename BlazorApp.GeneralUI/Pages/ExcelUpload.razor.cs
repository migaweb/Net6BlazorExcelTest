using BlazorApp.GeneralUI.Components.BusyOverlay;
using BlazorApp.WASM.Shared;
using BlazorApp.WASM.Shared.Contracts;
using BlazorApp.WASM.Shared.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Diagnostics;
using System.Net.Http.Json;

namespace BlazorApp.GeneralUI.Pages
{
  public partial class ExcelUpload : ComponentBase
  {
    [Inject]
    public IExcelReader<Article>? ArticleExcelReader { get; set; }

    [Inject]
    public BusyOverlayService? BusyOverlayService { get; set; }

    [Inject]
    public HttpClient? HttpClient { get; set; }

    public IList<Article> Articles { get; set; } = new List<Article>();

    public long TimeElapsedMs { get; set; }

    public IBrowserFile? File { get; set; }

    public bool SendToServer { get; set; }

    public string? ErrorMessage { get; set; }

    private bool Ascending { get; set; }

    private void OrderbyDescription()
    {
      BusyOverlayService?.SetBusyState(BusyEnum.Busy);

      Articles = Ascending ? 
         Articles.OrderBy(e => e.Description).ToList() :
         Articles.OrderByDescending(e => e.Description).ToList();

      Ascending = !Ascending;

      BusyOverlayService?.SetBusyState(BusyEnum.NotBusy);
    }

    private async Task ReadFileAsync()
    {
      BusyOverlayService?.SetBusyState(BusyEnum.Busy);
      if (File != null)
      {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        if (SendToServer)
        {
          await ParseOnServer();
        }
        else
        {
          using (MemoryStream ms = new MemoryStream())
          {
            await File.OpenReadStream().CopyToAsync(ms);
            Articles = await ArticleExcelReader.Read(ms);
          }
          //using (var reader = new System.IO.StreamReader(File.OpenReadStream()))
          //{
            
          //}
        }

        stopwatch.Stop();
        TimeElapsedMs = stopwatch.ElapsedMilliseconds;
        StateHasChanged();
      }
      BusyOverlayService?.SetBusyState(BusyEnum.NotBusy);
    }

    private async Task ParseOnServer()
    {
      try
      {
        Stream stream = File.OpenReadStream();
        MemoryStream ms = new MemoryStream();
        await stream.CopyToAsync(ms);
        stream.Close();

        UploadedFile uploadedFile = new UploadedFile();
        uploadedFile.FileName = File.Name;
        uploadedFile.FileContent = ms.ToArray();
        ms.Close();


        var response = await HttpClient.PostAsJsonAsync<UploadedFile>("/api/fileupload", uploadedFile);

        if (!response.IsSuccessStatusCode)
        {
          ErrorMessage = await response.Content.ReadAsStringAsync();
        }
        else
        {
          Articles = await response.Content.ReadFromJsonAsync<List<Article>>();
        }
      }
      catch (Exception ex)
      {
        ErrorMessage = ex.Message;
      }
    }

    private void OnInputFileChange(InputFileChangeEventArgs e)
    {
      BusyOverlayService.SetBusyState(BusyEnum.Busy);
      File = e.File;
      this.StateHasChanged();
      BusyOverlayService.SetBusyState(BusyEnum.NotBusy);
    }
  }
}
