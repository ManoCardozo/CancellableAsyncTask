using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var downloader = new ResourceDownloader();
            var tokenSource = new CancellationTokenSource();

            Console.WriteLine("A background task has started");

            var readKeyTask = Task.Run(() =>
            {
                Console.WriteLine("Enter any key to cancel it...");
                Console.ReadKey();
                Console.WriteLine();

                tokenSource.Cancel();
            });

            try
            {
                Task<long?> lengthTask = downloader.AggregateContentLength(tokenSource.Token);

                Console.WriteLine($"Total content length: { lengthTask.Result }");
                Console.WriteLine("Task completed. Enter any key to continue...");
            }
            catch (AggregateException ae)
            {
                foreach (var exception in ae.InnerExceptions)
                {
                    if (exception is TaskCanceledException)
                    {
                        Console.WriteLine("Task was cancelled. Enter any key to continue...");
                        Console.ReadKey();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            readKeyTask.Wait();
        }
    }
}
