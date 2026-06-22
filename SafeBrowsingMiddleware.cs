using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using WebsiteWacher.Services;

namespace WebsiteWacher
{
    public class SafeBrowsingMiddleware(SafeBrowsingService safeBrowsingService) : IFunctionsWorkerMiddleware
    {
        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            var request = await context.GetHttpRequestDataAsync();
            if(!context.BindingContext.BindingData.ContainsKey("Url"))
            {
                var responce = request.CreateResponse(System.Net.HttpStatusCode.BadRequest);
                await responce.WriteStringAsync("URL is missing in the request.");
                return;
            }
            var url = context.BindingContext.BindingData["Url"]?.ToString();
            if (string.IsNullOrEmpty(url) || !isValidUrl(url))
            {
                var responce = request.CreateResponse(System.Net.HttpStatusCode.BadRequest);
                await responce.WriteStringAsync("Invalid URL provided.");
                return;
            }
            var saferesuiltCheck = safeBrowsingService.CheckUrl(url);
            if(saferesuiltCheck.HasThreat)
            {
                var responce = request.CreateResponse(System.Net.HttpStatusCode.BadRequest);
                await responce.WriteStringAsync($"URL is flagged as unsafe. Threat type: {string.Join(", ", saferesuiltCheck.ThreatType)}");
                return;
            }
            else
            {
                await next(context);
            }
        }
        private bool isValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}
