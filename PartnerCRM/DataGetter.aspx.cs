using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Globalization;
using System.IO;
using System.IO.Compression;

public partial class DataGetter : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }


    [WebMethod]
    public static string add_new_survay(string name, string sum, string desc, string client_id)
    {
        try
        {
            Dictionary<string, Object> values = new Dictionary<string, object>();

            values.Add("name", name.Replace("'", ""));
            values.Add("sum", Convert.ToInt32(sum.Replace("'", "")));
            values.Add("description", desc.Replace("'", ""));
            values.Add("client_id", client_id.Replace("'", ""));
            values.Add("dateTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            return dbHandler.addNewContent("Jacob_cards", values).ToString();
        }
        catch (Exception)
        {
            return "false";
        }
       


    }

    [WebMethod]
    public static string get_all_survays(string client_id)
    {
        string templete_start = "<table class='table table-hover' style='direction:rtl'>" +
                        "<thead>" +
                            "<tr>" +
                                "<th>שם </th>" +
                                "<th>סכום</th>" +                                
                                "<th >תאריך</th>" +
                            "</tr>" +
                        "</thead>" +
                        "<tbody>";


        string templete_end = "</tbody>" +
                    "</table>";

        StringBuilder sb = new StringBuilder();
        sb.Append(templete_start);



        Dictionary<string, object> queryKeyAndValue = new Dictionary<string, object>();

        queryKeyAndValue.Add("client_id", client_id);

        List<Dictionary<string, string>> rows = dbHandler.getTableData("Jacob_cards", queryKeyAndValue);
        if (rows.Count > 0)
        {
            for (int i = rows.Count - 1; i > -1; i--)
            {
                Dictionary<string, string> row = rows[i];
                string tpl = "<tr>" +
                        "<td><a href='SurvayDetails.html?card_id={3}' style='color:black;text-decoration: underline;'>{0}</a></td>" +
                        "<td>{1} ₪</td>" +                        
                        "<td style='direction: ltr;'>{2}</td>" +
                    "</tr>";

                DateTime d = DateTime.Parse(row["dateTime"]);

                sb.Append(string.Format(tpl, row["name"], row["sum"], d.ToShortDateString(), row["id"]));
            }

        }


        sb.Append(templete_end);



        return sb.ToString();
    }


    [WebMethod]
    public static string get_all_govs(string survay_id, int page_start)
    {
        string templete_start = "<table class='table table-hover' style='direction:rtl'>" +
                        "<thead>" +
                            "<tr>" +
                                "<th>מספר גוב</th>" +
                                "<th>סוג גוב</th>" +
                                "<th>תאריך</th>" +
                                "<th> </th>" +
                            "</tr>" +
                        "</thead>" +
                        "<tbody>";


        string templete_end = "</tbody>" +
                    "</table>";

        int max_page_count = 100;


        StringBuilder sb = new StringBuilder();
        sb.Append(templete_start);
        int counter = 0;
        List<Dictionary<string, string>> rows = dbHandler.getTableData("Partner_CRM_Govs", "survay_id", survay_id);
        if (rows.Count > 0)
        {
            foreach (Dictionary<string, string> row in rows)
            {
                if (row["is_to_show"] == "True")
                {
                    string tpl = "<tr>" +
                        "<td>{0}</td>" +
                        "<td>{1}</td>" +
                        "<td>{2}</td>" +
                        "<td><a href='GovDetails.html?gov_id={3}&survay_id={4}'>פתח</a></td>" +
                    "</tr>";

                    if (counter >= page_start)
                    {
                        sb.Append(string.Format(tpl, row["number"], row["type"], row["dateTime"], row["id"], row["survay_id"]));

                        if (counter > (page_start + max_page_count))
                        {
                            break;
                        }
                    }
                    counter++;
                }

            }
        }


        string prev_link = "";
        string prev_link_display = "none";
        if (page_start > 0)
        {
            prev_link = "SingleSurvay.html?survay_id=" + survay_id + "&page_start=" + (page_start - max_page_count);
            prev_link_display = "block";
        }

        string next_link = "";
        string next_link_display = "none";
        if (page_start + max_page_count < rows.Count)
        {
            next_link = "SingleSurvay.html?survay_id=" + survay_id + "&page_start=" + (page_start + max_page_count);
            next_link_display = "block";
        }

        string res_text = "תוצאות " + (page_start + 1).ToString() + "-" + (counter) + "";

        string paging_row = "<tr>" +
                        "<td><a style='display:{3}' href='{0}'>דף קודם</a></td>" +
                        "<td colspan='2' >{1}</td>" +
                        "<td><a style='display:{4}' href='{2}'>דף הבא</a></td>" +
                    "</tr>";

        string paging = string.Format(paging_row, prev_link, res_text, next_link, prev_link_display, next_link_display);



        sb.Append(paging);

        sb.Append(templete_end);



        return sb.ToString();
    }

    public static string get_all_govsBK(string survay_id)
    {
        string templete_start = "<table class='table table-hover' style='direction:rtl'>" +
                        "<thead>" +
                            "<tr>" +
                                "<th>מספר גוב</th>" +
                                "<th>סוג גוב</th>" +
                                "<th>תאריך</th>" +
                                "<th> </th>" +
                            "</tr>" +
                        "</thead>" +
                        "<tbody>";


        string templete_end = "</tbody>" +
                    "</table>";

        StringBuilder sb = new StringBuilder();
        sb.Append(templete_start);
        int counter = 0;
        List<Dictionary<string, string>> rows = dbHandler.getTableData("Partner_CRM_Govs", "survay_id", survay_id);
        if (rows.Count > 0)
        {
            foreach (Dictionary<string, string> row in rows)
            {
                if (row["is_to_show"] == "True")
                {
                    string tpl = "<tr>" +
                        "<td>{0}</td>" +
                        "<td>{1}</td>" +
                        "<td>{2}</td>" +
                        "<td><a href='GovDetails.html?gov_id={3}&survay_id={4}'>פתח</a></td>" +
                    "</tr>";

                    //DateTime dt = DateTime.ParseExact(row["dateTime"], "dd/MM/yyyy h:mm:ss tt", CultureInfo.InvariantCulture); 

                    sb.Append(string.Format(tpl, row["number"], row["type"], row["dateTime"], row["id"], row["survay_id"]));

                    counter++;
                    if (counter > 100)
                    {
                        break;
                    }
                }

            }
        }


        sb.Append(templete_end);



        return sb.ToString();
    }

    [WebMethod]
    public static string get_card_data(string card_id)
    {
        card_id = card_id.Replace("#", "");
        List<Dictionary<string, string>> rows = dbHandler.getTableData("Jacob_cards", "id", int.Parse(card_id));
        if (rows.Count > 0)
        {
            foreach (Dictionary<string, string> row in rows)
            {

                return row["name"] + ";" + row["sum"] + ";" + row["description"] + ";" + row["id"];
            }
        }

        return "" + ";" + "" + ";" + "";
    }

    [WebMethod]
    public static string get_all_gov_ids(string survay_id)
    {

        StringBuilder sb = new StringBuilder();

        List<Dictionary<string, string>> rows = dbHandler.getTableData("Partner_CRM_Govs", "survay_id", survay_id);
        if (rows.Count > 0)
        {
            foreach (Dictionary<string, string> row in rows)
            {
                if (row["is_to_show"] == "True")
                {
                    string tpl = "<option id='option_{0}' value='{0}'>{0}</option>";
                    if (sb.ToString() == "")
                    {
                        // <option disabled selected>בחר גוב מקור</option>
                        sb.Append("<option disabled selected>בחר גוב מקור</option>");
                    }

                    sb.Append(string.Format(tpl, row["number"]));
                }

            }
        }

        return sb.ToString();
    }


    [WebMethod]
    public static string get_gov_details(string gov_id)
    {
        List<Dictionary<string, string>> rows = dbHandler.getTableData("Partner_CRM_Govs", "id", int.Parse(gov_id));
        if (rows.Count > 0)
        {
            foreach (Dictionary<string, string> row in rows)
            {

                return row["number"] + ";" +
                       row["type"] + ";" +
                       row["notes"] + ";" +
                       row["plumbing"] + ";" +
                        row["contraction_type"] + ";" +
                        row["image_path_url"] + ";" +
                        row["video_path_url"] + ";" +
                        row["distance"] + ";" +
                        row["distance_from_gov_id"];
            }
        }

        return "" + ";" + "" + ";" + "";
    }

    [WebMethod]
    public static string add_new_gov(string survay_id, string number, string type, string notes, string plumbing, string contraction_type, string image_path_url, string video_path_url, string distance, string distance_from_gov_id)
    {
        Dictionary<string, Object> values = new Dictionary<string, object>();

        values.Add("survay_id", survay_id);
        values.Add("number", number.Replace("'", ""));
        values.Add("type", type.Replace("'", ""));
        values.Add("notes", notes.Replace("'", ""));
        values.Add("plumbing", plumbing.Replace("'", ""));
        values.Add("contraction_type", contraction_type.Replace("'", ""));
        values.Add("image_path_url", image_path_url.Replace("'", ""));
        values.Add("video_path_url", video_path_url.Replace("'", ""));

        values.Add("distance", distance.Replace("'", ""));
        values.Add("distance_from_gov_id", distance_from_gov_id.Replace("'", ""));

        values.Add("is_to_show", 1);
        values.Add("dateTime", DateTime.Now.ToString());

        return dbHandler.addNewContent("Partner_CRM_Govs", values).ToString();


    }

    [WebMethod]
    public static string update_card(int card_id, string name, string sum, string description)
    {
        Dictionary<string, object> queryKeyAndValue = new Dictionary<string, object>();
        queryKeyAndValue.Add("id", card_id);

        Dictionary<string, Object> values = new Dictionary<string, object>();
        values.Add("name", name.Replace("'", ""));
        values.Add("sum", sum.Replace("'", ""));
        values.Add("description", description.Replace("'", ""));
        //values.Add("dateTime", DateTime.Now.ToString());

        return dbHandler.updateContent("Jacob_cards", queryKeyAndValue, values).ToString();
    }

    [WebMethod]
    public static string get_report_by_dates(string start_date, string end_date, string type)
    {
        if(start_date.IndexOf(' ') > 0){
            start_date = start_date.Split(' ')[0];
        }
        start_date = start_date.Trim() + " 00:00:00";

        if (end_date.IndexOf(' ') > 0)
        {
            end_date = end_date.Split(' ')[0];
        }
        end_date = end_date.Trim() + " 23:59:59";

        string sql = "select * from Jacob_cards where [dateTime] >= '" + start_date + "' and dateTime < '" + end_date + "'";

        List<Dictionary<string, string>> cards_list = dbHandler.getTableDataSqlQuery(sql);

        return get_report_with_sum(cards_list);
    }

    public static string get_report_with_sum(List<Dictionary<string, string>> rows)
    {
        StringBuilder sb = new StringBuilder();

        string templete_start = "<table class='table table-hover' style='direction:rtl'>" +
                        "<thead>" +
                            "<tr>" +
                                "<th>שם </th>" +
                                "<th>סכום</th>" +
                                "<th style='direction: ltr;'>תאריך</th>" +
                                "<th> </th>" +
                            "</tr>" +
                        "</thead>" +
                        "<tbody>";



        sb.Append(templete_start);
        string tpl = "";
        int total_sum = 0;
        if (rows.Count > 0)
        {
            for (int i = rows.Count - 1; i > -1; i--)
            {
                Dictionary<string, string> row = rows[i];
                tpl = "<tr>" +
                        "<td><a href='SurvayDetails.html?card_id={3}' style='color:black;text-decoration: underline;'>{0}</a></td>" +
                        "<td>{1} ₪</td>" +
                        "<td>{2}</td>" +
                    "</tr>";

                int sum = Convert.ToInt32(row["sum"].ToString());

                total_sum += sum;

                DateTime d = DateTime.Parse(row["dateTime"]);

                sb.Append(string.Format(tpl, row["name"], row["sum"], d.ToShortDateString(), row["id"]));
            }

        }


        tpl = "<tr>" +
                "<td > <b>{0}</b></td>" +
                "<td style='direction: ltr;'><b>{1}</b></td>" +
                "<td>{2}</td>" +
            "</tr>";

        sb.Append(string.Format(tpl, "סך הכל", " ₪ " + total_sum  , ""));


        string templete_end = "</tbody>" +
                    "</table>";

        sb.Append(templete_end);



        return sb.ToString();
    }

    [WebMethod]
    public static string update_gov(string gov_id, string number, string type, string notes, string plumbing, string contraction_type, string image_path_url, string video_path_url, string distance, string distance_from_gov_id)
    {
        Dictionary<string, object> queryKeyAndValue = new Dictionary<string, object>();
        queryKeyAndValue.Add("id", gov_id);

        Dictionary<string, Object> values = new Dictionary<string, object>();
        values.Add("number", number.Replace("'", ""));
        values.Add("type", type.Replace("'", ""));
        values.Add("notes", notes.Replace("'", ""));
        values.Add("plumbing", plumbing.Replace("'", ""));
        values.Add("contraction_type", contraction_type.Replace("'", ""));
        values.Add("image_path_url", image_path_url.Replace("'", ""));
        values.Add("video_path_url", video_path_url.Replace("'", ""));
        values.Add("distance", distance.Replace("'", ""));
        values.Add("distance_from_gov_id", distance_from_gov_id.Replace("'", ""));
        values.Add("is_to_show", 1);
        values.Add("dateTime", DateTime.Now.ToString());

        return dbHandler.updateContent("Partner_CRM_Govs", queryKeyAndValue, values).ToString();
    }

    [WebMethod]
    public static string check_user_login_details(string user_name, string password)
    {
        //Dictionary<string, object> queryKeyAndValue = new Dictionary<string, object>();
        //queryKeyAndValue.Add("user_name", user_name.ToLower());
        //queryKeyAndValue.Add("password", password.ToLower());

        if (password == "1234")
        {
            return "user_name" + ";" + "password" + ";" + "1" + ";" + "client_id";    
        }
        return "";

        
        //List<Dictionary<string, string>> rows = dbHandler.getTableData("Partner_CRM_admins", queryKeyAndValue);
        //if (rows.Count > 0)
        //{
        //    foreach (Dictionary<string, string> row in rows)
        //    {
        //        return row["user_name"] + ";" +row["password"] + ";" + row["type"] + ";" + row["client_id"];
        //    }
        //}

        // return "";
    }

    [WebMethod]
    public static string delete_gov(string gov_id)
    {
        Dictionary<string, object> queryKeyAndValue = new Dictionary<string, object>();
        queryKeyAndValue.Add("id", gov_id);

        Dictionary<string, object> fieldsAndValues = new Dictionary<string, object>();
        fieldsAndValues.Add("is_to_show", 0);

        return dbHandler.updateContent("Partner_CRM_Govs", queryKeyAndValue, fieldsAndValues).ToString();
    }

    [WebMethod]
    public static string delete_survey(string survey_id)
    {
        Dictionary<string, object> queryKeyAndValue = new Dictionary<string, object>();
        queryKeyAndValue.Add("id", int.Parse(survey_id));

        Dictionary<string, object> fieldsAndValues = new Dictionary<string, object>();
        fieldsAndValues.Add("is_to_show", 0);

        return dbHandler.updateContent("Partner_CRM_Survays", queryKeyAndValue, fieldsAndValues).ToString();
    }

    [WebMethod]
    public static string delete_card(string card_id)
    {

        Dictionary<string, object> queryKeyAndValue = new Dictionary<string, object>();
        queryKeyAndValue.Add("id", card_id);

        return dbHandler.deleteContent("Jacob_cards", queryKeyAndValue).ToString();
    }

    //private static int delete_file(string path) {

    //    string path = Server.MapPath("/photos");
    //    DirectoryInfo source = new DirectoryInfo(path);
    //    int count = 0;
    //    // Get info of each file into the directory
    //    foreach (FileInfo fi in source.GetFiles())
    //    {
    //        var creationTime = fi.CreationTime;

    //        DateTime week = DateTime.Now - new TimeSpan(7, 0, 0, 0);

    //        if (creationTime < week)
    //        {
    //            if (fi.Name.Contains(".jpg") || fi.Name.Contains(".mp4"))
    //            {
    //                //fi.Delete();
    //                count++;
    //                string s = count + " " + fi.Name + " " + creationTime.ToString();
    //                sb.AppendLine(s);
    //                //break;
    //            }


    //        }
    //    }

    //}

    [WebMethod]
    public static string server_add_new_command(string command, string server_name)
    {
        Dictionary<string, Object> values = new Dictionary<string, object>();

        values.Add("command", command.Replace("'", ""));
        values.Add("server_name", server_name.Replace("'", ""));
        values.Add("dateTime", DateTime.Now.ToString());

        dbHandler.addNewContent("Bet365_Server_commands", values);
        return dbHandler.getLastAddedIndex("Bet365_Server_commands").ToString();
    }

    [WebMethod]
    public static string client_get_server_command(string last_command_id)
    {
        int last_command = -1;

        int.TryParse(last_command_id, out last_command);

        List<Dictionary<string, string>> rows = dbHandler.getTableData("Bet365_Server_commands");
        if (rows.Count > 0)
        {
            //for (int i = rows.Count - 1; i > -1; i--)
            //{
            //    if (int.Parse(rows[i]["id"]) > last_command)
            //    {
            //        return rows[i]["id"] + ";" + rows[i]["command"];
            //    }
            //    else
            //        break;
            //}

        }

        return "" + ";" + "";
    }

    [WebMethod]
    public static string client_add_new_data(string html_data, string server_name)
    {
        Dictionary<string, Object> values = new Dictionary<string, object>();

        values.Add("html_data", html_data.Replace("'", ""));
        values.Add("server_name", server_name.Replace("'", ""));
        values.Add("dateTime", DateTime.Now.ToString());


        dbHandler.addNewContent("Bet365_Clients_data", values);

        return dbHandler.getLastAddedIndex("Bet365_Clients_data").ToString();

    }

    [WebMethod]
    public static string server_get_client_data(string last_command_id)
    {
        int last_command = -1;

        int.TryParse(last_command_id, out last_command);

        List<Dictionary<string, string>> rows = dbHandler.getTableData("Bet365_Server_commands");
        if (rows.Count > 0)
        {
            //for (int i = rows.Count - 1; i > -1; i--)
            //{
            //    if (int.Parse(rows[i]["id"]) > last_command)
            //    {
            //        return rows[i]["id"] + ";" + rows[i]["command"];
            //    }
            //    else
            //        break;
            //}

        }

        return "" + ";" + "";
    }

}

