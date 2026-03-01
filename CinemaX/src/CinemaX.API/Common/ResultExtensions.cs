using System.Net;
using CinemaX.Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace CinemaX.API.Common;

public static class ResultExtensions
{
    public static IActionResult ToActionResult<T>(this Result<T> result)
    {
        if (result.IsSuccess)
            return new OkObjectResult(result.Value);

        var status = result.Error?.StatusCode ?? HttpStatusCode.InternalServerError;
        
        return new ObjectResult(new {result.Error?.Code, result.Error?.Message}) { StatusCode = (int)status };
    }
}