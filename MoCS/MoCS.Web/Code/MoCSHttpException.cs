using System;

namespace MoCS.Web.Code
{
    public class MoCSHttpException : Exception
    {
        public int HttpCode { get; set; }

        public MoCSHttpException(int httpCode, string message)
            : base(message)
        {
            HttpCode = httpCode;
        }
    }
}