using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace BlazorApp.GeneralUI.Components
{
  public partial class FileInfo : ComponentBase
  {
    [Parameter]
    public IBrowserFile File { get; set; }
    
    [Parameter]
    public int ArticlesCount { get; set; }
    
    [Parameter]
    public long ProcessingTimeMs { get; set; }
  }
}
