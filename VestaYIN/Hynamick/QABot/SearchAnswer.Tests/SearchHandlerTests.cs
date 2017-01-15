// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SearchHandlerTests.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//     Defines the SearchHandlerTests.cs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Hynamick.Search.SearchAnswer.Tests
{
    using System.IO;

    using Hynamick.Search.SearchAnswer;
    using Hynamick.SearchAnswer.Tests;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Newtonsoft.Json;

    [TestClass]
    [DeploymentItem(@"Config", @"Config")]
    public class SearchHandlerTests
    {
        [Owner("vestayin")]
        [TestMethod]
        [DeploymentItem(@".\TestFiles\SourceResponses\QA_response.json", @".\TestFiles\SourceResponses")]
        [DeploymentItem(@".\TestFiles\ExprectedResults\QA_response_Expected.json", @".\TestFiles\ExprectedResults")]
        public void SearchHandler_TransformSearchResponse()
        {
            var handler = Constants.GetSearchHandler();
            var responseContent = File.ReadAllText(@".\TestFiles\SourceResponses\QA_response.json");
            var response =
                Vesta.Common.TestTools.UnitTesting.PrivateHelper.InvokePrivateMethod(handler, "ParseResponse",
                    responseContent) as SearchResponse;

            Assert.IsNotNull(response);
            File.WriteAllText(@".\TestFiles\ExprectedResults\QA_response_Expected.json.Actual.json",
                JsonConvert.SerializeObject(response, Formatting.Indented));
        }

        [Owner("vestayin")]
        [TestMethod]
        [DeploymentItem(@".\TestFiles\SourceResponses\QA_responseError.json", @".\TestFiles\SourceResponses")]
        [DeploymentItem(@".\TestFiles\ExprectedResults\QA_responseError_Expected.json", @".\TestFiles\ExprectedResults")
        ]
        public void SearchHandler_TransformSearchError()
        {
            var handler = Constants.GetSearchHandler();
            var errorContent = File.ReadAllText(@".\TestFiles\SourceResponses\QA_responseError.json");
            var error =
                Vesta.Common.TestTools.UnitTesting.PrivateHelper.InvokePrivateMethod(handler, "ParseResponse",
                    errorContent) as SearchResponse;

            Assert.IsNotNull(error);
            File.WriteAllText(@".\TestFiles\ExprectedResults\QA_responseError_Expected.json.Actual.json",
                JsonConvert.SerializeObject(error, Formatting.Indented));
        }
    }
}