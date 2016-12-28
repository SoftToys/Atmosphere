using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atmosphere.Web.Models
{
    public class FrameRequest
    {
        public IFormFile Frame { get; set; }
        public DateTime Timestamp { get; set; }
        public Guid CamID { get; set; }

    }
}
