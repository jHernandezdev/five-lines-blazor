using Microsoft.JSInterop;

namespace FiveLines.Client.Helpers;

public class CanvasJSHelper
{
    public static CanvasJSHelper CreateCanvasJSHelper(string canvasID, IJSRuntime? jS)
    {
        ArgumentNullException.ThrowIfNull(jS, nameof(jS));
        
        return new CanvasJSHelper(canvasID, jS);
    }

    private readonly IJSRuntime jS;
    private readonly string canvasID = string.Empty;
    public string FillStyle = string.Empty;

    private CanvasJSHelper(string canvasID, IJSRuntime jS)
    {
        this.jS = jS;
        this.canvasID = canvasID;
    }

    public async Task ClearCanvasRectAsync()
    {
        await jS.InvokeVoidAsync("clearRect", canvasID);
    }

    public async Task FillRectAsync(int x, int y, int width, int height)
    {
        await jS.InvokeVoidAsync("fillRect", canvasID, FillStyle, x, y, width, height);
    }
}
