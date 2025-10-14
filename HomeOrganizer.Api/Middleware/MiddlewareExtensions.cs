using FluentValidation;
using HomeOrganizer.Application.Common.Exceptions;
using HomeOrganizer.Application.Common.Models;

namespace HomeOrganizer.Api.Middleware;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ErrorHandlingMiddleware>();
    }
}