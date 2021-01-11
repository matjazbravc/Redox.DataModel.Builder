namespace Redox.DataModel.Builder
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Redox.DataModel.Builder.Configuration;
    using Redox.DataModel.Builder.Services;
    using Redox.DataModel.Builder.Services.Helpers;
    using System;

    internal class Program
    {
        public static ILogger<Program> Logger { get; private set; }

        private static void Main(string[] args)
        {
            // Global Exception Handler
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;

            // Add configuration
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true).Build();

            var serviceProvider = new ServiceCollection()
                .AddOptions()
                .AddLogging()
                .AddTransient<IDownloadService, DownloadService>()
                .AddTransient<IModelBuildStatistics, ModelBuildStatistics>()
                .AddTransient<IModelBuildService, ModelBuildService>()
                .AddTransient<ITaskHelper, TaskHelper>()
                // Configure AppConfig
                .Configure<AppConfig>(configuration.GetSection("AppConfig"))
                // Register configuration as Singleton service
                .AddSingleton(provider => provider.GetService<IOptions<AppConfig>>().Value)
                .BuildServiceProvider();

            // Create logger with filters
            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddFilter("Redox.DataModel.Builder.Program", LogLevel.Debug)
                    .AddConsole();
            });

            // Read configuration
            var config = serviceProvider.GetService<AppConfig>();

            Logger = loggerFactory.CreateLogger<Program>();
            Logger.LogInformation($"Start downloading file {config.RedoxDataModelJsonSchemaFileUri} ...");

            var downloadService = serviceProvider.GetService<IDownloadService>();
            var jsonFiles = downloadService.DownloadFileAsync(config.RedoxDataModelJsonSchemaFileUri).Result;

            // Build Data Model files
            var modelBuildService = serviceProvider.GetService<IModelBuildService>();
            modelBuildService.ProcessAsync(jsonFiles).Wait();

            Logger.LogInformation($"Data Model files were sucessfuly created in folder {config.DataModelFolder}. Press <Enter> key for exit.");
            Console.ReadLine();
        }

        internal static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.LogError($"{e.ExceptionObject} \r\nPress <Enter> key for exit.");
            Console.ReadLine();
            Environment.Exit(0);
        }
    }
}
