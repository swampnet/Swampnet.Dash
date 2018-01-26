using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Swampnet.Rules
{
    class ExpressionEvaluator
    {
		private static readonly Regex _match = new Regex(@"{(?<name>.*)}", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		public bool Evaluate(Expression expression, IContext context)
		{
			bool result = false;
			string lhs = GetValue(expression.LHS, context);
			string rhs = GetValue(expression.RHS, context);

			switch (expression.Operator)
			{
				case ExpressionOperatorType.MATCH_ALL:
					result = MatchAll(expression, context);
					break;

				case ExpressionOperatorType.MATCH_ANY:
					result = MatchAny(expression, context);
					break;

				case ExpressionOperatorType.EQ:
					result = EQ(lhs, rhs);
					break;

				case ExpressionOperatorType.NOT_EQ:
					result = !EQ(lhs, rhs);
					break;

				case ExpressionOperatorType.REGEX:
					result = MatchExpression(lhs, rhs);
					break;

				case ExpressionOperatorType.LT:
					result = LT(lhs, rhs);
					break;

				case ExpressionOperatorType.LTE:
					result = EQ(lhs, rhs)
						  || LT(lhs, rhs);
					break;

				case ExpressionOperatorType.GT:
					result = GT(lhs, rhs);
					break;

				case ExpressionOperatorType.GTE:
					result = EQ(lhs, rhs)
						  || GT(lhs, rhs);
					break;

				default:
					throw new NotImplementedException(expression.Operator.ToString());
			}

			return result;
		}


		private string GetValue(string source, IContext context)
		{
			string result = source; // default to literal value

			if (!string.IsNullOrEmpty(source))
			{
				var m = _match.Match(source);

				// It's a {lookup}
				if (m.Success)
				{
					var r = context.GetParameterValue(m.Groups["name"].Value);
					result = r == null
						? ""
						: r.ToString();
				}
			}

			return result;
		}



		private bool EQ(string operand, string value)
		{
			return operand.Equals(value, StringComparison.InvariantCultureIgnoreCase);
		}

		/// <summary>
		/// LT - Less than
		/// </summary>
		/// <remarks>
		/// Currently only supports numeric and dates
		/// </remarks>
		private bool LT(string operand, string value)
		{
			if (double.TryParse(operand, out double lhs_nmber) && double.TryParse(value, out double rhs_number))
			{
				return lhs_nmber < rhs_number;
			}

			if (DateTime.TryParse(operand, out DateTime lhs_date) && DateTime.TryParse(value, out DateTime rhs_date))
			{
				return lhs_date < rhs_date;
			}

			return false;
		}

		/// <summary>
		/// GT - Greater than
		/// </summary>
		/// <remarks>
		/// Currently only supports numeric and dates
		/// </remarks>
		private bool GT(string operand, string value)
		{
			if (double.TryParse(operand, out double lhs_nmber) && double.TryParse(value, out double rhs_number))
			{
				return lhs_nmber > rhs_number;
			}

			if (DateTime.TryParse(operand, out DateTime lhs_date) && DateTime.TryParse(value, out DateTime rhs_date))
			{
				return lhs_date > rhs_date;
			}

			return false;
		}

		/// <summary>
		/// Match regular expression
		/// </summary>
		/// <returns></returns>
		private bool MatchExpression(string operand, string value)
		{
			return Regex.IsMatch(operand, value, RegexOptions.IgnoreCase);
		}

		/// <summary>
		/// Returns true if all children expressions evaluate to true
		/// </summary>
		private bool MatchAll(Expression expression, IContext context)
		{
			foreach (var child in expression.Children)
			{
				if (!Evaluate(child, context))
				{
					return false;
				}
			}

			return true;
		}


		/// <summary>
		/// Returns true if at least one child expression evaluates to true
		/// </summary>
		/// <remarks>
		/// This will return true on the first expressionthat returns true, so may not evaluate all the expressions
		/// </remarks>
		private bool MatchAny(Expression expression, IContext context)
		{
			foreach (var child in expression.Children)
			{
				if (Evaluate(child, context))
				{
					return true;
				}
			}
			return false;
		}
	}
}
