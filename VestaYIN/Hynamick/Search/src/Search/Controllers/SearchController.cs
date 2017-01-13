// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SearchController.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//    Defines the SearchController.cs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Hynamick.Search.Controllers
{
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Hynamick.Search.Configurations;
    using Hynamick.Search.SearchAnswer;

    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;

    [Route("api/[controller]")]
    public class SearchController : Controller
    {
        private ISearchHandler searchHandler;

        private static readonly HttpClient ServiceHttpClient = new HttpClient();

        public SearchController(IHostingEnvironment hostEnvironment, IOptions<ServiceSettings> settingsOptions)
        //, ISearchHandler searchHandler)
        {
            this.searchHandler = new SearchHandler(
                ServiceHttpClient,
                settingsOptions.Value.ServiceUrl,
                Path.Combine(hostEnvironment.ContentRootPath, settingsOptions.Value.SearchTemplatePath),
                Path.Combine(hostEnvironment.ContentRootPath, settingsOptions.Value.SearchTransformFilePath));

            //this.searchHandler = searchHandler;
        }

        // GET api/search
        [HttpGet]
        public async Task<SearchResponse> Search(string source, string market, string query, int count)
        {
            var searchResponse = await this.searchHandler.Search(source, market, query, count);
            return searchResponse;
        }

        // GET api/get/id
        [HttpGet("{id}")]
        public async Task<SearchResult> Get(string source, string market, int id)
        {
            throw new NotImplementedException();
            ////var searchResult = await this.searchHandler.GetResult(source, market, id);
            ////return searchResult;
        }
    }
}