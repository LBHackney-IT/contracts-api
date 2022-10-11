using System;
using System.Linq;
using System.Threading.Tasks;
using ContractsApi.V1.Boundary.Requests;
using ContractsApi.V1.Boundary.Response;
using ContractsApi.V1.Factories;
using ContractsApi.V1.Gateways;
using ContractsApi.V1.UseCase.Interfaces;
using Hackney.Core.JWT;
using Hackney.Core.Sns;

namespace ContractsApi.V1.UseCase
{
    public class PatchContractUseCase : IPatchContractUseCase
    {
        private readonly IContractGateway _gateway;
        private readonly ISnsGateway _snsGateway;
        private readonly ISnsFactory _snsFactory;

        public PatchContractUseCase(IContractGateway gateway, ISnsGateway snsGateway, ISnsFactory snsFactory)
        {
            _gateway = gateway;
            _snsGateway = snsGateway;
            _snsFactory = snsFactory;
        }

        public async Task<ContractResponseObject> ExecuteAsync(Guid id, EditContractRequest contract, string requestBody,
            Token token, int? ifMatch)
        {
            var result = await _gateway.PatchContract(id, contract, requestBody, ifMatch);
            if (result == null) return null;

            if (result.NewValues.Any())
            {
                var contractSnsMessage = _snsFactory.UpdateContract(result, token);
                var contractTopicArn = Environment.GetEnvironmentVariable("CONTRACT_SNS_ARN");
                await _snsGateway.Publish(contractSnsMessage, contractTopicArn).ConfigureAwait(false);
            }

            return result.UpdatedEntity.ToDomain().ToResponse();
        }
    }
}
