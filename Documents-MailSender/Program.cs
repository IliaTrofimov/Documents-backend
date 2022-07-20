using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Documents_MailSender
{
    internal class Program
    {
        static async Task<int> Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.Error.WriteLine("Необходимо предоставить метод и url в качестве параметров");
                return -1;
            }

            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage() { RequestUri = new Uri(args[1])};
            switch (args[0])
            {
                case "GET": request.Method = HttpMethod.Get; break;
                case "PUT": request.Method = HttpMethod.Put; break;
                case "POST": request.Method = HttpMethod.Post; break;
                default:
                    Console.Error.WriteLine($"Недопустимый HTTP метод '{args[0]}', доступны GET, PUT, POST");
                    return -1;
            }

            try
            {
                var result = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead);
                string message = (await result.Content.ReadAsStringAsync()).Replace("{", "").Replace("}", "");

                if (result.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    Console.Error.WriteLine($"Произошла ошибка со стороны сервера:\n{message}");
                    return -1;
                }
                else
                {
                    Console.WriteLine($"Успешно: {message}");
                    return 1;
                }
                
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Неожиданная ошибка:\n{ex}");
                return -1;
            }
        }
    }
}
