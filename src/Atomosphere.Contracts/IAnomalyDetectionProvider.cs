using Atmosphere.Contracts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atmosphere.Contracts
{
    public interface IAnomalyDetectionProvider
    {
        Task<bool> IsAnomaly(EmotionStats emotion);
    }
}
