using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using Atmosphere.Web.Models;
using Atmosphere.Contracts;

namespace Atmosphere.Web.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {                
        private IEmotionService _emotion;

        public ValuesController(IEmotionService emotion)
        {
            _emotion = emotion;
        }
        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public async void Put([FromBody]FrameRequest request)
        {
            using (Stream stream = request.Frame.OpenReadStream())
            {
                await _emotion.ProcessFrame(stream, request.CamID, request.Timestamp);
            }
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
