using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Documents_backend.Utility
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
    }
}