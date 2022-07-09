using Blazored.Toast.Services;

using MESystem.Data.TRACE;
using MESystem.Service;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace MESystem.Pages.Warehouse;

public partial class MergePartialBox : ComponentBase
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
    string? Scanfield2 { get; set; }

    public string? Title { get; set; }
    public bool Sound { get; set; } = true;

    //Scan for making palette only
    public int QtyPerBox;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if(firstRender)
        {
            await jSRuntime.InvokeVoidAsync("focusEditorByID", "BarcodeBox1");
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
            Scanfield="";
            Scanfield2="";
            Box1=new FinishedGood();
            Box2=new FinishedGood();
            await UpdateUI();
            await jSRuntime.InvokeVoidAsync("focusEditorByID", "BarcodeBox1");

        }
        else
        {
            Infofield=new();
            InfoCssColor=new();
            Result=new();
            HighlightMsg=new();
        }


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



    private void GetInputfield(string content) { Scanfield=content; }
    private void GetInputfield2(string content) { Scanfield2=content; }

    public FinishedGood? Box1 { get; set; }
    public FinishedGood? Box2 { get; set; }

    private async void HandleBarcodeBox1(KeyboardEventArgs e)
    {
        if(e.Key=="Enter")
        {
            // Clear Info field:
            await ResetInfo(false);
            // Check Error/Exist Barcode 
            if(!await CheckExistBarcode(Scanfield))
            {

                Toast.ShowError("Barcode not existed or In another pallet", "Error");
                UpdateInfoField("red", "ERROR", $"Barcode Box 1: {Scanfield} is not existed or in another pallet");
                await ResetInfo(true);
                await UpdateUI();
                return;
            }
            Box1=(await TraceDataService.GetBoxContentInformation(Scanfield, Scanfield.Substring(0, 7))).FirstOrDefault();
            UpdateInfoField("green", "INFO", $"Barcode Box 1: {Box1.BarcodeBox} - PartNos: {Box1.PartNo} - Number of Box: {Box1.QtyBox} - Family: {await TraceDataService.GetFamilyFromPartNo(Box1.PartNo)}");
            await UpdateUI();
            await jSRuntime.InvokeVoidAsync("focusEditorByID", "BarcodeBox2");

        }
    }
    private async void HandleBarcodeBox2(KeyboardEventArgs e)
    {
        if(e.Key=="Enter")
        {


            // Check Error //Exist Barcode 
            if(!await CheckExistBarcode(Scanfield2))
            {
                Toast.ShowError("Barcode not existed or In another pallet ", "Error");
                UpdateInfoField("red", "ERROR", $"Barcode Box 2: {Scanfield} is not existed or in another pallet");
                await ResetInfo(true);
                await UpdateUI();
                return;
            }

            QtyPerBox=await TraceDataService.GetQtyFromTrace(3, Box1.PartNo);
            Box2=(await TraceDataService.GetBoxContentInformation(Scanfield2, Scanfield2.Substring(0, 7))).FirstOrDefault();
            UpdateInfoField("green", "INFO", $"Barcode Box 2: {Box2.BarcodeBox} - PartNos: {Box2.PartNo} - Number of Box: {Box2.QtyBox} - Family {await TraceDataService.GetFamilyFromPartNo(Box2.PartNo)}");
            await UpdateUI();

            // Check Duplicate Same Box
            if(Box1.BarcodeBox==Box2.BarcodeBox)
            {
                Toast.ShowError("Duplicated Box", "Error");
                UpdateInfoField("red", "ERROR", $"Two barcode boxs are the same");
                await ResetInfo(true);
                await UpdateUI();
                return;
            }
            else
            {
                UpdateInfoField("green", "PASS", $"Check Duplicate");
                await UpdateUI();
            }

            // Check PartNos
            if(!await CheckPartNo(Box1.PartNo, Box2.PartNo))
            {
                Toast.ShowError("Error PartNos", "Error");
                UpdateInfoField("red", "ERROR", $"Two boxes have different model");
                await ResetInfo(true);
                await UpdateUI();
                return;
            }
            else
            {
                UpdateInfoField("green", "PASS", $"Check Model");
                await UpdateUI();
            }

            // Check Revision
            if(await TraceDataService.GetFamilyFromPartNo(Box2.PartNo)=="Phoenix")
            {
                if(!await CheckRevisionBoxs(Box1.BarcodeBox, Box2.BarcodeBox))
                {
                    Toast.ShowError("Error Revision", "Error");
                    UpdateInfoField("red", "ERROR", $"Two boxes have different revision");
                    await ResetInfo(true);
                    await UpdateUI();
                    return;
                }
                else
                {
                    UpdateInfoField("green", "PASS", $"Check Revision");
                    await UpdateUI();
                }
            }

            // Check Quantity <= StandardQuality Box
            if(!await CheckQuantity(Box1.BarcodeBox, Box2.BarcodeBox))
            {
                Toast.ShowError("Quality of two partial boxs was exceed with standard quanity ob box", "Error");
                UpdateInfoField("red", "ERROR", $"Quality of two boxs was exceed");
                await ResetInfo(true);
                await UpdateUI();
                return;
            }
            else
            {
                UpdateInfoField("green", "PASS", $"Check Quality");
                await UpdateUI();
            }


            //Update Barcode 2 to Barcode 1
            if(!await TraceDataService.UpdateBarcodeBox(Scanfield, Scanfield2))
            {
                Toast.ShowError("Update Error", "Error");
                UpdateInfoField("red", "ERROR", $"Merge Barcode Box Error");
                await ResetInfo(true);
                await UpdateUI();
                return;
            }

            Toast.ShowSuccess("Merge Box Success, Please remove first barcode box !!!", "Success");
            UpdateInfoField("green", "SUCCESS", $"Update success two box: {Box1.BarcodeBox} and {Box2.BarcodeBox} into {Box2.BarcodeBox} with quality: {Box1.QtyBox+Box2.QtyBox}");
            await ResetInfo(true);
            await UpdateUI();
            await jSRuntime.InvokeVoidAsync("focusEditorByID", "BarcodeBox1");
        }
    }

    // Check Error/Exist Barcode 
    public async Task<bool> CheckExistBarcode(string barcodeBox)
    {
        IEnumerable<FinishedGood>? result = await TraceDataService.CheckBarcodeBoxExist(barcodeBox);
        if(result.Count()>0)
        {
            return true;
        }

        return false;
    }
    // Check PartNos 
    public async Task<bool> CheckPartNo(string partNo1, string partNo2)
    {
        if(partNo1==partNo2)
        {
            return true;
        }

        return false;
    }

    // Check Revision IFS
    //public async Task<bool> CheckRevision(string revision)
    //{
    //    if (CurrentIFSRevision == revision) return true;
    //    return false;
    //}

    // Check Revision 2 box if phoenix
    public async Task<bool> CheckRevisionBoxs(string revbox1, string revbox2)
    {
        if(revbox1.Substring(7, 2)==revbox2.Substring(7, 2))
        {
            return true;
        }

        return false;
    }



    // Check Quantity <= Standard Quanlity box
    public async Task<bool> CheckQuantity(string barcodeBox1, string barcodeBox2)
    {
        FinishedGood? box1 = (await TraceDataService.GetBoxContentInformation(barcodeBox1, barcodeBox1.Substring(0, 7))).FirstOrDefault();
        FinishedGood? box2 = (await TraceDataService.GetBoxContentInformation(barcodeBox2, barcodeBox2.Substring(0, 7))).FirstOrDefault();
        if(box1.QtyBox+box2.QtyBox<=QtyPerBox)
        {
            return true;
        }

        return false;
    }

}
