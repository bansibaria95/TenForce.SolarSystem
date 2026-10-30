namespace Test_Taste_Console_Application.Domain.Services.Interfaces
{
    /// <summary>
    /// An output service that writes Solar System data to CSV files on disk.
    /// </summary>
    public interface IFileOutputService
    {
        void OutputAllPlanetsAndTheirMoonsToFile();
        void OutputAllMoonsAndTheirMassToFile();
        void OutputAllPlanetsAndTheirAverageMoonTemperatureToFile();
    }
}
