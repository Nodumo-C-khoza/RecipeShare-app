using System.Net.Http.Json;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using RecipeShare.Controllers;
using RecipeShare.Models;

namespace RecipeShareBenchmarks
{
    [Config(typeof(BenchmarkConfig))]
    [MemoryDiagnoser]
    public class RecipeApiBenchmarks
    {
        private HttpClient _client;
        public const string BaseUrl = "http://localhost:5229"; // Update this to match your API's URL

        [GlobalSetup]
        public void Setup()
        {
            var handler = new HttpClientHandler()
            {
                // For local development with self-signed certificates
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };

            _client = new HttpClient(handler)
            {
                BaseAddress = new Uri(BaseUrl),
                Timeout = TimeSpan.FromSeconds(30) // Add timeout
            };
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            _client?.Dispose();
        }

        [Benchmark]
        [Arguments(100)]
        [Arguments(500)]
        [Arguments(1000)]
        [BenchmarkCategory("Sequential")]
        public async Task GetRecipes_Sequential(int callCount)
        {
            var successCount = 0;
            var errorCount = 0;

            for (int i = 0; i < callCount; i++)
            {
                try
                {
                    var response = await _client.GetAsync("/api/recipes/getrecipes");
                    response.EnsureSuccessStatusCode();
                    var result = await response.Content.ReadFromJsonAsync<
                        PaginatedResult<RecipeListViewModel>
                    >();
                    successCount++;
                }
                catch (HttpRequestException httpEx)
                {
                    Console.WriteLine($"HTTP Error in iteration {i}: {httpEx.Message}");
                    errorCount++;
                }
                catch (TaskCanceledException tcEx)
                {
                    Console.WriteLine($"Timeout in iteration {i}: {tcEx.Message}");
                    errorCount++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in iteration {i}: {ex.Message}");
                    errorCount++;
                }
            }

            Console.WriteLine(
                $"Completed {callCount} calls: {successCount} successful, {errorCount} errors"
            );
        }

        [Benchmark]
        [Arguments(100)]
        [Arguments(500)]
        [BenchmarkCategory("Parallel")]
        public async Task GetRecipes_Parallel(int callCount)
        {
            var tasks = new List<Task<bool>>();
            var semaphore = new SemaphoreSlim(50, 50); // Limit concurrent requests

            for (int i = 0; i < callCount; i++)
            {
                tasks.Add(MakeRequest(i, semaphore));
            }

            var results = await Task.WhenAll(tasks);
            var successCount = results.Count(r => r);
            var errorCount = results.Count(r => !r);

            Console.WriteLine(
                $"Parallel {callCount} calls: {successCount} successful, {errorCount} errors"
            );
        }

        private async Task<bool> MakeRequest(int iteration, SemaphoreSlim semaphore)
        {
            await semaphore.WaitAsync();
            try
            {
                var response = await _client.GetAsync("/api/recipes/getrecipes");
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<
                    PaginatedResult<RecipeListViewModel>
                >();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in parallel iteration {iteration}: {ex.Message}");
                return false;
            }
            finally
            {
                semaphore.Release();
            }
        }

        [Benchmark]
        [BenchmarkCategory("Single Call")]
        public async Task GetRecipes_Single_Call()
        {
            var response = await _client.GetAsync("/api/recipes/getrecipes");
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<
                PaginatedResult<RecipeListViewModel>
            >();
        }

        [Benchmark]
        [BenchmarkCategory("With Parameters")]
        public async Task GetRecipes_With_Parameters()
        {
            // Test with query parameters if your API supports them
            var response = await _client.GetAsync("/api/recipes/getrecipes?page=1&pageSize=10");
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<
                PaginatedResult<RecipeListViewModel>
            >();
        }
    }

    public class BenchmarkConfig : ManualConfig
    {
        public BenchmarkConfig()
        {
            // Use default configuration
            AddJob(Job.Default);

            // Add useful columns
            AddColumn(StatisticColumn.Mean);
            AddColumn(StatisticColumn.StdDev);
            AddColumn(StatisticColumn.Min);
            AddColumn(StatisticColumn.Max);
            AddColumn(StatisticColumn.P95);
            AddColumn(StatisticColumn.OperationsPerSecond);
            AddColumn(TargetMethodColumn.Method); // Updated to use TargetMethodColumn

            // Add exporters for results
            AddExporter(MarkdownExporter.GitHub);
            AddExporter(HtmlExporter.Default);

            // Add console logger
            AddLogger(ConsoleLogger.Default);
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Starting Recipe API Benchmarks...");
                Console.WriteLine($"Testing against: {RecipeApiBenchmarks.BaseUrl}");
                Console.WriteLine("Make sure your API is running before starting the benchmark!");

                var config = new BenchmarkConfig();
                var summary = BenchmarkRunner.Run<RecipeApiBenchmarks>(config);

                // Print summary information
                Console.WriteLine("\nBenchmark completed successfully!");
                Console.WriteLine($"Results saved to: {config.ArtifactsPath}");

                // Print the summary table
                Console.WriteLine("\nSummary:");
                Console.WriteLine(summary.Table);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Benchmark failed: {ex}");
                Console.WriteLine(ex.StackTrace);
                Environment.Exit(1);
            }
        }
    }
}
