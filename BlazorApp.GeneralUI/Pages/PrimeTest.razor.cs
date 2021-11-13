
using BlazorApp.GeneralUI.Components.BusyOverlay;
using BlazorApp.Services.Services;
using Microsoft.AspNetCore.Components;
using System.Diagnostics;

namespace BlazorApp.GeneralUI.Pages
{
  public partial class PrimeTest : ComponentBase
  {
    [Inject] public PrimeService? PrimeService { get; set; }
    [Inject] public BusyOverlayService? BusyOverlayService { get; set; }

    public List<int>? Primes { get; set; }
    public long TimeElapsedMs { get; set; }
    public int MaxNumber { get; set; } = 1000000;

    private async Task CalculatePrimes()
    {
      BusyOverlayService?.SetBusyState(BusyEnum.Busy);
      await Task.Delay(1);
      var stopwatch = new Stopwatch();
     
      stopwatch.Start();
      Primes = PrimeService?.FindPrimeNumbersUpTo(MaxNumber);
      stopwatch.Stop();

      TimeElapsedMs = stopwatch.ElapsedMilliseconds;

      BusyOverlayService?.SetBusyState(BusyEnum.NotBusy);
    }
  }
}
