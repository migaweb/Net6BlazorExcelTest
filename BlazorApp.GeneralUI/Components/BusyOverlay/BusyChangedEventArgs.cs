namespace BlazorApp.GeneralUI.Components.BusyOverlay
{
  public class BusyChangedEventArgs : EventArgs
  {
    public BusyEnum BusyState { get; set; }
  }
}
