using System;
using System.Text.RegularExpressions;
using System.Linq;
using Swampnet.Dash.Common.Entities;
using static Swampnet.Dash.Common.Entities.Expression;
using Swampnet.Dash.Common.Interfaces;

namespace Swampnet.Dash.Services
{
    /// <summary>
    /// Expression Evaluator
    /// </summary>
    class ExpressionEvaluator : IExpressionEvaluator
	{
		private static readonly Regex _match = new Regex(@"{(?<name>.*)}", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		public bool Evaluate(Expression expression, ElementState state)
        {
            bool result = false;
			string lhs = GetValue(expression.Operand, state);
			string rhs = GetValue(expression.Value, state);

            switch (expression.Operator)
            {
                case RuleOperatorType.MATCH_ALL:
                    result = MatchAll(expression, state);
                    break;

                case RuleOperatorType.MATCH_ANY:
                    result = MatchAny(expression, state);
                    break;

                case RuleOperatorType.EQ:
                    result = EQ(lhs, rhs);
                    break;

                case RuleOperatorType.NOT_EQ:
                    result = !EQ(lhs, rhs);
                    break;

                case RuleOperatorType.REGEX:
                    result = MatchExpression(lhs, rhs);
                    break;

                case RuleOperatorType.LT:
                    result = LT(lhs, rhs);
                    break;

                case RuleOperatorType.LTE:
                    result = EQ(lhs, rhs)
                          || LT(lhs, rhs);
                    break;

                case RuleOperatorType.GT:
                    result = GT(lhs, rhs);
                    break;

                case RuleOperatorType.GTE:
                    result = EQ(lhs, rhs)
                          || GT(lhs, rhs);
                    break;

                default:
                    throw new NotImplementedException(expression.Operator.ToString());
            }

            return result;
        }


		private string GetValue(string source, ElementState state)
		{
			string result = source;

			if (!string.IsNullOrEmpty(source))
			{
				var m = _match.Match(source);

				// It's a {lookup}
				if (m.Success)
				{
					string lookup = m.Groups["name"].Value;
					switch (lookup)
					{
						case "TimestampUtc":
							result = state.TimestampUtc.ToString("s");
							break;

						// Not a known property - Look at state output
						default:
							result = state.Output.StringValue(lookup);
							break;
					}
				}
			}

			return result;
		}



		private bool EQ(string operand, string value)
        {
            return operand.EqualsNoCase(value);
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
        private bool MatchAll(Expression expression, ElementState evt)
        {
            foreach (var child in expression.Children)
            {
                if (!Evaluate(child, evt))
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
        private bool MatchAny(Expression expression, ElementState evt)
        {
            foreach (var child in expression.Children)
            {
                if (Evaluate(child, evt))
                {
                    return true;
                }
            }
            return false;
        }
    }
}