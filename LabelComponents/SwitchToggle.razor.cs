using MESystem.Data.TRACE;

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;


namespace MESystem.LabelComponents;

public partial class SwitchToggle : ComponentBase
{
    [Inject]
    IJSRuntime? jSRuntime { get; set; }

    CustomerRevision? checkedValue;
    [Parameter]
    public CustomerRevision CheckedValue
    {
        get => checkedValue; set
        {
            if(checkedValue==value)
            {
                return;
            }

            checkedValue=value;
            _=CheckedValueChanged.InvokeAsync(value);

        }
    }

    [Parameter]
    public EventCallback<CustomerRevision> CheckedValueChanged { get; set; }

    [Parameter]
    public CustomerRevision? Value { get; set; }

    protected override async Task OnParametersSetAsync()
    {
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

        Console.WriteLine("UI is updated");

    }


    public async Task GetValueClick(CustomerRevision customerRevision, int status)
    {
        customerRevision.Status=status;
        CheckedValue=customerRevision;
        await jSRuntime.InvokeVoidAsync("ConsoleLog", customerRevision);

    }
}