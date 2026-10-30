# TenForce Developer – Test and Taste Assignment

## 1. Application Overview
`Test-Taste-Console-Application` is a .NET console application developed for the TenForce Developer – Test and Taste exercise. The application integrates with the public **Solar System OpenData API** (`https://api.le-systeme-solaire.net/`) to retrieve celestial body information, process domain objects representing planets and moons, and display structured data via console output and CSV disk output.

In addition to listing all planets with their moons and displaying all moon masses, the application implements the requirement to **calculate and display the average surface temperature of the moons belonging to each planet that has at least one moon**.

---

## 2. Project Structure
The solution is organized into clear architectural layers:

```
Test-Taste-Console-Application/
├── Constants/
│   ├── ConfigurationFileName.cs         # Configuration constants (e.g., log4net.config)
│   ├── ExceptionMessage.cs              # User-facing exception error messages
│   ├── HttpClientSettings.cs            # HTTP client media types and headers
│   ├── LoggerMessage.cs                 # Log4Net log event messages
│   ├── OutputString.cs                  # UI column labels, units, and progress messages
│   ├── PathName.cs                      # File and folder paths for CSV outputs
│   └── UriPath.cs                       # API endpoints and query parameter definitions
├── Domain/
│   ├── DataTransferObjects/
│   │   ├── JsonObjects/
│   │   │   ├── JsonPathConverter.cs     # Custom Newtonsoft.Json converter for nested dotted properties
│   │   │   └── JsonResult.cs            # Generic wrapper for API response bodies
│   │   ├── MoonDto.cs                   # Moon data transfer object (with avgTemp mapping)
│   │   └── PlanetDto.cs                 # Planet data transfer object
│   ├── Objects/
│   │   ├── Moon.cs                      # Moon domain entity (encapsulating mass & temperature)
│   │   └── Planet.cs                    # Planet domain entity (encapsulating moons & temperature calculation)
│   └── Services/
│       ├── Interfaces/
│       │   ├── IFileOutputService.cs    # Contract for writing CSV data to disk
│       │   ├── IMoonService.cs          # Contract for retrieving moon data
│       │   ├── IOutputService.cs        # Contract for rendering tables to the console
│       │   └── IPlanetService.cs        # Contract for retrieving planet data
│       ├── FileOutputService.cs         # Writes planets, moons, and average temperatures to CSV files
│       ├── HttpClientService.cs         # Configures HttpClient and optional Bearer authorization
│       ├── MoonService.cs               # Fetches moons with mass from API/fallback
│       ├── PlanetService.cs             # Fetches planets, maps child moons with optimization & fallback
│       ├── ScreenOutputService.cs       # Formats and renders tabular data to the console
│       └── SolarSystemDataFallback.cs   # Authentic Solar System dataset fallback for offline/unauthenticated runs
├── Utilities/
│   ├── ConsoleWriter.cs                 # Grid and column formatting utility
│   ├── CultureInfoUtility.cs            # Text casing and culture configuration
│   └── Logger.cs                        # Log4Net logger instance provider
├── log4net.config                       # Logging configuration
├── Program.cs                           # Composition root (Dependency Injection & Execution workflow)
└── Test-Taste-Console-Application.csproj

Test-Taste-Console-Application.Tests/
├── PlanetAverageMoonTemperatureTests.cs # Unit test suite covering all calculation edge cases
└── Test-Taste-Console-Application.Tests.csproj
```

---

## 3. Application Execution Steps

### Prerequisites
- .NET SDK (supports .NET 5.0 runtime or modern .NET SDKs with multi-targeting).

### Build Solution
```bash
dotnet build
```

### Run Application
```bash
dotnet run --project Test-Taste-Console-Application/Test-Taste-Console-Application.csproj
```

### Run Unit Tests
```bash
dotnet test
```

### Expected Output Files
The application automatically creates the `FileOutput/` directory in the working directory and outputs three CSV files:
1. `FileOutput/AllPlanetsAndTheirAverageMoonTemperature.csv`
2. `FileOutput/AllPlanetsAndTheirMoons.csv`
3. `FileOutput/AllMoonsAndTheirMass.csv`

---

## 4. API Integration

### Endpoint Details
- **Base URL:** `https://api.le-systeme-solaire.net`
- **Planets with Moons Query:** `/rest/bodies?data=id,semiMajorAxis,moons,moon,rel&filter[]=isPlanet,neq,false`
- **Moons Query:** `/rest/bodies?data=id,mass,massValue,massExponent,avgTemp&filter[]=aroundPlanet,gt,null`
- **Individual Moon Query:** `/rest/bodies/{id}`

### Authentication & Resiliency
- **Discrepancy Reported:** While the original exercise specification stated authentication is not expected, the live endpoint `https://api.le-systeme-solaire.net/rest/bodies` currently responds with `HTTP 401 Unauthorized` without an API key.
- **Bearer Token Support:** `HttpClientService` checks for an optional environment variable `SOLAR_SYSTEM_API_KEY`. If provided, it automatically attaches `Authorization: Bearer <token>`.
- **Authentic Fallback Dataset:** If an unauthenticated call returns `HTTP 401` or encounters network unavailability, the application logs an informative message and falls back to `SolarSystemDataFallback`, containing real astronomical data conforming to the OpenData schema. This guarantees end-to-end execution, full feature demonstration, and testability in any environment.
- **Request Optimization:** In `PlanetService`, moons are bulk-fetched and cached in-memory, reducing network traffic from **200+ serial HTTP requests down to 2 requests**, preventing rate limiting and network timeouts.

---

## 5. New Functionality

The application introduces the following functionality:
1. **Planets with Moons Filtering:** Evaluates each planet and includes only planets with at least one moon (`HasMoons() == true`). Planets with zero moons (e.g., Mercury, Venus) are excluded from the temperature list.
2. **Independent Moon Average Calculation:** For each qualified planet, computes the arithmetic mean of its associated moons' surface temperatures.
3. **Console Presentation:** Renders a formatted table showing each planet and its calculated average moon temperature with explicit unit `K` (Kelvin).
4. **File Output:** Exports `AllPlanetsAndTheirAverageMoonTemperature.csv` containing `PlanetNumber,PlanetId,AverageMoonTemperature,TemperatureUnit,TotalMoons`.
5. **Execution Progress Feedback:** Displays live progress updates across every processing step (`Loading data...`, `Processing Solar System bodies...`, `Identifying planets with moons...`, `Calculating average moon temperatures...`, `Writing data to console...`, `Writing data to file...`, `Completed successfully.`).

---

## 6. Average Moon Temperature Calculation

### Formula
For a given planet $P$ with child moons $\{m_1, m_2, \dots, m_k\}$:
$$\text{AverageMoonTemperature}(P) = \frac{\sum_{i=1}^{n} m_i.\text{AverageTemperature}}{n}$$
where $n$ is the count of moons having valid recorded temperatures ($m_i.\text{AverageTemperature} > 0$).

### Defensive Handling
- **Planets Without Moons:** Guarded by `HasMoons()`; returns `0.0f` and excluded from display.
- **Planet's Own Temperature:** Never included in the calculation.
- **Missing / Unrecorded Data:** Many small or unmeasured moons return `null` or `0` in astronomical databases. The calculation filters `m.AverageTemperature.HasValue && m.AverageTemperature.Value > 0` so unmeasured moons do not skew the physical mean.
- **Division by Zero Prevention:** Guarded by `if (!validMoons.Any()) return 0.0f;`.
- **Unit of Measurement:** **Kelvin ($K$)**, standard in celestial astrophysics and the Solar System OpenData API.

---

## 7. Domain Model Modifications

1. **`MoonDto.cs`:**
   - Added `[JsonProperty("avgTemp")] public float? AverageTemperature { get; set; }` to deserialize temperature from the API JSON payload.
2. **`Moon.cs`:**
   - Added `public float? AverageTemperature { get; set; }` to encapsulate physical temperature on the domain entity.
   - Initialized from `MoonDto.AverageTemperature`.
3. **`Planet.cs`:**
   - Implemented `public float AverageMoonTemperature { get; }` containing the defensive calculation.
   - Retained `public float AverageMoonGravity => AverageMoonTemperature;` to maintain backward compatibility with existing callers.

---

## 8. API Interface Modifications

1. **`IOutputService.cs`:**
   - Extended with `void OutputAllPlanetsAndTheirAverageMoonTemperatureToConsole();` to provide dedicated console output for the new requirement.
   - Preserved all existing methods (`OutputAllPlanetsAndTheirMoonsToConsole`, `OutputAllMoonsAndTheirMassToConsole`, `OutputAllPlanetsAndTheirAverageMoonGravityToConsole`).
2. **`IFileOutputService.cs` (New):**
   - Added clean interface for file operations:
     - `OutputAllPlanetsAndTheirMoonsToFile()`
     - `OutputAllMoonsAndTheirMassToFile()`
     - `OutputAllPlanetsAndTheirAverageMoonTemperatureToFile()`
3. **`FileOutputService.cs` (New):**
   - Implements `IFileOutputService` to export UTF-8 CSV files into `FileOutput/`.

---

## 9. Design Decisions & Motivations

1. **Fixing Initial Compiler Syntax Defect:**
   - Corrected double-quoted syntax error in `Program.cs` (`$""...""` -> `$"..."`).
2. **Interface Segregation (ISP):**
   - Created a separate `IFileOutputService` instead of polluting `IOutputService` (whose methods specify `...ToConsole()`), ensuring single responsibility and clean DI registration.
3. **Preserving Backward Compatibility:**
   - Maintained `AverageMoonGravity` and all existing console output methods so that existing features remain functional.
4. **Resilience & Fault Tolerance:**
   - Wrapped individual API calls and bulk retrievals in try-catch blocks with `log4net` logging.
   - Added authentic fallback data so the application never breaks or produces empty screens even when running offline or against endpoints requiring API keys.
5. **Focused Unit Testing:**
   - Created `Test-Taste-Console-Application.Tests` with 8 comprehensive unit tests verifying single moon, multiple moons, empty moons, null/zero temperature filtering, and divide-by-zero protection.
