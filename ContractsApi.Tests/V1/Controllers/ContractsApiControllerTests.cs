using ContractsApi.V1.Controllers;
using ContractsApi.V1.UseCase.Interfaces;
using Hackney.Core.Http;
using Hackney.Core.JWT;
using Hackney.Core.Testing.Shared;
using Moq;
using NUnit.Framework;

namespace ContractsApi.Tests.V1.Controllers
{
    [TestFixture]
    public class ContractsApiControllerTests : LogCallAspectFixture
    {
        private ContractsApiController _classUnderTest;
        private Mock<IGetContractByIdUseCase> _mockGetByIdUseCase;
        private Mock<IPostNewContractUseCase> _mockPostNewContractUseCase;
        private Mock<ITokenFactory> _mockTokenFactory;
        private Mock<IHttpContextWrapper> _mockContextWrapper;

        [SetUp]
        public void SetUp()
        {
            _mockGetByIdUseCase = new Mock<IGetContractByIdUseCase>();
            _mockPostNewContractUseCase = new Mock<IPostNewContractUseCase>();
            _mockTokenFactory = new Mock<ITokenFactory>();
            _mockContextWrapper = new Mock<IHttpContextWrapper>();
            _classUnderTest = new ContractsApiController(_mockGetByIdUseCase.Object, _mockPostNewContractUseCase.Object,
               _mockTokenFactory.Object, _mockContextWrapper.Object);
        }


        //Add Tests Here
    }
}
