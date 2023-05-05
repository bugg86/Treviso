using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using ClosedXML.Excel;
using Newtonsoft.Json;


namespace oTSPA.AppService.BotFunctions.Services;

public class DataManipulationTool
{
    public static DataTable GetDataTableFromCSVFile(MemoryStream stream)
    {
        var csvString = Encoding.ASCII.GetString(stream.ToArray());
        string[] lines;
        lines = csvString.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        string[] Fields;
        Fields = lines[0].Split(new char[] { ',' });
        int Cols = Fields.GetLength(0);
        DataTable dt = new DataTable();
        for (int i = 0; i < Cols; i++)
        {
            dt.Columns.Add(Fields[i], typeof(string));
        }
        DataRow Row;
        for (int i = 1; i < lines.GetLength(0); i++)
        {
            Fields = lines[i].Split(new char[] { ',' });
            Row = dt.NewRow();
            for (int f = 0; f < Cols; f++)
                Row[f] = Fields[f];
            dt.Rows.Add(Row);
        }
        return dt;
    }
    public static string DataTableToJson(DataTable dataTable)
    {
        var settings = new JsonSerializerSettings { DateFormatString = "MM/dd/yy HH:mm" };
        string JSONString = string.Empty;
        JSONString = JsonConvert.SerializeObject(dataTable, Formatting.Indented, settings);
        JSONString = JSONString.Replace("null", "\"\"");
        return JSONString;
    }
}