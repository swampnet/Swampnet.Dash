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

		public Expression(RuleOperatorType op, string operand, object value)
			: this()
		{
			Operator = op;
			LHS = operand;
			RHS = value?.ToString();
		}



		public Expression(RuleOperatorType op, object value)
			: this(op, null, value)
		{
		}


		public Expression(RuleOperatorType op)
			: this(op, null)
		{
		}

		[XmlAttribute]
		[JsonConverter(typeof(StringEnumConverter))]
		public RuleOperatorType Operator { get; set; }

		[XmlAttribute]
		public string LHS { get; set; }

		[XmlAttribute]
		public string RHS { get; set; }

		public Expression[] Children { get; set; }

		[XmlAttribute]
		public bool IsActive { get; set; }

		[JsonIgnore]
		public bool IsContainer => Operator == RuleOperatorType.MATCH_ALL || Operator == RuleOperatorType.MATCH_ANY;

		public override string ToString()
		{
			return IsContainer
				? $"{Operator} ({Children.Length} children)"
				: $"{LHS} {Operator} {RHS}";
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
