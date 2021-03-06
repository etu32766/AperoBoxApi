﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AperoBoxApi.DTO;

namespace AperoBoxApi.ExceptionsPackage
{
    public class PersonnalExceptionFilter : IExceptionFilter
    {
        private readonly ILogger _logger;

        public PersonnalExceptionFilter(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger("AperoBoxApi.Exceptions");
        }

        void IExceptionFilter.OnException(ExceptionContext context)
        {
            _logger.LogError(context.Exception, "Une erreur inattendue s'est produite");

            if (context.Exception.GetType() == typeof(DbUpdateConcurrencyException))
            {

                var result = new ContentResult()
                {
                    StatusCode = (int)HttpStatusCode.Conflict,
                    Content = Newtonsoft.Json.JsonConvert.SerializeObject(new PersonnalError() { Message = context.Exception.Message }),
                    ContentType = "application/json"

                };
                context.Result = result;
            }
            else
            if (context.Exception.GetType().IsSubclassOf(typeof(PersonnalException)))
            {
                var result = new ContentResult()
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Content = Newtonsoft.Json.JsonConvert.SerializeObject(new PersonnalError() { Message = context.Exception.Message }),
                    ContentType = "application/json"

                };
                context.Result = result;
            }
        }
    }
}