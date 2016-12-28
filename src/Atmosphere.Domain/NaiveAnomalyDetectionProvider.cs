using Atmosphere.Contracts;
using Atmosphere.Contracts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atmosphere.Domain
{
    public class NaiveAnomalyDetectionProvider : IAnomalyDetectionProvider
    {
        private IEmotionRespository _emotionsRepo;

        public NaiveAnomalyDetectionProvider(IEmotionRespository emotionsRepo)
        {
            _emotionsRepo = emotionsRepo;
        }
        public async Task<bool> IsAnomaly(EmotionStats emotion)
        {
            var dailyResponse = await _emotionsRepo.DailyStats();

            var list = dailyResponse.Data.Reverse();
            var lastKnownDailySadness = list.First(kvp => kvp.Value > 0);

            if (lastKnownDailySadness.Value > 0)
            {
                var diff = Math.Abs(emotion.Emotion.Scores.Sadness - lastKnownDailySadness.Value);
                if (diff > 0.5)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
