using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ICSharpCode.SharpZipLib.Zip;

public partial class DownloadSurvayData : System.Web.UI.Page
{


    protected void Page_Load(object sender, EventArgs e)
    {
        string survay_id = Request.QueryString["survay_id"] + "";

        if (survay_id != "")
        {
            string zip_name = survay_id + ".zip";

            string report_name = survay_id + ".csv";
            List<Dictionary<string, string>> rows = dbHandler.getTableData("Partner_CRM_Survays", "id", int.Parse(survay_id));
            if (rows.Count > 0)
            {
                report_name = rows[0]["name"] + " - " + rows[0]["city"] + ".csv";
                zip_name = rows[0]["name"] + " - " + rows[0]["city"] + ".zip";
            }

            // we have to generate the csv report
            string report = generate_csv_report(survay_id);
            
            string path = Server.MapPath("photos/" + report_name);

            using (var w = new StreamWriter(path, false, Encoding.UTF8))
            {
                w.Write(report);
                w.Flush();

            }
            System.Threading.Thread.Sleep(3000);
            List<string> zipFileList = new List<string>();
            zipFileList.Add("photos/" + report_name);
            zipFileList.AddRange(get_media_files(survay_id));            

            DownloadZipToBrowser(zipFileList, zip_name);
        }


        /*List<string> zipFileList = new List<string>();
        zipFileList.Add("photos/290678.jpg");
        zipFileList.Add("photos/290682.jpg");
        zipFileList.Add("photos/report.txt");


        DownloadZipToBrowser(zipFileList, "290682.zip");*/
    }



    private List<string> get_media_files(string survay_id)
    {
        List<string> ret = new List<string>();
        List<Dictionary<string, string>> rows = dbHandler.getTableData("Partner_CRM_Govs", "survay_id", survay_id);
        if (rows.Count > 0)
        {
            foreach (Dictionary<string, string> row in rows)
            {
                if (row["is_to_show"] == "True")
                {
                    string image = row["image_path_url"].Replace("#", "");
                    string movie = row["video_path_url"].Replace("#", "");

                    if (image != "")
                    {
                        ret.Add(image);
                    }
                    if (movie != "")
                    {
                        ret.Add(movie);
                    }
                }

            }
        }


        //sb.Append(templete_end);



        return ret;

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

    // the aspx page has just one line e.g. <%@ Page language="c#" Codebehind=...
    // but if you must run this from within a page that has other output, start with a Response.Clear();


    // This will accumulate each of the files named in the fileList into a zip file,
    // and stream it to the browser.
    // This approach writes directly to the Response OutputStream.
    // The browser starts to receive data immediately which should avoid timeout problems.
    // This also avoids an intermediate memorystream, saving memory on large files.
    //
    private void DownloadZipToBrowser(List<string> zipFileList, string zipFileName)
    {

        Response.ContentType = "application/zip";
        // If the browser is receiving a mangled zipfile, IIS Compression may cause this problem. Some members have found that
        //Response.ContentType = "application/octet-stream" has solved this. May be specific to Internet Explorer.

        Response.AppendHeader("content-disposition", "attachment; filename=\"" + zipFileName + "\"");
        Response.CacheControl = "Private";
        Response.Cache.SetExpires(DateTime.Now.AddMinutes(3)); // or put a timestamp in the filename in the content-disposition

        byte[] buffer = new byte[4096];

        ZipOutputStream zipOutputStream = new ZipOutputStream(Response.OutputStream);
        zipOutputStream.SetLevel(3); //0-9, 9 being the highest level of compression

        foreach (string fileName in zipFileList)
        {
            string path = Server.MapPath(fileName);
            try
            {
                Stream fs = File.OpenRead(path);    // or any suitable inputstream

                ZipEntry entry = new ZipEntry(ZipEntry.CleanName(fileName));
                entry.Size = fs.Length;
                // Setting the Size provides WinXP built-in extractor compatibility,
                //  but if not available, you can set zipOutputStream.UseZip64 = UseZip64.Off instead.

                zipOutputStream.PutNextEntry(entry);

                int count = fs.Read(buffer, 0, buffer.Length);
                while (count > 0)
                {
                    zipOutputStream.Write(buffer, 0, count);
                    count = fs.Read(buffer, 0, buffer.Length);
                    if (!Response.IsClientConnected)
                    {
                        break;
                    }
                    Response.Flush();
                }
                fs.Close();
            }
            catch (Exception)
            {

                //throw;
            }
        }
        zipOutputStream.Close();

        Response.Flush();
        Response.End();
    }
}