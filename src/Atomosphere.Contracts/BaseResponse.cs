using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atmosphere.Contracts
{
    public class BaseResponse
    {
        public int ErrorCode { get; set; }
        public string Description { get; set; }

        public bool IsOk { get { return ErrorCode == 0; } }

        public BaseResponse Set(int errorCode = 0, string description = null)
        {
            ErrorCode = errorCode;
            Description = description;
            return this;
        }

        public BaseResponse Reset()
        {
            return Set();
        }

        public static BaseResponse Ok()
        {
            return new BaseResponse();
        }
        public static BaseResponse TechnicalError()
        {
            return new BaseResponse().Set(1, "TechnicalError");
        }
    }
}
