using ContractsApi.V1.Boundary.Requests;
using ContractsApi.V1.Boundary.Response;
using ContractsApi.V1.Domain;
using ContractsApi.V1.Factories;
using ContractsApi.V1.Gateways;
using ContractsApi.V1.UseCase.Interfaces;
using Hackney.Core.JWT;
using Hackney.Core.Sns;
using System;
using System.Threading.Tasks;

namespace ContractsApi.V1.UseCase
{
    public class PostNewContractUseCase : IPostNewContractUseCase
    {
        private readonly IContractGateway _contractGateway;
        private readonly ISnsGateway _snsGateway;
        private readonly ISnsFactory _snsFactory;

        public PostNewContractUseCase(IContractGateway contractGateway, ISnsGateway snsGateway, ISnsFactory snsFactory)
        {
            _contractGateway = contractGateway;
            _snsGateway = snsGateway;
            _snsFactory = snsFactory;
        }

        public async Task<ContractResponseObject> ExecuteAsync(CreateContractRequestObject createContractRequestObject, Token token)
        {
            var contract = await _contractGateway.PostNewContractAsync(createContractRequestObject.ToDatabase()).ConfigureAwait(false);
            if (contract != null && token != null)
            {
                var contractSnsMessage = _snsFactory.CreateContract(contract, token);
                var contractTopicArn = Environment.GetEnvironmentVariable("CONTRACTS_SNS_ARN");

                await _snsGateway.Publish(contractSnsMessage, contractTopicArn).ConfigureAwait(false);
            }
            return contract.ToResponse();
        }
    }
}
