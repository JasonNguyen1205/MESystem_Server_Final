using Blazored.Toast.Services;
using DevExpress.Blazor;
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

    public IGrid? Grid { get; set; }

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
    public IEnumerable<Shipment> ShipmentsShow { get; set; }
    public List<string> ShipmentIdList { get; set; } = new();
    public string ShipmentId { get; set; } = "";
    public bool IsReadOnlyBarcode { get; set; } = true;
    public bool IsReadOnlyContainer { get; set; } = true;
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            Shipments = await TraceDataService.GetLogisticData(shipmentId: "ALL") ?? new List<Shipment>();
            Shipments = Shipments.OrderBy(e => e.ShipmentId);
            foreach (Shipment s in Shipments.Where(s => s.ShipmentId != null).ToList())
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
        if (reset)
        {
            InfoCssColor = new();
            Result = new();
            Infofield = new();
            HighlightMsg = new();
        }

        if (result == "ERROR")
        {
            //await ResetInfo(false);
            if (Sound)
            {
                await jSRuntime.InvokeVoidAsync("playSound", "/sounds/alert.wav");
            }

        }

        if (string.IsNullOrEmpty(cssTextColor) || string.IsNullOrEmpty(content))
        {
            return;
        }

        InfoCssColor.Add(cssTextColor);

        if (result != null)
        {
            Result.Add(result);
        }
        else
        {
            Result.Add("INFO");
        }

        Infofield.Add(content);

        if (highlightMsg != null)
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
        if (backToStart)
        {
            IsReadOnlyContainer = true;
            IsReadOnlyBarcode = false;
            Scanfield = "";
            await jSRuntime.InvokeVoidAsync("focusEditorByID", "Barcode");
        }
        else
        {
            await Task.Run(() =>
            {

                Infofield = new();
                InfoCssColor = new();
                Result = new();
                HighlightMsg = new();
                IsReadOnlyBarcode = true;
                IsReadOnlyContainer = false;
            });
        }
        await UpdateUI();

    }

    async Task UpdateUI()
    {
        //Update UI
        if (ShouldRender())
        {
            await Task.Delay(5);
            await InvokeAsync(StateHasChanged);
        }
#if DEBUG
        Console.WriteLine("UI is updated");
#endif
    }



    private async void GetInputfield(string content)
    {
        Scanfield = content;
    }

    public FinishedGood? Box1 { get; set; }

    private async void HandleBarcode(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {

            IsReadOnlyBarcode = false;
            IsReadOnlyContainer = true;
            await UpdateUI();

            // When Scan Barcode Pallet
            Shipment shipmentsByBacodePallet = Shipments.Where(e => e.ShipmentId.Equals(ShipmentId) && e.TracePalletBarcode.Equals(Scanfield)).FirstOrDefault();

            // When Scan Barcode Box
            var barcodePalletFromBarcodeBox = await TraceDataService.GetPalletByBarcodeBox(Scanfield);
            if (!string.IsNullOrEmpty(barcodePalletFromBarcodeBox))
            {
                shipmentsByBacodePallet = Shipments.Where(e => e.ShipmentId.Equals(ShipmentId) && e.TracePalletBarcode.Equals(barcodePalletFromBarcodeBox)).FirstOrDefault();

            }


            // Check containerNo
            if (string.IsNullOrEmpty(ContainerNo))
            {
                await ResetInfo(true);
                UpdateInfoField("red", "ERROR", "ContainerNo not found", null, false);
                return;
            }


            // Clear Info field:
            await ResetInfo(false);
            UpdateInfoField("green", "INFO", "Selected ShipmentId: " + ShipmentId, null, false);
            UpdateInfoField("green", "INFO", "Container No: " + ContainerNo, null, false);
            UpdateInfoField("green", "INFO", "Scanned Barcode: " + Scanfield, null, false);


            if (shipmentsByBacodePallet != null) UpdateInfoField("green", "INFO", "Number pcs of Pallet: " + shipmentsByBacodePallet.RealPalletQty, null, false);

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
                        UpdateInfoField("green", "SUCCESS", "Selected Shipment is matched with barcode shipment(" + result + ").", null, false);
                    }
                    else
                    {
                        UpdateInfoField("red", "ERROR", "Selected Shipment id not match with barcode shipment(" + result + ").", null, false);
                    }

                    break;
            }

            // Check IsContainer and insert container no
            if(shipmentsByBacodePallet != null && string.IsNullOrEmpty(shipmentsByBacodePallet.ContainerNo)){
                // Insert Container No
                if(await TraceDataService.UpdateContainerNo(shipmentsByBacodePallet, ContainerNo) && await TraceDataService.UpdateVerifyPallet(shipmentsByBacodePallet, 2))
                {
                    await LoadData();
                    UpdateInfoField("green", "SUCCESS", "Insert ContainerNo & Verified Success.", null, false);
                } else
                {
                    await ResetInfo(true);
                    UpdateInfoField("red", "ERROR", "Insert ContainerNo & Verified Fail.", null, false);
                    return;

                }
              
            }
            else if (shipmentsByBacodePallet != null && !string.IsNullOrEmpty(shipmentsByBacodePallet.ContainerNo) && !shipmentsByBacodePallet.ContainerNo.Equals(ContainerNo))
            {
                IsContainer = true;
                await ResetInfo(true);
                UpdateInfoField("red", "ERROR", "Pallet is on another container.", null, false);
                return;
            } else
            {
                UpdateInfoField("green", "SUCCESS", "ContainerNo is matched.", null, false);
            }


            await ResetInfo(true);
        }
    }

    public async void ShipmentIdChanged(string shipmentId)
    {
        ShipmentId = shipmentId;
        await LoadData();
        await ResetInfo(false);
        await jSRuntime.InvokeVoidAsync("focusEditorByID", "ContainerNo");
        //await jSRuntime.InvokeVoidAsync("focusEditorByID", "Barcode");
    }

    // Container 
    public string ContainerNo { get; set; }
    public bool IsContainer { get; set; } = false;
    private async void GetInputContainerfield(string content)
    {
        ContainerNo = content;
    }
    private async void HandleContainer(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            IsReadOnlyContainer = true;
            IsReadOnlyBarcode = false;
            await UpdateUI();
            await jSRuntime.InvokeVoidAsync("focusEditorByID", "Barcode");
        }
    }

    public async Task LoadData()
    {
        Shipments = await TraceDataService.GetLogisticData(ShipmentId) ?? new List<Shipment>();
        ShipmentsShow = Shipments.Where(e => e.ShipmentId.Equals(ShipmentId)).OrderBy(e => e.PartNo).ThenBy(e => e.PoNo).ThenBy(e => e.CustomerPo).ThenByDescending(e => e.RealPalletQty);
        await UpdateUI();
    }
}
