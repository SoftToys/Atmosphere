using Atmosphere.Contracts.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Atmosphere.Contracts
{
    public interface IFrameRepository
    {
        Task<DataResponse<Uri>> Store(Stream imgStream);
        Task<DataResponse<List<Uri>>> List(DateTime day);
    }
}
