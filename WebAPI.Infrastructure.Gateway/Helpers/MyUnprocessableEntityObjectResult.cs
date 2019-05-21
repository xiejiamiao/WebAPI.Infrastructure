using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace WebAPI.Infrastructure.Gateway.Helpers
{
    public class MyUnprocessableEntityObjectResult:UnprocessableEntityObjectResult
    {
        public MyUnprocessableEntityObjectResult(ModelStateDictionary modelState):base(new ResourceValidationResult(modelState))
        {
            if(modelState==null)
                throw new ArgumentNullException(nameof(modelState));
            StatusCode = 422;
        }
    }
}