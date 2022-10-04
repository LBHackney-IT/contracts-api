using Hackney.Core.Testing.Shared;
using Xunit;

namespace ContractsApi.Tests
{
    [CollectionDefinition("LogCall collection")]
    public class LogCallAspectFixtureCollection : ICollectionFixture<LogCallAspectFixture>
    { }
}
