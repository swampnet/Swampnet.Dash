﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Swampnet.Dash.Common.Entities
{
    // @TODO: Yeah, this looks exactly like TestDefinition doesn't it...
    public class ArgosDefinition
    {
        [XmlAttribute]
        public string Id { get; set; }

        [XmlAttribute]
        public string Description { get; set; }

        [XmlAttribute]
        public string Type { get; set; }

        [XmlIgnore]
        public TimeSpan Heartbeat { get; set; }

        // XmlSerializer does not support TimeSpan, so use this property for 
        // serialization instead.
        [Browsable(false)]
        [XmlAttribute(DataType = "duration", AttributeName = "Heartbeat")]
        public string __heartbeat
        {
            get
            {
                return XmlConvert.ToString(Heartbeat);
            }
            set
            {
                Heartbeat = string.IsNullOrEmpty(value)
                    ? TimeSpan.Zero
                    : XmlConvert.ToTimeSpan(value);
            }
        }

        public List<Property> Parameters { get; set; }
    }
}