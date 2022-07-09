using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;

namespace MESystem.LabelComponents;

public class BarcodeReader : ComponentBase, IAsyncDisposable
{
    private IJSObjectReference? module;

    private DotNetObjectReference<BarcodeReader>? objRef;

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        if(module!=null)
        {
            await module.InvokeVoidAsync("destroy", barcodeScannerElement.Id);
            await module!.DisposeAsync();
        }

        objRef?.Dispose();
    }

    [Inject]
    private IJSRuntime? JS { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder __builder)
    {
        if(UseBuiltinDiv)
        {
            __builder.OpenElement(0, "div");
            __builder.AddAttribute(1, "class", "modal alert-popup");
            __builder.AddAttribute(2, "tabindex", "-1");
            __builder.AddAttribute(3, "style", "display:block");
            __builder.AddAttribute(4, "role", "dialog");
            __builder.OpenElement(5, "div");
            __builder.AddAttribute(6, "class", "modal-dialog");
            __builder.OpenElement(7, "div");
            __builder.AddAttribute(8, "class", "modal-content");
            __builder.OpenElement(9, "div");
            __builder.AddAttribute(10, "class", "modal-body");
            __builder.AddElementReferenceCapture(11, delegate (ElementReference __value)
            {
                barcodeScannerElement=__value;
            });
            __builder.OpenElement(12, "button");
            __builder.AddAttribute(13, "class", "btn btn-primary p-2 m-1 w-25");
            __builder.AddAttribute(14, "data-action", "startButton");
            __builder.AddContent(15, ScanBtnTitle);
            __builder.CloseElement();
            __builder.AddMarkupContent(16, "\r\n                    ");
            __builder.OpenElement(17, "button");
            __builder.AddAttribute(18, "class", "btn btn-secondary p-2 m-1 w-25");
            __builder.AddAttribute(19, "data-action", "resetButton");
            __builder.AddContent(20, ResetBtnTitle);
            __builder.CloseElement();
            __builder.AddMarkupContent(21, "\r\n                    ");
            __builder.OpenElement(22, "button");
            __builder.AddAttribute(23, "type", "button");
            __builder.AddAttribute(24, "class", "btn btn-info p-2 m-1 w-25");
            __builder.AddAttribute(25, "data-action", "closeButton");
            __builder.AddContent(26, CloseBtnTitle);
            __builder.CloseElement();
            __builder.AddMarkupContent(27, "\r\n\r\n                    ");
            __builder.OpenElement(28, "div");
            __builder.AddAttribute(29, "data-action", "sourceSelectPanel");
            __builder.AddAttribute(30, "style", "display:none");
            __builder.OpenElement(31, "label");
            __builder.AddAttribute(32, "for", "sourceSelect");
            __builder.AddContent(33, SelectDeviceBtnTitle);
            __builder.AddContent(34, ":");
            __builder.CloseElement();
            __builder.AddMarkupContent(35, "\r\n                        <select data-action=\"sourceSelect\" style=\"max-width:100%\" class=\"form-control\"></select>");
            __builder.CloseElement();
            __builder.AddMarkupContent(36, "\r\n                    ");
            __builder.AddMarkupContent(37, "<div><video id=\"video\" style=\"min-height:150px;max-height:60%; max-width: 100%;border: 1px solid gray\"></video></div>");
            __builder.CloseElement();
            __builder.CloseElement();
            __builder.CloseElement();
            __builder.CloseElement();
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        try
        {
            if(firstRender)
            {
                objRef=DotNetObjectReference.Create(this);
                module=await JSRuntimeExtensions.InvokeAsync<IJSObjectReference>(JS, "import", new object[1] { "./_content/ZXingBlazor/lib/zxing/zxingjs.js" });
                await module.InvokeVoidAsync("init", true, objRef, barcodeScannerElement, barcodeScannerElement.Id);
            }
        }
        catch(Exception ex)
        {
            if(OnError!=null)
            {
                await OnError!(ex.Message);
            }
        }
    }

    [JSInvokable]
    public async Task CloseScan()
    {
        await Close.InvokeAsync();
    }

    [JSInvokable]
    public async Task GetError(string err)
    {
        if(OnError!=null)
        {
            await OnError!(err);
        }
    }

    [JSInvokable]
    public async Task GetResult(string val)
    {
        await ScanResult.InvokeAsync(val);
    }

    public ElementReference barcodeScannerElement { get; set; }

    [Parameter]
    public EventCallback Close { get; set; }


    [Parameter]
    public string CloseBtnTitle { get; set; } = "Close";

    [Parameter]
    public Func<string, Task>? OnError { get; set; }


    [Parameter]
    public string ResetBtnTitle { get; set; } = "Reset";

    [Parameter]
    public string? Result { get; set; }

    [Parameter]
    public string ScanBtnTitle { get; set; } = "Scan";


    [Parameter]
    public EventCallback<string> ScanResult { get; set; }


    [Parameter]
    public string SelectDeviceBtnTitle { get; set; } = "Select device";


    [Parameter]
    [Obsolete]
    public bool ShowScanBarcode { get; set; }

    [Parameter]
    public bool UseBuiltinDiv { get; set; } = true;
}