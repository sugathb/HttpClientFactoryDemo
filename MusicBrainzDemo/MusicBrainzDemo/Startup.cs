using System;
using System.Net.Http;
using System.Net.Http.Headers;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MusicBrainzDemo.ApplicationServices;
using MusicBrainzDemo.Infrastructure;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Extensions.Http;

namespace MusicBrainzDemo.Api
{
    public class Startup
    {
        private const string InvalidBaseUrlEmptyError = "MusicBrainzApiBaseUrl cannot be empty.";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(Configuration.GetValue<string>("SwaggerVersion"), new OpenApiInfo
                {
                    Title = Configuration.GetValue<string>("SwaggerTitle"),
                    Version = Configuration.GetValue<string>("SwaggerVersion")
                });
            });


            services.AddHttpClient<ISearchMusicClient, SearchMusicClient>(c =>
                    {
                        c.BaseAddress = new Uri(Configuration.GetValue<string>("MusicBrainzApiBaseUrl") ?? throw new ArgumentException(InvalidBaseUrlEmptyError));
                        c.DefaultRequestHeaders.Add("User-Agent", Configuration.GetValue<string>("ApplicationId"));
                        c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    }
                )
                .SetHandlerLifetime(TimeSpan.FromMinutes(5)) //Set lifetime to five minutes
                .AddPolicyHandler(GetRetryPolicy());

            services.AddMediatR(typeof(GetArtistsQuery));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint(Configuration.GetValue<string>("SwaggerEndPoint"), Configuration.GetValue<string>("SwaggerApiName")));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        //Wait and retry with Jittered back-off
        //https://github.com/Polly-Contrib/Polly.Contrib.WaitAndRetry#wait-and-retry-with-jittered-back-off
        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(1), retryCount: 5);

            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(delay);
        }
    }
}
