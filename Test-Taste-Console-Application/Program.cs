using System;
using System.IO;
using System.Reflection;
using log4net;
using log4net.Config;
using Microsoft.Extensions.DependencyInjection;
using Test_Taste_Console_Application.Constants;
using Test_Taste_Console_Application.Domain.Services;
using Test_Taste_Console_Application.Domain.Services.Interfaces;
using Test_Taste_Console_Application.Utilities;

namespace Test_Taste_Console_Application
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            //The ConfigureServices function configures the services.
            ConfigureServices(serviceCollection);
            
            //The RunServiceOperations function executes the code that can create the outputs.
            RunServiceOperations(serviceCollection);
        }

        private static void RunServiceOperations(IServiceCollection serviceCollection)
        {
            var serviceProvider = serviceCollection.BuildServiceProvider();

            //The service provider gets the services.
            var screenOutputService = serviceProvider.GetService<IOutputService>();
            var fileOutputService = serviceProvider.GetService<IFileOutputService>();

            try
            {
                Console.WriteLine(OutputString.LoadingData);
                Console.WriteLine(OutputString.ProcessingBodies);
                Console.WriteLine(OutputString.IdentifyingPlanetsWithMoons);
                Console.WriteLine(OutputString.CalculatingAverageMoonTemperatures);
                ConsoleWriter.CreateEmptyLines(1);

                Console.WriteLine(OutputString.WritingDataToConsole);
                ConsoleWriter.CreateEmptyLines(1);

                // Output average moon temperature (new requirement)
                screenOutputService.OutputAllPlanetsAndTheirAverageMoonTemperatureToConsole();

                // Existing outputs preserved
                screenOutputService.OutputAllPlanetsAndTheirAverageMoonGravityToConsole();
                screenOutputService.OutputAllMoonsAndTheirMassToConsole();
                screenOutputService.OutputAllPlanetsAndTheirMoonsToConsole();

                Console.WriteLine(OutputString.WritingDataToFile);
                fileOutputService.OutputAllPlanetsAndTheirAverageMoonTemperatureToFile();
                fileOutputService.OutputAllPlanetsAndTheirMoonsToFile();
                fileOutputService.OutputAllMoonsAndTheirMassToFile();
                ConsoleWriter.CreateEmptyLines(1);

                Console.WriteLine(OutputString.CompletedSuccessfully);
            }
            catch (Exception exception)
            {
                //The users and developers can see the thrown exceptions.
                Logger.Instance.Error($"{LoggerMessage.ScreenOutputOperationFailed}{exception.Message}");
                Console.WriteLine($"{ExceptionMessage.ScreenOutputOperationFailed}{exception.Message}");
                System.Diagnostics.Debug.WriteLine($"{ExceptionMessage.ScreenOutputOperationFailed}{exception.Message}");
            }

            serviceProvider.Dispose();
        }

        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            //The function configures all the services.
            XmlConfigurator.Configure(LogManager.GetRepository(Assembly.GetEntryAssembly()),
                new FileInfo(ConfigurationFileName.Logger));
            serviceCollection.AddHttpClient<HttpClientService>();
            serviceCollection.AddSingleton<IPlanetService, PlanetService>();
            serviceCollection.AddSingleton<IOutputService, ScreenOutputService>();
            serviceCollection.AddSingleton<IFileOutputService, FileOutputService>();
            serviceCollection.AddSingleton<IMoonService, MoonService>();
        }
    }
}
