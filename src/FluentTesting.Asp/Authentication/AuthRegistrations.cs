using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.Extensions.DependencyInjection;

namespace FluentTesting.Asp.Authentication
{
    /// <summary>
    /// Auth registrations
    /// </summary>
    public static class AuthRegistrations
    {
        /// <summary>
        /// Adds fake test policy with fake test evaluator and fake JWT handler, so auth attributes
        /// are still working on all controllers, but there is no need of IDM or stuff
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterAuth(this IServiceCollection services)
        {
            services.AddTransient<IPolicyEvaluator>(serviceProvider => new TestPolicyEvaluator(
                        ActivatorUtilities.CreateInstance<PolicyEvaluator>(serviceProvider)));

            services
                .AddAuthentication(opts =>
                {
                    opts.DefaultAuthenticateScheme = "Test";
                })
                .AddScheme<JwtBearerOptions, JwtAuthHandler>("Test", opts => { });
            services.AddAuthorization(opts =>
            {
                opts.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes("Test")
                    .RequireAuthenticatedUser()
                    .Build();
            });

            return services;
        }
    }
}
