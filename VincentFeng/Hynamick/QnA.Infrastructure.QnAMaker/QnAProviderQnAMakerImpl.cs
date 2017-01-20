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
            QnAMakerRequest request = new QnAMakerRequest()
            {
                Question = question.Text,
                ApiUrl = this.envInfo.ApiUrl,
                Method = HttpPostMethod,
                ContentType = ApplicationJsonContentType,
                Encoding = this.envInfo.Encoding,
                OcpApimSubscriptionKey = this.envInfo.OcpApimSubscriptionKey
            };

            QnAMakerResponse response = this.DoGetAnswer(request);

            return new Answer()
            {
                Text = response.Answer,
                Relevance = response.Score
            };
        }

        private QnAMakerResponse DoGetAnswer(QnAMakerRequest request)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create();
            webRequest.Method = HttpPostMethod;
            webRequest.ContentType = ApplicationJsonContentType;
            webRequest.Headers.Add(OcpApimSubscriptionKeyHeaderName, this.envInfo.OcpApimSubscriptionKey);

            string data = JsonConvert.SerializeObject(question);
            byte[] dataBytes = Encoding.GetEncoding(this.envInfo.Encoding).GetBytes(data);
            webRequest.ContentLength = dataBytes.Length;
            using (Stream stream = webRequest.GetRequestStream())
            {
                stream.Write(dataBytes, 0, dataBytes.Length);
            }

            try
            {
                using (HttpWebResponse webResponse = webRequest.GetResponse() as HttpWebResponse)
                {
                    using (StreamReader reader = new StreamReader(webResponse.GetResponseStream()))
                    {
                        return JsonConvert.DeserializeObject<QnAMakerResponse>(reader.ReadToEnd());
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

        private QnAMakerEnvInfo envInfo = null;

        private const string HttpPostMethod = "POST";

        private const string ContentTypeHeaderName = "Content-Type";
        private const string OcpApimSubscriptionKeyHeaderName = "Ocp-Apim-Subscription-Key";

        private const string ApplicationJsonContentType = "application/json";
    }
}
