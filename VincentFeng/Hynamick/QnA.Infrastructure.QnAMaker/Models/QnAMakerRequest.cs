namespace Hynamick.QnA.Infrastructure.QnAMaker.Models
{
    public class QnAMakerRequest
    {
        public string Question
        {
            get;
            set;
        }

        public string ApiUrl
        {
            get;
            set;
        }

        public string Method
        {
            get;
            set;
        }

        public string ContentType
        {
            get;
            set;
        }

        public string Encoding
        {
            get;
            set;
        }

        public string OcpApimSubscriptionKey
        {
            get;
            set;
        }
    }
}
