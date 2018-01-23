using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Swampnet.Rules
{
    public class Expression
    {
		private readonly ExpressionEvaluator _eval = new ExpressionEvaluator();

		public Expression()
		{
			IsActive = true;
			Children = new Expression[0];
			Operator = ExpressionOperatorType.MATCH_ALL;
		}

		public Expression(ExpressionOperatorType op, string operand, object value)
			: this()
		{
			Operator = op;
			LHS = operand;
			RHS = value?.ToString();
		}



		public Expression(ExpressionOperatorType op, object value)
			: this(op, null, value)
		{
		}


		public Expression(ExpressionOperatorType op)
			: this(op, null)
		{
		}

		[XmlAttribute]
		//[JsonConverter(typeof(StringEnumConverter))]
		public ExpressionOperatorType Operator { get; set; }

		[XmlAttribute]
		public string LHS { get; set; }

		[XmlAttribute]
		public string RHS { get; set; }

		public Expression[] Children { get; set; }

		[XmlAttribute]
		public bool IsActive { get; set; }

		//[JsonIgnore]
		public bool IsContainer => Operator == ExpressionOperatorType.MATCH_ALL || Operator == ExpressionOperatorType.MATCH_ANY;

		/// <summary>
		/// Evaluate expression against supplied arguments
		/// </summary>
		/// <param name="args"></param>
		/// <returns></returns>
		public bool Evaluate(IContext context)
		{
			return _eval.Evaluate(this, context);
		}

		public override string ToString()
		{
			return IsContainer
				? $"{Operator} ({Children.Length} children)"
				: $"{LHS} {Operator} {RHS}";
		}
	}

	public enum ExpressionOperatorType
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
