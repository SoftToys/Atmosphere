using Microsoft.ProjectOxford.Emotion.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atmosphere.Contracts.Models
{
    public class EmotionStats
    {
        public EmotionStats()
        {
            Id = new Guid();
        }
        public Guid Id { get; set; }
        public DateTime TimeStamp { get; set; }

        public Emotion Emotion { get; set; }
        public bool Anomaly { get; set; }
        public string ImageUri { get; set; }
    }
}
