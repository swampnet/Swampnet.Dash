using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Swampnet.Dash.Common.Entities
{
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

		[XmlAttribute]
		public string Argument { get; set; }

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
}
