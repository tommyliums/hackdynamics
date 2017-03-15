using Hynamick.QnA.Core.DomainModels;
using Hynamick.QnA.Core.Interfaces;
using Hynamick.QnA.Infrastructure.ElasticSearch.Exceptions;
using Hynamick.QnA.Infrastructure.ElasticSearch.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace Hynamick.QnA.Infrastructure.ElasticSearch
{
    public class QnAProviderElasticSearchImpl : IQnAProvider
    {
        public QnAProviderElasticSearchImpl(ElasticSearchEnvInfo envInfo)
        {
            this.envInfo = envInfo;
        }

        public AnswerCollection Answer(Question question)
        {
            string queryString = $"source={this.envInfo.SearchSource}&locale={question.Locale}&query={HttpUtility.UrlEncode(question.QuestionText)}&count={question.ResultCount}";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create($"{this.envInfo.SearchUrl}?{queryString}");
            request.Method = "GET";

            ElasticSearchResponse response = null;

            try
            {
                using (HttpWebResponse webResponse = request.GetResponse() as HttpWebResponse)
                {
                    using (StreamReader reader = new StreamReader(webResponse.GetResponseStream()))
                    {
                        string responseString = reader.ReadToEnd();

                        response = JsonConvert.DeserializeObject<ElasticSearchResponse>(responseString);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ElasticSearchException(
                    this.envInfo.SearchUrl,
                    this.envInfo.SearchSource,
                    question.QuestionText,
                    question.ResultCount,
                    question.Locale,
                    "Failed to get answers from Elastic Search.",
                    ex);
            }

            if (response == null)
            {
                throw new ElasticSearchException(
                    this.envInfo.SearchUrl,
                    this.envInfo.SearchSource,
                    question.QuestionText,
                    question.ResultCount,
                    question.Locale,
                    "Elastic Search returns null.",
                    null);
            }

            if (response.Error != null)
            {
                StringBuilder message = new StringBuilder();
                message.AppendLine("Elastic Search returns error.");
                message.AppendLine($"  Error code: {response.Error.Code}");
                message.AppendLine($"  Error message: {response.Error.Message}");

                throw new ElasticSearchException(
                    this.envInfo.SearchUrl,
                    this.envInfo.SearchSource,
                    question.QuestionText,
                    question.ResultCount,
                    question.Locale,
                    message.ToString(),
                    null);
            }

            List<Answer> answers = new List<Core.DomainModels.Answer>();
            foreach (ElasticSearchResult result in response.Results)
            {
                answers.Add(new Answer()
                {
                    Question = result.Question,
                    AnswerText = result.Answer,
                    Type = result.Type,
                    Url = result.Url
                });
            }

            return new AnswerCollection()
            {
                TotalCount = response.TotalCount,
                ResultCount = response.ResultCount,
                Answers = answers
            };
        }

        private ElasticSearchEnvInfo envInfo = null;
    }
}
