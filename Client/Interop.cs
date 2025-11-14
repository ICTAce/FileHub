// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub;

public class Interop
{
    private readonly IJSRuntime _jsRuntime;

    public Interop(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }
}
