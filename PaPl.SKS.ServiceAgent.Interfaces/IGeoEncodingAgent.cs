
using NetTopologySuite.Geometries;
using PaPl.SKS.BusinessLogic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaPl.SKS.DataAccess.ServiceAgent.Interfaces
{
    public interface IGeoEncodingAgent
    {
        Coordinate EncodeAddress(Recipient address);
    }
}
