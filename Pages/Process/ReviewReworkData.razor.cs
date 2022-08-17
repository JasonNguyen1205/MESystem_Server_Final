
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
using static Microsoft.AspNetCore.Razor.Language.TagHelperMetadata;
using System.IO;

namespace MESystem.Pages.Process;

public partial class ReviewReworkData : ComponentBase
{
    public ColumnFilterPopupMode ColumnFilterPopupMode { get; set; }
    [Inject]
    private TraceService? TraceDataService { get; set; }

    [Inject]
    private UploadFileService? UploadFileService { get; set; }

    [Inject]
    IJSRuntime? jSRuntime { get; set; }

    public IGrid? Grid { get; set; }
    public IEnumerable<Rework> masterData { get; set; } = new List<Rework>();
    public string? FocusElement { get; set; }
    public string? ReadOnlyElement { get; set; }

    protected override async Task OnInitializedAsync()
    {
        
        masterData = await TraceDataService.GetAllDataRework();
        
        Console.WriteLine("dff");
        
    }

    public async void ExportExcel()
    {
        List<Rework> results = new();
        int total = Grid.GetVisibleRowCount();
        for(int i = 0; i< total; i++)
        {
           Rework temp = (Rework)Grid.GetDataItem(i);
            if (temp != null)
            {
                results.Add(temp);
            }
        }
       
        var fileContent = await UploadFileService.ExportExcelRework(results);
        await jSRuntime.InvokeVoidAsync("saveAsFile", $"Rework_{DateTime.Now}.xlsx", Convert.ToBase64String(fileContent));
    }
}
