using MESystem.Data.TRACE;
using Microsoft.AspNetCore.Components;

namespace MESystem.LabelComponents;

public partial class SwitchToggle : ComponentBase
{

    int checkedValue;
    [Parameter]
    public int CheckedValue { get => checkedValue; set { if (checkedValue == value) return; checkedValue = value; CheckedValueChanged.InvokeAsync(value); } }

    [Parameter]
    public EventCallback<int> CheckedValueChanged { get; set; }

    [Parameter]
    public int Value { get; set; }

    protected override async Task OnParametersSetAsync()
    {
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

        Console.WriteLine("UI is updated");

    }
}