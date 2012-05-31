using System;
using System.Globalization;
using System.Linq;
using MvcMiniProfiler;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Utilities.Extensions;

namespace Web.App_Start
{
    /// <summary>
    /// Formats Oracle queries with a DECLARE up top for parameter values
    /// </summary>
    public class OracleFormatter : ISqlFormatter
    {

        static readonly Dictionary<DbType, Func<SqlTimingParameter, string>> paramTranslator;

        static Func<SqlTimingParameter, string> GetWithLenFormatter(string native)
        {
            var capture = native;
            return p =>
                       {
                           if (p.Size < 1) { return capture; }
                           return capture + "(" + (p.Size > 8000 ? "max" : p.Size.ToString(CultureInfo.InvariantCulture)) + ")";
                       };
        }

        static OracleFormatter()
        {
            paramTranslator = new Dictionary<DbType, Func<SqlTimingParameter, string>>
            {
                {DbType.AnsiString, GetWithLenFormatter("varchar2")},
                {DbType.String, GetWithLenFormatter("nvarchar2")},
                {DbType.AnsiStringFixedLength, GetWithLenFormatter("char")},
                {DbType.StringFixedLength, GetWithLenFormatter("nchar")},
                {DbType.Binary, GetWithLenFormatter("raw") },
                {DbType.Byte, p => "byte"},
                {DbType.Double, p => "double"},
                {DbType.Decimal, p => "decimal"},
                {DbType.Int16,  GetWithLenFormatter("number")},
                {DbType.Int32,  GetWithLenFormatter("number")},
                {DbType.Int64,  GetWithLenFormatter("number")},                
                {DbType.DateTime, p => "date"},
                {DbType.Guid, GetWithLenFormatter("raw")},
                {DbType.Boolean, p => "char(1)"},                
                {DbType.Time, p => "TimeStamp"},                
                {DbType.Single, p => "single"},                
            };

        }

        /// <summary>
        /// Formats the SQL in a Oracle-Server friendly way, with DECLARE statements for the parameters up top.
        /// </summary>
        /// <param name="timing">The SqlTiming to format</param>
        /// <returns>A formatted SQL string</returns>
        public string FormatSql(SqlTiming timing)
        {
            if (timing.Parameters == null || timing.Parameters.Count == 0)
            {
                return timing.CommandString;
            }

            var buffer = new StringBuilder();

            buffer.Append("DECLARE ");

            var first = true;
            foreach (var p in timing.Parameters)
            {
                if (first)
                {
                    first = false;                    
                }
                else
                {
                    buffer.AppendLine(",").Append(new string(' ', 8));
                }

                DbType parsed;
                string resolvedType = null;
                if (!Enum.TryParse(p.DbType, out parsed))
                {
                    resolvedType = p.DbType;
                }

                if (resolvedType.IsNull())
                {
                    Func<SqlTimingParameter, string> translator;
                    if (paramTranslator.TryGetValue(parsed, out translator))
                    {
                        resolvedType = translator(p);
                    }
                    resolvedType = resolvedType ?? p.DbType;
                }

                var niceName = p.Name;
                //variable/paramter prefix
                //if (!niceName.StartsWith("v_"))
                //{
                //    niceName = "v_" + niceName;
                //}

                buffer.Append(niceName).Append(" ").Append(resolvedType).Append(" = ").Append(PrepareValue(p));
            }
            return buffer
                .AppendLine()
                .AppendLine()
                .Append(timing.CommandString)
                .ToString();
        }

        static readonly string[] dontQuote = new[] { "Int16", "Int32", "Int64", "Boolean" };
        private static string PrepareValue(SqlTimingParameter p)
        {
            if (p.Value.IsNull())
            {
                return "null";
            }

            if (dontQuote.Contains(p.DbType))
            {
                return p.Value;
            }

            const string prefix = "";
            return prefix + "'" + p.Value.Replace("'", "''") + "'";
        }
    }
}