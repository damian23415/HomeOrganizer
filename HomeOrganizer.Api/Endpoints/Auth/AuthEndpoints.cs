public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth");

        group.MapPost("/register", async (
            RegisterUserRequest request,
            IUserRegistrationService registrationService) =>
        {
            var response = await registrationService.RegisterAsync(request);
            return Results.Ok(response);
        })
        .WithName("RegisterUser")
        .WithTags("Auth");
    }
}