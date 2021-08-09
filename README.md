# Music Brainz Microservice

Music Brains Demo API allows user to seach music by artist name from an external web api service. It will return a list of all maching artists.

### API Endpoints

**Search Music By Artist Name**
		
		URL: https://localhost:44363/api/v1/SearchMusic?ArtistName=someArtist
		Method: GET

#### Retry HTTP Calls 
Polly library is used to retry http calls when transient errors occured. Polly.Contrib.WaitAndRetry library is used to Wait and Retry with Jittered Back-off. In a high-throughput scenario, with many concurrent requests; using a wait and retry policy with fixed progression(such as Wait and Retry with Exponential Back-off) could cause issues. Jittered Back-off provides a random factor to seperate the retries in such scenarios.

```CSharp
 services.AddHttpClient<ISearchMusicClient, SearchMusicClient>(c =>
            {
                c.BaseAddress = new Uri(Configuration.GetValue<string>("MusicBrainzApiBaseUrl") ?? throw new ArgumentException("Invalid base url."));
                c.DefaultRequestHeaders.Add("User-Agent", Configuration.GetValue<string>("ApplicationId"));
                c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            })
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))  //Set lifetime to five minutes
                .AddPolicyHandler(GetRetryPolicy());
		
private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
 {
     var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(1), retryCount: 5);

     return HttpPolicyExtensions
        .HandleTransientHttpError()
        .WaitAndRetryAsync(delay);
}
```


