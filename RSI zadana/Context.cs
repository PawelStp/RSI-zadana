using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RSI_zadana
{
    public class Context
    {
        public readonly  IList<WeatherForecast> _weathers;
        private static readonly string[] Summaries = new[]
{
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public Context()
        {
            var rng = new Random();
            _weathers = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Id = index,
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToList();
        }
    }
}
