using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;


public static class ApiErrorHandler
{

    public static IActionResult HandleApiError(Exception ex, bool isDevelopment)
    {
        var (statusCode, body) = GenerateErrorResponse(ex, isDevelopment);

        Console.WriteLine(JsonSerializer.Serialize(new { Error = ex.Message, StackTrace = ex.StackTrace }));
        return new ObjectResult(body) { StatusCode = statusCode };
    }

   
    public static (int StatusCode, object Body) GenerateErrorResponse(Exception ex, bool isDevelopment)
    {
        var statusCode = ex switch
        {
            ArgumentNullException => 400,
            ValidationException => 400,
            UnauthorizedAccessException => 401,
            InvalidOperationException => 409,
            _ => 500
        };

        var response = new
        {
            Success = false,
            Message = ex switch
            {
                ArgumentNullException => "A required argument was null.",
                ValidationException => ex.Message,
                UnauthorizedAccessException => "You are not authorized to perform this action.",
                InvalidOperationException => "A conflict occurred in the request.",
                _ => "An unexpected error occurred."
            },
            Details = isDevelopment ? ex.ToString() : null
        };

        return (statusCode, response);
    }
}
