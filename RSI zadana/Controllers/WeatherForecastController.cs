using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RSI_zadana.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [TypeFilter(typeof(CustomFilter))]
    public class WeatherForecastController : ControllerBase
    {
        private readonly Context _context;
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IUrlHelperFactory helperfactory;
        private readonly IActionContextAccessor context2;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, Context context, IUrlHelperFactory helperfactory, IActionContextAccessor context2)
        {
            _logger = logger;
            _context = context;
            this.helperfactory = helperfactory;
            this.context2 = context2;
        }

        [Produces("application/json", "application/xml")]
        [HttpGet(Name = nameof(Get))]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();

            return _context._weathers.Select(x => CreateLinks(x));
        }

        [HttpPost(Name = nameof(Post))]
        public IActionResult Post(WeatherForecast weather)
        {
            weather.Id = _context._weathers.Last().Id++;

            _context._weathers.Add(weather);

            weather = CreateLinks(weather);

            Response.Headers.Add("Location", weather.Links.FirstOrDefault(y => y.Rel == "get-by-id").Href);

            return Ok(weather);
        }

        [HttpDelete("{id}", Name = nameof(Delete))]
        public IActionResult Delete(int id)
        {
            _context._weathers.Remove(_context._weathers.FirstOrDefault(y => y.Id == id));

            return NoContent();
        }

        [HttpPut("{id}")]
        public IActionResult Put([FromRoute] long id, WeatherForecast weather)
        {
            var w = _context._weathers.FirstOrDefault(y => y.Id == id);

            weather.Summary = weather.Summary;

            return Ok(CreateLinks(weather));
        }

        [HttpGet("{id}", Name = nameof(GetById))]
        public IActionResult GetById(long id)
        {
            return Ok(CreateLinks(_context._weathers.Where(x => x.Id == id).FirstOrDefault()));
        }

        [HttpGet("search")]
        public IActionResult Get(string zaczynaSie)
        {
            return Ok(_context._weathers.Where(x => x.Summary.ToLower().Contains(zaczynaSie.ToLower())));
        }

        [HttpGet("search/byobject")]
        public IActionResult Get([FromBody] SearchParam searchPAram)
        {
            return Ok(_context._weathers
                    .Where(x => (!string.IsNullOrEmpty(searchPAram.Summary) && x.Summary.ToLower().Contains(searchPAram.Summary.ToLower()))
                        || (searchPAram.Temperatura > 0 && x.TemperatureC == searchPAram.Temperatura)));
        }

        private WeatherForecast CreateLinks(WeatherForecast ob)
        {
            var urlHelper = this.helperfactory.GetUrlHelper(this.context2.ActionContext);
            var idObj = new { id = ob.Id };

            ob.Links = new List<LinkDTO>();

            ob.Links.Add(
                new LinkDTO(urlHelper.Link(nameof(this.Get), null),
                "all",
                "GET"));


            ob.Links.Add(
                new LinkDTO(urlHelper.Link(nameof(this.GetById), idObj),
                "get-by-id",
                "GET"));

            ob.Links.Add(
                new LinkDTO(urlHelper.Link(nameof(this.Delete), idObj),
                "delete",
                "DELETE"));

            ob.Links.Add(
                new LinkDTO(urlHelper.Link(nameof(this.Post), null),
                "create",
                "POST"));


            return ob;
        }

    }

    public class SearchParam
    {
        public string Summary { get; set; }

        public int Temperatura { get; set; }
    }
}
