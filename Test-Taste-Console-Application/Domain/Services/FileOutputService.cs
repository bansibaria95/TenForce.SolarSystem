using System;
using System.IO;
using System.Linq;
using System.Text;
using Test_Taste_Console_Application.Constants;
using Test_Taste_Console_Application.Domain.Services.Interfaces;
using Test_Taste_Console_Application.Utilities;

namespace Test_Taste_Console_Application.Domain.Services
{
    /// <inheritdoc />
    public class FileOutputService : IFileOutputService
    {
        private readonly IPlanetService _planetService;
        private readonly IMoonService _moonService;

        public FileOutputService(IPlanetService planetService, IMoonService moonService)
        {
            _planetService = planetService;
            _moonService = moonService;
        }

        public void OutputAllPlanetsAndTheirMoonsToFile()
        {
            try
            {
                EnsureOutputFolderExists();
                var planets = _planetService.GetAllPlanets().ToArray();
                var filePath = Path.Combine(PathName.PathToOutputFolder, PathName.AllPlanetsAndTheirMoonsFile);

                var sb = new StringBuilder();
                sb.AppendLine("PlanetNumber,PlanetId,PlanetSemiMajorAxis,TotalMoons,MoonNumber,MoonId");

                for (int i = 0, j = 1; i < planets.Length; i++, j++)
                {
                    var planet = planets[i];
                    var planetName = CultureInfoUtility.TextInfo.ToTitleCase(planet.Id);

                    if (planet.HasMoons())
                    {
                        for (int k = 0, l = 1; k < planet.Moons.Count; k++, l++)
                        {
                            var moonName = CultureInfoUtility.TextInfo.ToTitleCase(planet.Moons.ElementAt(k).Id);
                            sb.AppendLine($"{j},{planetName},{planet.SemiMajorAxis},{planet.Moons.Count},{l},{moonName}");
                        }
                    }
                    else
                    {
                        sb.AppendLine($"{j},{planetName},{planet.SemiMajorAxis},0,-,-");
                    }
                }

                File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
                Console.WriteLine($"{OutputString.FileCreated}{filePath}");
                Logger.Instance.Info($"{OutputString.FileCreated}{filePath}");
            }
            catch (Exception ex)
            {
                Logger.Instance.Error($"{LoggerMessage.FileOutputOperationFailed}{ex.Message}", ex);
                Console.WriteLine($"{ExceptionMessage.FileOutputOperationFailed}{ex.Message}");
            }
        }

        public void OutputAllMoonsAndTheirMassToFile()
        {
            try
            {
                EnsureOutputFolderExists();
                var moons = _moonService.GetAllMoons().ToArray();
                var filePath = Path.Combine(PathName.PathToOutputFolder, PathName.AllMoonsAndTheirMassFile);

                var sb = new StringBuilder();
                sb.AppendLine("MoonNumber,MoonId,MoonMassExponent,MoonMassValue");

                for (int i = 0, j = 1; i < moons.Length; i++, j++)
                {
                    var moon = moons[i];
                    var moonName = CultureInfoUtility.TextInfo.ToTitleCase(moon.Id);
                    sb.AppendLine($"{j},{moonName},{moon.MassExponent},{moon.MassValue}");
                }

                File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
                Console.WriteLine($"{OutputString.FileCreated}{filePath}");
                Logger.Instance.Info($"{OutputString.FileCreated}{filePath}");
            }
            catch (Exception ex)
            {
                Logger.Instance.Error($"{LoggerMessage.FileOutputOperationFailed}{ex.Message}", ex);
                Console.WriteLine($"{ExceptionMessage.FileOutputOperationFailed}{ex.Message}");
            }
        }

        public void OutputAllPlanetsAndTheirAverageMoonTemperatureToFile()
        {
            try
            {
                EnsureOutputFolderExists();
                var planets = _planetService.GetAllPlanets().Where(p => p.HasMoons()).ToArray();
                var filePath = Path.Combine(PathName.PathToOutputFolder, PathName.AllPlanetsAndTheirAverageMoonTemperatureFile);

                var sb = new StringBuilder();
                sb.AppendLine("PlanetNumber,PlanetId,AverageMoonTemperature,TemperatureUnit,TotalMoons");

                for (int i = 0, j = 1; i < planets.Length; i++, j++)
                {
                    var planet = planets[i];
                    var planetName = CultureInfoUtility.TextInfo.ToTitleCase(planet.Id);
                    sb.AppendLine($"{j},{planetName},{planet.AverageMoonTemperature:F2},{OutputString.TemperatureUnit},{planet.Moons.Count}");
                }

                File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
                Console.WriteLine($"{OutputString.FileCreated}{filePath}");
                Logger.Instance.Info($"{OutputString.FileCreated}{filePath}");
            }
            catch (Exception ex)
            {
                Logger.Instance.Error($"{LoggerMessage.FileOutputOperationFailed}{ex.Message}", ex);
                Console.WriteLine($"{ExceptionMessage.FileOutputOperationFailed}{ex.Message}");
            }
        }

        private static void EnsureOutputFolderExists()
        {
            if (!Directory.Exists(PathName.PathToOutputFolder))
            {
                Directory.CreateDirectory(PathName.PathToOutputFolder);
                Console.WriteLine($"{OutputString.FolderCreated}{PathName.PathToOutputFolder}");
                Logger.Instance.Info($"{OutputString.FolderCreated}{PathName.PathToOutputFolder}");
            }
        }
    }
}
