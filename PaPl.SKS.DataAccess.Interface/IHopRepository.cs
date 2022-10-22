using PaPl.SKS.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaPl.SKS.DataAccess.Repository
{
    public interface IHopRepository
    {
        public void Create(Hop hop);
        public void Update(Hop hop);
        public void Delete(string code);

        public IEnumerable<Hop> GetAllHops();
        public Hop GetHopByCode(string code);
        public Hop GetRootHop(Hop hop);
        public Hop GetHops();

        void BigRedButton();
    }
}
