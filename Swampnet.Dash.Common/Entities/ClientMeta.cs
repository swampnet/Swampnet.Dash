using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Swampnet.Dash.Common.Entities
{
    public class ClientMeta
    {
        public ClientMeta()
        {
            Items = new List<ClientItemMeta>();
        }

		[XmlAttribute]
        public string Id { get; set; }
        public List<ClientItemMeta> Items { get; set; }
    }


    public class ClientItemMeta
    {
        public ClientItemMeta()
        {
            Meta = new List<Meta>();
        }

		[XmlAttribute]
		public string Id { get; set; }

		[XmlAttribute]
		public string Name { get; set; }

		public List<Meta> Meta { get; set; }
    }
}
