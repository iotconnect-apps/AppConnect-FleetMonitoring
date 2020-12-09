using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace host.iot.solution.Filter
{
    public class ValidationError
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Field { get; }

        public string Message { get; }

        public ValidationError(string field, string message)
        {
            Field = field != string.Empty ? field : null;
            Message = message;
        }
    }
    public class ValidationResultModel
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public string Time { get; set; }
        public string LastSyncDate { get; set; }
        public List<ValidationError> Data { get; set; }
        public ValidationResultModel(ModelStateDictionary modelState)
        {
           
                this.IsSuccess = false;
                this.Time = DateTime.Now.ToLongDateString();
                this.Message = "Validation Failed";
                this.Data = modelState.Keys
                    .SelectMany(key => modelState[key].Errors.Select(x => new ValidationError(key, x.ErrorMessage)))
                    .ToList();
            
        }
    }
    public class ValidationFailedResult : OkObjectResult
    {
        public ValidationFailedResult(ModelStateDictionary modelState) 
            : base(new ValidationResultModel(modelState))
        {
        }
    }
}