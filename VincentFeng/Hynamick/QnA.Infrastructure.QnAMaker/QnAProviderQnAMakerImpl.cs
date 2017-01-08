using Hynamick.QnA.Core.DomainModels;
using Hynamick.QnA.Core.Interfaces;
using Hynamick.QnA.Infrastructure.QnAMaker.Exceptions;
using Hynamick.QnA.Infrastructure.QnAMaker.Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace Hynamick.QnA.Infrastructure.QnAMaker
{
    public class QnAProviderQnAMakerImpl : IQnAProvider
    {
        public QnAProviderQnAMakerImpl(QnAMakerEnvInfo envInfo)
        {
            this.envInfo = envInfo;
        }

        public Answer Answer(Question question)
        {
            QnAMakerQuestion qnaQuestion = new QnAMakerQuestion()
            {
                Question = question.Text
            };

            QnAMakerAnswer qnaAnswer = this.DoGetAnswer(qnaQuestion);

            return new Answer()
            {
                Text = qnaAnswer.Answer
            };
        }

        private QnAMakerAnswer DoGetAnswer(QnAMakerQuestion question)
        {
            HttpWebRequest request = this.CreateRequest(question);

            try
            {
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        return JsonConvert.DeserializeObject<QnAMakerAnswer>(reader.ReadToEnd());
                    }
                }
            }
            catch (Exception ex)
            {
                throw new QnAMakerApiException(
                    question.Question,
                    this.envInfo.ApiUrl,
                    this.envInfo.OcpApimSubscriptionKey,
                    this.envInfo.Encoding,
                    HttpPostMethod,
                    "Failed to get answer from QnAMaker api.",
                    ex);
            }
        }

        private HttpWebRequest CreateRequest(QnAMakerQuestion question)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(this.envInfo.ApiUrl);
            request.Method = HttpPostMethod;
            request.ContentType = ApplicationJsonContentType;
            request.Headers.Add(OcpApimSubscriptionKeyHeaderName, this.envInfo.OcpApimSubscriptionKey);

            string data = JsonConvert.SerializeObject(question);
            byte[] dataBytes = Encoding.GetEncoding(this.envInfo.Encoding).GetBytes(data);
            request.ContentLength = dataBytes.Length;
            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(dataBytes, 0, dataBytes.Length);
            }

            return request;
        }

        private QnAMakerEnvInfo envInfo = null;

        private const string HttpPostMethod = "POST";

        private const string ContentTypeHeaderName = "Content-Type";
        private const string OcpApimSubscriptionKeyHeaderName = "Ocp-Apim-Subscription-Key";

        private const string ApplicationJsonContentType = "application/json";
    }
}
