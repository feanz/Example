using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data;
using System.Reflection;
using System.ComponentModel;

namespace Example.Extensions
{
    public static class CollectionExtensions
    {
        /// <summary>
        /// Append the specified bit array to the specific bit array
        /// </summary>
        /// <param name="current"></param>
        /// <param name="after"></param>
        /// <returns></returns>
        public static BitArray Append(this BitArray current, BitArray after)
        {
            var bools = new bool[current.Count + after.Count];
            current.CopyTo(bools, 0);
            after.CopyTo(bools, current.Count);
            return new BitArray(bools);
        }

        /// <summary>
        /// Creates a generic list of the specified type from the source items
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public static IList<T> AsEnumerable<T>(this IEnumerable items)
        {
            if (items.IsNotNull())
            {
                var list = new List<T>();
                foreach (var item in items)
                {
                    list.Add(item.As<T>());
                }
                return list;
            }
            return null;
        }
        
        /// <summary>
        /// Combines the source items using the specified separator with optional spacing
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        public static string Combine<T>(this IEnumerable<T> items, string delimiter = ",", bool includeSpacing = false)
        {
            if (items.IsNotEmpty())
            {
                return string.Join(delimiter + (includeSpacing ? " " : string.Empty), items.Select(i => i.ToString()).ToArray());
            }
            return null;
        }

        /// <summary>
        /// Enumerates the specified list of items and applies the specified action
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (var item in collection) action(item);
            return collection;
        }

        /// <summary>
        /// Indicates if the specified list is empty
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public static bool IsEmpty<T>(this IEnumerable<T> items)
        {
            return (items.IsNull() || items.Count().Equals(0));
        }

        /// <summary>
        /// Indicates if the specified list contains some items
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public static bool IsNotEmpty<T>(this IEnumerable<T> items)
        {
            return !items.IsEmpty();
        }

        /// <summary>
        /// Converts string[] array to arry of integers
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static int[] ToIntArray(this string[] source)
        {
            return source.Select(x => int.Parse(x, System.Globalization.CultureInfo.CurrentCulture)).ToArray();
        }

        /// <summary>
        /// Creates a text list from the specified items
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="newLine"></param>
        /// <returns></returns>
        public static string TextList(this IEnumerable<string> items, string newLine = "\r\n")
        {
            return string.Join(newLine, items.ToArray());
        }

        /// <summary>
        /// Change list to comma seperated string
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string ToCommaSeparatedString(this IEnumerable<string> list)
        {
            return ToDelimitedString(list, ",");
        }

        /// <summary>
        /// Convert IEnumerable to a datatable
        /// </summary>
        /// <param name="varlist"></param>
        /// <returns></returns>
        public static DataTable ToDataTable(this IEnumerable varlist)
        {
            DataTable dtReturn = new DataTable();
            PropertyInfo[] oProps = null;

            foreach (var rec in varlist)
            {
                if (dtReturn.Columns.Count == 0)
                {
                    // Use reflection to get property names, to create table
                    // column names
                    var type = (Type)rec.GetType();
                    dtReturn.TableName = type.Name;
                    oProps = type.GetProperties();

                    foreach (PropertyInfo pi in oProps)
                    {
                        if (!pi.PropertyType.IsNullableType())
                            dtReturn.Columns.Add(pi.GetName(), pi.PropertyType);
                        else
                        {
                            NullableConverter converter = new NullableConverter(pi.PropertyType);
                            dtReturn.Columns.Add(pi.GetName(), converter.UnderlyingType);
                        }
                    }
                }

                DataRow dr = dtReturn.NewRow();
                foreach (PropertyInfo pi in oProps)
                {
                    if (!pi.PropertyType.IsNullableType())
                        dr[pi.GetName()] = pi.GetValue(rec, null);
                    else
                    {
                        object obj = pi.GetValue(rec, null);
                        if (obj != null)
                        {
                            NullableConverter converter = new NullableConverter(pi.PropertyType);
                            dr[pi.GetName()] = Convert.ChangeType(obj, converter.UnderlyingType);
                        }
                    }
                }
                dtReturn.Rows.Add(dr);
            }

            return (dtReturn);
        }

        /// <summary>
        /// Converts a generic List collection to a single string using the specified delimitter.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="delimiter">The delimiter.</param>
        /// <returns></returns>
        public static string ToDelimitedString(this IEnumerable<string> list, string delimiter)
        {
            return list == null ? string.Empty : string.Join(delimiter, list);
        }

        /// <summary>
        /// Validates that the specified items contains at least one item
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="context"></param>
        public static IList<T> ValidateNotEmpty<T>(this IList<T> items, string context = "list")
        {
            if (items.IsEmpty())
            {
                throw new Exception(string.Format("The specified {0} was empty.", context));
            }
            return items;
        }
    }
}
