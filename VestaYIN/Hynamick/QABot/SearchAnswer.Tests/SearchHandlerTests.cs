using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Hynamick.SearchAnswer;
using System.IO;
using Newtonsoft.Json;

namespace SearchAnswer.Tests
{
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
            var handler = new SearchHandler();
            var responseContent = File.ReadAllText(@".\TestFiles\SourceResponses\QA_response.json");
            var response = Vesta.Common.TestTools.UnitTesting.PrivateHelper.InvokePrivateMethod(handler, "ParseResponse", responseContent) as SearchResponse;

            Assert.IsNotNull(response);
            File.WriteAllText(@".\TestFiles\ExprectedResults\QA_response_Expected.json.Actual.json", JsonConvert.SerializeObject(response, Formatting.Indented));
        }

        [Owner("vestayin")]
        [TestMethod]
        [DeploymentItem(@".\TestFiles\SourceResponses\QA_responseError.json", @".\TestFiles\SourceResponses")]
        [DeploymentItem(@".\TestFiles\ExprectedResults\QA_responseError_Expected.json", @".\TestFiles\ExprectedResults")]
        public void SearchHandler_TransformSearchError()
        {
            var handler = new SearchHandler();
            var errorContent = File.ReadAllText(@".\TestFiles\SourceResponses\QA_responseError.json");
            var error = Vesta.Common.TestTools.UnitTesting.PrivateHelper.InvokePrivateMethod(handler, "ParseResponse", errorContent) as SearchResponse;

            Assert.IsNotNull(error);
            File.WriteAllText(@".\TestFiles\ExprectedResults\QA_responseError_Expected.json.Actual.json", JsonConvert.SerializeObject(error, Formatting.Indented));
        }
    }
}
