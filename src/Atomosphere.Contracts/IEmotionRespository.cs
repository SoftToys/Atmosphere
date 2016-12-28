using Atmosphere.Contracts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atmosphere.Contracts
{
    public interface IEmotionRespository
    {
        Task<BaseResponse> Store(EmotionStats emotion);
        Task<DataResponse<IDictionary<string, double>>> DailyStats(DateTime? from = null, DateTime? to = null);
    }
}
