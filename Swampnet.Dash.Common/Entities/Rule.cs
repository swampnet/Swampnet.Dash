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
        public Rule()
        {
            StateModifiers = new List<StateModifier>();
        }


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

        public Expression(RuleOperatorType op, RuleOperandType operand, string arg, object value)
            : this()
        {
            Operator = op;
            Operand = operand;
            Argument = arg;
            Value = value?.ToString();
        }


        public Expression(RuleOperatorType op, RuleOperandType operand, object value)
            : this(op, operand, null, value)
        {
        }

        public Expression(RuleOperatorType op, object value)
            : this(op, RuleOperandType.Null, null, value)
        {
        }


        public Expression(RuleOperatorType op)
            : this(op, RuleOperandType.Null, null, null)
        {
        }

        [XmlAttribute]
        [JsonConverter(typeof(StringEnumConverter))]
        public RuleOperatorType Operator { get; set; }

        [XmlAttribute]
        [JsonConverter(typeof(StringEnumConverter))]
        public RuleOperandType Operand { get; set; }

        [XmlAttribute]
        public string Argument { get; set; }     // eg, property name

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
                : $"{Operand} {Operator} {Argument} '{Value}'";
        }



        public enum RuleOperandType
        {
            Null,
            TestType,
            Category,
            Summary,
            Property
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
        /// The order in which to process this modifier
        /// </summary>
        [XmlAttribute]
        public int Order { get; set; }

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
        public string Value { get; set; }
    }

}