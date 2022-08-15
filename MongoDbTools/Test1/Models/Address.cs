using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDbTools.Test1.Models
{
    internal class Address
    {
        public string Label { get; set; }

        public string Description { get; set; }

        public AddressType AddressType { get; set; }
    }

    public enum AddressType
    {
        Home,
        Work
    }
}
