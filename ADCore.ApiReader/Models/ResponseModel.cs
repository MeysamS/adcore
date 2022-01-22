using System;

namespace ADCore.ApiReader.Models
{

    public class ResponseModel
    {
        public string Data { get; set; }
        public Exception Exception { get; set; }
        public bool IsSuccess => Exception == null;
    }
}
