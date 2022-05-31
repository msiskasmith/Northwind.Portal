using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Northwind.Portal.Controllers;
using Northwind.Portal.Models;
using System;

namespace Northwind.Portal.CustomFilters
{
    public class LogExceptionFilter : IExceptionFilter
    {
        private readonly ILogger _logger;
        public LogExceptionFilter(ILogger<LogExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            var errorMessage = $"{context.Exception.StackTrace} | Time {DateTime.UtcNow.ToLongTimeString()}" ;
            _logger.LogInformation(errorMessage);
        }
    }
}
