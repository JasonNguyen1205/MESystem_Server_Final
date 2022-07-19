using Blazored.Toast.Services;

using MESystem.Data.TRACE;
using MESystem.Service;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace MESystem.Pages.Warehouse;

public partial class CheckShipmentId : ComponentBase
{
    [Inject]
    IJSRuntime? jSRuntime { get; set; }

    [Inject]
    TraceService? TraceDataService { get; set; }

    [Inject]
    IToastService? Toast { get; set; }

    public List<string>? Infofield { get; set; } = new();
    public List<string>? InfoCssColor { get; set; } = new();
    public List<string>? Result { get; set; } = new();
    public List<string>? HighlightMsg { get; set; } = new();

    string? Scanfield { get; set; }

    public string? Title { get; set; }
    public bool Sound { get; set; } = true;

    //Scan for making palette only
    public int QtyPerBox;
    public IEnumerable<Shipment> Shipments { get; set; }
    public List<string> ShipmentIdList { get; set; } = new();
    public string ShipmentId { get; set; } = "";
    public bool IsReadOnly { get; set; } = true; 
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if(firstRender)
        {
            Shipments = await TraceDataService.GetLogisticData(shipmentId: "ALL") ?? new List<Shipment>();
            foreach (Shipment s in Shipments.Where(s => s.ShipmentId != null && s.RawData >= 0).ToList())
            {
                if (!ShipmentIdList.Contains(s.ShipmentId))
                {
                    ShipmentIdList.Add(s.ShipmentId);
                }
            }
            //await jSRuntime.InvokeVoidAsync("focusEditorByID", "ComboBoxShipmentId");
        }
    }

    async void UpdateInfoField(string cssTextColor, string? result = null, string? content = null, string? highlightMsg = null, bool reset = false)
    {
        if(reset)
        {
            InfoCssColor=new();
            Result=new();
            Infofield=new();
            HighlightMsg=new();
        }

        if(result=="ERROR")
        {
            //await ResetInfo(false);
            if(Sound)
            {
                await jSRuntime.InvokeVoidAsync("playSound", "/sounds/alert.wav");
            }

        }

        if(string.IsNullOrEmpty(cssTextColor)||string.IsNullOrEmpty(content))
        {
            return;
        }

        InfoCssColor.Add(cssTextColor);

        if(result!=null)
        {
            Result.Add(result);
        }
        else
        {
            Result.Add("INFO");
        }

        Infofield.Add(content);

        if(highlightMsg!=null)
        {
            HighlightMsg.Add(highlightMsg);
        }
        else
        {
            HighlightMsg.Add(string.Empty);
        }

        await UpdateUI();
    }

    async Task ResetInfo(bool backToStart)
    {
        if(backToStart)
        {
            Scanfield = "";
            await jSRuntime.InvokeVoidAsync("focusEditorByID", "Barcode");
        }else
        {
            await Task.Run(() =>
            {
            
                Infofield = new();
                InfoCssColor = new();
                Result = new();
                HighlightMsg = new();
                IsReadOnly = false;
            });
        }
           await UpdateUI();

    }

    async Task UpdateUI()
    {
        //Update UI
        if(ShouldRender())
        {
            await Task.Delay(5);
            await InvokeAsync(StateHasChanged);
        }
#if DEBUG
        Console.WriteLine("UI is updated");
#endif
    }



    private async void GetInputfield(string content) {
       
        Scanfield = content;
        
    }

    public FinishedGood? Box1 { get; set; }

    private async void HandleBarcode(KeyboardEventArgs e)
    {
        if(e.Key=="Enter")
        {
            // Clear Info field:
            await ResetInfo(false);
            UpdateInfoField("green", "INFO", "Selected ShipmentId: " + ShipmentId, null, false);
            UpdateInfoField("green", "INFO", "Scanned Barcode: " + Scanfield, null, false);

            string result;
            switch (result = await TraceDataService.GetShipmentIdByBarcode(Scanfield))
            {
                case "NO_STOCK":
                    UpdateInfoField("red", "ERROR", "Barcode dont have shipment.", null, false);
                    break;
                case "Problem Access To Server":
                    UpdateInfoField("red", "ERROR", "Server Connection Error", null, false);
                    break;
                default:
                    if (result.Equals(ShipmentId))
                    {
                        UpdateInfoField("green", "SUCCESS", "Selected Shipment id match with barcode shipment(" + result + ").", null, false);
                    } else
                    {
                        UpdateInfoField("red", "ERROR", "Selected Shipment id not match with barcode shipment("+ result + ").", null, false);
                    }
                    
                    break;
            }

            IsReadOnly = false;
            await ResetInfo(true);
            await UpdateUI();
        }
    }

    public async void ShipmentIdChanged(string shipmentId)
    {
        await ResetInfo(false);
        await ResetInfo(true);
        //await jSRuntime.InvokeVoidAsync("focusEditorByID", "Barcode");
    }

}
