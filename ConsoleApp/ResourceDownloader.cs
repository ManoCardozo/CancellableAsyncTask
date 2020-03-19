using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ConsoleApp
{
    class ResourceDownloader
    {
        private readonly HttpClient client;

        public ResourceDownloader()
        {
            client = new HttpClient();
        }

        public async Task<long?> AggregateContentLength(CancellationToken cancellationToken)
        {
            //await Task.Delay(5000, cancellationToken); //Simulates a long running operation

            var tasks = new List<Task<HttpResponseMessage>>
            {
                client.GetAsync("https://reqres.in/api/users?page=1", cancellationToken),
                client.GetAsync("https://jsonplaceholder.typicode.com/todos/1", cancellationToken),
                client.GetAsync("https://reqres.in/api/users?page=3", cancellationToken)
            };

            long? totalLength = 0;
            while (tasks.Count > 0)
            {
                Task<HttpResponseMessage> firstFinishedTask = await Task.WhenAny(tasks);
                tasks.Remove(firstFinishedTask);

                HttpResponseMessage length = await firstFinishedTask;

                totalLength += length.Content.Headers.ContentLength;
            }

            return totalLength;
        }
    }
}
