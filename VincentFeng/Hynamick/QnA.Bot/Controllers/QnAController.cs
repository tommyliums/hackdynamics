using Hynamick.QnA.Core.DomainModels;
using Hynamick.QnA.Core.Interfaces;
using Hynamick.QnA.Infrastructure.Mock;
using Microsoft.Bot.Connector;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Hynamick.QnA.Infrastructure.QnAMaker;
using Hynamick.QnA.Infrastructure.QnAMaker.Models;
using System.Configuration;

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
            QnAMakerEnvInfo qnaMakerEnvInfo = new QnAMakerEnvInfo()
            {
                ApiUrl = ConfigurationManager.AppSettings["QnAMakerApiUrl"],
                OcpApimSubscriptionKey = ConfigurationManager.AppSettings["QnAMakerOcpApimSubscriptionKey"],
                Encoding = ConfigurationManager.AppSettings["QnAMakerEncoding"]
            };

            this.qnaProvider = new QnAProviderQnAMakerImpl(qnaMakerEnvInfo);
            this.answerFormatter = new MockAnswerFormatter();
        }

        [Route("")]
        [HttpPost()]
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, new ArgumentNullException("activity"));
            }

            if (string.IsNullOrWhiteSpace(activity.Text))
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, new ArgumentNullException("activity.Text"));
            }

            try
            {
                Question question = new Question()
                {
                    Text = activity.Text
                };
                Answer answer = this.qnaProvider.Answer(question);

                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));

                Activity reply = activity.CreateReply(this.answerFormatter.Format(answer));
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
    }
}