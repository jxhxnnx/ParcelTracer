using PaPl.SKS.BusinessLogic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaPl.SKS.BusinessLogic.Interfaces
{
    public interface ITrackParcelLogic
    {
        public DataAccess.Entities.Parcel TrackParcel(string trackingId);
    }
}
