using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atmosphere.Contracts
{
    public interface IEnvironmentConfig
    {
        string CognitiveSubscriptionKey { get; }
        string ElasticUri { get; set; }
        string ESAuthToken { get; set; }
        string StorageConnectionString { get; }
    }
}
