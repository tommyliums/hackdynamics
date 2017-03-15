using Hynamick.QnA.Core.DomainModels;
using Hynamick.QnA.Core.Interfaces;
using Hynamick.QnA.Infrastructure.ElasticSearch;
using Hynamick.QnA.Infrastructure.ElasticSearch.Models;
using Microsoft.Bot.Connector;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Hynamick.QnA.Bot.Controllers
{
    [BotAuthentication()]
    [RoutePrefix("api/qna")]
    public class QnAController : ApiController
    {
        public QnAController(IQnAProvider qnaProvider, IAnswerFormatter answerFormatter)
        {
            this.qnaProvider = qnaProvider;
            this.answerFormatter = answerFormatter;
        }

        public QnAController()
        {
            ElasticSearchEnvInfo envInfo = new ElasticSearchEnvInfo()
            {
                SearchUrl = "http://hynamick-qa.chinacloudapp.cn/api/search",
                SearchSource = "hwfaq"
            };

            this.qnaProvider = new QnAProviderElasticSearchImpl(envInfo);
            this.answerFormatter = new SimpleAnswerFormatter();
        }

        [Route("")]
        [HttpPost()]
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, new ArgumentNullException("activity"));
            }

            if (activity.Type != ActivityTypes.Message)
            {
                return Request.CreateResponse(HttpStatusCode.OK, activity.Type.ToString());
            }

            if (string.IsNullOrWhiteSpace(activity.Text))
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, new ArgumentNullException("activity.Text"));
            }

            try
            {
                Question question = new Question()
                {
                    QuestionText = activity.Text,
                    ResultCount = 3,
                    Locale = "en-us"
                };
                AnswerCollection answers = this.qnaProvider.Answer(question);

                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));

                Activity reply = activity.CreateReply(this.answerFormatter.Format(answers));
                await connector.Conversations.ReplyToActivityAsync(reply);

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        private IQnAProvider qnaProvider = null;
        private IAnswerFormatter answerFormatter = null;

        private class SimpleAnswerFormatter : IAnswerFormatter
        {
            public string Format(AnswerCollection answers)
            {
                StringBuilder result = new StringBuilder();

                result.AppendLine($"Total answer count: {answers.TotalCount}");
                result.AppendLine($"Result answer count: {answers.ResultCount}");
                result.AppendLine("Answers:");

                for (int ix = 0; ix < answers.Answers.Count(); ix++)
                {
                    Answer answer = answers.Answers.ElementAt(ix);
                    result.AppendLine($"  Answer {ix}");
                    result.AppendLine($"    Question: {answer.Question}");
                    result.AppendLine($"    Answer: {answer.AnswerText}");
                    result.AppendLine($"    Type: {answer.Type}");
                    result.AppendLine($"    Reference: {answer.Url}");
                }

                return result.ToString();
            }
        }
    }
}