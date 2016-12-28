using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Atmosphere.Contracts
{
    public interface IEmotionService
    {        
        Task<BaseResponse> ProcessFrame(Stream stream, Guid camID, DateTime timestamp);
    }
}
