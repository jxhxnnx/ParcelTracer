using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaPl.SKS.Package.Services.Interfaces
{
    interface IParcel
    {
        public float? Weight { get; set; }
        public IRecipient recipient { get; set; }
        public IRecipient Sender { get; set; }

        public  string ToString();
        public string ToJson();
        public bool Equals(object obj);
        public bool Equals(IParcel other);
        public int GetHashCode();
    }
}
