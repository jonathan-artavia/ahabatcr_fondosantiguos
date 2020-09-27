using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Fondos_Antiguos
{
    public class SqlUtil
    {
        #region Consts
        public const char NAME_ENCLOSURE_CHAR = '`';

        public const char VARCHAR_ENCLOSURE = '"';

        public const string AND = "AND";

        public const string OR = "OR";
        #endregion


        public static string Surround(string unsurroundedString) => $"{VARCHAR_ENCLOSURE}{unsurroundedString}{VARCHAR_ENCLOSURE}";

        public static string SurroundColumn(string columnName) => $"{NAME_ENCLOSURE_CHAR}{columnName}{NAME_ENCLOSURE_CHAR}";

        public static string Encode(string unescapedString)
        {
            return unescapedString;
        }

        public static string Encode(string unescapedString, bool surround)
        {
            string lc_EscapeString(string unescaped)
            {
                StringBuilder res = new StringBuilder();
                int iq = unescaped.IndexOf(VARCHAR_ENCLOSURE);
                int lastIq = -1;
                int inserts = 0;
                while (iq >= 0)
                {
                    res.Append(unescaped.Substring(lastIq + 1, iq - (lastIq + 1)));
                    res.Append(VARCHAR_ENCLOSURE + $" + char({(int)VARCHAR_ENCLOSURE}) + " + VARCHAR_ENCLOSURE);
                    inserts++;
                    lastIq = iq;
                    iq = unescaped.IndexOf(VARCHAR_ENCLOSURE, lastIq + 1);
                }
                if (lastIq > 0)
                    res.Append(unescaped.Substring(lastIq + 1));
                if (res.Length > 0 & !surround)
                    return Surround(res.ToString());
                return res.Length > 0 ? res.ToString() : unescaped;
            }

            if (!surround)
                return lc_EscapeString(unescapedString);
            return Surround(lc_EscapeString(unescapedString));
        }

        public static string Equals(string column, string value) => $"{SurroundColumn(column)} = {value}";

        public static string Equals(string column, string value, bool surround) => $"{SurroundColumn(column)} = { (!surround ? value : Surround(value))}";

        public static string NotEquals(string column, string value) => $"{SurroundColumn(column)} != {value}";

        public static string NotEquals(string column, string value, bool surround) => $"{SurroundColumn(column)} != { (!surround ? value : Surround(value))}";
    }

    public class QueryExpresion
    {
        public string Junction { get; set; }

        public QueryExpresion NextExpression { get; set; }

        public string Expresion { get; set; }

        public QueryExpresion(string junction, string expression)
        {
            this.Junction = junction;
            this.Expresion = expression;
        }

        public QueryExpresion(string expression) : this(null, expression)
        {
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return this.NextExpression != null ? $"{Expresion} {Junction} {this.ToString(NextExpression)}" : $"{Expresion}";
        }

        public virtual string ToString(QueryExpresion last)
        {
            return last.ToString();
        }
    }

    public static class QueryExpresionExtensions
    {
        public static QueryExpresion Append(this QueryExpresion expr, string junction, string nextExpression)
        {
            if (!string.IsNullOrEmpty(nextExpression) && !string.IsNullOrEmpty(junction))
            {
                expr.Junction = junction;
                expr.Last().NextExpression = new QueryExpresion(nextExpression);
            }
            return expr;
        }

        public static QueryExpresion Or(this QueryExpresion expr, string nextExpression)
        {
            if (!string.IsNullOrEmpty(nextExpression))
            {
                expr.Junction = SqlUtil.OR;
                expr.Last().NextExpression = new QueryExpresion(SqlUtil.OR, nextExpression);
            }
            return expr;
        }

        public static QueryExpresion And(this QueryExpresion expr, string nextExpression)
        {
            if (!string.IsNullOrEmpty(nextExpression))
            {
                expr.Junction = SqlUtil.AND;
                expr.Last().NextExpression = new QueryExpresion(SqlUtil.AND, nextExpression);
            }
            return expr;
        }

        /// <summary>
        /// Returns the last expression of the query string
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static QueryExpresion Last(this QueryExpresion expr)
        {
            QueryExpresion result = expr;
            while (expr.NextExpression != null)
            {
                result = expr.NextExpression;
                expr = result;
            }
            return result;
        }
    }
}