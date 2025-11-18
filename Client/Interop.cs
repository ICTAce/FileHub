// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Client;

public class Interop(IJSRuntime jsRuntime)
{
    // Keep field for future use
#pragma warning disable IDE0052, S4487 // Remove unread private members
#pragma warning disable S1144 // Unused private types or members should be removed
#pragma warning disable CA1823 // Avoid unused private fields
#pragma warning disable RCS1213 // Remove unused member declaration
    private readonly IJSRuntime _jsRuntime = jsRuntime;
#pragma warning restore RCS1213 // Remove unused member declaration
#pragma warning restore CA1823 // Avoid unused private fields
#pragma warning restore S1144 // Unused private types or members should be removed
#pragma warning restore IDE0052, S4487
}
