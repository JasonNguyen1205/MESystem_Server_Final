using Microsoft.AspNetCore.Components;

namespace MESystem.LabelComponents;

public partial class PalleteLabel : ComponentBase
{
    [Parameter]
    public string? Src { get => src; set => src=value; }
    private string? src;
    [Parameter]
    public string? Content { get => content; set => content=value; }
    private string? content;
    [Parameter]
    public string? Id { get; set; }

    private bool shouldRender = true;

    protected override bool ShouldRender()
    {
        return shouldRender;
    }

    protected override async Task OnParametersSetAsync()
    {
        await InvokeAsync(() => StateHasChanged());
    }
}
