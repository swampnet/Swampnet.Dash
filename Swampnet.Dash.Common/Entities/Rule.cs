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


    // Expression yoinked directly from evl
    public class Expression
    {
        public Expression()
        {
            IsActive = true;
            Children = new Expression[0];
            Operator = RuleOperatorType.MATCH_ALL;
        }

        public Expression(RuleOperatorType op, RuleOperandType operand, object value)
            : this()
        {
            Operator = op;
            Operand = operand;
            Value = value?.ToString();
        }



        public Expression(RuleOperatorType op, object value)
            : this(op, RuleOperandType.Null, value)
        {
        }


        public Expression(RuleOperatorType op)
            : this(op, RuleOperandType.Null, null)
        {
        }

        [XmlAttribute]
        [JsonConverter(typeof(StringEnumConverter))]
        public RuleOperatorType Operator { get; set; }

        [XmlAttribute]
        [JsonConverter(typeof(StringEnumConverter))]
        public RuleOperandType Operand { get; set; }

        //[XmlAttribute]
        //public string Argument { get; set; }     // eg, property name

		public Property[] Arguments { get; set; }

		[XmlAttribute]
        public string Value { get; set; }

        public Expression[] Children { get; set; }

        [XmlAttribute]
        public bool IsActive { get; set; }

        [JsonIgnore]
        public bool IsContainer => Operator == RuleOperatorType.MATCH_ALL || Operator == RuleOperatorType.MATCH_ANY;

        public override string ToString()
        {
            return IsContainer
                ? $"{Operator} ({Children.Length} children)"
                : $"{Operand} {Operator} '{Value}'";
        }



        public enum RuleOperandType
        {
            Null,
            TestType,
            Category,
            Summary,
            PropertyValue,
			PropertyAverageValue
        }

        public enum RuleOperatorType
        {
            //[Display(Name = "@TODO: Friendly name")]
            NULL,

            EQ,
            NOT_EQ,
            REGEX,
            GT,
            GTE,
            LT,
            LTE,

            TAGGED,
            NOT_TAGGED,

            MATCH_ALL,
            MATCH_ANY
        }
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