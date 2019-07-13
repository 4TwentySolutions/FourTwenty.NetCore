using System;
using System.Collections.Generic;
using System.Text;

namespace FourTwenty.Core.Exceptions
{
    public class ModelException : Exception
    {
        public enum ModelExceptionCode
        {
            NotFound,
            Conflict,
            Custom
        }

        public ModelExceptionCode ErrorCode { get; set; }

        public ModelException(ModelExceptionCode code, string message) : base(message)
        {
            ErrorCode = code;
        }

        public ModelException(ModelExceptionCode code, string message, Exception inner) : base(message, inner)
        {
            ErrorCode = code;
        }

        public ModelException(ModelExceptionCode code)
        {
            ErrorCode = code;
        }
    }
}
