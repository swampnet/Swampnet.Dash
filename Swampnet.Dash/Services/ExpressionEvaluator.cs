using System;
using System.Text.RegularExpressions;
using System.Linq;
using Swampnet.Dash.Common.Entities;
using static Swampnet.Dash.Common.Entities.Expression;

namespace Swampnet.Dash.Services
{
    /// <summary>
    /// Expression Evaluator
    /// </summary>
    class ExpressionEvaluator
    {
        public bool Evaluate(Expression expression, Element evt)
        {
            bool result = false;

            switch (expression.Operator)
            {
                case RuleOperatorType.MATCH_ALL:
                    result = MatchAll(expression, evt);
                    break;

                case RuleOperatorType.MATCH_ANY:
                    result = MatchAny(expression, evt);
                    break;

                case RuleOperatorType.EQ:
                    result = EQ(GetOperand(expression, evt), expression.Value);
                    break;

                case RuleOperatorType.NOT_EQ:
                    result = !EQ(GetOperand(expression, evt), expression.Value);
                    break;

                case RuleOperatorType.REGEX:
                    result = MatchExpression(GetOperand(expression, evt), expression.Value);
                    break;

                case RuleOperatorType.LT:
                    result = LT(GetOperand(expression, evt), expression.Value);
                    break;

                case RuleOperatorType.LTE:
                    result = EQ(GetOperand(expression, evt), expression.Value)
                          || LT(GetOperand(expression, evt), expression.Value);
                    break;

                case RuleOperatorType.GT:
                    result = GT(GetOperand(expression, evt), expression.Value);
                    break;

                case RuleOperatorType.GTE:
                    result = EQ(GetOperand(expression, evt), expression.Value)
                          || GT(GetOperand(expression, evt), expression.Value);
                    break;

                default:
                    throw new NotImplementedException(expression.Operator.ToString());
            }

            return result;
        }


        private string GetOperand(Expression expression, Element evt)
        {
            string op = "";

            switch (expression.Operand)
            {
                // Just Output then?
                case RuleOperandType.Property:
                    op = evt.Output.StringValue(expression.Argument);
                    break;
            }

            return op;
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
        private bool MatchAll(Expression expression, Element evt)
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
        private bool MatchAny(Expression expression, Element evt)
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