using Atmosphere.Contracts;
using Atmosphere.Contracts.Models;
using Nest;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

namespace Atmosphere.Domain
{
    public class ElasticEmotionRespository : IEmotionRespository
    {
        private ElasticClient _client;
        private IEnvironmentConfig _config;
        private ConnectionSettings _settings;

        public ElasticEmotionRespository(IEnvironmentConfig config)
        {
            _config = config;
            var node = new Uri(config.ElasticUri);
            _settings = new ConnectionSettings(node);
            _settings.GlobalHeaders(new NameValueCollection { {
                "Authorization", config.ESAuthToken } });
            _settings.DefaultIndex("atmo-emotions");
            _settings.RequestTimeout(TimeSpan.FromSeconds(2));
            _settings.ThrowExceptions(true);
            _client = new ElasticClient(_settings);
        }
        public async Task<BaseResponse> Store(EmotionStats emotion)
        {
            try
            {
                await _client.IndexAsync<EmotionStats>(emotion);
            }
            catch (Exception e)
            {
                BaseResponse.TechnicalError();
            }

            return BaseResponse.Ok();
        }

        public async Task<DataResponse<IDictionary<string, double>>> DailyStats(DateTime? from = null, DateTime? to = null)
        {
            IDictionary<string, double> dailyAvg = new SortedDictionary<string, double>();
            var aggs = await _client.SearchAsync<EmotionStats>(s => s
                .Aggregations(a => a
                    .Filter("dates", f => f

                        .Filter(fd => fd
                            .DateRange(dr => dr.GreaterThan(from ?? DateTime.Now.AddDays(-14)).Field(o => o.TimeStamp).LessThanOrEquals(to ?? DateTime.Now.Date))
                        )
                        .Aggregations(fa => fa
                            .DateHistogram("dh", dh => dh
                                .Format("dd-MM")
                                .Interval(DateInterval.Day)
                                .Aggregations(innerAg => innerAg
                                    .Average("sadnessAvg", e => e.Field("sadness"))
                                )
                              )
                         )
                    )
                )
            );

            var buckets = aggs.Aggs.Filter("dates").DateHistogram("dh").Buckets;

            foreach (var bucket in buckets)
            {
                dailyAvg[bucket.KeyAsString] = bucket.Average("sadnessAvg").Value ?? 0;
            }
            return new DataResponse<IDictionary<string, double>>() { Data = dailyAvg };
        }

    }
}
