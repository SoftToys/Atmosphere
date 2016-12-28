using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atmosphere.Contracts.Models
{
    public class DataResponse<T> : BaseResponse
    {
        public T Data { get; set; }
    }
}
