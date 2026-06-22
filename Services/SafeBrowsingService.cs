using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Google.Apis.Safebrowsing.v4;
using Google.Apis.Safebrowsing.v4.Data;

namespace WebsiteWacher.Services
{
    public class SafeBrowsingService
    {
        private readonly IConfiguration _configuration;

        public SafeBrowsingService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public (bool HasThreat, IReadOnlyList<string> ThreatType) CheckUrl(string url)
        {
            var initializer = new Google.Apis.Services.BaseClientService.Initializer
            {
                ApiKey = _configuration.GetValue<string>("SafeBrowsingApiKey"),
                ApplicationName = "WebsiteWacher"
            };

            using var safeBrowsing = new SafebrowsingService(initializer);

            var threatMatchesRequest = new GoogleSecuritySafebrowsingV4FindThreatMatchesRequest
            {
                Client = new GoogleSecuritySafebrowsingV4ClientInfo
                {
                    ClientId = "WebsiteWacher",
                    ClientVersion = "1.0"
                },
                ThreatInfo = new GoogleSecuritySafebrowsingV4ThreatInfo
                {
                    ThreatTypes = new[] { "MALWARE", "SOCIAL_ENGINEERING" },
                    PlatformTypes = new[] { "WINDOWS" },
                    ThreatEntryTypes = new[] { "URL" },
                    ThreatEntries = new[]
                    {
                           new GoogleSecuritySafebrowsingV4ThreatEntry { Url = url }
                       }
                }
            };
            var threadlist = new List<string>();
            var hasThread = false;

            var threatMatchesResponse = safeBrowsing.ThreatMatches.Find(threatMatchesRequest).Execute();

            if (threatMatchesResponse?.Matches != null && threatMatchesResponse.Matches.Any())
            {
                hasThread = true;
                var threatTypes = threatMatchesResponse.Matches.Select(match => match.ThreatType).ToList();
                foreach (var match in threatMatchesResponse.Matches)
                {
                    threadlist.Add($"Threat found:{match.ThreatType} at {match.Threat.Url}");
                }

            }
            else
            {
                threadlist.Add("No threats found.");
            }

            return (hasThread, threadlist);
        }
    }
}
