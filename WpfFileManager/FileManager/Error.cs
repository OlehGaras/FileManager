using System;

namespace FileManager
{
    public class Error
    {
        public string ErrorType { get; set; }
        public string ErrorMessage { get; set; }

        public Error(Exception exception)
        {
            ErrorType = exception.GetType().ToString();
            ErrorMessage = exception.Message;
        }

        public Error()
        {
        }
    }
}
