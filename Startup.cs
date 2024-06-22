using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SampleWebApp.Data;
using SampleWebApp.Model;
using System;

namespace SampleWebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Create a SecretClient using the managed identity for authentication
            var keyVaultUrl = new Uri("https://kvyoutubedemowithdotnet.vault.azure.net/");
            var secretClient = new SecretClient(keyVaultUrl, new DefaultAzureCredential());

            // Retrieve the secret by name
            KeyVaultSecret secret = secretClient.GetSecret("KeyVaultDemo-ConnectionStrings--DefaultConnection");

            // Access the secret value
            string defaultConnectionString = secret.Value;

            // Use the retrieved connection string in your DbContext configuration
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(defaultConnectionString));

            // Other service configurations
            services.AddRazorPages();
            services.AddSingleton<DAL>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
