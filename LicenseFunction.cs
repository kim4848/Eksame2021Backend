using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using License.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace License.Function
{
    public class LicenseFunction
    {
        private readonly Container container;

        public LicenseFunction(Container container)
        {
            this.container = container;
        }
        [FunctionName("AddLicense")]
        [OpenApiOperation(operationId: "Create")]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiRequestBody("application/json", typeof(CreateLicenseRequest))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(CreateLicenseResponse), Description = "The OK response")]
        public async Task<IActionResult> Create(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] CreateLicenseRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            var newLicense = new Models.License() { LicenseId = Guid.NewGuid().ToString(), CompanyName = req.CompanyName, DomainName = req.DomainName, CustomerId = req.CustomerId };
            await container.CreateItemAsync(newLicense);

            return new OkObjectResult(new CreateLicenseResponse() { LicenseId = newLicense.LicenseId });
        }

        [FunctionName("GetLicense")]
        [OpenApiOperation(operationId: "Get")]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "LicenseId")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Models.License), Description = "The OK response")]
        public async Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Function, "Get", Route = "License/{id}")] HttpRequest req, string id,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var result = await container.ReadItemAsync<Models.License>(id.ToString(), new PartitionKey("DynamicTemplate"));

            return new OkObjectResult(result.Resource);
        }

        [FunctionName("UpdateLicense")]
        [OpenApiOperation(operationId: "Get")]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiRequestBody("application/json", typeof(Models.License))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(OkResult), Description = "The OK response")]
        public async Task<IActionResult> Update(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "License/{id}")] Models.License license,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var result = await container.UpsertItemAsync<Models.License>(license, new PartitionKey("DynamicTemplate"));

            if (result.StatusCode == HttpStatusCode.OK)
            {
                return new OkResult();
            }
            else
            {
                return new BadRequestObjectResult(result.StatusCode.ToString());
            }


        }
    }
}

