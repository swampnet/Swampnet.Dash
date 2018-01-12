using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Swampnet.Dash.Common.Entities
{
    // Yeah, see, this is all looking very much like the Evl stuff. Maybe it's time we pulled all this out
    // into some kind of generic rule / expression parsing service.
    public class Rule
    {
		private static long _id = 0;

        public Rule()
        {
			Id = ++_id;
            StateModifiers = new List<StateModifier>();
        }

		[XmlIgnore]
		public long Id { get; private set; }
		public Expression Expression { get; set; }
        public List<StateModifier> StateModifiers { get; set; }
    }




    public class StateModifier
    {
        /// <summary>
        /// How many consecucive times the expression has to be true before applying this state
        /// </summary>
        public int? ConsecutiveHits { get; set; }

        /// <summary>
        /// How long the expression has to be true for before applying this state
        /// </summary>
        public TimeSpan? Elapsed { get; set; }

        /// <summary>
        /// The target state
        /// </summary>
        [XmlAttribute]
        public Status Value { get; set; }
    }

}