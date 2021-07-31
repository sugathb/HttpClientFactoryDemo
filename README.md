# Music Brainz Microservice

Music Brains Demo API allows user to seach music by artist name. If there are more than one artist for a searched name, it'll return a list of all maching artists. If there is only one, the API will return a collection of releases of the artist.

### Consume API
The docker image for this api can be downloaded from,

https://hub.docker.com/r/sugathb/musicbrainsdemoapi

The API is also hosted as an Azure Container Instance and can be accessed via,

http://musicbrains.australiasoutheast.azurecontainer.io/swagger/index.html

### API Endpoints

**Search Music By Artist Name**
		
		URL: http://musicbrains.australiasoutheast.azurecontainer.io/api/v1/SearchMusic?artistName=someArtist
		Method: GET
    
### Design & Architecture
The API is developed as an ASP.NET Core WebAPI that targets to .NET 5.0 with Docker support enabled. Please note that if you are running the application locally as a docker container, you'll need to have Docker Desktop installed.   

#### Domain-driven design
DDD approach is used to structure the code into sepearate layers,
- API
- Application Services
- Infrastructure
- Domain

#### Application Configuration Settings
All application settings can be found in appsettings.json file in MusicBrainsDemo.Api project


#### Retry HTTP Calls 
Polly library is used to retry http calls when transient errors occured. Polly.Contrib.WaitAndRetry library is used to Wait and Retry with Jittered Back-off. In a high-throughput scenario, with many concurrent requests; using a wait and retry policy with fixed progression(such as Wait and Retry with Exponential Back-off) could cause issues. Jittered Back-off provides a random factor to seperate the retries in such scenarios.

```CSharp
 services.AddHttpClient("musicApiClient", c =>
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

#### Distributed cache
API use Azure Cache for Redis. By default caching is disabled throuh an application setting. (I have also removed the actual Azure Cache for Redis connection string when uploading the code to GitHub.)

StackExchange.Redis and Microsoft.Extensions.Caching.StackExchangeRedis package is used.

#### Serilog
Microsoft.Extensions.Logging.ILogger is configured to write logs to serilog

```CSharp
public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                //Configure Microsoft.Extensions.Logging.ILogger to write to Serilog
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddSerilog();
                })
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
```
Serilog logger is configured to sink logs to console and Datadog,
```CSharp
private static void ConfigureLogger(string datadogApiKey, string environment)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.DatadogLogs(
                    apiKey: datadogApiKey,
                    host: Environment.MachineName,
                    tags: new[] { $"env:{environment}" }
                )
                .CreateLogger();
        }
```



Following nuget packages are used,
- Serilog.Extensions.Logging
- Serilog.Sinks.Console
- Serilog.Sinks.Datadog.Logs

#### Unit Tests
Nunit and Moq packages are used to write unit tests. Separate unit test projects added for API, ApplicationServices and Infrastructure unit tests. 

#### To DO Tasks
- Implement Authentication/Authorization for the API.
- Support Pagination. 



