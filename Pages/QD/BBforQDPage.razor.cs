using MESystem.Service;
using MESystem.Data.TRACE;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using DevExpress.Blazor;
using Microsoft.JSInterop;
using Blazored.Toast.Services;
using System.Text.RegularExpressions;

namespace MESystem.Pages.QD;

    public partial class BBforQDPage : ComponentBase
{
    private string selectedNgCode;


    [Inject]
    TraceService? TraceDataService { get; set; }

    public string? FocusElement { get; set; }
    [Inject]
    IJSRuntime? jSRuntime { get; set; }


    [Inject]
    IToastService? Toast { get; set; }

    public string? ReadOnlyElement { get; set; }
    bool ShowCheckboxes { get; set; } = true;
    public IEnumerable<FinishedGood>? masterData { get; set; }
    public List<FinishedGood>? localData { get; set; } = new List<FinishedGood>();
    public string SelectedNgCode { get => selectedNgCode; set { selectedNgCode = value; UpdateUI(); } }
    public List<string> NgCodeList { get; set; } = new List<string>();
    //DataGridFilteringMode FilteringMode { get; set; } = DataGridFilteringMode.Contains;
    //string? ng_Code { get; set; }
    //string? last_Code { get; set; }
    //string? first_Code { get; set; }
    string? remark { get; set; }
    public bool IsReady { get; set; }
    public IEnumerable<ModelProperties>? partNoData { get; set; }
    public int? numberBox { get; set; } = 0;
    public int? numberFG { get; set; } = 0;
    public string? partNo { get; set; }
    int flag { get; set; }
    public FinishedGood selectedBox { get; set; }

    public List<string> department = new List<string> {"ICT","ATS","ATE","HIGH VOLTAGE","OTHER" };
    private string departmentSelected;
    private static Regex re = new Regex("^\\d{7}([-])\\d{7}([-])\\d{6}([-])\\d{3}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    public bool Sound { get; set; }
    protected override async Task OnInitializedAsync()
    {
        //Sound = true;
        //Data = await TraceDataService.GetScrapCode();
        //if (Data.Count() > 0)
        //{

        //    foreach (Scrap r in Data)
        //    {
        //        Rework temp = new();
        //        if (!string.IsNullOrEmpty(r.NGDescriptionVN))
        //        {
        //            NgCodeList.Add(r.NGDescriptionVN);
        //            int? ngNo = int.Parse((r.NG_Description_VN).Split(".")[0]);
        //            temp.NG_Code = ngNo;
        //            temp.NG_Description_VN = (r.NG_Description_VN).Split(".")[1];
        //            if (temp != null)
        //                NgCodeList.Add(temp);
        //        }

        //    }
        //    NgCodeList.Sort();
        //    await UpdateUI();
        //}

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
            //SelectedRework = Data.Where(e => e.NGDescriptionVN.Contains(SelectedNgCode.ToUpper())).FirstOrDefault();
            if (selectedBox != null)
            {
                SelectedNgCode = selectedBox.BarcodeBox;
                FocusElement = "barcode";

            }
            else
            {
                Toast.ShowError("Error Input", "Error");
            }
            await UpdateUI();
        }
    }

    public async void NgCodeChange(string ngCode)
    {
        if (ngCode != null)
        {
            SelectedNgCode = ngCode;
        }
        //SelectedRework = Data.Where(e => e.NGDescriptionVN.Contains(ngCode.ToUpper())).FirstOrDefault();
        FocusElement = "barcode";
        await UpdateUI();
    }

    public async void DepartmentChange(string ngCode)
    {
        if (ngCode != null)
        {
            departmentSelected = ngCode;
        }
        await UpdateUI();
    }
    public async void EnterDepartment(KeyboardEventArgs e)
    {
        if (e.Code == "Enter")
        {
           
        }
    }
    public bool success { get; set; }
    public string message { get; set; } = "";
    public int? boxQty { get; set; } = 0;
    public int? currentQty { get; set; } = 0;
    List<string>? Infofield { get; set; } = new();

    List<string>? InfoCssColor { get; set; } = new();

    public List<string>? Result { get; set; } = new List<string>();

    public List<string>? HighlightMsg { get; set; } = new List<string>();

    private async void HandleBarcodeInput(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            await ResetInfo(false);

            
                await ResetInfo(true);
                await UpdateUI();
           

        }
    }

    async Task ResetInfo(bool backToStart)
    {
        if (backToStart)
        {
           
            
            await jSRuntime.InvokeVoidAsync("focusEditorByID", "barcode");
        }
        else
        {
            await Task.Run(() =>
            {

                Infofield = new();
                InfoCssColor = new();
                Result = new();
                HighlightMsg = new();
            });
        }
        await UpdateUI();

    }
    async void UpdateInfoField(
        string cssTextColor,
        string? result = null,
        string? content = null,
        string? highlightMsg = null,
        bool reset = false)
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

        if (result == "SUCCESS")
        {
            //await ResetInfo(false);
            if (Sound)
            {
                await jSRuntime.InvokeVoidAsync("playSound", "/sounds/success.mp3");
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

    private async void HandleOldBoxInput(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            await ResetInfo(false);
            if (numberBox == 0 || partNo.Equals(remark.Substring(0, 7)))
            {
                selectedBox = new FinishedGood();
                selectedBox = localData.Where(e => e.BarcodeBox.Contains(remark)).FirstOrDefault();
                if (selectedBox == null)
                {
                    masterData = await TraceDataService.GetFGByBox(remark);
                    if (masterData.Count() > 0)
                    {
                        partNo = remark.Substring(0, 7);
                        partNoData = await TraceDataService.GetPalletContentInfoByPartNo(partNo);
                        boxQty = partNoData.FirstOrDefault().QtyPerBox;
                        localData.AddRange(masterData);
                        UpdateInfoField("green", "SUCCESS", $"Success Insert");
                        numberBox = numberBox + 1;
                        numberFG = localData.Count();

                    }
                    else
                    {
                        UpdateInfoField("red", "ERROR", $"Insert data fail");
                    }

                }
                else
                {
                    UpdateInfoField("red", "ERROR", $"Insert data fail");
                }
            }
            else {
                UpdateInfoField("red", "ERROR", $"Wong Part No");
            }
            remark = "";
            await UpdateUI();
            Console.Write("");
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

