using System;
using System.Collections.Generic;
using System.Text;

namespace Swampnet.Dash.Common.Entities
{
    public class ClientMeta
    {
        public ClientMeta()
        {
            Items = new List<ClientItemMeta>();
        }
        public string Id { get; set; }
        public ICollection<ClientItemMeta> Items { get; set; }
    }


    public class ClientItemMeta
    {
        public ClientItemMeta()
        {
            Meta = new List<Meta>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Meta> Meta { get; set; }
    }
}
