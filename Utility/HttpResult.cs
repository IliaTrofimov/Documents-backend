using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace Documents.Utility
{
    internal class HttpResult : IHttpActionResult
    {
        protected HttpRequestMessage request = null;
        public string Message { get; set; } = null;
        public HttpStatusCode Status { get; set; }


        public HttpResult(HttpStatusCode status)
        {
            Status = status;
        }

        public HttpResult(HttpStatusCode status, HttpRequestMessage request)
        {
            Status = status;
            this.request = request;
        }

        public HttpResult(HttpStatusCode status, HttpRequestMessage request, string message)
        {
            Status = status;
            Message = message;
            this.request = request;
        }


        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage()
            {
                StatusCode = Status,
                Content = new StringContent(Message),
                RequestMessage = request
            };
            return Task.FromResult(response);
        }
    }
}
