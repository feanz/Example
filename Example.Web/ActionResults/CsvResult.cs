using System;
using System.Linq;
using System.Web.Mvc;
using System.IO;
using System.Collections;

namespace Web.ActionResults
{
    public class CsvResult: ExportActionResultBase
    {
        /// <summary>
        /// This constructor will assume that the property names of the object are the column headers.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="records"></param>
        public CsvResult(string fileName, IEnumerable records):
            this(fileName, records: records, columnHeaders: GetType(records).GetProperties().Select(p => p.Name).ToArray()) { }

        /// <summary>
        /// This constructor will map the properties to the columnHeaders in order.
        /// </summary>
        /// <param name="fileName"> </param>
        /// <param name="records"></param>
        /// <param name="columnHeaders"></param>
        public CsvResult(string fileName, IEnumerable records, string[] columnHeaders)
        {
            if (GetType(records).GetProperties().Length != columnHeaders.Length)
                throw new ArgumentException("The number of column headers must match the number of properties for the type");

            FileName = fileName;
            Records = records;
            ColumnHeaders = columnHeaders;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var sw = new StringWriter();
            
            foreach (var header in ColumnHeaders)
                sw.Write(string.Format("\"{0}\",", header));

            var properties = GetType(Records).GetProperties();

            foreach (var item in Records)
            {
                if (item != null)
                {
                    sw.WriteLine();
                    foreach (var property in properties)
                    {
                        var obj = property.GetValue(item, null);
                        if (obj != null)
                        {
                            var strValue = obj.ToString();
                            sw.Write(string.Format("\"{0}\",", ReplaceSpecialCharacters(strValue)));
                        }
                        else
                            sw.Write("\"\"");
                    }
                }
            }

            WriteFile(FileName, "application/CSV", sw.ToString());
        }        
    }
}
