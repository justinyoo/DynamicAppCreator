using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAC.core.abstractions
{
    public interface IErrorField
    {
        string Name { get; set; }
        string Message { get; set; }
    }
    public interface IApiResult<ResultType>
    {
        public string Message { get; set; }
        public ResultType Result { get; set; }
        public bool state { get; set; }
        public int status { get; set; }
        public Dictionary<string, string> Errors { get; set; }


    }

    public class ApiResult<ResultType> : IApiResult<ResultType>
    {
        public string Message { get; set; }
        public ResultType Result { get; set; }
        public bool state { get; set; } = true;
        public Dictionary<string, string> Errors { get; set; } = new Dictionary<string, string>();
        public int status { get; set; } = 200;
    }

    public class ErrorField : IErrorField
    {
        public static ErrorField Create(string key, string value)
        {
            return new ErrorField() { Name = key, Message = value };
        }
        public string Name { get; set; }
        public string Message { get; set; }
    }

}
