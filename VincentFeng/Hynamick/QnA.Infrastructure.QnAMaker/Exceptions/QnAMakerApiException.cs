using System;
using System.Text;

namespace Hynamick.QnA.Infrastructure.QnAMaker.Exceptions
{
    public class QnAMakerApiException : Exception
    {
        public QnAMakerApiException(
            string question,
            string apiUrl,
            string ocpApimSubscriptionKey,
            string encoding,
            string method,
            string message,
            Exception innerException)
            : base(message, innerException)
        {
            this.Question = question;
            this.ApiUrl = apiUrl;
            this.OcpApimSubscriptionKey = ocpApimSubscriptionKey;
            this.Encoding = encoding;
            this.Method = method;
        }

        public override string ToString()
        {
            StringBuilder message = new StringBuilder();

            message.AppendLine("Error in calling QnAMaker api.");
            message.AppendLine($"  Error message: {this.Message}");
            message.AppendLine($"  Question: {this.Question}");
            message.AppendLine($"  Api url: {this.ApiUrl}");
            message.AppendLine($"  Ocp apim subscription key: {this.OcpApimSubscriptionKey}");
            message.AppendLine($"  Encoding: {this.Encoding}");
            message.AppendLine($"  Method: {this.Method}");

            Exception innerEx = this.InnerException;
            while (innerEx != null)
            {
                message.AppendLine("----------Inner Exception----------");
                message.AppendLine($"  Error message: {innerEx.Message}");
                message.AppendLine($"  Stack trace: {innerEx.StackTrace}");

                innerEx = innerEx.InnerException;
            }

            return message.ToString();
        }

        public string Question
        {
            get;
            private set;
        }

        public string ApiUrl
        {
            get;
            private set;
        }

        public string OcpApimSubscriptionKey
        {
            get;
            private set;
        }

        public string Encoding
        {
            get;
            private set;
        }

        public string Method
        {
            get;
            private set;
        }
    }
}
