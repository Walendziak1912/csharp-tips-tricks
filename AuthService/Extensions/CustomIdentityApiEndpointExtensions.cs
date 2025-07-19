using System.ComponentModel.DataAnnotations;
using System.Text;
using AuthService.Dtos.User;
using AuthService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace AuthService.Extensions;

public static class IdentityApiEndpointRouteBuilderExtensions
{
    private static readonly EmailAddressAttribute _emailAddressAttribute = new();

    public static IEndpointConventionBuilder CustomMapIdentityApi<TUser>(this IEndpointRouteBuilder endpoints)
        where TUser : User, new()
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        var routeGroup = endpoints.MapGroup("/");

        routeGroup.MapPost("/register", async Task<Results<Ok, ValidationProblem>>
            ([FromBody] CreateUserDto registration, HttpContext context, [FromServices] IServiceProvider sp) =>
        {
            var userManager = sp.GetRequiredService<UserManager<TUser>>();

            if (!userManager.SupportsUserEmail)
            {
                throw new NotSupportedException($"{nameof(CustomMapIdentityApi)} requires a user store with email support.");
            }

            var userStore = sp.GetRequiredService<IUserStore<TUser>>();
            var emailStore = (IUserEmailStore<TUser>)userStore;
            var email = registration.Email;

            if (string.IsNullOrEmpty(email) || !_emailAddressAttribute.IsValid(email))
            {
                return CreateValidationProblem(IdentityResult.Failed(userManager.ErrorDescriber.InvalidEmail(email)));
            }

            // Add customization to the Endpoint
            var user = new TUser
            {
                FirstName = registration.FirstName,
                LastName = registration.LastName
            };

            await userStore.SetUserNameAsync(user, email, CancellationToken.None);
            await emailStore.SetEmailAsync(user, email, CancellationToken.None);
            var result = await userManager.CreateAsync(user, registration.Password);

            if (!result.Succeeded)
            {
                return CreateValidationProblem(result);
            }

            return TypedResults.Ok();
        });

        routeGroup.MapPost("/login", async Task<Results<SignInHttpResult, ValidationProblem, ChallengeHttpResult>>
            ([FromBody] LoginRequest login, [FromQuery] bool? useCookies, [FromQuery] bool? useSessionCookies, [FromServices] IServiceProvider sp) =>
        {
            var signInManager = sp.GetRequiredService<SignInManager<TUser>>();
            var useCookieScheme = (useCookies == true) || (useSessionCookies == true);
            var isPersistent = (useCookies == true) && (useSessionCookies != true);
            signInManager.AuthenticationScheme = useCookieScheme ? IdentityConstants.ApplicationScheme : IdentityConstants.BearerScheme;

            var result = await signInManager.PasswordSignInAsync(login.Email, login.Password, isPersistent, lockoutOnFailure: true);

            if (result.RequiresTwoFactor)
            {
                if (!string.IsNullOrEmpty(login.TwoFactorCode))
                {
                    result = await signInManager.TwoFactorAuthenticatorSignInAsync(login.TwoFactorCode, isPersistent, rememberClient: isPersistent);
                }
                else if (!string.IsNullOrEmpty(login.TwoFactorRecoveryCode))
                {
                    result = await signInManager.TwoFactorRecoveryCodeSignInAsync(login.TwoFactorRecoveryCode);
                }
            }

            if (!result.Succeeded)
            {
                return TypedResults.Challenge();
            }

            // The signInManager already produced the needed response in the form of a cookie or bearer token.
            return TypedResults.SignIn(result.Principal, result.AuthenticationScheme);
        });

        routeGroup.MapPost("/logout", async Task<IResult> ([FromServices] IServiceProvider sp, [FromBody] object empty) =>
        {
            var signInManager = sp.GetRequiredService<SignInManager<TUser>>();

            if (empty != null)
            {
                await signInManager.SignOutAsync();
                return Results.Ok();
            }
            return Results.Unauthorized();
        })
        .RequireAuthorization();

        return new IdentityEndpointsConventionBuilder(routeGroup);
    }

    private static ValidationProblem CreateValidationProblem(IdentityResult result)
    {
        // In a real world scenario, you'd want to log this error, and you'd want to return a generic error message to the client.
        return TypedResults.ValidationProblem(result.Errors.ToDictionary(e => e.Code, e => new[] { e.Description }));
    }
}

file sealed class IdentityEndpointsConventionBuilder(RouteGroupBuilder group) : IEndpointConventionBuilder
{
    private readonly RouteGroupBuilder _group = group;

    public void Add(Action<EndpointBuilder> convention) => _group.Add(convention);

    public void Finally(Action<EndpointBuilder> finallyConvention) => _group.Finally(finallyConvention);
}

// NOTE: Endpoint invokes this method, this is the workaround for the generic parameter limitation issue mentioned above.
internal sealed class LoginRequest
{
    public required string Email { get; set; }
    public required string Password { get; set; }
    public string? TwoFactorCode { get; set; }
    public string? TwoFactorRecoveryCode { get; set; }
} 