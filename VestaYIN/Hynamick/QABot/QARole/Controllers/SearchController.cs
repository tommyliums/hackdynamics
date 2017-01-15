// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SearchController.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//     Defines the SearchController.cs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace QARole.Controllers
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;

    using Hynamick.Search.SearchAnswer;

    using Microsoft.Azure;

    using Swashbuckle.Swagger.Annotations;

    public class SearchController : ApiController
    {
        public static readonly SearchHandler Handler = InitializeSearchHandler();

        private static SearchHandler InitializeSearchHandler()
        {
            var Client = new HttpClient();
            var serviceUrl = CloudConfigurationManager.GetSetting("SearchUrl");
            var localeMappingPath = Path.Combine(AppDomain.CurrentDomain.RelativeSearchPath,
                CloudConfigurationManager.GetSetting("LocaleMappingPath"));
            var searchTemplatePath = Path.Combine(AppDomain.CurrentDomain.RelativeSearchPath,
                CloudConfigurationManager.GetSetting("SearchTemplateFile"));
            var transformFilePath = Path.Combine(AppDomain.CurrentDomain.RelativeSearchPath,
                CloudConfigurationManager.GetSetting("TransformFilePath"));
            return new SearchHandler(Client, serviceUrl, localeMappingPath, searchTemplatePath, transformFilePath);
        }

        // GET api/values
        [SwaggerOperation("Get")]
        public async Task<SearchResponse> Get(string source, string locale, string query, int count)
        {
            return await Handler.SearchAsync(source, locale, query, count);
        }
        /*
        // GET api/values/5
        [SwaggerOperation("GetById")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [SwaggerOperation("Create")]
        [SwaggerResponse(HttpStatusCode.Created)]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [SwaggerOperation("Update")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [SwaggerOperation("Delete")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public void Delete(int id)
        {
        }
        */
    }
}