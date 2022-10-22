using PaPl.SKS.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaPl.SKS.DataAccess.Repository
{
    public interface IParcelRepository
    {
        public void Create(Parcel parcel);
        public void Update(Parcel parcel, HopArrival deleteArrival);
        public void Delete(string id);

        IEnumerable<Parcel> GetAllParcels();
        Parcel GetParcelById(string id);
        public void GetHopArrivalsToCode(Parcel parcel);
    }
}
