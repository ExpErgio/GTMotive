using System;
using GtMotive.Estimate.Microservice.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace GtMotive.Estimate.Microservice.Api.Filters {
    public sealed class BusinessExceptionFilter : IExceptionFilter {
        private readonly ILogger<BusinessExceptionFilter> _logger;

        public BusinessExceptionFilter(ILogger<BusinessExceptionFilter> logger) => _logger = logger;

        public void OnException(ExceptionContext context) {
            ArgumentNullException.ThrowIfNull(context);
            var status = context.Exception switch {
                NotFoundException => StatusCodes.Status404NotFound,
                DomainException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
            };
            if (status == StatusCodes.Status500InternalServerError) _logger.LogError(context.Exception, "Excepción no controlada.");
            else _logger.LogWarning("Excepción de negocio: {Status} - {Detail}", status, context.Exception.Message);
            var problemDetails = new ProblemDetails { Status = status, Title = ReasonPhrases.GetReasonPhrase(status), Detail = context.Exception.Message, Instance = context.HttpContext.Request.Path };
            context.Result = new ObjectResult(problemDetails) { StatusCode = status };
            context.ExceptionHandled = true;
        }
    }
}
