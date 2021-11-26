using License.Function;
using License.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace LicenseBackend.Tests
{
    public class LicenseFunctionTests
    {
        private MockRepository mockRepository;
        private readonly Mock<ItemResponse<License.Models.License>> _mockItemResponse;
        private Mock<Container> _mockContainer; 
        private readonly Mock<ILogger> _mockLogger;

        public LicenseFunctionTests()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);
            _mockItemResponse = this.mockRepository.Create<ItemResponse<License.Models.License >> ();
            this._mockContainer = this.mockRepository.Create<Container>();
            _mockLogger = this.mockRepository.Create<ILogger>();
        }

        private LicenseFunction CreateLicenseFunction()
        {
            return new LicenseFunction(
                this._mockContainer.Object);
        }

        [Fact]
        public async Task Create_Success()
        {
            // Arrange
            var licenseFunction = this.CreateLicenseFunction();
            CreateLicenseRequest req = new CreateLicenseRequest() { CompanyName="Test", CustomerId=Guid.NewGuid().ToString(), DomainName="Test.dk"};
            License.Models.License expectedResponse = new License.Models.License() { Id="someid" };
            string expectedId = "";
            ILogger log = _mockLogger.Object;
            _mockItemResponse.Setup(x => x.StatusCode).Returns(HttpStatusCode.Created);
            _mockContainer.Setup(x => x.CreateItemAsync(It.Is<License.Models.License>(x => x.CompanyName == req.CompanyName), null,null,It.IsAny<CancellationToken>())).ReturnsAsync((License.Models.License x,object x1,object x2,object x3)=>
            {
                expectedId = x.Id;
                return _mockItemResponse.Object;
             });

            // Act
            OkObjectResult result = (OkObjectResult)(await licenseFunction.Create( req,log));

            // Assert
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(expectedId,  ((CreateLicenseResponse)result.Value).LicenseId);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task Create_Failed()
        {
            // Arrange
            var licenseFunction = this.CreateLicenseFunction();
            CreateLicenseRequest req = new CreateLicenseRequest() { CompanyName = "Test", CustomerId = Guid.NewGuid().ToString(), DomainName = "Test.dk" };
            License.Models.License expectedResponse = new License.Models.License() { Id = "someid" };
            string expectedId = "";
            ILogger log = _mockLogger.Object;
            _mockItemResponse.Setup(x => x.StatusCode).Returns(HttpStatusCode.BadRequest);
            _mockContainer.Setup(x => x.CreateItemAsync(It.Is<License.Models.License>(x => x.CompanyName == req.CompanyName), null, null, It.IsAny<CancellationToken>())).ReturnsAsync((License.Models.License x, object x1, object x2, object x3) =>
            {
                expectedId = x.Id;
                return _mockItemResponse.Object;
            });

            SetUpLogger();


            // Act
            StatusCodeResult result = (StatusCodeResult)(await licenseFunction.Create(req, log));

            // Assert
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);            
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task Get_License_Success()
        {
            // Arrange
            var licenseFunction = this.CreateLicenseFunction();
            HttpRequest req = null;
            License.Models.License expectedResponse = new License.Models.License() { Id = "someid" };
            string id = expectedResponse.Id;
            ILogger log = null;

            _mockItemResponse.Setup(x => x.StatusCode).Returns(HttpStatusCode.OK);
            _mockItemResponse.Setup(x => x.Resource).Returns(expectedResponse);
            _mockContainer.Setup(x => x.ReadItemAsync<License.Models.License>(It.Is<string>(x =>x==id),It.IsAny<PartitionKey>(),null, It.IsAny<CancellationToken>())).ReturnsAsync(_mockItemResponse.Object);
            

            // Act
            var result = await licenseFunction.Get(req,id,log) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedResponse, (License.Models.License)result.Value);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task Get_License_NoSuccess()
        {
            // Arrange
            var licenseFunction = this.CreateLicenseFunction();
            HttpRequest req = null;
            License.Models.License expectedResponse = new License.Models.License() { Id = "someid" };
            string id = expectedResponse.Id;
            ILogger log = _mockLogger.Object;

            _mockItemResponse.Setup(x => x.StatusCode).Returns(HttpStatusCode.BadRequest);
            _mockContainer.Setup(x => x.ReadItemAsync<License.Models.License>(It.Is<string>(x => x == id), It.IsAny<PartitionKey>(), null, It.IsAny<CancellationToken>())).ReturnsAsync(_mockItemResponse.Object);

            SetUpLogger();
            // Act
            var result = await licenseFunction.Get(req, id, log) as StatusCodeResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task GetAll_Success()
        {
            // Arrange
            var licenseFunction = this.CreateLicenseFunction();
            Mock<FeedIterator<License.Models.License>> mockFeed = mockRepository.Create<FeedIterator<License.Models.License>>();
            List<License.Models.License> expectedResult = new List<License.Models.License>();
            expectedResult.Add(new License.Models.License() { Id="1" });
            expectedResult.Add(new License.Models.License() { Id = "2" });

            FakeResponse fakeResponse = new FakeResponse(expectedResult);

            mockFeed.Setup(x => x.ReadNextAsync(It.IsAny<System.Threading.CancellationToken>())).ReturnsAsync(fakeResponse);
            _mockContainer.Setup(x => x.GetItemQueryIterator<License.Models.License>(It.IsAny<string>(), null, null)).Returns(mockFeed.Object);
            int firstTime = 0;
            mockFeed.Setup(x => x.HasMoreResults).Returns(() =>
            {
                if (firstTime == 0)
                {
                    firstTime = 1;
                    return true;
                }
                return false;
            });
            
            HttpRequest req = null;
            ILogger log = null;

            // Act
            var result = await licenseFunction.GetAll(  req, log) as OkObjectResult;

            // Assert
            Assert.NotNull(false);
            Assert.Equal(expectedResult.Count, ((List<License.Models.License>)result.Value).Count);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task Update_Success()
        {
            // Arrange
            var licenseFunction = this.CreateLicenseFunction();
            License.Models.License license = new License.Models.License() { Partitionkey= "DynamicTemplate", Id="test" };
            ILogger log = null;
            string id = Guid.NewGuid().ToString();

            var input = license;
            _mockItemResponse.Setup(x => x.StatusCode).Returns(System.Net.HttpStatusCode.OK);
            _mockContainer.Setup(x => x.UpsertItemAsync<License.Models.License>(It.Is<License.Models.License>(x => x.Id == license.Id && x.Partitionkey == "DynamicTemplate"), It.IsAny<PartitionKey>(),null, It.IsAny<System.Threading.CancellationToken>())).ReturnsAsync(_mockItemResponse.Object);

            // Act
            var result = await licenseFunction.Update(license,log)  as StatusCodeResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            this.mockRepository.VerifyAll();
        }        

        private void SetUpLogger()
        {
            _mockLogger.Setup(
                  x => x.Log(
                      LogLevel.Error,
                      It.IsAny<EventId>(),
                      It.IsAny<It.IsAnyType>(),
                      It.IsAny<Exception>(),
                      It.IsAny<Func<It.IsAnyType, Exception, string>>()));
        }

        private class FakeResponse : FeedResponse<License.Models.License>
        {
            private IEnumerable<License.Models.License> licenseDocuments;

            public FakeResponse(IEnumerable<License.Models.License> licenseDocuments)
            {
                this.licenseDocuments = licenseDocuments;
            }

            public override string ContinuationToken => null;

            public override int Count => licenseDocuments.Count();

            public override string IndexMetrics => null;

            public override Headers Headers => null;

            public override IEnumerable<License.Models.License> Resource => licenseDocuments;

            public override HttpStatusCode StatusCode => HttpStatusCode.OK;

            public override CosmosDiagnostics Diagnostics => null;

            public override IEnumerator<License.Models.License> GetEnumerator()
            {
                return licenseDocuments.GetEnumerator();
            }
        }
    }
}
