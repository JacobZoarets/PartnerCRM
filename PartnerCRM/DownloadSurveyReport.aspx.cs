using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class DownloadSurveyReport : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string survay_id = Request.QueryString["survay_id"] + "";

        if (survay_id != "")
        {
            // we have to generate the csv report
            string report = generate_csv_report(survay_id);

            string report_name = survay_id + ".csv";
            List<Dictionary<string, string>> rows = dbHandler.getTableData("Partner_CRM_Survays", "id", int.Parse(survay_id));
            if (rows.Count > 0)
            {
                report_name = rows[0]["name"] + " - " + rows[0]["city"] + ".csv";
            }



            string path = Server.MapPath("photos/" + report_name);

            using (var w = new StreamWriter(path, false, Encoding.UTF8))
            {
                w.Write(report);
                w.Flush();

            }
            System.Threading.Thread.Sleep(3000);

            // read input etx
            Response.Buffer = false;
            Response.ContentType = "text/plain";
            Response.AppendHeader("content-Disposition", "attachment;filename=" + report_name );
            //string path = Server.MapPath("photos/" + survay_id + ".csv");

            FileInfo file = new FileInfo(path);
            int len = (int)file.Length, bytes;
            Response.AppendHeader("content-length", len.ToString());
            byte[] buffer = new byte[1024];
            Stream outStream = Response.OutputStream;
            using (Stream stream = File.OpenRead(path))
            {
                while (len > 0 && (bytes =
                    stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    outStream.Write(buffer, 0, bytes);
                    len -= bytes;
                }
            }

        }


    }



    private string generate_csv_report(string survay_id)
    {

        string templete_title =
            "מספר גוב" + "," +
            "סוג הגוב" + "," +
            "הערות" + "," +
            "תפוסת צנרת" + "," +
            "סוג מתקן" + "," +
            "מרחק" + "," +
            "מרחק מגוב מספר" + "," +           
            "תאריך " + "," +           
            "תמונה ";


        StringBuilder sb = new StringBuilder();
        sb.AppendLine(templete_title);

        List<Dictionary<string, string>> rows = dbHandler.getTableData("Partner_CRM_Govs", "survay_id", survay_id);
        if (rows.Count > 0)
        {
            foreach (Dictionary<string, string> row in rows)
            {
                if (row["is_to_show"] == "True")
                {
                    string templete_line =
                         "{0}" + "," +
                         "{1}" + "," +
                         "{2}" + "," +
                         "{3}" + "," +
                         "{4}" + "," +
                         "{5}" + "," +
                         "{6}" + "," +
			 "{7}" + "," +
                         "{8}";

                    sb.AppendLine(string.Format(templete_line,
                        row["number"],
                        row["type"],
                        row["notes"],
                        row["plumbing"],
                        row["contraction_type"],
                        row["distance"],
                        row["distance_from_gov_id"],
                        row["dateTime"],
                        row["image_path_url"]
                        ));
                }

            }
        }


        //sb.Append(templete_end);



        return sb.ToString();

    }


}