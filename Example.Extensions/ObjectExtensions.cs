using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Reflection;
using System.Xml.Serialization;
using System.IO;

namespace Example.Extensions
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Casts the specified instance as the specified type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static T As<T>(this object instance)
        {
            if (instance.IsNotNull())
            {
                if (instance.Is<IConvertible>())
                {
                    return (T)Convert.ChangeType(instance, typeof(T));
                }
                else
                {
                    return (T)instance;
                }
            }
            return default(T);
        }

        /// <summary>
        /// Parses the source instance as the specified enumeration
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public static T AsEnum<T>(this object instance, bool ignoreCase = true)
        {
            if (instance.IsNotNull())
            {
                return (T)Enum.Parse(typeof(T), instance.ToString(), ignoreCase);
            }
            return default(T);
        }

        /// <summary>
        /// Determines if the specified instance is of the specified type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool Is<T>(this object instance)
        {
            var checkType = typeof(T);
            var instanceType = (instance is Type ? (Type)instance : instance.GetType());
            if (checkType.IsInterface)
            {
                return instanceType.Equals(checkType) || instanceType.GetInterface(checkType.Name).IsNotNull();
            }
            else
            {
                return instanceType.Equals(checkType) || instanceType.IsSubclassOf(checkType);
            }
        }

        /// <summary>
        /// Determines if the specified instance is not null
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool IsNotNull(this object instance)
        {
            return !instance.IsNull();
        }

        /// <summary>
        /// Determines if the source instance is not equal to zero
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool IsNotZero(this object instance)
        {
            return !instance.IsZero();
        }

        /// <summary>
        /// Determines if the specified instance is null
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool IsNull(this object instance)
        {
            return instance == null;
        }

        /// <summary>
        /// Determines if the source instance is equal to zero
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool IsZero(this object instance)
        {
            return instance.IsNotNull() && instance.ToString().Equals("0");
        }

        /// <summary>
        /// Validates that the specified source instance is not null using the specified context in the error message if it is
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static T ValidateNotNull<T>(this T instance, string context = "object")
        {
            if (instance.IsNull())
            {
                throw new Exception(string.Format("The specified {0} was null.", context));
            }
            return instance;
        }

        /// <summary>
        /// Validates that the specified instance is not zero
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static T ValidateNotZero<T>(this T instance, string context = "object")
        {
            instance.ValidateNotNull(context);
            if (instance.ToString().Equals("0"))
            {
                throw new Exception(string.Format("The specified {0} was zero and failed validation."));
            }
            return instance;
        }

        /// <summary>
        /// Returns a string representation using an invariant culture
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static string ToStringInvariantCulture(this object instance)
        {
            return instance.ToString().ToString(System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns a string representation using the Current culture
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static string ToStringCurrentCulture(this object instance)
        {
            return instance.ToString().ToString(System.Globalization.CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Serialize object to XML
        /// </summary>
        /// <param name="toBeSerialized"></param>
        /// <returns></returns>
        public static string ToXML(this object toBeSerialized)
        {
            if (!toBeSerialized.GetType().FullName.Contains("AnonymousType"))
            {
                XmlSerializer serializer = new XmlSerializer(toBeSerialized.GetType());

                using (StringWriter sw = new StringWriter())
                {
                    serializer.Serialize(sw, toBeSerialized);
                    return sw.ToString();
                }
            }

            // Create the default root XML element
            XElement root = new XElement("AnonymousTypeRoot");

            // Start to build the child XML elements by recursion method
            BuildXmlElement(toBeSerialized, ref root);

            return root.ToString();
        }

        private static void BuildXmlElement(object obj, ref XElement element)
        {
            if (obj == null)
                return;

            // Get all properyInfo

            PropertyInfo[] pis = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo pi in pis)
            {
                // Get the property value
                object o = pi.GetValue(obj, null);

                if (o != null)
                {
                    Type t = o.GetType();

                    // If the property value is an array, retrieve each object value inside
                    if (t.IsArray)
                    {
                        // Build the element with the property's name
                        XElement newElement = new XElement(pi.Name);

                        object[] arrs = o as object[];

                        for (int i = 0; i < arrs.Length; i++)
                        {
                            XElement arrayElement = new XElement("Element");

                            // For each the array element, build the child XML elements
                            BuildXmlElement(arrs[i], ref arrayElement);
                            newElement.Add(arrayElement);
                        }

                        element.Add(newElement);
                    }

                    else
                    {
                        // For the anonymous type and other entity class type
                        if (t.IsClass && t.Name != "String")
                        {
                            XElement newElement = new XElement(pi.Name);
                            BuildXmlElement(o, ref newElement);
                            element.Add(newElement);
                        }

                        // For other value type and string type, build the XML element
                        else
                        {
                            XElement newElement = new XElement(pi.Name, o.ToString());
                            element.Add(newElement);
                        }

                    }

                }

            }
        }        
    }
}
