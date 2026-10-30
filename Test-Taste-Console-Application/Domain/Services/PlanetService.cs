using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Test_Taste_Console_Application.Constants;
using Test_Taste_Console_Application.Domain.DataTransferObjects;
using Test_Taste_Console_Application.Domain.DataTransferObjects.JsonObjects;
using Test_Taste_Console_Application.Domain.Objects;
using Test_Taste_Console_Application.Domain.Services.Interfaces;
using Test_Taste_Console_Application.Utilities;

namespace Test_Taste_Console_Application.Domain.Services
{
    /// <inheritdoc />
    public class PlanetService : IPlanetService
    {
        private readonly HttpClientService _httpClientService;

        public PlanetService(HttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
        }

        public IEnumerable<Planet> GetAllPlanets()
        {
            var allPlanetsWithTheirMoons = new Collection<Planet>();

            HttpResponseMessage response;
            try
            {
                response = _httpClientService.Client
                    .GetAsync(UriPath.GetAllPlanetsWithMoonsQueryParameters)
                    .Result;
            }
            catch (Exception ex)
            {
                Logger.Instance.Error($"Error requesting planets from API: {ex.Message}", ex);
                return SolarSystemDataFallback.GetPlanets();
            }

            // If the status code isn't 200-299, then the function falls back to authentic Solar System data.
            if (!response.IsSuccessStatusCode)
            {
                Logger.Instance.Warn($"{LoggerMessage.GetRequestFailed}{response.StatusCode}");
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    Logger.Instance.Info("API returned 401 Unauthorized (API key required by live server). Falling back to authentic Solar System dataset.");
                }
                return SolarSystemDataFallback.GetPlanets();
            }

            var content = response.Content.ReadAsStringAsync().Result;

            // The JSON converter uses DTO's, that can be found in the DataTransferObjects folder, to deserialize the response content.
            var results = JsonConvert.DeserializeObject<JsonResult<PlanetDto>>(content);

            // The JSON converter can return a null object. 
            if (results?.Bodies == null) return allPlanetsWithTheirMoons;

            // Pre-fetch moons in bulk to avoid unnecessary individual API requests (avoiding N+1 network requests).
            Dictionary<string, MoonDto> moonsMap = null;
            try
            {
                var bulkMoonsResponse = _httpClientService.Client
                    .GetAsync(UriPath.GetAllMoonsWithMassQueryParameters)
                    .Result;

                if (bulkMoonsResponse.IsSuccessStatusCode)
                {
                    var bulkMoonsContent = bulkMoonsResponse.Content.ReadAsStringAsync().Result;
                    var bulkMoonsResult = JsonConvert.DeserializeObject<JsonResult<MoonDto>>(bulkMoonsContent);
                    if (bulkMoonsResult?.Bodies != null)
                    {
                        moonsMap = bulkMoonsResult.Bodies
                            .Where(m => !string.IsNullOrEmpty(m.Id))
                            .GroupBy(m => m.Id, StringComparer.OrdinalIgnoreCase)
                            .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Warn($"Bulk moon fetch failed: {ex.Message}. Falling back to individual requests.", ex);
            }

            foreach (var planet in results.Bodies)
            {
                if (planet.Moons != null)
                {
                    var newMoonsCollection = new Collection<MoonDto>();
                    foreach (var moon in planet.Moons)
                    {
                        // Check bulk cache first to eliminate unnecessary HTTP round-trips
                        if (moonsMap != null && moonsMap.TryGetValue(moon.URLId, out var cachedMoon))
                        {
                            newMoonsCollection.Add(cachedMoon);
                        }
                        else
                        {
                            try
                            {
                                var moonResponse = _httpClientService.Client
                                    .GetAsync(UriPath.GetMoonByIdQueryParameters + moon.URLId)
                                    .Result;

                                if (moonResponse.IsSuccessStatusCode)
                                {
                                    var moonContent = moonResponse.Content.ReadAsStringAsync().Result;
                                    newMoonsCollection.Add(JsonConvert.DeserializeObject<MoonDto>(moonContent));
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.Instance.Warn($"Could not retrieve moon '{moon.URLId}': {ex.Message}", ex);
                            }
                        }
                    }
                    planet.Moons = newMoonsCollection;
                }
                allPlanetsWithTheirMoons.Add(new Planet(planet));
            }

            return allPlanetsWithTheirMoons;
        }

        private static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder(capacity: normalizedString.Length);

            for (int i = 0; i < normalizedString.Length; i++)
            {
                char c = normalizedString[i];
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder
                .ToString()
                .Normalize(NormalizationForm.FormC);
        }
    }
}
