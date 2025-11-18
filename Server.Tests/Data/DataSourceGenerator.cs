// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Server.Tests.Data;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
public class DataGenerator : DataSourceGeneratorAttribute<int, int, int>
{
    protected override IEnumerable<Func<(int, int, int)>> GenerateDataSources(DataGeneratorMetadata dataGeneratorMetadata)
    {
        yield return () => (1, 1, 2);
        yield return () => (1, 2, 3);
        yield return () => (4, 5, 9);
    }
}
