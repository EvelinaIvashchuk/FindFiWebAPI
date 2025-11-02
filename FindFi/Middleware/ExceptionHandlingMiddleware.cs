using System.Net;
using System.Text.Json;
using FindFi.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace FindFi.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var factory = context.RequestServices.GetService(typeof(ProblemDetailsFactory)) as ProblemDetailsFactory;

        ProblemDetails problem;
        int statusCode;

        switch (ex)
        {
            case NotFoundException nf:
                statusCode = StatusCodes.Status404NotFound;
                problem = factory?.CreateProblemDetails(context, statusCode, title: "Resource not found", detail: nf.Message) 
                          ?? new ProblemDetails { Status = statusCode, Title = "Resource not found", Detail = nf.Message };
                break;
            case ValidationException ve:
                statusCode = StatusCodes.Status400BadRequest;
                problem = factory?.CreateProblemDetails(context, statusCode, title: "Validation failed", detail: ve.Message)
                          ?? new ProblemDetails { Status = statusCode, Title = "Validation failed", Detail = ve.Message };
                problem.Extensions["errors"] = ve.Errors;
                break;
            case BusinessConflictException bc:
                statusCode = StatusCodes.Status409Conflict;
                problem = factory?.CreateProblemDetails(context, statusCode, title: "Business conflict", detail: bc.Message)
                          ?? new ProblemDetails { Status = statusCode, Title = "Business conflict", Detail = bc.Message };
                break;
            case DomainException de:
                statusCode = StatusCodes.Status422UnprocessableEntity;
                problem = factory?.CreateProblemDetails(context, statusCode, title: "Domain error", detail: de.Message)
                          ?? new ProblemDetails { Status = statusCode, Title = "Domain error", Detail = de.Message };
                break;
            default:
                statusCode = StatusCodes.Status500InternalServerError;
                problem = factory?.CreateProblemDetails(context, statusCode, title: "Unexpected error", detail: "An unexpected error occurred.")
                          ?? new ProblemDetails { Status = statusCode, Title = "Unexpected error", Detail = "An unexpected error occurred." };
                break;
        }

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = problem.Status ?? statusCode;

        var json = JsonSerializer.Serialize(problem, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        });
        await context.Response.WriteAsync(json);
    }
}