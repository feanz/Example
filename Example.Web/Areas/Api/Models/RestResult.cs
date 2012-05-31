namespace Web.Areas.Api.Models
{
    public class RestResult
    {
        public int ErrorCode;
        public string Message = string.Empty;
        public object Data;

        public RestResult()
        {
        }

        public RestResult(string message)
        {
            Message = message;
        }

        public RestResult(int code, string message)
        {
            ErrorCode = code;
            Message = message;
        }

        public RestResult(int code, object data)
        {
            ErrorCode = code;
            Data = data;
        }

        public RestResult(int code, string message, object data)
        {
            ErrorCode = code;
            Message = message;
            Data = data;
        }
    }
}