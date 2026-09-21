using Amazon.DynamoDBv2;
using Hackney.Core.DynamoDb;
using Hackney.Core.Sns;
using Hackney.Core.Testing.DynamoDb;
using Hackney.Core.Testing.Sns;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net.Http;
using Amazon.DynamoDBv2.Model;

namespace ContractsApi.Tests
{
    public class MockWebApplicationFactory<TStartup>
        : WebApplicationFactory<TStartup> where TStartup : class
    {
        private readonly List<TableDef> _tables = new List<TableDef>
        {
            new TableDef
            {
                Name = "Contracts", KeyName = "id", KeyType = ScalarAttributeType.S,
                GlobalSecondaryIndexes = new List<GlobalSecondaryIndex>
                {
                    new GlobalSecondaryIndex
                    {
                        IndexName = "ContractsByTargetId",
                        KeySchema = new List<KeySchemaElement>
                        {
                            new KeySchemaElement("targetId", KeyType.HASH),
                        },
                        Projection = new Projection { ProjectionType = ProjectionType.ALL },
                        ProvisionedThroughput = new ProvisionedThroughput(3, 3)
                    }
                }

            }
        };

        public HttpClient Client { get; private set; }
        public IDynamoDbFixture DynamoDbFixture { get; private set; }
        public ISnsFixture SnsFixture { get; private set; }

        public MockWebApplicationFactory()
        {
            EnsureEnvVarConfigured("DynamoDb_LocalMode", "true");
            EnsureEnvVarConfigured("DynamoDb_LocalServiceUrl", "http://localhost:8000");

            EnsureEnvVarConfigured("Sns_LocalMode", "true");
            EnsureEnvVarConfigured("Localstack_SnsServiceUrl", "http://localhost:4566");

            // Startup registers X-Ray for all AWS services, but the fixture setup below runs during
            // host build, where there is no active segment. appsettings sets UseRuntimeErrors, so
            // without this the instrumented DynamoDb client throws instead of logging.
            EnsureEnvVarConfigured("AWS_XRAY_CONTEXT_MISSING", "LOG_ERROR");

            Client = CreateClient();
        }

        private bool _disposed;
        protected override void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                if (null != DynamoDbFixture)
                    DynamoDbFixture.Dispose();
                if (null != SnsFixture)
                    SnsFixture.Dispose();
                if (null != Client)
                    Client.Dispose();
                _disposed = true;
            }
        }

        private static void EnsureEnvVarConfigured(string name, string defaultValue)
        {
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(name)))
                Environment.SetEnvironmentVariable(name, defaultValue);
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration(b => b.AddEnvironmentVariables())
                .UseStartup<Startup>();
            // The Hackney.Core testing fixtures register a singleton IDynamoDbFixture that consumes
            // a scoped IDynamoDBContext, and an ISnsEventVerifier needing an unregistered IAmazonSQS.
            // Host.CreateDefaultBuilder turns on ValidateOnBuild in Development, which rejects both
            // even though neither is resolved through this provider.
            builder.UseDefaultServiceProvider(options => options.ValidateOnBuild = false);
            builder.ConfigureServices(services =>
            {
                services.ConfigureDynamoDB();
                services.ConfigureDynamoDbFixture();

                services.ConfigureSns();
                services.ConfigureSnsFixture();

                var serviceProvider = services.BuildServiceProvider();

                DynamoDbFixture = serviceProvider.GetRequiredService<IDynamoDbFixture>();
                DynamoDbFixture.EnsureTablesExist(_tables);

                SnsFixture = serviceProvider.GetRequiredService<ISnsFixture>();
                SnsFixture.CreateSnsTopic<EntityEventSns>("contracts.fifo", "CONTRACTS_SNS_ARN");
            });
        }
    }
}

