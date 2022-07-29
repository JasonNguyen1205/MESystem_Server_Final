
using MESystem.Service;
using MESystem.Data.TRACE;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using DevExpress.Blazor;
using Microsoft.JSInterop;
using Blazored.Toast.Services;
using System.Text.RegularExpressions;

namespace MESystem.Pages.Process;

public partial class ReworkPage : ComponentBase
{
    private string selectedNgCode;

    [Inject]
    private TraceService? TraceDataService { get; set; }
 
    public string? FocusElement { get; set; }
    [Inject]
    IJSRuntime? jSRuntime { get; set; }


    [Inject]
    IToastService? Toast { get; set; }

    public string? ReadOnlyElement { get; set; }
    bool ShowCheckboxes { get; set; } = true;
    public IEnumerable<Rework>? Data { get; set; }

    public string SelectedNgCode { get => selectedNgCode; set { selectedNgCode = value;  UpdateUI(); } }
    public List<string> NgCodeList { get; set; } = new List<string>();
    //DataGridFilteringMode FilteringMode { get; set; } = DataGridFilteringMode.Contains;
    //string? ng_Code { get; set; }
    //string? last_Code { get; set; }
    //string? first_Code { get; set; }
    string? remark { get; set; }
    public bool IsReady { get; set; }
    public string? ngCode { get; set; }
    public string? barcode { get; set; }
    public string? internalCode { get; set; }
    int flag { get; set; }
    public Rework SelectedRework { get; set; }

    private static Regex re = new Regex("^\\d{7}([-])\\d{7}([-])\\d{6}([-])\\d{3}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    public bool Sound { get; set; }
    protected override async Task OnInitializedAsync()
    {
        Sound = true;
        Data = await TraceDataService.GetNGCode();
        if (Data.Count() > 0)
        {

            foreach (Rework r in Data)
            {
                Rework temp = new();
                if (!string.IsNullOrEmpty(r.NG_Description_VN))
                {
                    NgCodeList.Add(r.NG_Description_VN);
                    //int? ngNo = int.Parse((r.NG_Description_VN).Split(".")[0]);
                    //temp.NG_Code = ngNo;
                    //temp.NG_Description_VN = (r.NG_Description_VN).Split(".")[1];
                    //if (temp != null)
                    //    NgCodeList.Add(temp);
                }

            }
            NgCodeList.Sort();
            await UpdateUI();
        }

        flag = 0;
    }
    async void GetData()
    {

    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            ShouldUpdateUI = true;
        }

    }
    public bool ShouldUpdateUI { get; private set; }
    protected override bool ShouldRender() { return ShouldUpdateUI; }
    private async Task UpdateUI()
    {
        ShouldUpdateUI = true;
        //Update UI
        if (ShouldRender())
        {
            await Task.Delay(5);
            await InvokeAsync(StateHasChanged);
            await Task.Delay(5);
            //ShouldUpdateUI = false;
        }

        Console.WriteLine("UI is updated");
    }
    public async void Enter(KeyboardEventArgs e)
    {
        if (e.Code == "Enter")
        {
            SelectedNgCode = await jSRuntime.InvokeAsync<string>("getValueById", "ngCode");
            SelectedRework = Data.Where(e => e.NG_Description_VN.Contains(SelectedNgCode.ToUpper())).FirstOrDefault();
            if(SelectedRework != null)
            {
                SelectedNgCode = SelectedRework.NG_Description_VN;
                FocusElement = "barcode";
               
            } else
            {
                Toast.ShowError("Error Input", "Error");
            }
            await UpdateUI();
        }
    }

    public async void NgCodeChange(string ngCode)
    {
        if (ngCode != null)
            SelectedNgCode = ngCode;
        SelectedRework = Data.Where(e => e.NG_Description_VN.Contains(ngCode.ToUpper())).FirstOrDefault();
        FocusElement = "barcode";
        await UpdateUI();
    }
    public bool success { get; set; }
    public string message { get; set; } = "";
    private async void HandleBarcodeInput(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            success = false;
            message = "";
            bool checkBarcode = false;
            checkBarcode = re.IsMatch(barcode);
            if (checkBarcode)
            {
                internalCode = barcode;
            }
            else
            {
                internalCode = await TraceDataService.GetBarcodeLink(barcode);
            }
            if (string.IsNullOrEmpty(internalCode)) {
                //Toast.ShowError("Wrong barcode!", "Error");
                barcode = "";
                if (Sound)
                {
                    success = false;
                    message = "Error input";
                    await jSRuntime.InvokeVoidAsync("playSound", "/sounds/alert.wav");
                }
                await UpdateUI();
                return;
            }
            else {
                ngCode = selectedNgCode.Split(".")[0].ToString();
                Rework input_data = new Rework(internalCode, null, int.Parse(ngCode), "", "", "", "");
                await TraceDataService.InsertReworkData(input_data);
                //Toast.ShowSuccess("Insert OK!", "Success");
                success = true;
                message = "Insert Success";

                barcode = ""; 
                internalCode = "";
                await UpdateUI();
            }
            
        }
    }

    private async void HandleRemarkInput(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            FocusElement = "barcode";
            ReadOnlyElement = "remark";
        }
    }

    public string EmployeeId { get; set; } = "";
    private async void HandleEmployeeIdInput(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            FocusElement = "ngCode";
            //ReadOnlyElement = "remark";
        }
    }
}
