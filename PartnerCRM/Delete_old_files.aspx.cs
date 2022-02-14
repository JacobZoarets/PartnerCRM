using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

public partial class Delete_old_filest : System.Web.UI.Page
{

    protected void Page_Load(object sender, EventArgs e)
    {
        string survey_id =  Request.QueryString["survey_id"] + "";
        if (survey_id != "")
        {
            Response.Write(delete_survey_media(survey_id));
        }
    }


    public string delete_survey_media(string survey_id)
    {
        List<Dictionary<string, string>> rows = dbHandler.getTableData("Partner_CRM_Govs", "survay_id", int.Parse(survey_id));
        bool res = false;
        int counter = 0;
        foreach (Dictionary<string, string> row in rows)
        {
            string image_path = row["image_path_url"] + "";
            image_path = image_path.Replace("#", "");

            if (image_path != "")
            {
                res = delete_file(image_path);
                if (res == true)
                {
                    counter += 1;
                }
            }
            string video_path = row["video_path_url"] + "";
            video_path = video_path.Replace("#", "");
            if (video_path != "")
            {
                res = delete_file(video_path);
                if (res == true)
                {
                    counter += 1;
                }
            }
        }

        return counter.ToString();

    }

    protected bool delete_file(string file_name)
    {
        bool ret = false;
        string path = Server.MapPath("/photos");
        DirectoryInfo source = new DirectoryInfo(path);
//        int count = 0;
        StringBuilder sb = new StringBuilder();

        string[] vals = file_name.Split('/');
        if (vals.Length >1 )
        {
            file_name = vals[vals.Length - 1];
        }


        //sb.AppendLine("Deletion Report - " + DateTime.Now.ToString());
        //sb.AppendLine("-----------------------------------------");
        // Get info of each file into the directory
        foreach (FileInfo fi in source.GetFiles())
        {
            if (fi.Name == file_name)
            {
                fi.Delete();                
                string s = DateTime.Now.ToString() + " " + fi.Name + " " + fi.CreationTime;
                sb.AppendLine(s);
                ret = true;
                break;
            }
        }
        if (sb.ToString() != "")
        {
            write_log(sb.ToString());
        }

        return ret;
    }

    



protected void Page_Load_old(object sender, EventArgs e)
{
    string path = Server.MapPath("/photos");
    DirectoryInfo source = new DirectoryInfo(path);
    int count = 0;
    StringBuilder sb = new StringBuilder();
    sb.AppendLine("Deletion Report - " + DateTime.Now.ToString());
    sb.AppendLine("-----------------------------------------");
    // Get info of each file into the directory
    foreach (FileInfo fi in source.GetFiles())
    {
        var creationTime = fi.CreationTime;

        DateTime week = DateTime.Now - new TimeSpan(7, 0, 0, 0);

        if (creationTime < week)
        {
            if (fi.Name.Contains(".jpg") || fi.Name.Contains(".mp4"))
            {
                //fi.Delete();
                count++;
                string s = count + " " + fi.Name + " " + creationTime.ToString();
                sb.AppendLine(s);
                //break;
            }


        }
    }

    //send_notification_mail(sb.ToString());
    //send();
    sb.AppendLine("");
    write_log(sb.ToString());

}

private void write_log(string delete_notification)
{


    System.IO.StreamWriter sw = new System.IO.StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "photos/deleteImagesLog.txt", true);
    sw.Write(delete_notification);
    sw.Close();

}

private void send_notification_mail(string delete_notification)
{
    var client = new SmtpClient("smtp.gmail.com", 587);
    client.UseDefaultCredentials = true;
    //(2)
    client.Credentials = new System.Net.NetworkCredential("john.hariss11@gmail.com", "yac6597216");

    /* {
         Host = "smtp.gmail.com",
         Port = 587,
         DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network,
         UseDefaultCredentials = false,
         Credentials = new NetworkCredential("john.hariss11@gmail.com", "yac6597216")

         //Credentials = new NetworkCredential("yacovz@gmail.com", "0507966679"),
         //EnableSsl = true,
         //UseDefaultCredentials = false
     };*/

    client.Send("john.hariss11@gmail.com", "yacovz@gmail.com", "patner crm server notification " + DateTime.Now.ToString(), delete_notification);
    //Console.WriteLine("Sent");
    //Console.ReadLine();
}

private void send1()
{
    // System.Web.Mail.SmtpMail.SmtpServer is obsolete in 2.0
    // System.Net.Mail.SmtpClient is the alternate class for this in 2.0
    SmtpClient smtpClient = new SmtpClient();
    MailMessage message = new MailMessage();

    try
    {
        MailAddress fromAddress = new MailAddress("john.hariss11@gmail.com", "john.hariss11@gmail.com");

        // You can specify the host name or ipaddress of your server
        // Default in IIS will be localhost
        smtpClient.Host = "localhost";

        //Default port will be 25
        smtpClient.Port = 25;
        smtpClient.Host = "smtp.gmail.com";

        //From address will be given as a MailAddress Object
        message.From = fromAddress;

        // To address collection of MailAddress
        message.To.Add("yacovz@gmail.com");
        message.Subject = "Feedback";

        // CC and BCC optional
        // MailAddressCollection class is used to send the email to various users
        // You can specify Address as new MailAddress("admin1@yoursite.com")
        //message.CC.Add("shashik@xxxxx.com");
        //message.CC.Add("admin2@yoursite.com");

        //Body can be Html or text format
        //Specify true if it  is html message
        message.IsBodyHtml = false;

        // Message body content
        message.Body = "hi";



        smtpClient.UseDefaultCredentials = false;
        smtpClient.Credentials = new NetworkCredential("john.hariss11@gmail.com", "yac6597216");


        // Send SMTP mail
        smtpClient.Send(message);

        //lblStatus.Text = "Email successfully sent.";
    }
    catch (System.Net.Mail.SmtpException ex)
    {
        Response.Write(ex.ToString());
    }
    catch (Exception ex)
    {
        Response.Write("Send Email Failed.<br>" + ex.Message);

    }



}


private void send()
{
    MailMessage m = new MailMessage();
    SmtpClient sc = new SmtpClient();
    try
    {
        m.From = new MailAddress("malbs@partner-surveys-crm.com");
        m.To.Add("yacovz@gmail.com");
        m.Subject = "This is a Test Mail";
        m.IsBodyHtml = true;
        m.Body = "test gmail";
        sc.Host = "smtp.gmail.com";
        sc.Port = 587;
        sc.Credentials = new System.Net.NetworkCredential("malbs@partner-surveys-crm.com", "Yac:6597216");

        sc.EnableSsl = true;
        sc.Send(m);
        //Response.Write("Email Send successfully");
    }
    catch (Exception ex)
    {
        Response.Write(ex.Message);
    }
}
}
