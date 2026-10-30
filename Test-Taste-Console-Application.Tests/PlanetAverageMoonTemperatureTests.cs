using System.Collections.ObjectModel;
using Test_Taste_Console_Application.Domain.DataTransferObjects;
using Test_Taste_Console_Application.Domain.Objects;
using Xunit;

namespace Test_Taste_Console_Application.Tests
{
    public class PlanetAverageMoonTemperatureTests
    {
        [Fact]
        public void PlanetWithNoMoons_ShouldReturnZeroTemperature_AndHasMoonsBeFalse()
        {
            // Arrange
            var planetDto = new PlanetDto
            {
                Id = "mercure",
                SemiMajorAxis = 57909050.0f,
                Moons = null
            };

            // Act
            var planet = new Planet(planetDto);

            // Assert
            Assert.False(planet.HasMoons());
            Assert.Equal(0.0f, planet.AverageMoonTemperature);
            Assert.Equal(0.0f, planet.AverageMoonGravity);
        }

        [Fact]
        public void PlanetWithEmptyMoonsCollection_ShouldReturnZeroTemperature()
        {
            // Arrange
            var planetDto = new PlanetDto
            {
                Id = "venus",
                SemiMajorAxis = 108208475.0f,
                Moons = new Collection<MoonDto>()
            };

            // Act
            var planet = new Planet(planetDto);

            // Assert
            Assert.False(planet.HasMoons());
            Assert.Equal(0.0f, planet.AverageMoonTemperature);
        }

        [Fact]
        public void PlanetWithSingleMoon_ShouldReturnMoonTemperature()
        {
            // Arrange (Earth and Moon)
            var planetDto = new PlanetDto
            {
                Id = "terre",
                SemiMajorAxis = 149598262.0f,
                Moons = new Collection<MoonDto>
                {
                    new MoonDto
                    {
                        Id = "lune",
                        AverageTemperature = 250.0f
                    }
                }
            };

            // Act
            var planet = new Planet(planetDto);

            // Assert
            Assert.True(planet.HasMoons());
            Assert.Single(planet.Moons);
            Assert.Equal(250.0f, planet.AverageMoonTemperature);
        }

        [Fact]
        public void PlanetWithMultipleMoons_ShouldCalculateCorrectAverage()
        {
            // Arrange (Mars with Phobos: 233 K, Deimos: 233 K)
            var planetDto = new PlanetDto
            {
                Id = "mars",
                SemiMajorAxis = 227939366.0f,
                Moons = new Collection<MoonDto>
                {
                    new MoonDto { Id = "phobos", AverageTemperature = 233.0f },
                    new MoonDto { Id = "deimos", AverageTemperature = 233.0f }
                }
            };

            // Act
            var planet = new Planet(planetDto);

            // Assert
            Assert.True(planet.HasMoons());
            Assert.Equal(2, planet.Moons.Count);
            Assert.Equal(233.0f, planet.AverageMoonTemperature);
        }

        [Fact]
        public void PlanetWithMultipleMoonsDifferentTemperatures_ShouldCalculateExactMean()
        {
            // Arrange
            var planetDto = new PlanetDto
            {
                Id = "test_planet",
                SemiMajorAxis = 500000.0f,
                Moons = new Collection<MoonDto>
                {
                    new MoonDto { Id = "m1", AverageTemperature = 100.0f },
                    new MoonDto { Id = "m2", AverageTemperature = 200.0f },
                    new MoonDto { Id = "m3", AverageTemperature = 300.0f }
                }
            };

            // Act
            var planet = new Planet(planetDto);

            // Assert: (100 + 200 + 300) / 3 = 200.0f
            Assert.Equal(200.0f, planet.AverageMoonTemperature);
        }

        [Fact]
        public void PlanetWithSomeMoonsMissingTemperature_ShouldExcludeMissingValuesFromAverage()
        {
            // Arrange: 2 moons with valid temperature (100 K, 200 K) and 2 moons with null / 0 temperature
            var planetDto = new PlanetDto
            {
                Id = "jupiter_sample",
                SemiMajorAxis = 778340821.0f,
                Moons = new Collection<MoonDto>
                {
                    new MoonDto { Id = "m1", AverageTemperature = 100.0f },
                    new MoonDto { Id = "m2", AverageTemperature = 200.0f },
                    new MoonDto { Id = "m3_null", AverageTemperature = null },
                    new MoonDto { Id = "m4_zero", AverageTemperature = 0.0f }
                }
            };

            // Act
            var planet = new Planet(planetDto);

            // Assert: only m1 and m2 should contribute -> (100 + 200) / 2 = 150.0f
            Assert.Equal(150.0f, planet.AverageMoonTemperature);
        }

        [Fact]
        public void PlanetWithAllMoonsMissingTemperature_ShouldSafelyReturnZero_WithoutDivisionByZero()
        {
            // Arrange: all moons have null or 0 temperature
            var planetDto = new PlanetDto
            {
                Id = "unknown_system",
                SemiMajorAxis = 999999.0f,
                Moons = new Collection<MoonDto>
                {
                    new MoonDto { Id = "m1", AverageTemperature = null },
                    new MoonDto { Id = "m2", AverageTemperature = 0.0f }
                }
            };

            // Act
            var planet = new Planet(planetDto);

            // Assert
            Assert.True(planet.HasMoons());
            Assert.Equal(0.0f, planet.AverageMoonTemperature);
        }

        [Fact]
        public void AverageMoonGravity_ShouldAliasAverageMoonTemperature_ForBackwardCompatibility()
        {
            // Arrange
            var planetDto = new PlanetDto
            {
                Id = "jupiter",
                Moons = new Collection<MoonDto>
                {
                    new MoonDto { Id = "io", AverageTemperature = 130.0f },
                    new MoonDto { Id = "europa", AverageTemperature = 110.0f }
                }
            };

            // Act
            var planet = new Planet(planetDto);

            // Assert
            Assert.Equal(planet.AverageMoonTemperature, planet.AverageMoonGravity);
            Assert.Equal(120.0f, planet.AverageMoonGravity);
        }
    }
}
