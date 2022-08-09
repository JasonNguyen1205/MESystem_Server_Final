
using MESystem.Service;
using MESystem.Data.TRACE;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using DevExpress.Blazor;
using Microsoft.JSInterop;
using Blazored.Toast.Services;
using System.Text.RegularExpressions;
using DevExpress.XtraGrid.Columns;
using System.ComponentModel;

namespace MESystem.Pages.Process;

public partial class ReviewReworkData : ComponentBase
{
    public ColumnFilterPopupMode ColumnFilterPopupMode { get; set; }
    [Inject]
    private TraceService? TraceDataService { get; set; }
    public IGrid? Grid { get; set; }
    public IEnumerable<Rework> masterData { get; set; } = new List<Rework>();
    public string? FocusElement { get; set; }
    public string? ReadOnlyElement { get; set; }

    protected override async Task OnInitializedAsync()
    {
        
        masterData = await TraceDataService.GetAllDataRework();
        
        Console.WriteLine("dff");
        
    }
}
