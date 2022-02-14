using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

public partial class DataGetter : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    

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
        // get all commands issued in the last 10 seconds
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