using FluentValidation;
using Fundo.Application.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace Fundo.Applications.WebApi.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ValidationException vex)
            {
                Log.Warning(vex, "Validation error at {Path}", context.Request.Path);
                await WriteProblemDetailsAsync(context, StatusCodes.Status400BadRequest, "Validation error", vex.Message);
            }
            catch (NotFoundException nfex)
            {
                Log.Information(nfex, "Resource not found at {Path}", context.Request.Path);
                await WriteProblemDetailsAsync(context, StatusCodes.Status404NotFound, "Resource not found", nfex.Message);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unhandled exception at {Path}", context.Request.Path);
                await WriteProblemDetailsAsync(context, StatusCodes.Status500InternalServerError, "Internal Server Error", "An unexpected error occurred.");
            }
        }

        private static async Task WriteProblemDetailsAsync(HttpContext context, int statusCode, string title, string detail)
        {
            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail,
                Instance = context.Request.Path
            };

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/problem+json";
            var result = JsonSerializer.Serialize(problemDetails);
            await context.Response.WriteAsync(result);
        }
    }
}
