
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
using System.Collections.Generic;

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
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] CreateLicenseRequest req, ILogger log)
        {

            var newLicense = new Models.License() { Id = Guid.NewGuid().ToString(), CompanyName = req.CompanyName, DomainName = req.DomainName, CustomerId = req.CustomerId };
            var result = await container.CreateItemAsync(newLicense);

            if (result.StatusCode == HttpStatusCode.Created)
                return new OkObjectResult(new CreateLicenseResponse() { LicenseId = newLicense.Id });
            else
            {
                log.LogError($"Create request for returned: {result.StatusCode}");
                return new StatusCodeResult((int)result.StatusCode);
            }

        }


        [FunctionName("GetLicense")]
        [OpenApiOperation(operationId: "Get")]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "LicenseId")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Models.License), Description = "The OK response")]
        public async Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, "Get", Route = "License/{id}")] HttpRequest req, string id, ILogger log)
        {

            var result = await container.ReadItemAsync<Models.License>(id.ToString(), new PartitionKey("DynamicTemplate"));

            if (result.StatusCode == HttpStatusCode.OK)
                return new OkObjectResult(result.Resource);
            else
            {
                log.LogError($"Get request for id {id} returned: {result.StatusCode}");
                return new StatusCodeResult((int)result.StatusCode);
            }

        }

        [FunctionName("GetLicenses")]
        [OpenApiOperation(operationId: "GetAll")]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<Models.License>), Description = "The OK response")]
        public async Task<IActionResult> GetAll(
           [HttpTrigger(AuthorizationLevel.Anonymous, "Get", Route = "License")] HttpRequest req, ILogger log)
        {
            var feedIterator = container.GetItemQueryIterator<Models.License>();

            var results = new List<Models.License>();
            while (feedIterator.HasMoreResults)
            {
                results.AddRange(await feedIterator.ReadNextAsync());
            }

            return new OkObjectResult(results);
        }

        [FunctionName("UpdateLicense")]
        [OpenApiOperation(operationId: "Update")]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiRequestBody("application/json", typeof(Models.License))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(OkResult), Description = "The OK response")]
        public async Task<IActionResult> Update(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "License")] Models.License license, ILogger log)
        {

            var result = await container.UpsertItemAsync<Models.License>(license, new PartitionKey("DynamicTemplate"));
            if ((int)result.StatusCode > 299)
                log.LogError($"Update request returned: {result.StatusCode}");

            return new StatusCodeResult((int)result.StatusCode);
        }

        [FunctionName("DeleteLicense")]
        [OpenApiOperation(operationId: "Delete")]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiRequestBody("application/json", typeof(Models.License))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(OkResult), Description = "The OK response")]
        public async Task<IActionResult> Delete(
           [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "License/{id}")] HttpRequest req, string id, ILogger log)
        {

            var result = await container.DeleteItemAsync<Models.License>(id, new PartitionKey("DynamicTemplate"));
            if ((int)result.StatusCode > 299)
                log.LogError($"Delete request returned: {result.StatusCode}");

            return new StatusCodeResult((int)result.StatusCode);
        }
    }
}

