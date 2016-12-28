using System;
using Atmosphere.Contracts;
using Microsoft.Extensions.Configuration;

namespace Atmosphere.Web
{
    internal class EnvironmentConfig : IEnvironmentConfig
    {
        private IConfiguration _root;

        //public EnvironmentConfig(IConfiguration configuration)
        //{
        //    _root = configuration;
        //}

        public string CognitiveSubscriptionKey
        {
            get
            {
                return _root.GetValue<string>("CognitiveSubscriptionKey");
            }
        }

        public string ElasticUri
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public string ESAuthToken
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public string StorageConnectionString
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}