using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Net;
using System.Text;
using System.Drawing.Imaging;

public partial class Uploader : System.Web.UI.Page
{
    
    static string _uploadsFolderName = "photos";
    

    protected void Page_Load(object sender, EventArgs e)
    {
        Response.Write(SaveFile());        
    }

    private string SaveFile()
    {
        try
        {

           HttpFileCollection files;
            files = Page.Request.Files;

           //for (int index = 0; index < files.AllKeys.Length; index++)
           // {

           //    HttpPostedFile postedFile = files[index];

           //     string imagefileName = getFileName( postedFile.FileName);

           //     string path = Server.MapPath(_uploadsFolderName + "/" + imagefileName);

           //     if (imagefileName.Contains(".jpg") || imagefileName.Contains(".jpeg"))
           //     {
           //         // reduce size
           //         // First load the image somehow
           //         System.Drawing.Image myImage = System.Drawing.Image.FromStream(postedFile.InputStream, true, true);
           //         // Save the image with a quality of 50% 
           //         SaveJpeg(path, myImage, 20);

           //         // save file to hard disk - same size
           //         //postedFile.SaveAs(path);
           //     }
           //     else {
           //         // save file to hard disk
           //         postedFile.SaveAs(path);
           //     }




           //     path =  _uploadsFolderName + "/" + imagefileName;

           //     return path;
           // }
        }
        catch (Exception ex)
        {
            writeToLog(ex.Message + " " + ex.StackTrace);
            //throw;
        }
        return "";
    }


    /// <summary> 
    /// Saves an image as a jpeg image, with the given quality 
    /// </summary> 
    /// <param name="path"> Path to which the image would be saved. </param> 
    /// <param name="quality"> An integer from 0 to 100, with 100 being the highest quality. </param> 
    public static void SaveJpeg(string path, System.Drawing.Image img, int quality)
    {
        if (quality < 0 || quality > 100)
            throw new ArgumentOutOfRangeException("quality must be between 0 and 100.");

        // Encoder parameter for image quality 
        EncoderParameter qualityParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
        // JPEG image codec 
        ImageCodecInfo jpegCodec = GetEncoderInfo("image/jpeg");
        EncoderParameters encoderParams = new EncoderParameters(1);
        encoderParams.Param[0] = qualityParam;
        img.Save(path, jpegCodec, encoderParams);
    }

    /// <summary> 
    /// Returns the image codec with the given mime type 
    /// </summary> 
    private static ImageCodecInfo GetEncoderInfo(string mimeType)
    {
        // Get image codecs for all image formats 
        ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

        // Find the correct image codec 
        for (int i = 0; i < codecs.Length; i++)
            if (codecs[i].MimeType == mimeType)
                return codecs[i];

        return null;
    }

    private bool saveFileToQueue(Dictionary<string, string> vars)
    {
        bool ret = false;
        System.IO.StreamWriter sw;
        try
        {
            // save file
            sw = new System.IO.StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "_uploadsQueuedFiles" + "/" + vars["ImageRefID"] + ".html", true);
            sw.Write(vars["text"]);
            sw.Close();
        }
        catch (Exception ex)
        {
            writeToLog(ex.Message + " " + ex.StackTrace);
            //throw;
        }       

        
        return ret;
    }

    private void writeToLog(string msg)
    {
        System.IO.StreamWriter sw = new System.IO.StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "photos/logTxt.txt", true);
        sw.WriteLine(DateTime.Now.ToString() + " " + msg);
        sw.WriteLine("");
        sw.Close();
    }

    private string[] delimiter = new string[1] { "_" };
    private string getFileName(string imageName)
    {
        // image name structure    
        // ImageRefID + cameraNumber + namespace + location in event (big weel)    
        // ImageRefID_1_SuperLand_1
        string imageNameOnly = imageName.Replace(".jpg", "");

        Dictionary<string, string> retVars = new Dictionary<string, string>();

        int index = 0;
        while (System.IO.File.Exists(Server.MapPath(_uploadsFolderName + "/" + imageName)))
        {
            imageName = index + "_" + imageName;
            index++;
        }
               
        return imageName;
    }

   
    private string getNameDisplayFormat(string userName)
    {
        if (System.Text.RegularExpressions.Regex.IsMatch(userName, ".*[א-ת].*"))
        {
            char[] chars = userName.ToCharArray();
            Array.Reverse(chars);
            return new string(chars);
        }
        return userName;
    }


    private string extractID(string filename)
    {
        string baseString = "shutterstock_";
        if (filename.Contains(baseString))
        {
            string imageID = filename.Replace(baseString, "").Replace(".jpg", "");
            int n;
            bool isNumeric = int.TryParse(imageID, out n);
            if (isNumeric)
                return imageID;
            else
                return "";
        }
        return "";
    }
    public static string table_name = "Images1";
    /// <summary>
    /// CallShuterStockAPI
    /// </summary>
    /// <param name="imageID"></param>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /*private ImageProps GetImageProperties(string imageID)
    {
        string username = "6497e58912f0b6fb9df3";
        string password = "81c8cd448e0a9e92ad56c96183dbc00c5c89404a";

        try
        {


            string url = "https://api.shutterstock.com/v2/images/" + imageID + "?view=full";

            HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            myHttpWebRequest.UserAgent = ".NET Framework Test Client";

            string authInfo = username + ":" + password;
            authInfo = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(authInfo));
            myHttpWebRequest.Headers["Authorization"] = "Basic " + authInfo;

            // Assign the response object of 'HttpWebRequest' to a 'HttpWebResponse' variable.
            HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
            // Display the contents of the page to the console.
            Stream streamResponse = myHttpWebResponse.GetResponseStream();
            StreamReader streamRead = new StreamReader(streamResponse);
            string jsonResponseFromServer = streamRead.ReadToEnd();

            ImageProps m = null;//JsonConvert.DeserializeObject<ImageProps>(jsonResponseFromServer);
            return m;
        }
        catch (Exception)
        {
            return null;
        }
    }


    private void addToDb(ImageProps imageProps)
    {

        Dictionary<string, Object> values = new Dictionary<string, object>();

        //values.Add("dateTime", (DateTime.Now.ToUniversalTime() + new TimeSpan(3, 0, 0)).ToString());
        values.Add("imageID", imageProps.id);
        values.Add("large_thumb_url", imageProps.assets.large_thumb.url);
        values.Add("small_thumb_url", imageProps.assets.small_thumb.url);
        values.Add("file_path", imageProps.image_path);
        values.Add("preview_url", imageProps.assets.preview.url);
        values.Add("description", imageProps.description);
        values.Add("keywords", string.Join(" ", imageProps.keywords));

        dbHandler.addNewContent(table_name, values);
    }
    */
    private bool IsInDB(string imageID)
    {
        List<Dictionary<string, string>> imageRow = dbHandler.getTableData(table_name, "imageID", imageID);
        return imageRow.Count > 0;
    }

    private void createAutoCompleteTable()
    {
        List<Dictionary<string, string>> rows = dbHandler.getTableData(table_name);
        System.Collections.Hashtable hash = new System.Collections.Hashtable();
        List<string> words = new List<string>();

        char[] v = new char[(' ')];
        string[] s = new string[] { " " };
        foreach (Dictionary<string, string> row in rows)
        {
            string line = row["keywords"].ToString();
            string[] keywords = line.Split(s, StringSplitOptions.RemoveEmptyEntries);
            foreach (var keyword in keywords)
            {
                if (hash.ContainsKey(keyword))
                {
                    int count = int.Parse(hash[keyword].ToString());
                    hash[keyword] = count + 1;
                    if (count + 1 == 10)
                        words.Add(keyword);
                }
                else
                {
                    hash.Add(keyword, 1);
                }
            }
        }


        foreach (string keyword in words)
        {
            Dictionary<string, Object> values = new Dictionary<string, object>();
            int count = int.Parse(hash[keyword].ToString());

            values.Add("keyword", keyword);
            values.Add("count", count);

            dbHandler.addNewContent("AutoComplete", values);
        }

    }

}