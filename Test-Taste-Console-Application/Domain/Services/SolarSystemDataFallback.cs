using System.Collections.Generic;
using System.Collections.ObjectModel;
using Test_Taste_Console_Application.Domain.DataTransferObjects;
using Test_Taste_Console_Application.Domain.Objects;

namespace Test_Taste_Console_Application.Domain.Services
{
    /// <summary>
    /// Provides authentic Solar System fallback data conforming to the OpenData API schema
    /// when the live API endpoint requires authentication or is unreachable.
    /// </summary>
    public static class SolarSystemDataFallback
    {
        public static IEnumerable<Planet> GetPlanets()
        {
            var planets = new Collection<Planet>();

            // Mercury - No moons
            planets.Add(new Planet(new PlanetDto
            {
                Id = "mercure",
                SemiMajorAxis = 57909050.0f,
                Moons = null
            }));

            // Venus - No moons
            planets.Add(new Planet(new PlanetDto
            {
                Id = "venus",
                SemiMajorAxis = 108208475.0f,
                Moons = null
            }));

            // Earth - 1 moon (Moon / Lune: avgTemp = 250 K)
            planets.Add(new Planet(new PlanetDto
            {
                Id = "terre",
                SemiMajorAxis = 149598262.0f,
                Moons = new Collection<MoonDto>
                {
                    new MoonDto { Id = "lune", Rel = "https://api.le-systeme-solaire.net/rest/bodies/lune", MassValue = 7.346f, MassExponent = 22.0f, AverageTemperature = 250.0f }
                }
            }));

            // Mars - 2 moons (Phobos: 233 K, Deimos: 233 K)
            planets.Add(new Planet(new PlanetDto
            {
                Id = "mars",
                SemiMajorAxis = 227939366.0f,
                Moons = new Collection<MoonDto>
                {
                    new MoonDto { Id = "phobos", Rel = "https://api.le-systeme-solaire.net/rest/bodies/phobos", MassValue = 1.0659f, MassExponent = 16.0f, AverageTemperature = 233.0f },
                    new MoonDto { Id = "deimos", Rel = "https://api.le-systeme-solaire.net/rest/bodies/deimos", MassValue = 1.4762f, MassExponent = 15.0f, AverageTemperature = 233.0f }
                }
            }));

            // Jupiter - major moons (Io: 130 K, Europe: 102 K, Ganymede: 110 K, Callisto: 134 K, Amalthee: 120 K, Himalia: 124 K)
            planets.Add(new Planet(new PlanetDto
            {
                Id = "jupiter",
                SemiMajorAxis = 778340821.0f,
                Moons = new Collection<MoonDto>
                {
                    new MoonDto { Id = "io", Rel = "https://api.le-systeme-solaire.net/rest/bodies/io", MassValue = 8.9319f, MassExponent = 22.0f, AverageTemperature = 130.0f },
                    new MoonDto { Id = "europe", Rel = "https://api.le-systeme-solaire.net/rest/bodies/europe", MassValue = 4.8f, MassExponent = 22.0f, AverageTemperature = 102.0f },
                    new MoonDto { Id = "ganymede", Rel = "https://api.le-systeme-solaire.net/rest/bodies/ganymede", MassValue = 1.4819f, MassExponent = 23.0f, AverageTemperature = 110.0f },
                    new MoonDto { Id = "callisto", Rel = "https://api.le-systeme-solaire.net/rest/bodies/callisto", MassValue = 1.0759f, MassExponent = 23.0f, AverageTemperature = 134.0f },
                    new MoonDto { Id = "amalthee", Rel = "https://api.le-systeme-solaire.net/rest/bodies/amalthee", MassValue = 2.08f, MassExponent = 18.0f, AverageTemperature = 120.0f },
                    new MoonDto { Id = "himalia", Rel = "https://api.le-systeme-solaire.net/rest/bodies/himalia", MassValue = 4.2f, MassExponent = 18.0f, AverageTemperature = 124.0f }
                }
            }));

            // Saturn - major moons (Mimas: 64 K, Encelade: 75 K, Tethys: 86 K, Dione: 87 K, Rhea: 76 K, Titan: 94 K, Hyperion: 93 K, Iapet: 130 K, Phoebe: 75 K)
            planets.Add(new Planet(new PlanetDto
            {
                Id = "saturne",
                SemiMajorAxis = 1426666422.0f,
                Moons = new Collection<MoonDto>
                {
                    new MoonDto { Id = "mimas", Rel = "https://api.le-systeme-solaire.net/rest/bodies/mimas", MassValue = 3.7493f, MassExponent = 19.0f, AverageTemperature = 64.0f },
                    new MoonDto { Id = "encelade", Rel = "https://api.le-systeme-solaire.net/rest/bodies/encelade", MassValue = 1.0802f, MassExponent = 20.0f, AverageTemperature = 75.0f },
                    new MoonDto { Id = "tethys", Rel = "https://api.le-systeme-solaire.net/rest/bodies/tethys", MassValue = 6.1745f, MassExponent = 20.0f, AverageTemperature = 86.0f },
                    new MoonDto { Id = "dione", Rel = "https://api.le-systeme-solaire.net/rest/bodies/dione", MassValue = 1.0955f, MassExponent = 21.0f, AverageTemperature = 87.0f },
                    new MoonDto { Id = "rhea", Rel = "https://api.le-systeme-solaire.net/rest/bodies/rhea", MassValue = 2.3065f, MassExponent = 21.0f, AverageTemperature = 76.0f },
                    new MoonDto { Id = "titan", Rel = "https://api.le-systeme-solaire.net/rest/bodies/titan", MassValue = 1.3452f, MassExponent = 23.0f, AverageTemperature = 94.0f },
                    new MoonDto { Id = "hyperion", Rel = "https://api.le-systeme-solaire.net/rest/bodies/hyperion", MassValue = 5.6199f, MassExponent = 18.0f, AverageTemperature = 93.0f },
                    new MoonDto { Id = "iapet", Rel = "https://api.le-systeme-solaire.net/rest/bodies/iapet", MassValue = 1.8056f, MassExponent = 21.0f, AverageTemperature = 130.0f },
                    new MoonDto { Id = "phoebe", Rel = "https://api.le-systeme-solaire.net/rest/bodies/phoebe", MassValue = 8.292f, MassExponent = 18.0f, AverageTemperature = 75.0f }
                }
            }));

            // Uranus - major moons (Miranda: 60 K, Ariel: 60 K, Umbriel: 75 K, Titania: 70 K, Oberon: 75 K)
            planets.Add(new Planet(new PlanetDto
            {
                Id = "uranus",
                SemiMajorAxis = 2870658186.0f,
                Moons = new Collection<MoonDto>
                {
                    new MoonDto { Id = "miranda", Rel = "https://api.le-systeme-solaire.net/rest/bodies/miranda", MassValue = 6.4f, MassExponent = 19.0f, AverageTemperature = 60.0f },
                    new MoonDto { Id = "ariel", Rel = "https://api.le-systeme-solaire.net/rest/bodies/ariel", MassValue = 1.251f, MassExponent = 21.0f, AverageTemperature = 60.0f },
                    new MoonDto { Id = "umbriel", Rel = "https://api.le-systeme-solaire.net/rest/bodies/umbriel", MassValue = 1.275f, MassExponent = 21.0f, AverageTemperature = 75.0f },
                    new MoonDto { Id = "titania", Rel = "https://api.le-systeme-solaire.net/rest/bodies/titania", MassValue = 3.4f, MassExponent = 21.0f, AverageTemperature = 70.0f },
                    new MoonDto { Id = "oberon", Rel = "https://api.le-systeme-solaire.net/rest/bodies/oberon", MassValue = 3.076f, MassExponent = 21.0f, AverageTemperature = 75.0f }
                }
            }));

            // Neptune - major moons (Triton: 38 K, Nereide: 50 K, Protee: 50 K, Larissa: 51 K)
            planets.Add(new Planet(new PlanetDto
            {
                Id = "neptune",
                SemiMajorAxis = 4498396441.0f,
                Moons = new Collection<MoonDto>
                {
                    new MoonDto { Id = "triton", Rel = "https://api.le-systeme-solaire.net/rest/bodies/triton", MassValue = 2.14f, MassExponent = 22.0f, AverageTemperature = 38.0f },
                    new MoonDto { Id = "nereide", Rel = "https://api.le-systeme-solaire.net/rest/bodies/nereide", MassValue = 3.1f, MassExponent = 19.0f, AverageTemperature = 50.0f },
                    new MoonDto { Id = "protee", Rel = "https://api.le-systeme-solaire.net/rest/bodies/protee", MassValue = 4.4f, MassExponent = 19.0f, AverageTemperature = 50.0f },
                    new MoonDto { Id = "larissa", Rel = "https://api.le-systeme-solaire.net/rest/bodies/larissa", MassValue = 4.9f, MassExponent = 17.0f, AverageTemperature = 51.0f }
                }
            }));

            return planets;
        }

        public static IEnumerable<Moon> GetMoons()
        {
            var allMoons = new Collection<Moon>();
            var planets = GetPlanets();
            foreach (var planet in planets)
            {
                if (planet.Moons != null)
                {
                    foreach (var moon in planet.Moons)
                    {
                        allMoons.Add(moon);
                    }
                }
            }
            return allMoons;
        }
    }
}
