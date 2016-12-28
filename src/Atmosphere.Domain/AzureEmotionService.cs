using Atmosphere.Contracts;
using Atmosphere.Contracts.Models;
using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Emotion.Contract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Atmosphere.Domain
{
    public class AzureEmotionService : IEmotionService
    {
        private IAnomalyDetectionProvider _anomalyService;
        EmotionServiceClient _client;
        private IEnvironmentConfig _config;
        private IEmotionRespository _emotionStorage;
        private IFrameRepository _framesStorage;

        public AzureEmotionService(IAnomalyDetectionProvider anomalyService, IFrameRepository framesStorage,
            IEmotionRespository emotionStorage, IEnvironmentConfig config)
        {
            _client = new Microsoft.ProjectOxford.Emotion.EmotionServiceClient(_config.CognitiveSubscriptionKey);
            _anomalyService = anomalyService;
            _framesStorage = framesStorage;
            _emotionStorage = emotionStorage;
            _config = config;
        }

        public async Task<BaseResponse> ProcessFrame(Stream imgStream, Guid camID, DateTime timestamp)
        {
            Emotion[] frameEmotions = await _client.RecognizeAsync(imgStream);
            IEnumerable<EmotionStats> emotions = frameEmotions.Select(e => new EmotionStats()
            {
                Emotion = e,
                TimeStamp = timestamp,

            });

            foreach (var emo in emotions)
            {
                if (await _anomalyService.IsAnomaly(emo))
                {
                    emo.Anomaly = true;
                    var res = await _framesStorage.Store(imgStream);
                    emo.ImageUri = res.Data.AbsoluteUri;
                }

                await _emotionStorage.Store(emo);
            }

            return BaseResponse.Ok();
        }

        private IDictionary<string, float> aggregateEmotions(Emotion[] frameEmotions)
        {
            Dictionary<string, float> dic = new Dictionary<string, float>();

            dic[nameof(Scores.Anger)] = frameEmotions.Average(emo => emo.Scores.Anger);
            dic[nameof(Scores.Contempt)] = frameEmotions.Average(emo => emo.Scores.Contempt);
            dic[nameof(Scores.Disgust)] = frameEmotions.Average(emo => emo.Scores.Disgust);
            dic[nameof(Scores.Fear)] = frameEmotions.Average(emo => emo.Scores.Fear);
            dic[nameof(Scores.Happiness)] = frameEmotions.Average(emo => emo.Scores.Happiness);
            dic[nameof(Scores.Neutral)] = frameEmotions.Average(emo => emo.Scores.Neutral);
            dic[nameof(Scores.Sadness)] = frameEmotions.Average(emo => emo.Scores.Sadness);
            dic[nameof(Scores.Surprise)] = frameEmotions.Average(emo => emo.Scores.Surprise);

            return dic;
        }
    }
}
