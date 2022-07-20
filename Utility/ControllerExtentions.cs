using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Documents.Utility
{
    public static class ControllerExtentions
    {
        /// <summary>
        /// Throws HttpResponse exception with given message and code.
        /// </summary>
        /// <exception cref="HttpResponseException"></exception>
        public static void ThrowResponseException(this ApiController controller, HttpStatusCode statusCode, string message)
        {
            var errorResponse = controller.Request.CreateErrorResponse(statusCode, message);
            throw new HttpResponseException(errorResponse);
        }

        public static IHttpActionResult HttpResponse(this ApiController controller, HttpStatusCode statusCode, string message)
        {
            return new HttpResult(statusCode, controller.Request, message);
        }

        public static IHttpActionResult HttpResponse(this ApiController controller, HttpStatusCode statusCode)
        {
            return new HttpResult(statusCode, controller.Request);
        }
    }
}