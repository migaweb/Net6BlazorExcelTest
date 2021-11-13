using BlazorApp.GeneralUI.Components.BusyOverlay;
using BlazorApp.WASM.Shared.Contracts;
using BlazorApp.WASM.Shared.Entities;
using Microsoft.AspNetCore.Components;
using System.Diagnostics;

namespace BlazorApp.GeneralUI.Components
{
  public partial class FileContent : ComponentBase
  {
    [Inject]
    public IPersistenceService PersistenceService { get; set; }

    [Inject]
    public BusyOverlayService BusyOverlayService { get; set; }

    [Parameter]
    public IList<Article> Articles { get; set; } = new List<Article>();

    private long ProcessingTimeMs { get; set; }

    private int ItemCount { get; set; } = 0;

    private bool UiUpdates { get; set; } = true;

    protected override async Task OnInitializedAsync()
    {
      await PersistenceService.InitAsync();
      await base.OnInitializedAsync();
    }

    private async Task AddTenToPrice()
    {
      ProcessingTimeMs = 0;
      BusyOverlayService.SetBusyState(BusyEnum.Busy);
      var stopwatch = new Stopwatch();
      stopwatch.Start();

      // Load all articles from the DB
      Articles = (await PersistenceService.GetAllAsync<Article>()).ToList();
      // Display in table
      ProcessingTimeMs = stopwatch.ElapsedMilliseconds;
      await Task.Yield();
      StateHasChanged();
      // Add price
      int counter = 0;
      foreach (var article in Articles)
      {
        article.Price += 10;

        if (!UiUpdates) continue;

        ItemCount = ++counter;
        ProcessingTimeMs = stopwatch.ElapsedMilliseconds;

        if ((counter % 100) == 0 || counter == Articles.Count)
        {
          StateHasChanged();
          await Task.Delay(1);
        }
      }

      var counter2 = 0;
      // Update all articles in DB
      foreach (var article in Articles)
      {
        await PersistenceService.UpdateAsync<Article>(article);

        if (!UiUpdates) continue;

        ItemCount = ++counter2;
        ProcessingTimeMs = stopwatch.ElapsedMilliseconds;

        if ((counter2 % 100) == 0 || counter2 == Articles.Count)
        {
          StateHasChanged();
          await Task.Delay(1);
        }
      }

      stopwatch.Stop();
      ProcessingTimeMs = stopwatch.ElapsedMilliseconds;
      BusyOverlayService.SetBusyState(BusyEnum.NotBusy);
    }

    private void HandleClearTable()
    {
      BusyOverlayService.SetBusyState(BusyEnum.Busy);
      var stopwatch = new Stopwatch();
      stopwatch.Start();

      Articles.Clear();

      ItemCount = 0;

      stopwatch.Stop();
      ProcessingTimeMs = stopwatch.ElapsedMilliseconds;
      BusyOverlayService.SetBusyState(BusyEnum.NotBusy);
    }

    private async Task HandleLoadArticlesFromIndexedDb()
    {
      BusyOverlayService.SetBusyState(BusyEnum.Busy);
      var stopwatch = new Stopwatch();
      stopwatch.Start();

      var result = await PersistenceService.GetAllAsync<Article>();

      Articles = result.ToList();

      ItemCount = Articles.Count();

      stopwatch.Stop();
      ProcessingTimeMs = stopwatch.ElapsedMilliseconds;
      BusyOverlayService.SetBusyState(BusyEnum.NotBusy);
    }

    private async Task HandleAddToIndexedDb()
    {
      BusyOverlayService.SetBusyState(BusyEnum.Busy);
      var stopwatch = new Stopwatch();
      stopwatch.Start();

      foreach (var article in Articles)
      {
        await PersistenceService.DeleteAsync<Article>(article);
        if (!UiUpdates) continue;

        ItemCount = Math.Max(0, ItemCount--);
        ProcessingTimeMs = stopwatch.ElapsedMilliseconds;

        if ((ItemCount % 100) == 0 || ItemCount  == Articles.Count)
        {
          StateHasChanged();
          await Task.Delay(1);
        }
        
      }

      ItemCount = 0;
      foreach (var article in Articles)
      {
        await PersistenceService.InsertAsync<Article>(article);
        if (!UiUpdates) continue;

        ItemCount++;
        ProcessingTimeMs = stopwatch.ElapsedMilliseconds;

        if ((ItemCount % 100) == 0 || ItemCount == Articles.Count)
        {
          StateHasChanged();
          await Task.Delay(1);
        }
        
      }

      stopwatch.Stop();
      ProcessingTimeMs = stopwatch.ElapsedMilliseconds;
      BusyOverlayService.SetBusyState(BusyEnum.NotBusy);
    }

    private async Task HandleDeleteFromIndexedDb()
    {
      BusyOverlayService.SetBusyState(BusyEnum.Busy);
      var stopwatch = new Stopwatch();
      stopwatch.Start();

      ItemCount = Articles.Count;

      foreach (var article in Articles)
      {
        await PersistenceService.DeleteAsync<Article>(article);
        if (!UiUpdates) continue;

        ItemCount = Math.Max(0, ItemCount--);
        ProcessingTimeMs = stopwatch.ElapsedMilliseconds;

        if ((ItemCount % 100) == 0 || ItemCount == Articles.Count)
        {
          StateHasChanged();
          await Task.Delay(1);
        }
      }

      stopwatch.Stop();
      ProcessingTimeMs = stopwatch.ElapsedMilliseconds;
      BusyOverlayService.SetBusyState(BusyEnum.NotBusy);
    }
  }
}
