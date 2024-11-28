using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;

namespace FluentTesting.Asp.Authentication
{
    /// <summary>
    /// Test policy evaluator
    /// override of basic one, so we can use specific Auth policy
    /// to fake jwt and bypass Auth attributes
    /// </summary>
    internal class TestPolicyEvaluator(PolicyEvaluator innerEvaluator) : IPolicyEvaluator
    {
        /// <inheritdoc/>
        public Task<AuthenticateResult> AuthenticateAsync(AuthorizationPolicy policy,
            HttpContext context)
        {
            var testPolicy = new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes("Test")
                .Combine(new AuthorizationPolicyBuilder("Test").RequireAuthenticatedUser().Build())
                .Build();

            return innerEvaluator.AuthenticateAsync(testPolicy, context);
        }

        /// <inheritdoc/>
        public Task<PolicyAuthorizationResult> AuthorizeAsync(AuthorizationPolicy policy,
            AuthenticateResult authenticationResult, HttpContext context, object? resource)
        {
            var testPolicy = new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes("Test")
                .Combine(new AuthorizationPolicyBuilder("Test").RequireAuthenticatedUser().Build())
                .Build();

            return innerEvaluator.AuthorizeAsync(testPolicy, authenticationResult,
                context, resource);
        }
    }
}
