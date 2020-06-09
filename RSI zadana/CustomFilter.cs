using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Security.Claims;
using System.Text;

namespace RSI_zadana
{
    public class CustomFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            context.HttpContext.Response.Headers.Add("moj-naglowek", "rsi test");
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.Request.Headers.TryGetValue("Authorization", out var token))
            {
                token = token.ToString().Substring("Basic ".Length).Trim();
                System.Console.WriteLine(token);
                var credentialstring = Encoding.UTF8.GetString(Convert.FromBase64String(token));
                var credentials = credentialstring.Split(':');
                if (credentials[0] == "admin" && credentials[1] == "admin")
                {
                    var claims = new[] { new Claim("name", credentials[0]), new Claim(ClaimTypes.Role, "Admin") };
                    var identity = new ClaimsIdentity(claims, "Basic");
                }
            }
        }
    }
}
