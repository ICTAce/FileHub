---
description: 'Meziantou.Analyzer rules for enforcing C# best practices in design, usage, security, performance, and style'
applyTo: '**/*.cs'
---

# Meziantou.Analyzer Rules

This file contains guidelines based on Meziantou.Analyzer rules to enforce best practices in C# development.

## General Instructions

Follow all Meziantou.Analyzer rules to ensure code quality, performance, security, and maintainability. The analyzer enforces best practices across multiple categories: Usage, Performance, Design, Security, Style, and Naming.

## Usage Rules

### String Comparison and Culture

- **MA0001**: Always specify StringComparison when comparing strings (e.g., `StringComparison.OrdinalIgnoreCase`)
- **MA0002**: Provide IEqualityComparer<string> or IComparer<string> when using collections or LINQ methods
- **MA0006**: Use `String.Equals` instead of equality operator (`==`) for string comparisons
- **MA0021**: Use `StringComparer.GetHashCode` instead of `string.GetHashCode` for hash-based collections
- **MA0024**: Use an explicit StringComparer when possible (e.g., in dictionaries, sets)
- **MA0074**: Avoid implicit culture-sensitive methods; specify culture explicitly
- **MA0127**: Use `String.Equals` instead of is pattern for string comparisons

### Format Providers and Globalization

- **MA0011**: Specify IFormatProvider when formatting or parsing values
- **MA0075**: Do not use implicit culture-sensitive ToString
- **MA0076**: Do not use implicit culture-sensitive ToString in interpolated strings

### Task and Async/Await

- **MA0004**: Use `Task.ConfigureAwait(false)` in library code to avoid deadlocks
- **MA0022**: Return `Task.FromResult` instead of returning null from async methods
- **MA0032**: Use overloads with CancellationToken argument when available
- **MA0040**: Forward the CancellationToken parameter to methods that accept one
- **MA0042**: Do not use blocking calls (e.g., `.Result`, `.Wait()`) in async methods
- **MA0045**: Do not use blocking calls in sync methods; make the method async instead
- **MA0079**: Forward CancellationToken using `.WithCancellation()` for IAsyncEnumerable
- **MA0080**: Use a cancellation token with `.WithCancellation()` when iterating IAsyncEnumerable
- **MA0100**: Await task before disposing of resources
- **MA0129**: Await task in using statement
- **MA0134**: Observe result of async calls
- **MA0147**: Avoid async void methods for delegates
- **MA0155**: Do not use async void methods (except event handlers)

### Argument Validation

- **MA0015**: Specify the parameter name in ArgumentException
- **MA0043**: Use nameof operator in ArgumentException
- **MA0050**: Validate arguments correctly in iterator methods
- **MA0131**: ArgumentNullException.ThrowIfNull should not be used with non-nullable types

### Exception Handling

- **MA0012**: Do not raise reserved exception types (e.g., NullReferenceException, IndexOutOfRangeException)
- **MA0013**: Types should not extend System.ApplicationException
- **MA0014**: Do not raise System.ApplicationException
- **MA0027**: Prefer rethrowing an exception implicitly using `throw;` instead of `throw ex;`
- **MA0054**: Embed the caught exception as innerException when rethrowing
- **MA0072**: Do not throw from a finally block
- **MA0086**: Do not throw from a finalizer

### Events

- **MA0019**: Use `EventArgs.Empty` instead of creating new instance
- **MA0085**: Anonymous delegates should not be used to unsubscribe from events
- **MA0091**: Sender should be 'this' for instance events
- **MA0092**: Sender should be 'null' for static events
- **MA0093**: EventArgs should not be null when raising an event

### Collections and LINQ

- **MA0103**: Use `SequenceEqual` instead of equality operator for arrays/sequences
- **MA0099**: Use explicit enum value instead of 0 in enums

### Regex

- **MA0009**: Add regex evaluation timeout to prevent ReDoS attacks

### DateTime and DateTimeOffset

- **MA0132**: Do not convert implicitly to DateTimeOffset
- **MA0133**: Use DateTimeOffset instead of relying on implicit conversion
- **MA0113**: Use `DateTime.UnixEpoch` instead of new DateTime(1970, 1, 1)
- **MA0114**: Use `DateTimeOffset.UnixEpoch` instead of constructing manually

### Process Execution

- **MA0161**: UseShellExecute must be explicitly set when using Process.Start
- **MA0162**: Use Process.Start overload with ProcessStartInfo
- **MA0163**: UseShellExecute must be false when redirecting standard input or output

### Pattern Matching

- **MA0141**: Use pattern matching instead of inequality operators for null check
- **MA0142**: Use pattern matching instead of equality operators for null check
- **MA0148**: Use pattern matching instead of equality operators for discrete values
- **MA0149**: Use pattern matching instead of inequality operators for discrete values
- **MA0171**: Use pattern matching instead of HasValue for Nullable<T> check

### Other Usage Rules

- **MA0037**: Remove empty statements
- **MA0060**: The value returned by Stream.Read/Stream.ReadAsync is not used
- **MA0101**: String contains an implicit end of line character
- **MA0108**: Remove redundant argument value
- **MA0128**: Use 'is' operator instead of SequenceEqual for constant arrays
- **MA0130**: GetType() should not be used on System.Type instances
- **MA0136**: Raw string contains an implicit end of line character
- **MA0165**: Make interpolated string instead of concatenation
- **MA0166**: Forward the TimeProvider to methods that take one
- **MA0167**: Use an overload with a TimeProvider argument

## Performance Rules

### Array and Collection Optimization

- **MA0005**: Use `Array.Empty<T>()` instead of `new T[0]` or `new T[] {}`
- **MA0020**: Use direct methods instead of LINQ (e.g., `List<T>.Count` instead of `Count()`)
- **MA0029**: Combine LINQ methods when possible
- **MA0030**: Remove useless OrderBy call
- **MA0031**: Optimize `Enumerable.Count()` usage (use `Count` property when available)
- **MA0063**: Use Where before OrderBy for better performance
- **MA0078**: Use 'Cast' instead of 'Select' to cast
- **MA0098**: Use indexer instead of LINQ methods (e.g., `list[0]` instead of `First()`)
- **MA0112**: Use 'Count > 0' instead of 'Any()' when Count property is available
- **MA0159**: Use 'Order' instead of 'OrderBy' when ordering by self
- **MA0160**: Use ContainsKey instead of TryGetValue when value is not needed

### String and StringBuilder

- **MA0028**: Optimize StringBuilder usage
- **MA0044**: Remove useless ToString call
- **MA0089**: Optimize string method usage
- **MA0111**: Use string.Create instead of FormattableString for performance

### Struct and Memory Layout

- **MA0008**: Add StructLayoutAttribute to structs for explicit memory layout
- **MA0065**: Avoid using default ValueType.Equals or HashCode for struct equality
- **MA0066**: Hash table unfriendly types should not be used in hash tables
- **MA0102**: Make member readonly when possible
- **MA0168**: Use readonly struct for 'in' or 'ref readonly' parameters

### Regex Optimization

- **MA0023**: Add RegexOptions.ExplicitCapture to improve performance
- **MA0110**: Use the Regex source generator (.NET 7+)

### Closure and Lambda Optimization

- **MA0105**: Use lambda parameters instead of using a closure
- **MA0106**: Avoid closure by using overload with 'factoryArgument' parameter

### Task and Async Optimization

- **MA0152**: Use Unwrap instead of using await twice

### Other Performance Rules

- **MA0052**: Replace constant `Enum.ToString` with nameof
- **MA0120**: Use InvokeVoidAsync when the returned value is not used (Blazor)
- **MA0144**: Use System.OperatingSystem to check the current OS instead of RuntimeInformation
- **MA0158**: Use System.Threading.Lock (.NET 9+) instead of object lock
- **MA0176**: Optimize GUID creation (prefer parsing over constructors)
- **MA0178**: Use TimeSpan.Zero instead of TimeSpan.FromXXX(0)

## Design Rules

### Class and Type Design

- **MA0010**: Mark attributes with AttributeUsageAttribute
- **MA0016**: Prefer using collection abstraction instead of implementation
- **MA0017**: Abstract types should not have public or internal constructors
- **MA0018**: Do not declare static members on generic types (deprecated, use CA1000)
- **MA0025**: Implement functionality instead of throwing NotImplementedException
- **MA0026**: Fix TODO comments
- **MA0033**: Do not tag instance fields with ThreadStaticAttribute
- **MA0036**: Make class static when all members are static
- **MA0038**: Make method static when possible (deprecated, use CA1822)
- **MA0041**: Make property static when possible (deprecated, use CA1822)
- **MA0046**: Use EventHandler<T> to declare events
- **MA0047**: Declare types in namespaces
- **MA0048**: File name must match type name
- **MA0049**: Type name should not match containing namespace
- **MA0051**: Method is too long
- **MA0053**: Make class or record sealed when not intended for inheritance
- **MA0055**: Do not use finalizers
- **MA0056**: Do not call overridable members in constructor
- **MA0061**: Method overrides should not change default values
- **MA0062**: Non-flags enums should not be marked with FlagsAttribute
- **MA0064**: Avoid locking on publicly accessible instance
- **MA0067**: Use `Guid.Empty` instead of `new Guid()`
- **MA0068**: Invalid parameter name for nullable attribute
- **MA0069**: Non-constant static fields should not be visible
- **MA0070**: Obsolete attributes should include explanations
- **MA0081**: Method overrides should not omit params keyword
- **MA0082**: NaN should not be used in comparisons
- **MA0083**: ConstructorArgument parameters should exist in constructors
- **MA0084**: Local variables should not hide other symbols
- **MA0087**: Parameters with [DefaultParameterValue] should also be marked [Optional]
- **MA0088**: Use [DefaultParameterValue] instead of [DefaultValue]
- **MA0090**: Remove empty else/finally block
- **MA0104**: Do not create a type with a name from the BCL
- **MA0107**: Do not use object.ToString (too generic)
- **MA0109**: Consider adding an overload with Span<T> or Memory<T>
- **MA0121**: Do not overwrite parameter value
- **MA0140**: Both if and else branches have identical code
- **MA0143**: Primary constructor parameters should be readonly
- **MA0150**: Do not call the default object.ToString explicitly
- **MA0169**: Use Equals method instead of operator for types without operator overload
- **MA0170**: Type cannot be used as an attribute argument
- **MA0172**: Both sides of the logical operation are identical
- **MA0173**: Use LazyInitializer.EnsureInitialized for lazy initialization

### Interface Implementation

- **MA0077**: A class that provides Equals(T) should implement IEquatable<T>
- **MA0094**: A class that provides CompareTo(T) should implement IComparable<T>
- **MA0095**: A class that implements IEquatable<T> should override Equals(object)
- **MA0096**: A class that implements IComparable<T> should also implement IEquatable<T>
- **MA0097**: A class that implements IComparable<T> or IComparable should override comparison operators

### Method Naming

- **MA0137**: Use 'Async' suffix when a method returns an awaitable type
- **MA0138**: Do not use 'Async' suffix when a method does not return an awaitable type
- **MA0156**: Use 'Async' suffix when a method returns IAsyncEnumerable<T>
- **MA0157**: Do not use 'Async' suffix when a method does not return IAsyncEnumerable<T>

### Blazor-Specific Design

- **MA0115**: Unknown component parameter
- **MA0116**: Parameters with [SupplyParameterFromQuery] should also be marked as [Parameter]
- **MA0117**: Parameters with [EditorRequired] should also be marked as [Parameter]
- **MA0118**: [JSInvokable] methods must be public
- **MA0119**: JSRuntime must not be used in OnInitialized or OnInitializedAsync
- **MA0122**: Parameters with [SupplyParameterFromQuery] are only valid in routable components (@page)

### Logging Design

- **MA0123**: Sequence number must be a constant in LoggerMessage
- **MA0124**: Log parameter type is not valid
- **MA0125**: The list of log parameter types contains an invalid type
- **MA0126**: The list of log parameter types contains a duplicate
- **MA0135**: The log parameter has no configured type
- **MA0139**: Log parameter type is not valid
- **MA0153**: Do not log symbols decorated with DataClassificationAttribute directly

### UnsafeAccessor

- **MA0145**: Signature for [UnsafeAccessorAttribute] method is not valid
- **MA0146**: Name must be set explicitly on local functions with UnsafeAccessor

### XML Comments

- **MA0151**: DebuggerDisplay must contain valid members
- **MA0154**: Use langword in XML comments (e.g., `<see langword="null"/>`)

## Security Rules

- **MA0009**: Add regex evaluation timeout to prevent ReDoS attacks
- **MA0039**: Do not write your own certificate validation method
- **MA0035**: Do not use dangerous threading methods (e.g., Thread.Abort, Thread.Suspend)

## Style Rules

### Code Formatting

- **MA0003**: Add parameter name to improve readability (for boolean/null arguments)
- **MA0007**: Add a comma after the last value in enums
- **MA0071**: Avoid using redundant else
- **MA0073**: Avoid comparison with bool constant (e.g., `if (condition == true)`)
- **MA0164**: Use parentheses to make 'not' pattern clearer
- **MA0174**: Record should use explicit 'class' keyword
- **MA0175**: Record should not use explicit 'class' keyword
- **MA0177**: Use single-line XML comment syntax when possible

## Naming Rules

- **MA0057**: Class name should end with 'Attribute' for attribute classes
- **MA0058**: Class name should end with 'Exception' for exception classes
- **MA0059**: Class name should end with 'EventArgs' for EventArgs classes

## Blazor-Specific Rules

Apply these rules when working with Blazor components:

- **MA0115**: Ensure all component parameters are defined correctly
- **MA0116**: Mark parameters with [SupplyParameterFromQuery] as [Parameter]
- **MA0117**: Mark parameters with [EditorRequired] as [Parameter]
- **MA0118**: Make [JSInvokable] methods public
- **MA0119**: Do not use JSRuntime in OnInitialized or OnInitializedAsync
- **MA0120**: Use InvokeVoidAsync when the return value is not needed
- **MA0122**: Only use [SupplyParameterFromQuery] in routable components (@page)

## Code Examples

### Good Example - String Comparison

```csharp
// Correct: Specify StringComparison
if (string.Equals(str1, str2, StringComparison.OrdinalIgnoreCase))
{
    // ...
}

// Correct: Use StringComparer in collections
var dictionary = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
```

### Bad Example - String Comparison

```csharp
// Avoid: Missing StringComparison
if (str1 == str2) // MA0006
{
    // ...
}

// Avoid: Missing StringComparer
var dictionary = new Dictionary<string, int>(); // MA0002
```

### Good Example - ConfigureAwait

```csharp
// Correct: Use ConfigureAwait(false) in library code
await httpClient.GetAsync(url).ConfigureAwait(false);
```

### Bad Example - ConfigureAwait

```csharp
// Avoid: Missing ConfigureAwait
await httpClient.GetAsync(url); // MA0004
```

### Good Example - CancellationToken

```csharp
// Correct: Forward CancellationToken
public async Task ProcessAsync(CancellationToken cancellationToken)
{
    await DoWorkAsync(cancellationToken);
}
```

### Bad Example - CancellationToken

```csharp
// Avoid: Not forwarding CancellationToken
public async Task ProcessAsync(CancellationToken cancellationToken)
{
    await DoWorkAsync(); // MA0040
}
```

### Good Example - Array.Empty

```csharp
// Correct: Use Array.Empty<T>()
return Array.Empty<string>();
```

### Bad Example - Array.Empty

```csharp
// Avoid: Allocating empty array
return new string[0]; // MA0005
return new string[] { }; // MA0005
```

### Good Example - Exception Handling

```csharp
// Correct: Rethrow without losing stack trace
catch (Exception)
{
    // Cleanup
    throw;
}

// Correct: Include inner exception
catch (IOException ex)
{
    throw new CustomException("Failed to read file", ex);
}
```

### Bad Example - Exception Handling

```csharp
// Avoid: Loses stack trace
catch (Exception ex)
{
    throw ex; // MA0027
}

// Avoid: Missing inner exception
catch (IOException ex)
{
    throw new CustomException("Failed to read file"); // MA0054
}
```

### Good Example - LINQ Optimization

```csharp
// Correct: Use Count property
if (list.Count > 0)
{
    // ...
}

// Correct: Use indexer
var first = list[0];
```

### Bad Example - LINQ Optimization

```csharp
// Avoid: Unnecessary LINQ method
if (list.Any()) // MA0112 (when Count is available)
{
    // ...
}

// Avoid: LINQ when indexer is available
var first = list.First(); // MA0098
```

## Configuration Notes

You can configure rule severity in `.editorconfig`:

```ini
[*.cs]
# Disable specific rule
dotnet_diagnostic.MA0004.severity = none

# Change severity
dotnet_diagnostic.MA0026.severity = suggestion
```

## Validation

- Build the solution to ensure all Meziantou.Analyzer rules are followed
- Review analyzer warnings in the Error List
- Use code fixes where available to automatically correct violations
- Configure rule severity in .editorconfig based on project requirements
