using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;
using System.Data;

namespace Example.Extensions
{
    public static class ReflectionExtensions
    {
        /// <summary>
        /// Gets the specified attribute from the source property
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        public static T GetAttribute<T>(this PropertyInfo property, bool inherit = true) where T : Attribute
        {
            if (property.IsNotNull())
            {
                return property.GetCustomAttributes(typeof(T), inherit).ValidateNotEmpty("attribute").First().As<T>();
            }
            return null;
        }

        /// <summary>
        /// Get the name of the provided property info
        /// </summary>
        /// <param name="pi"></param>
        /// <returns></returns>
        public static string GetName(this PropertyInfo pi)
        {
            var name = pi.Name;
            var displayName = pi.GetCustomAttributes(typeof(DisplayNameAttribute), true);
            if (displayName.Any())
                name = ((DisplayNameAttribute)displayName.First()).DisplayName;

            return name;
        }       

        /// <summary>
        /// Gets the specified property value from the source object
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static object GetValue(this object instance, string propertyName)
        {
            if (instance.IsNotNull())
            {
                return instance
                    .GetType()
                    .GetProperty(propertyName)
                    .ValidateNotNull("property")
                    .GetValue(instance, null);
            }
            return null;
        }

        /// <summary>
        /// Gets the specified property value from the source object as the specified type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static T GetValue<T>(this object instance, string propertyName)
        {
            if (instance.IsNotNull())
            {
                return instance
                    .GetType()
                    .GetProperty(propertyName)
                    .ValidateNotNull("property")
                    .GetValue(instance, null)
                    .As<T>();
            }
            return default(T);
        }

        /// <summary>
        /// Verifies that the source property has the specified generic attribute
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property"></param>
        /// <param name="inhert"></param>
        /// <returns></returns>
        public static bool HasAttribute<T>(this PropertyInfo property, bool inhert = true) where T : Attribute
        {
            return (property.GetCustomAttributes(typeof(T), true).Length > 0);
        }

        /// <summary>
        /// Checks if type is nullable 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsNullableType(this Type type)
        {
            return type.IsGenericType
            && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>));
        }

        /// <summary>
        /// Sets the specified property to the specified value on the source object
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object SetValue(this object instance, string propertyName, object value)
        {
            if (instance.IsNotNull())
            {
                instance
                    .GetType()
                    .GetProperty(propertyName)
                    .ValidateNotNull("property")
                    .SetValue(instance, value, null);
            }
            return instance;
        }

        /// <summary>
        /// This method creates a DataTable that has a column for each public property of the type.
        /// This will get used for creating excel templates.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static DataTable ToDataTable(this Type type)
        {
            DataTable dtReturn = new DataTable();
            PropertyInfo[] oProps = null;

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
            dtReturn.TableName = type.Name;
            return dtReturn;
        }
    }
}
