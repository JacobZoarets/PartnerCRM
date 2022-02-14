using System;
using System.Collections.Generic;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
//using System.Data.EntityClient;
//using System.Data.SqlClient;

/// <summary>
/// Summary description for dbHandler
/// </summary>
public class dbHandler
{
    public dbHandler()
    {
        //
        // TODO: Add constructor logic here
        //-
    }

    private static SqlConnection GetConnection()
    {
        //string connString = "";// System.Configuration.ConfigurationManager.ConnectionStrings["ShieldDBConnectionString"].ConnectionString;
        //string connString = @"Server=localhost\SQLEXPRESS01;Database=BotFrameworkDB;Trusted_Connection=True;";
        //string connString = "Data Source=SQL6005.site4now.net;Initial Catalog=DB_A44DA3_johnhariss;User Id=DB_A44DA3_johnhariss_admin;Password=yac6597216;";
        string connString = "Data Source=192.114.70.243;Initial Catalog=Couponphone;User Id=couponphonesql;Password=couponphonesql!Q@W;";
        //string connString = @"Server=62.219.14.170;Database=BotFrameworkDB;User Id=LeoBotUser;Password=Yac_6597216";
        //string connString = @"Server=tcp:botframeworkdb.database.windows.net,1433;Initial Catalog=BotFrameworkDB;Persist Security Info=False;User ID=LeoBotUser;Password=Yac_6597216;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        return new SqlConnection(connString);
    }

    public static int getLastAddedIndex(string table_name)
    {
        SqlDataReader MyReader = null;
        SqlConnection cn = GetConnection();
        int ret = -1;

        try
        {
            cn.Open();
            SqlCommand cmd = new SqlCommand("SELECT IDENT_CURRENT('" + table_name + "') as lastID", cn);


            try
            {
                ret = int.Parse(cmd.ExecuteScalar().ToString());
            }
            catch (Exception)
            {

                //throw;
            }

            MyReader.Close();
            cmd.Dispose();
            cmd = null;
        }
        catch (Exception )
        {
            //throw ee;
        }
        finally
        {
            if (MyReader != null && !MyReader.IsClosed)
            {
                MyReader.Close();
            }

            if (cn != null && cn.State == ConnectionState.Open)
            {
                cn.Close();
            }
        }

        if (cn.State == ConnectionState.Open)
        {
            cn.Close();
        }
        return ret;
    }

    private static SqlConnection GetConnection(string connectionName)
    {
        string connString = "";//System.Configuration.ConfigurationManager.ConnectionStrings[connectionName].ConnectionString;
        //string connString = "Server =62.219.14.246; Database=cocalcola_sl; Uid=alon; Pwd=vE7ut!uN;";
        return new SqlConnection(connString);
    }

    private static SqlConnection GetConnectionBBR()
    {
        string connString = System.Configuration.ConfigurationManager.ConnectionStrings["ActiveConnectionString"].ConnectionString;
        //string connString = "Server =62.219.14.246; Database=cocalcola_sl; Uid=alon; Pwd=vE7ut!uN;";
        return new SqlConnection(connString);
    }
    public static bool addNewContent(string tableName, Dictionary<string, object> fieldsAndValues)
    {
        SqlConnection cn = GetConnection();
        cn.Open();
        bool ret = true;

        try
        {
            string sqlLine = generateInsertQuery(tableName, fieldsAndValues);
            SqlCommand cmd = new SqlCommand(sqlLine, cn);

            cmd.ExecuteNonQuery();

            cmd.Dispose();
            cmd = null;

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            ret = false;
            //throw ee;
            //writeToLog("addLikeInstance " + ee.Message);
        }
        finally
        {

            if (cn != null && cn.State == ConnectionState.Open)
            {
                cn.Close();
            }
        }

        if (cn.State == ConnectionState.Open)
        {
            cn.Close();
        }
        return ret;
    }

    private static string generateInsertQuery(string tableName, Dictionary<string, object> fieldsAndValues)
    {
        string fields = "";
        string values = "";
        foreach (KeyValuePair<string, object> pair in fieldsAndValues)
        {
            fields = fields + ", " + pair.Key;
            if (pair.Value.GetType() == typeof(string))
                values = values + ",N'" + pair.Value + "'";
            else
                values = values + ", " + pair.Value + " ";
        }

        fields = "(" + fields.Substring(1) + ")";
        values = "(" + values.Substring(1) + ")";

        string sqlLine = "INSERT INTO [" + tableName + "]" + fields + "VALUES " + values;

        return sqlLine;

    }

    public static bool updateContent(string tableName, Dictionary<string, object> queryKeyAndValue, Dictionary<string, object> fieldsAndValues)
    {
        SqlConnection cn = GetConnection();
        cn.Open();
        bool ret = true;

        try
        {
            string sqlLine = generateUpdateQuery(tableName, queryKeyAndValue, fieldsAndValues);
            SqlCommand cmd = new SqlCommand(sqlLine, cn);

            cmd.ExecuteNonQuery();

            cmd.Dispose();
            cmd = null;

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            ret = false;
            //throw ee;
            //writeToLog("addLikeInstance " + ee.Message);
        }
        finally
        {

            if (cn != null && cn.State == ConnectionState.Open)
            {
                cn.Close();
            }
        }

        if (cn.State == ConnectionState.Open)
        {
            cn.Close();
        }
        return ret;
    }

    private static string generateUpdateQuery(string tableName, Dictionary<string, object> queryKeyAndValue, Dictionary<string, object> fieldsAndValues)
    {

        string values = "";
        foreach (KeyValuePair<string, object> pair in fieldsAndValues)
        {

            if (pair.Value.GetType() == typeof(string))
                values = values + "," + pair.Key + "='" + pair.Value + "'";
            else
                values = values + "," + pair.Key + "=" + pair.Value;
        }
        values = " " + values.Substring(1) + " ";


        string whereClouse = "";
        foreach (KeyValuePair<string, object> pair in queryKeyAndValue)
        {

            if (pair.Value.GetType() == typeof(string))
                whereClouse = whereClouse + " and " + pair.Key + "='" + pair.Value + "'";
            else
                whereClouse = whereClouse + " and " + pair.Key + "=" + pair.Value + "";
        }
        whereClouse = " where (" + whereClouse.Substring(5) + ")";

        string sqlLine = "UPDATE [" + tableName + "]" + " SET " + values + " " + whereClouse;

        return sqlLine;

    }

    public static bool deleteContent(string tableName, Dictionary<string, object> queryKeyAndValue)
    {
        SqlConnection cn = GetConnection();
        cn.Open();
        bool ret = true;

        try
        {
            string sqlLine = generateDeleteQuery(tableName, queryKeyAndValue);
            SqlCommand cmd = new SqlCommand(sqlLine, cn);

            cmd.ExecuteNonQuery();

            cmd.Dispose();
            cmd = null;

        }
        catch (Exception )
        {
            ret = false;
            //throw ee;
            //writeToLog("addLikeInstance " + ee.Message);
        }
        finally
        {

            if (cn != null && cn.State == ConnectionState.Open)
            {
                cn.Close();
            }
        }

        if (cn.State == ConnectionState.Open)
        {
            cn.Close();
        }
        return ret;
    }

    private static string generateDeleteQuery(string tableName, Dictionary<string, object> queryKeyAndValue)
    {

        string whereClouse = "";
        foreach (KeyValuePair<string, object> pair in queryKeyAndValue)
        {

            if (pair.Value.GetType() == typeof(string))
                whereClouse = whereClouse + " and " + pair.Key + "='" + pair.Value + "'";
            else
                whereClouse = whereClouse + " and " + pair.Key + "=" + pair.Value + "";
        }
        whereClouse = " where (" + whereClouse.Substring(5) + ")";

        string sqlLine = "DELETE FROM [" + tableName + "]" + whereClouse;

        return sqlLine;

    }

    public static bool addStartGameTime(string gameID, string fbID)
    {
        SqlConnection cn = GetConnection();
        cn.Open();
        bool ret = true;

        try
        {
            string[] flds = {"gameID",
                            "startTime",
                            "startTimeMilisecond",
                            "fbID"};

            string[] vals = {"'" + gameID + "'",
                            "'" + DateTime.Now.ToString() + "'",
                                    "'" + DateTime.Now.TimeOfDay.Milliseconds.ToString() + "'",
                            "'" + fbID + "'"};

            string sqlLine = "INSERT INTO [gameTimes] (" + string.Join(", ", flds) + ")" +
                                "VALUES (" + string.Join(", ", vals) + ")";

            SqlCommand cmd = new SqlCommand(sqlLine, cn);

            cmd.ExecuteNonQuery();

            cmd.Dispose();
            cmd = null;

        }
        catch (Exception )
        {
            ret = false;
            //throw ee;
            //writeToLog("addLikeInstance " + ee.Message);
        }
        finally
        {

            if (cn != null && cn.State == ConnectionState.Open)
            {
                cn.Close();
            }
        }

        if (cn.State == ConnectionState.Open)
        {
            cn.Close();
        }
        return ret;
    }

    public static bool updateGameEndTime(string gameID)
    {
        SqlConnection cn = GetConnection();
        cn.Open();
        bool ret = true;

        try
        {

            string sqlLine = "UPDATE [gameTimes] SET [endTime] = '" + DateTime.Now.ToString() + "', [endTimeMilisecond] = '" + DateTime.Now.TimeOfDay.Milliseconds.ToString() + "' where gameID='" + gameID + "'";

            SqlCommand cmd = new SqlCommand(sqlLine, cn);

            cmd.ExecuteNonQuery();

            cmd.Dispose();
            cmd = null;

        }
        catch (Exception )
        {
            ret = false;
            //throw ee;
            //writeToLog("addLikeInstance " + ee.Message);
        }
        finally
        {

            if (cn != null && cn.State == ConnectionState.Open)
            {
                cn.Close();
            }
        }

        if (cn.State == ConnectionState.Open)
        {
            cn.Close();
        }
        return ret;
    }



    public static string getGameTime(string gameID)
    {
        string ret = "1000";

        SqlDataReader MyReader = null;
        SqlConnection cn = GetConnection();

        try
        {
            cn.Open();
            SqlCommand cmd = new SqlCommand("SELECT * " +
            " FROM [gameTimes] " +
            "where [gameID] = '" + gameID + "'", cn);

            MyReader = cmd.ExecuteReader();
            while (MyReader.Read())
            {
                string start = MyReader["startTime"].ToString();
                try
                {
                    string[] arDate = start.Split(' ');
                    start = arDate[1] + " " + arDate[2];
                }
                catch (Exception)
                { }


                string startTimeMilisecond = MyReader["startTimeMilisecond"].ToString();

                DateTime st = DateTime.Parse(start);
                if (startTimeMilisecond != "")
                {
                    st.AddMilliseconds(double.Parse(startTimeMilisecond));
                }

                string endTime = MyReader["endTime"].ToString();
                try
                {
                    string[] arEndDate = endTime.Split(' ');
                    endTime = arEndDate[1] + " " + arEndDate[2];
                }
                catch (Exception)
                { }



                string endTimeMilisecond = MyReader["endTimeMilisecond"].ToString();
                DateTime et = DateTime.Parse(endTime);
                if (endTimeMilisecond != "")
                {
                    et.AddMilliseconds(double.Parse(endTimeMilisecond));
                }


                TimeSpan span = et - st;

                ret = (span.TotalMilliseconds + int.Parse(endTimeMilisecond)).ToString();

                break;
            }

            MyReader.Close();
            cmd.Dispose();
            cmd = null;
        }
        catch (Exception )
        {
            //throw ee;
        }
        finally
        {
            if (MyReader != null && !MyReader.IsClosed)
            {
                MyReader.Close();
            }

            if (cn != null && cn.State == ConnectionState.Open)
            {
                cn.Close();
            }
        }

        if (cn.State == ConnectionState.Open)
        {
            cn.Close();
        }
        return ret;

    }

    public static bool isGameIDNew(string gameID)
    {
        bool ret = false;

        SqlDataReader MyReader = null;
        SqlConnection cn = GetConnection();

        try
        {
            cn.Open();
            SqlCommand cmd = new SqlCommand("SELECT * " +
            " FROM [games] " +
            "where [gameID] = '" + gameID + "'", cn);

            MyReader = cmd.ExecuteReader();
            while (MyReader.Read())
            {
                ret = true;
                break;
            }

            MyReader.Close();
            cmd.Dispose();
            cmd = null;
        }
        catch (Exception )
        {
            //throw ee;
        }
        finally
        {
            if (MyReader != null && !MyReader.IsClosed)
            {
                MyReader.Close();
            }

            if (cn != null && cn.State == ConnectionState.Open)
            {
                cn.Close();
            }
        }

        if (cn.State == ConnectionState.Open)
        {
            cn.Close();
        }
        return ret;

    }

    public static bool isGameIDRegistred(string gameID)
    {
        bool ret = false;

        SqlDataReader MyReader = null;
        SqlConnection cn = GetConnection();

        try
        {
            cn.Open();
            SqlCommand cmd = new SqlCommand("SELECT * " +
            " FROM [gameTimes] " +
            "where [gameID] = '" + gameID + "'", cn);

            MyReader = cmd.ExecuteReader();
            while (MyReader.Read())
            {
                ret = true;
                break;
            }

            MyReader.Close();
            cmd.Dispose();
            cmd = null;
        }
        catch (Exception )
        {
            //throw ee;
        }
        finally
        {
            if (MyReader != null && !MyReader.IsClosed)
            {
                MyReader.Close();
            }

            if (cn != null && cn.State == ConnectionState.Open)
            {
                cn.Close();
            }
        }

        if (cn.State == ConnectionState.Open)
        {
            cn.Close();
        }
        return ret;

    }
    /*
    public static List<Dictionary<string, string>> getWinners(string fbID, int numberToReturn, scorer.gameTimeFrames timeframe)
    {
        SqlDataReader MyReader = null;
        SqlConnection cn = GetConnection();
        List<Dictionary<string, string>> ret = new List<Dictionary<string, string>>();

        try
        {
            cn.Open();

            // monthly time frame  - get all scores
            string sql = "SELECT * FROM [users] order by [totalScore] desc";

            if (timeframe == scorer.gameTimeFrames.daily)
            {
                sql = "SELECT * FROM [users] where updateDay=" + DateTime.Now.Day.ToString() + " and updateMonth=" + DateTime.Now.Month.ToString() + " order by [dailyScore] desc";
            }
            else if (timeframe == scorer.gameTimeFrames.weekly)
            {
                DateTime weekStartDate = scorer.getStartOfTheWeekDate();
                if(weekStartDate.Month < DateTime.Now.Month)
                    sql = "SELECT * FROM [users] where updateDay >=0 and updateMonth >=" + weekStartDate.Month.ToString() + " order by [weeklyScore] desc";
                else
                    sql = "SELECT * FROM [users] where updateDay >=" + weekStartDate.Day.ToString() + " and updateMonth >=" + weekStartDate.Month.ToString() + " order by [weeklyScore] desc";
            }
               

            SqlCommand cmd = new SqlCommand(sql, cn);

            System.Collections.Hashtable hash = new System.Collections.Hashtable();

            bool foundUser = false;
            int position = 0;
            MyReader = cmd.ExecuteReader();
            while (MyReader.Read())
            {
                Dictionary<string, string> retInner = new Dictionary<string, string>();

                for (int i = 0; i < MyReader.FieldCount; i++)
                {
                    retInner.Add(MyReader.GetName(i).ToString(), MyReader[i].ToString());
                }
                position++;

                retInner.Add("position",position.ToString());

                ret.Add(retInner);

                if (MyReader["fbID"].ToString() == fbID)
                    foundUser = true;

                if (ret.Count == numberToReturn)
                {
                    if (foundUser == true)
                        break;
                    else
                    {
                        ret.RemoveAt(ret.Count - 1);
                    }
                }
            }

            MyReader.Close();
            cmd.Dispose();
            cmd = null;
        }
        catch (Exception )
        {
            //throw ee;
        }
        finally
        {
            if (MyReader != null && !MyReader.IsClosed)
            {
                MyReader.Close();
            }

            if (cn != null && cn.State == ConnectionState.Open)
            {
                cn.Close();
            }
        }

        if (cn.State == ConnectionState.Open)
        {
            cn.Close();
        }
        return ret;
    }

    public static List<Dictionary<string, string>> getWinnersFriends(string fbID, int numberToReturn, string[] friendsID, scorer.gameTimeFrames timeframe)
    {
        SqlDataReader MyReader = null;
        SqlConnection cn = GetConnection();
        List<Dictionary<string, string>> ret = new List<Dictionary<string, string>>();



        try
        {
            cn.Open();

            string friendsSqline = "fbID='" + fbID + "'";

            foreach (string id in friendsID)
            {
                friendsSqline = friendsSqline + " OR fbID='" + id + "'";
            }
            //friendsSqline = friendsSqline.Substring(4);

            // monthly time frame  - get all scores
            string sql = "SELECT * FROM [users] where (" + friendsSqline + ") order by [totalScore] desc";

            if (timeframe == scorer.gameTimeFrames.daily)
            {
                sql = "SELECT * FROM [users] where updateDay=" + DateTime.Now.Day.ToString() + " and updateMonth=" + DateTime.Now.Month.ToString() + " and (" + friendsSqline + ") order by [dailyScore] desc";
            }
            else if (timeframe == scorer.gameTimeFrames.weekly)
            {
                DateTime weekStartDate = scorer.getStartOfTheWeekDate();
                sql = "SELECT * FROM [users] where updateDay >=" + DateTime.Now.Day.ToString() + " and updateMonth >=" + DateTime.Now.Month.ToString() + " and (" + friendsSqline + ") order by [weeklyScore] desc";
            }

            SqlCommand cmd = new SqlCommand(sql, cn);

            System.Collections.Hashtable hash = new System.Collections.Hashtable();

            bool foundUser = false;
            int position = 0;
            MyReader = cmd.ExecuteReader();
            while (MyReader.Read())
            {
                Dictionary<string, string> retInner = new Dictionary<string, string>();

                for (int i = 0; i < MyReader.FieldCount; i++)
                {
                    retInner.Add(MyReader.GetName(i).ToString(), MyReader[i].ToString());
                }
                position++;

                retInner.Add("position", position.ToString());

                ret.Add(retInner);

                if (MyReader["fbID"].ToString() == fbID)
                    foundUser = true;

                if (ret.Count == numberToReturn)
                {
                    if (foundUser == true)
                        break;
                    else
                    {
                        ret.RemoveAt(ret.Count - 1);
                    }
                }

            }

            MyReader.Close();
            cmd.Dispose();
            cmd = null;
        }
        catch (Exception )
        {
            //throw ee;
        }
        finally
        {
            if (MyReader != null && !MyReader.IsClosed)
            {
                MyReader.Close();
            }

            if (cn != null && cn.State == ConnectionState.Open)
            {
                cn.Close();
            }
        }

        if (cn.State == ConnectionState.Open)
        {
            cn.Close();
        }
        return ret;
    }

    public static List<Dictionary<string, string>> getTrueWinners(scorer.gameTimeFrames timeframe)
    {
        SqlDataReader MyReader = null;
        SqlConnection cn = GetConnection();
        List<Dictionary<string, string>> ret = new List<Dictionary<string, string>>();

        try
        {
            cn.Open();

            // monthly time frame  - get all scores
            string sql = "SELECT * FROM [users] where isMonthlyWinner is not null";

            if (timeframe == scorer.gameTimeFrames.daily)
            {
                sql = "SELECT * FROM [users] where isDailyWinner is not null";
            }
            else if (timeframe == scorer.gameTimeFrames.weekly)
            {
                sql = "SELECT * FROM [users] where isWeeklyWinner is not null";
            }


            SqlCommand cmd = new SqlCommand(sql, cn);
                               
            MyReader = cmd.ExecuteReader();
            while (MyReader.Read())
            {
                Dictionary<string, string> retInner = new Dictionary<string, string>();

                if (timeframe == scorer.gameTimeFrames.weekly && MyReader["isWeeklyWinner"] != null && MyReader["isWeeklyWinner"].ToString().Trim() != "")
                {
                    for (int i = 0; i < MyReader.FieldCount; i++)
                    {
                        retInner.Add(MyReader.GetName(i).ToString(), MyReader[i].ToString());
                    }
                    ret.Add(retInner);
                }
                else if (MyReader["isMonthlyWinner"] != null && MyReader["isMonthlyWinner"].ToString().Trim() != "")
                {
                    for (int i = 0; i < MyReader.FieldCount; i++)
                    {
                        retInner.Add(MyReader.GetName(i).ToString(), MyReader[i].ToString());
                    }
                    ret.Add(retInner);
                }              
            }

            MyReader.Close();
            cmd.Dispose();
            cmd = null;
        }
        catch (Exception )
        {
            //throw ee;
        }
        finally
        {
            if (MyReader != null && !MyReader.IsClosed)
            {
                MyReader.Close();
            }

            if (cn != null && cn.State == ConnectionState.Open)
            {
                cn.Close();
            }
        }

        if (cn.State == ConnectionState.Open)
        {
            cn.Close();
        }
        return ret;
    }
    */

    public static Dictionary<string, string> getPlayerDetails(string fbID)
    {
        SqlDataReader MyReader = null;
        SqlConnection cn = GetConnection();
        Dictionary<string, string> ret = new Dictionary<string, string>();

        try
        {
            cn.Open();
            SqlCommand cmd = new SqlCommand("SELECT * " +
                                            " FROM [users] " +
                                            " where [fbID] = '" + fbID + "'", cn);

            MyReader = cmd.ExecuteReader();
            while (MyReader.Read())
            {

                for (int i = 0; i < MyReader.FieldCount; i++)
                {
                    ret.Add(MyReader.GetName(i).ToString(), MyReader[i].ToString());
                }
                break;
            }

            MyReader.Close();
            cmd.Dispose();
            cmd = null;
        }
        catch (Exception )
        {
            //throw ee;
        }
        finally
        {
            if (MyReader != null && !MyReader.IsClosed)
            {
                MyReader.Close();
            }

            if (cn != null && cn.State == ConnectionState.Open)
            {
                cn.Close();
            }
        }

        if (cn.State == ConnectionState.Open)
        {
            cn.Close();
        }
        return ret;
    }

    public static bool isPlayerRegistered(string fbID)
    {
        Dictionary<string, string> playerDetails = dbHandler.getPlayerDetails(fbID);

        if (playerDetails.Count > 0)
            return true;

        return false;
    }

    public static List<Dictionary<string, string>> getPlayerGames(string fbID)
    {
        SqlDataReader MyReader = null;
        SqlConnection cn = GetConnection();
        List<Dictionary<string, string>> ret = new List<Dictionary<string, string>>();

        try
        {
            cn.Open();
            SqlCommand cmd = new SqlCommand("SELECT * " +
                                            " FROM [games] " +
                                            " where [fbID] = '" + fbID + "'", cn);

            Dictionary<string, string> retinner = null;
            MyReader = cmd.ExecuteReader();
            while (MyReader.Read())
            {
                retinner = new Dictionary<string, string>();
                for (int i = 0; i < MyReader.FieldCount; i++)
                {
                    retinner.Add(MyReader.GetName(i).ToString(), MyReader[i].ToString());
                }
                ret.Add(retinner);
            }

            MyReader.Close();
            cmd.Dispose();
            cmd = null;
        }
        catch (Exception )
        {
            //throw ee;
        }
        finally
        {
            if (MyReader != null && !MyReader.IsClosed)
            {
                MyReader.Close();
            }

            if (cn != null && cn.State == ConnectionState.Open)
            {
                cn.Close();
            }
        }

        if (cn.State == ConnectionState.Open)
        {
            cn.Close();
        }
        return ret;
    }

    public static List<Dictionary<string, string>> getAllAccounts()
    {
        SqlDataReader MyReader = null;
        SqlConnection cn = GetConnection();
        List<Dictionary<string, string>> ret = new List<Dictionary<string, string>>();

        try
        {
            cn.Open();
            SqlCommand cmd = new SqlCommand("SELECT * " +
                                            " FROM [Accounts] ", cn);

            Dictionary<string, string> retinner = null;
            MyReader = cmd.ExecuteReader();
            while (MyReader.Read())
            {
                retinner = new Dictionary<string, string>();
                for (int i = 0; i < MyReader.FieldCount; i++)
                {
                    retinner.Add(MyReader.GetName(i).ToString(), MyReader[i].ToString());
                }
                ret.Add(retinner);
            }

            MyReader.Close();
            cmd.Dispose();
            cmd = null;
        }
        catch (Exception )
        {
            //throw ee;
        }
        finally
        {
            if (MyReader != null && !MyReader.IsClosed)
            {
                MyReader.Close();
            }

            if (cn != null && cn.State == ConnectionState.Open)
            {
                cn.Close();
            }
        }

        if (cn.State == ConnectionState.Open)
        {
            cn.Close();
        }
        return ret;
    }

    public static List<Dictionary<string, string>> getAllTranspotationOrders()
    {
        SqlDataReader MyReader = null;
        SqlConnection cn = GetConnection();
        List<Dictionary<string, string>> ret = new List<Dictionary<string, string>>();

        try
        {
            cn.Open();
            SqlCommand cmd = new SqlCommand("SELECT * " +
                                            " FROM [TranspotationOrders] ", cn);

            Dictionary<string, string> retinner = null;
            MyReader = cmd.ExecuteReader();
            while (MyReader.Read())
            {
                retinner = new Dictionary<string, string>();
                for (int i = 0; i < MyReader.FieldCount; i++)
                {
                    retinner.Add(MyReader.GetName(i).ToString(), MyReader[i].ToString());
                }
                ret.Add(retinner);
            }

            MyReader.Close();
            cmd.Dispose();
            cmd = null;
        }
        catch (Exception )
        {
            //throw ee;
        }
        finally
        {
            if (MyReader != null && !MyReader.IsClosed)
            {
                MyReader.Close();
            }

            if (cn != null && cn.State == ConnectionState.Open)
            {
                cn.Close();
            }
        }

        if (cn.State == ConnectionState.Open)
        {
            cn.Close();
        }
        return ret;
    }

    public static int getPlayerGamesCount(string fbID)
    {
        SqlDataReader MyReader = null;
        SqlConnection cn = GetConnection();
        int ret = -1;

        try
        {
            cn.Open();
            SqlCommand cmd = new SqlCommand("SELECT COUNT([id]) " +
                                            " FROM [games] as AllGames" +
                                            " where [fbID] = '" + fbID + "'", cn);


            try
            {
                ret = int.Parse(cmd.ExecuteScalar().ToString());
            }
            catch (Exception)
            {

                //throw;
            }
            //MyReader = cmd.ExecuteReader();
            //while (MyReader.Read())
            //{
            //    string ff = MyReader["AllGames"].ToString();
            //    ret = int.Parse(ff);
            //}

            MyReader.Close();
            cmd.Dispose();
            cmd = null;
        }
        catch (Exception )
        {
            //throw ee;
        }
        finally
        {
            if (MyReader != null && !MyReader.IsClosed)
            {
                MyReader.Close();
            }

            if (cn != null && cn.State == ConnectionState.Open)
            {
                cn.Close();
            }
        }

        if (cn.State == ConnectionState.Open)
        {
            cn.Close();
        }
        return ret;
    }

    public static List<Dictionary<string, string>> getRandomGames(int numberToReturn, string userFbid)
    {
        SqlDataReader MyReader = null;
        SqlConnection cn = GetConnection();
        List<Dictionary<string, string>> ret = new List<Dictionary<string, string>>();

        try
        {
            cn.Open();
            SqlCommand cmd = new SqlCommand("SELECT * " +
                                            " FROM [games] " +
                                            " where [canBeVirtual] = 1 order by id desc", cn);

            Dictionary<string, string> retinner = null;
            MyReader = cmd.ExecuteReader();

            while (MyReader.Read())
            {
                if (MyReader["fbID"].ToString() != userFbid)
                {
                    retinner = new Dictionary<string, string>();
                    for (int i = 0; i < MyReader.FieldCount; i++)
                    {
                        retinner.Add(MyReader.GetName(i).ToString(), MyReader[i].ToString());
                    }
                    ret.Add(retinner);

                    if (ret.Count > numberToReturn)
                        break;
                }
            }



            MyReader.Close();
            cmd.Dispose();
            cmd = null;
        }
        catch (Exception )
        {
            //throw ee;
        }
        finally
        {
            if (MyReader != null && !MyReader.IsClosed)
            {
                MyReader.Close();
            }

            if (cn != null && cn.State == ConnectionState.Open)
            {
                cn.Close();
            }
        }

        if (cn.State == ConnectionState.Open)
        {
            cn.Close();
        }
        return ret;
    }
    private static bool isHigherScore(string oldScore, string newScore)
    {
        bool ret = false;

        int old = 0;
        int news = 0;
        if (int.TryParse(oldScore, out old) && int.TryParse(newScore, out news))
        {
            return news > old;
        }

        return ret;
    }


    public static string iaAdminListed(string name, string password)
    {
        string ret = "";

        SqlDataReader MyReader = null;
        SqlConnection cn = GetConnection();

        try
        {
            cn.Open();
            SqlCommand cmd = new SqlCommand("SELECT [id], [username],[password]" +
            " FROM [admins] " +
            "where [username] = '" + name + "' AND [password]='" + password + "'", cn);

            MyReader = cmd.ExecuteReader();
            while (MyReader.Read())
            {
                ret = MyReader["id"].ToString();
                break;
            }

            MyReader.Close();
            cmd.Dispose();
            cmd = null;
        }
        catch (Exception )
        {
            //throw ee;
        }
        finally
        {
            if (MyReader != null && !MyReader.IsClosed)
            {
                MyReader.Close();
            }

            if (cn != null && cn.State == ConnectionState.Open)
            {
                cn.Close();
            }
        }

        if (cn.State == ConnectionState.Open)
        {
            cn.Close();
        }
        return ret;

    }


    public static List<Dictionary<string, string>> getLastAddedRowID(string tableName)
    {

        SqlDataReader MyReader = null;
        SqlConnection cn = GetConnection();
        List<Dictionary<string, string>> ret = new List<Dictionary<string, string>>();


        try
        {
            cn.Open();
            SqlCommand cmd = new SqlCommand("SELECT IDENT_CURRENT('" + tableName + "') as id", cn);

            MyReader = cmd.ExecuteReader();
            while (MyReader.Read())
            {

                Dictionary<string, string> innerRet = new Dictionary<string, string>();
                for (int i = 0; i < MyReader.FieldCount; i++)
                {
                    innerRet.Add(MyReader.GetName(i).ToString(), MyReader[i].ToString());
                }
                ret.Add(innerRet);

            }

            MyReader.Close();
            cmd.Dispose();
            cmd = null;
        }
        catch (Exception )
        {
            //throw ee;
        }
        finally
        {
            if (MyReader != null && !MyReader.IsClosed)
            {
                MyReader.Close();
            }

            if (cn != null && cn.State == ConnectionState.Open)
            {
                cn.Close();
            }
        }

        if (cn.State == ConnectionState.Open)
        {
            cn.Close();
        }
        return ret;


    }

    public static List<Dictionary<string, string>> getTableData(string tableName, string whereClouse, string whereValue)
    {
        SqlDataReader MyReader = null;
        SqlConnection cn = GetConnection();
        List<Dictionary<string, string>> ret = new List<Dictionary<string, string>>();


        try
        {
            cn.Open();
            SqlCommand cmd = new SqlCommand("SELECT * " +
                                            " FROM  " + tableName + " where " + whereClouse + "='" + whereValue + "'", cn);

            MyReader = cmd.ExecuteReader();
            while (MyReader.Read())
            {

                Dictionary<string, string> innerRet = new Dictionary<string, string>();
                for (int i = 0; i < MyReader.FieldCount; i++)
                {
                    innerRet.Add(MyReader.GetName(i).ToString(), MyReader[i].ToString());
                }
                ret.Add(innerRet);

            }

            MyReader.Close();
            cmd.Dispose();
            cmd = null;
        }
        catch (Exception )
        {
            //throw ee;
        }
        finally
        {
            if (MyReader != null && !MyReader.IsClosed)
            {
                MyReader.Close();
            }

            if (cn != null && cn.State == ConnectionState.Open)
            {
                cn.Close();
            }
        }

        if (cn.State == ConnectionState.Open)
        {
            cn.Close();
        }
        return ret;
    }
    public static List<Dictionary<string, string>> getTableData(string tableName, string whereClouse, string whereValue, string order_by)
    {
        SqlDataReader MyReader = null;
        SqlConnection cn = GetConnection();
        List<Dictionary<string, string>> ret = new List<Dictionary<string, string>>();


        try
        {
            cn.Open();
            SqlCommand cmd = new SqlCommand("SELECT * " +
                                            " FROM  " + tableName + " where " + whereClouse + "='" + whereValue + "'" + " order by " + order_by, cn);

            MyReader = cmd.ExecuteReader();
            while (MyReader.Read())
            {

                Dictionary<string, string> innerRet = new Dictionary<string, string>();
                for (int i = 0; i < MyReader.FieldCount; i++)
                {
                    innerRet.Add(MyReader.GetName(i).ToString(), MyReader[i].ToString());
                }
                ret.Add(innerRet);

            }

            MyReader.Close();
            cmd.Dispose();
            cmd = null;
        }
        catch (Exception )
        {
            //throw ee;
        }
        finally
        {
            if (MyReader != null && !MyReader.IsClosed)
            {
                MyReader.Close();
            }

            if (cn != null && cn.State == ConnectionState.Open)
            {
                cn.Close();
            }
        }

        if (cn.State == ConnectionState.Open)
        {
            cn.Close();
        }
        return ret;
    }

    public static List<Dictionary<string, string>> getTableData(string tableName, string whereClouse, int whereValue)
    {
        SqlDataReader MyReader = null;
        SqlConnection cn = GetConnection();
        List<Dictionary<string, string>> ret = new List<Dictionary<string, string>>();


        try
        {
            cn.Open();
            SqlCommand cmd = new SqlCommand("SELECT * " +
                                            " FROM  " + tableName + " where " + whereClouse + "=" + whereValue + "", cn);

            MyReader = cmd.ExecuteReader();
            while (MyReader.Read())
            {

                Dictionary<string, string> innerRet = new Dictionary<string, string>();
                for (int i = 0; i < MyReader.FieldCount; i++)
                {
                    innerRet.Add(MyReader.GetName(i).ToString(), MyReader[i].ToString());
                }
                ret.Add(innerRet);

            }

            MyReader.Close();
            cmd.Dispose();
            cmd = null;
        }
        catch (Exception )
        {
            //throw ee;
        }
        finally
        {
            if (MyReader != null && !MyReader.IsClosed)
            {
                MyReader.Close();
            }

            if (cn != null && cn.State == ConnectionState.Open)
            {
                cn.Close();
            }
        }

        if (cn.State == ConnectionState.Open)
        {
            cn.Close();
        }
        return ret;
    }


    public static List<Dictionary<string, string>> getTableData(string tableName, string whereClouse, int whereValue, string order_by)
    {
        SqlDataReader MyReader = null;
        SqlConnection cn = GetConnection();
        List<Dictionary<string, string>> ret = new List<Dictionary<string, string>>();


        try
        {
            cn.Open();

            string sql = "SELECT * " + " FROM  " + tableName + " where " + whereClouse + "=" + whereValue + " order by " + order_by;

            SqlCommand cmd = new SqlCommand(sql, cn);

            MyReader = cmd.ExecuteReader();
            while (MyReader.Read())
            {

                Dictionary<string, string> innerRet = new Dictionary<string, string>();
                for (int i = 0; i < MyReader.FieldCount; i++)
                {
                    innerRet.Add(MyReader.GetName(i).ToString(), MyReader[i].ToString());
                }
                ret.Add(innerRet);

            }

            MyReader.Close();
            cmd.Dispose();
            cmd = null;
        }
        catch (Exception )
        {
            //throw ee;
        }
        finally
        {
            if (MyReader != null && !MyReader.IsClosed)
            {
                MyReader.Close();
            }

            if (cn != null && cn.State == ConnectionState.Open)
            {
                cn.Close();
            }
        }

        if (cn.State == ConnectionState.Open)
        {
            cn.Close();
        }
        return ret;
    }

    public static List<Dictionary<string, string>> getTableData(string tableName)
    {
        SqlDataReader MyReader = null;
        SqlConnection cn = GetConnection();
        List<Dictionary<string, string>> ret = new List<Dictionary<string, string>>();


        try
        {
            cn.Open();
            SqlCommand cmd = new SqlCommand("SELECT * " +
                                            " FROM  " + tableName, cn);

            MyReader = cmd.ExecuteReader();
            while (MyReader.Read())
            {

                Dictionary<string, string> innerRet = new Dictionary<string, string>();
                for (int i = 0; i < MyReader.FieldCount; i++)
                {
                    innerRet.Add(MyReader.GetName(i).ToString(), MyReader[i].ToString());
                }
                ret.Add(innerRet);

            }

            MyReader.Close();
            cmd.Dispose();
            cmd = null;
        }
        catch (Exception )
        {
            //throw ee;
        }
        finally
        {
            if (MyReader != null && !MyReader.IsClosed)
            {
                MyReader.Close();
            }

            if (cn != null && cn.State == ConnectionState.Open)
            {
                cn.Close();
            }
        }

        if (cn.State == ConnectionState.Open)
        {
            cn.Close();
        }
        return ret;
    }
    public static List<Dictionary<string, string>> getTableDataBBr(string tableName)
    {
        SqlDataReader MyReader = null;
        SqlConnection cn = GetConnectionBBR();
        List<Dictionary<string, string>> ret = new List<Dictionary<string, string>>();


        try
        {
            cn.Open();
            SqlCommand cmd = new SqlCommand("SELECT * " +
                                            " FROM  " + tableName + " order by id desc;", cn);

            MyReader = cmd.ExecuteReader();
            while (MyReader.Read())
            {

                Dictionary<string, string> innerRet = new Dictionary<string, string>();
                for (int i = 0; i < MyReader.FieldCount; i++)
                {
                    innerRet.Add(MyReader.GetName(i).ToString(), MyReader[i].ToString());
                }
                ret.Add(innerRet);

            }

            MyReader.Close();
            cmd.Dispose();
            cmd = null;
        }
        catch (Exception )
        {
            //throw ee;
        }
        finally
        {
            if (MyReader != null && !MyReader.IsClosed)
            {
                MyReader.Close();
            }

            if (cn != null && cn.State == ConnectionState.Open)
            {
                cn.Close();
            }
        }

        if (cn.State == ConnectionState.Open)
        {
            cn.Close();
        }
        return ret;
    }

    public static Dictionary<string, string> getTableData_lastRow(string tableName, Dictionary<string, object> queryKeyAndValue)
    {
        SqlDataReader MyReader = null;
        SqlConnection cn = GetConnection();
        Dictionary<string, string> ret = new Dictionary<string, string>();


        try
        {
            string sql = generateTableDataQuery(tableName, queryKeyAndValue);

            sql = sql + " ORDER BY Id DESC";

            cn.Open();
            SqlCommand cmd = new SqlCommand(sql, cn);

            MyReader = cmd.ExecuteReader();
            while (MyReader.Read())
            {

                Dictionary<string, string> innerRet = new Dictionary<string, string>();
                for (int i = 0; i < MyReader.FieldCount; i++)
                {
                    innerRet.Add(MyReader.GetName(i).ToString(), MyReader[i].ToString());
                }
                ret = innerRet;
                break;
            }

            MyReader.Close();
            cmd.Dispose();
            cmd = null;
        }
        catch (Exception )
        {
            //throw ee;
        }
        finally
        {
            if (MyReader != null && !MyReader.IsClosed)
            {
                MyReader.Close();
            }

            if (cn != null && cn.State == ConnectionState.Open)
            {
                cn.Close();
            }
        }

        if (cn.State == ConnectionState.Open)
        {
            cn.Close();
        }
        return ret;
    }


    public static List<Dictionary<string, string>> getTableData(string tableName, Dictionary<string, object> queryKeyAndValue, string order_by)
    {
        SqlDataReader MyReader = null;
        SqlConnection cn = GetConnection();
        List<Dictionary<string, string>> ret = new List<Dictionary<string, string>>();


        try
        {
            string sql = generateTableDataQuery(tableName, queryKeyAndValue) + " order by " + order_by;
            cn.Open();
            SqlCommand cmd = new SqlCommand(sql, cn);

            MyReader = cmd.ExecuteReader();
            while (MyReader.Read())
            {

                Dictionary<string, string> innerRet = new Dictionary<string, string>();
                for (int i = 0; i < MyReader.FieldCount; i++)
                {
                    innerRet.Add(MyReader.GetName(i).ToString(), MyReader[i].ToString());
                }
                ret.Add(innerRet);

            }

            MyReader.Close();
            cmd.Dispose();
            cmd = null;
        }
        catch (Exception )
        {
            //throw ee;
        }
        finally
        {
            if (MyReader != null && !MyReader.IsClosed)
            {
                MyReader.Close();
            }

            if (cn != null && cn.State == ConnectionState.Open)
            {
                cn.Close();
            }
        }

        if (cn.State == ConnectionState.Open)
        {
            cn.Close();
        }
        return ret;
    }
    public static List<Dictionary<string, string>> getTableData(string tableName, Dictionary<string, object> queryKeyAndValue)
    {
        SqlDataReader MyReader = null;
        SqlConnection cn = GetConnection();
        List<Dictionary<string, string>> ret = new List<Dictionary<string, string>>();


        try
        {
            string sql = generateTableDataQuery(tableName, queryKeyAndValue);
            cn.Open();
            SqlCommand cmd = new SqlCommand(sql, cn);

            MyReader = cmd.ExecuteReader();
            while (MyReader.Read())
            {

                Dictionary<string, string> innerRet = new Dictionary<string, string>();
                for (int i = 0; i < MyReader.FieldCount; i++)
                {
                    innerRet.Add(MyReader.GetName(i).ToString(), MyReader[i].ToString());
                }
                ret.Add(innerRet);

            }

            MyReader.Close();
            cmd.Dispose();
            cmd = null;
        }
        catch (Exception )
        {
            //throw ee;
        }
        finally
        {
            if (MyReader != null && !MyReader.IsClosed)
            {
                MyReader.Close();
            }

            if (cn != null && cn.State == ConnectionState.Open)
            {
                cn.Close();
            }
        }

        if (cn.State == ConnectionState.Open)
        {
            cn.Close();
        }
        return ret;
    }

    public static List<Dictionary<string, string>> getTableData_recipes(string tableName, Dictionary<string, object> queryKeyAndValue)
    {
        SqlDataReader MyReader = null;
        SqlConnection cn = GetConnection();
        List<Dictionary<string, string>> ret = new List<Dictionary<string, string>>();


        try
        {
            string sql = generateTableDataQuery_recipe(tableName, queryKeyAndValue);
            cn.Open();
            SqlCommand cmd = new SqlCommand(sql, cn);

            MyReader = cmd.ExecuteReader();
            while (MyReader.Read())
            {

                Dictionary<string, string> innerRet = new Dictionary<string, string>();
                for (int i = 0; i < MyReader.FieldCount; i++)
                {
                    innerRet.Add(MyReader.GetName(i).ToString(), MyReader[i].ToString());
                }
                ret.Add(innerRet);

            }

            MyReader.Close();
            cmd.Dispose();
            cmd = null;
        }
        catch (Exception )
        {
            //throw ee;
        }
        finally
        {
            if (MyReader != null && !MyReader.IsClosed)
            {
                MyReader.Close();
            }

            if (cn != null && cn.State == ConnectionState.Open)
            {
                cn.Close();
            }
        }

        if (cn.State == ConnectionState.Open)
        {
            cn.Close();
        }
        return ret;
    }

    public static List<Dictionary<string, string>> getTableDataLikeQuery(string tableName, string whereClouse, string whereValue)
    {
        SqlDataReader MyReader = null;
        SqlConnection cn = GetConnection();
        List<Dictionary<string, string>> ret = new List<Dictionary<string, string>>();


        try
        {
            cn.Open();
            SqlCommand cmd = new SqlCommand("SELECT * " +
                                            " FROM  " + tableName + " where " + whereClouse + " like '" + whereValue + "' order by id desc", cn);

            MyReader = cmd.ExecuteReader();
            while (MyReader.Read())
            {

                Dictionary<string, string> innerRet = new Dictionary<string, string>();
                for (int i = 0; i < MyReader.FieldCount; i++)
                {
                    innerRet.Add(MyReader.GetName(i).ToString(), MyReader[i].ToString());
                }
                ret.Add(innerRet);

            }

            MyReader.Close();
            cmd.Dispose();
            cmd = null;
        }
        catch (Exception )
        {
            //throw ee;
        }
        finally
        {
            if (MyReader != null && !MyReader.IsClosed)
            {
                MyReader.Close();
            }

            if (cn != null && cn.State == ConnectionState.Open)
            {
                cn.Close();
            }
        }

        if (cn.State == ConnectionState.Open)
        {
            cn.Close();
        }
        return ret;
    }

    public static List<Dictionary<string, string>> getTableDataSqlQuery(string sqlQuery)
    {
        SqlDataReader MyReader = null;
        SqlConnection cn = GetConnection();
        List<Dictionary<string, string>> ret = new List<Dictionary<string, string>>();


        try
        {
            cn.Open();
            SqlCommand cmd = new SqlCommand(sqlQuery, cn);

            MyReader = cmd.ExecuteReader();
            while (MyReader.Read())
            {

                Dictionary<string, string> innerRet = new Dictionary<string, string>();
                for (int i = 0; i < MyReader.FieldCount; i++)
                {
                    innerRet.Add(MyReader.GetName(i).ToString(), MyReader[i].ToString());
                }
                ret.Add(innerRet);

            }

            MyReader.Close();
            cmd.Dispose();
            cmd = null;
        }
        catch (Exception)
        {
            //throw ee;
        }
        finally
        {
            if (MyReader != null && !MyReader.IsClosed)
            {
                MyReader.Close();
            }

            if (cn != null && cn.State == ConnectionState.Open)
            {
                cn.Close();
            }
        }

        if (cn.State == ConnectionState.Open)
        {
            cn.Close();
        }
        return ret;
    }
    private static string generateTableDataQuery(string tableName, Dictionary<string, object> queryKeyAndValue)
    {

        string whereClouse = "";
        foreach (KeyValuePair<string, object> pair in queryKeyAndValue)
        {

            if (pair.Value.GetType() == typeof(string))
                whereClouse = whereClouse + " and " + pair.Key + "='" + pair.Value + "'";
            else
                whereClouse = whereClouse + " and " + pair.Key + "=" + pair.Value + "";
        }
        whereClouse = " where (" + whereClouse.Substring(5) + ")";

        string sqlLine = "SELECT * FROM [" + tableName + "]" + whereClouse;

        return sqlLine;

    }

    private static string generateTableDataQuery_recipe(string tableName, Dictionary<string, object> queryKeyAndValue)
    {

        string whereClouse = "";
        foreach (KeyValuePair<string, object> pair in queryKeyAndValue)
        {

            whereClouse = whereClouse + " and " + "CHARINDEX('"+ pair.Value + "', [recipe_tags]) > 0";// whereClouse + " and " + pair.Key + "='" + pair.Value + "'";
            
        }
        whereClouse = " where (" + whereClouse.Substring(5) + ")";

        string sqlLine = "SELECT * FROM [" + "recipe_tags" + "]" + whereClouse;

        return sqlLine;

    }
    public static int getTableDataCount(string tableName, string whereClouse, int whereValue)
    {
        int ret = 0;
        SqlDataReader MyReader = null;
        SqlConnection cn = GetConnection();


        try
        {
            cn.Open();
            SqlCommand cmd = new SqlCommand("SELECT count(*) " +
                                            " FROM  " + tableName + " where " + whereClouse + "=" + whereValue + "", cn);

            ret = (int)cmd.ExecuteScalar();

            cmd.Dispose();
            cmd = null;

        }
        catch (Exception )
        {
            //throw ee;
        }
        finally
        {
            if (MyReader != null && !MyReader.IsClosed)
            {
                MyReader.Close();
            }

            if (cn != null && cn.State == ConnectionState.Open)
            {
                cn.Close();
            }
        }

        if (cn.State == ConnectionState.Open)
        {
            cn.Close();
        }
        return ret;
    }
    public static int getTableDataCount_distinct(string tableName,string distinctFields, string whereClouse)
    {
        int ret = 0;
        SqlDataReader MyReader = null;
        SqlConnection cn = GetConnection();


        try
        {
            cn.Open();
            SqlCommand cmd = new SqlCommand("SELECT count(distinct "+ distinctFields + ") " +
                                            " FROM  " + tableName + " where " + whereClouse , cn);

            ret = (int)cmd.ExecuteScalar();

            cmd.Dispose();
            cmd = null;

        }
        catch (Exception )
        {
            //throw ee;
        }
        finally
        {
            if (MyReader != null && !MyReader.IsClosed)
            {
                MyReader.Close();
            }

            if (cn != null && cn.State == ConnectionState.Open)
            {
                cn.Close();
            }
        }

        if (cn.State == ConnectionState.Open)
        {
            cn.Close();
        }
        return ret;
    }
    public static int getTableDataDistinct(string tableName, string distinctFields, string whereClouse)
    {
        int ret = 0;
        SqlDataReader MyReader = null;
        SqlConnection cn = GetConnection();


        try
        {
            cn.Open();
            SqlCommand cmd = new SqlCommand("SELECT distinct " + distinctFields + " " +
                                            " FROM  " + tableName + " where " + whereClouse, cn);
                        
            MyReader = cmd.ExecuteReader();
            while (MyReader.Read())
            {
                ret += 1;
            }
            MyReader.Close();
            cmd.Dispose();
            cmd = null;

            ret = (int)cmd.ExecuteScalar();

            cmd.Dispose();
            cmd = null;

        }
        catch (Exception )
        {
            //throw ee;
        }
        finally
        {
            if (MyReader != null && !MyReader.IsClosed)
            {
                MyReader.Close();
            }

            if (cn != null && cn.State == ConnectionState.Open)
            {
                cn.Close();
            }
        }

        if (cn.State == ConnectionState.Open)
        {
            cn.Close();
        }
        return ret;
    }
    public static System.Collections.Hashtable getUserVotes(string userfbID)
    {
        SqlDataReader MyReader = null;
        SqlConnection cn = GetConnection();
        System.Collections.Hashtable ret = new System.Collections.Hashtable();

        try
        {
            cn.Open();
            SqlCommand cmd = new SqlCommand("SELECT [fbID],[storyID] " +
                                            "FROM [Votes]" +
                                            "where [fbID]='" + userfbID + "'", cn);

            MyReader = cmd.ExecuteReader();
            while (MyReader.Read())
            {
                string storyID = MyReader["storyID"].ToString();
                if (!ret.ContainsKey(storyID))
                    ret.Add(storyID, storyID);
            }
            MyReader.Close();
            cmd.Dispose();
            cmd = null;
        }
        catch (Exception )
        {
            throw ;
        }
        finally
        {
            if (MyReader != null && !MyReader.IsClosed)
            {
                MyReader.Close();
            }

            if (cn != null && cn.State == ConnectionState.Open)
            {
                cn.Close();
            }
        }

        if (cn.State == ConnectionState.Open)
        {
            cn.Close();
        }
        return ret;
    }

    public static bool canVote(string userfbID, string storyID)
    {
        SqlDataReader MyReader = null;
        SqlConnection cn = GetConnection();
        bool ret = true;

        try
        {
            cn.Open();
            SqlCommand cmd = new SqlCommand("SELECT [fbID],[storyID] " +
                                            "FROM [Votes]" +
                                            "where [fbID]='" + userfbID + "' and [storyID]='" + storyID + "'", cn);

            MyReader = cmd.ExecuteReader();
            while (MyReader.Read())
            {
                ret = false;
                break;
            }
            MyReader.Close();
            cmd.Dispose();
            cmd = null;
        }
        catch (Exception )
        {
            throw ;
        }
        finally
        {
            if (MyReader != null && !MyReader.IsClosed)
            {
                MyReader.Close();
            }

            if (cn != null && cn.State == ConnectionState.Open)
            {
                cn.Close();
            }
        }

        if (cn.State == ConnectionState.Open)
        {
            cn.Close();
        }
        return ret;
    }

    public static bool addNewVote(string userfbID, string storyID)
    {
        SqlConnection cn = GetConnection();
        cn.Open();
        bool ret = true;

        try
        {
            string[] flds = {"fbID",
                            "storyID"};

            string[] vals = {"'" + userfbID + "'",
                            "'" + storyID + "'"};

            string sqlLine = "INSERT INTO [Votes] (" + string.Join(", ", flds) + ")" +
                                "VALUES (" + string.Join(", ", vals) + ")";

            SqlCommand cmd = new SqlCommand(sqlLine, cn);

            cmd.ExecuteNonQuery();

            cmd.Dispose();
            cmd = null;

        }
        catch (Exception )
        {
            ret = false;
            //throw ee;
            //writeToLog("addLikeInstance " + ee.Message);
        }
        finally
        {

            if (cn != null && cn.State == ConnectionState.Open)
            {
                cn.Close();
            }
        }

        if (cn.State == ConnectionState.Open)
        {
            cn.Close();
        }
        return ret;
    }

    public static List<Dictionary<string, string>> getGallery(string fbID, int numberToReturn, string order)
    {
        SqlDataReader MyReader = null;
        SqlConnection cn = GetConnection();
        List<Dictionary<string, string>> ret = new List<Dictionary<string, string>>();

        try
        {
            cn.Open();

            // monthly time frame  - get all scores
            string sql = "SELECT * FROM [users] where showInGallery=1 order by id desc";

            if (order == "1")
                sql = "SELECT * FROM [users] where showInGallery=1  order by [totalVotes] desc";

            SqlCommand cmd = new SqlCommand(sql, cn);

            System.Collections.Hashtable hash = getUserVotes(fbID);

            MyReader = cmd.ExecuteReader();
            while (MyReader.Read())
            {
                Dictionary<string, string> retInner = new Dictionary<string, string>();

                retInner.Add("id", MyReader["id"].ToString());
                retInner.Add("fbID", MyReader["fbID"].ToString());
                retInner.Add("fbName", MyReader["fbName"].ToString());
                string source = MyReader["imageID"].ToString();// "https://ssl.e-dologic.co.il/facebook_App/Danone/Health_wall/DanoneWall/uploads/" + MyReader["imageID"].ToString() + ".jpg";
                retInner.Add("src", source);
                retInner.Add("txt", MyReader["txt"].ToString());
                retInner.Add("totalVotes", MyReader["totalVotes"].ToString());

                if (!hash.ContainsKey(MyReader["id"].ToString()))
                    retInner.Add("can_like", "1");
                else
                    retInner.Add("can_like", "0");

                ret.Add(retInner);

                if (ret.Count == numberToReturn)
                    break;
            }

            MyReader.Close();
            cmd.Dispose();
            cmd = null;
        }
        catch (Exception )
        {
            //throw ee;
        }
        finally
        {
            if (MyReader != null && !MyReader.IsClosed)
            {
                MyReader.Close();
            }

            if (cn != null && cn.State == ConnectionState.Open)
            {
                cn.Close();
            }
        }

        if (cn.State == ConnectionState.Open)
        {
            cn.Close();
        }
        return ret;
    }


    public static List<Dictionary<string, string>> getWallGallery(string fbID, int numberToReturn)
    {
        SqlDataReader MyReader = null;
        SqlConnection cn = GetConnection();
        List<Dictionary<string, string>> ret = new List<Dictionary<string, string>>();

        try
        {
            cn.Open();

            // monthly time frame  - get all scores
            string sql = "SELECT * FROM [users] where fbID='" + fbID + "' and updatedOnWall=1 and WallMailSent=1 order by id desc";


            SqlCommand cmd = new SqlCommand(sql, cn);

            System.Collections.Hashtable hash = getUserVotes(fbID);

            MyReader = cmd.ExecuteReader();
            while (MyReader.Read())
            {
                Dictionary<string, string> retInner = new Dictionary<string, string>();

                retInner.Add("fbID", MyReader["fbID"].ToString());
                retInner.Add("fbName", MyReader["fbName"].ToString());
                string source = "https://ssl.e-dologic.co.il/facebook_App/Danone/Health_wall/DanoneWall/uploads/" + MyReader["imageID"].ToString() + ".jpg";
                retInner.Add("src", source);
                retInner.Add("txt", MyReader["txt"].ToString());
                retInner.Add("likes", MyReader["totalVotes"].ToString());

                string letterNum = "0";
                List<Dictionary<string, string>> list = getTableData("WallIDs", "imageID", MyReader["imageID"].ToString());
                if (list != null && list.Count > 0)
                {
                    letterNum = list[0]["letterNum"];
                }

                retInner.Add("letter", letterNum);

                ret.Add(retInner);

                if (ret.Count == numberToReturn)
                    break;
            }

            MyReader.Close();
            cmd.Dispose();
            cmd = null;
        }
        catch (Exception )
        {
            //throw ee;
        }
        finally
        {
            if (MyReader != null && !MyReader.IsClosed)
            {
                MyReader.Close();
            }

            if (cn != null && cn.State == ConnectionState.Open)
            {
                cn.Close();
            }
        }

        if (cn.State == ConnectionState.Open)
        {
            cn.Close();
        }
        return ret;
    }



    public static List<Dictionary<string, string>> getGalleryFriends(string fbID, int numberToReturn, string[] friendsID, string order)
    {
        SqlDataReader MyReader = null;
        SqlConnection cn = GetConnection();
        List<Dictionary<string, string>> ret = new List<Dictionary<string, string>>();



        try
        {
            cn.Open();

            string friendsSqline = "fbID='" + fbID + "'";

            foreach (string id in friendsID)
            {
                friendsSqline = friendsSqline + " OR fbID='" + id + "'";
            }
            //friendsSqline = friendsSqline.Substring(4);

            // monthly time frame  - get all scores
            string sql = "SELECT * FROM [users] where showInGallery=1 and (" + friendsSqline + ") order by id desc";

            if (order == "1")
                sql = "SELECT * FROM [users] where showInGallery=1 and (" + friendsSqline + ") order by [totalVotes] desc";

            SqlCommand cmd = new SqlCommand(sql, cn);

            System.Collections.Hashtable hash = getUserVotes(fbID);

            MyReader = cmd.ExecuteReader();
            while (MyReader.Read())
            {
                Dictionary<string, string> retInner = new Dictionary<string, string>();

                retInner.Add("id", MyReader["id"].ToString());
                retInner.Add("fbID", MyReader["fbID"].ToString());
                retInner.Add("fbName", MyReader["fbName"].ToString());
                string source = "https://ssl.e-dologic.co.il/facebook_App/Danone/Health_wall/DanoneWall/uploads/" + MyReader["imageID"].ToString() + ".jpg";
                retInner.Add("src", source);
                retInner.Add("txt", MyReader["txt"].ToString());
                retInner.Add("totalVotes", MyReader["totalVotes"].ToString());

                if (!hash.ContainsKey(MyReader["id"].ToString()))
                    retInner.Add("can_like", "1");
                else
                    retInner.Add("can_like", "0");

                ret.Add(retInner);

                if (ret.Count == numberToReturn)
                    break;
            }

            MyReader.Close();
            cmd.Dispose();
            cmd = null;
        }
        catch (Exception )
        {
            //throw ee;
        }
        finally
        {
            if (MyReader != null && !MyReader.IsClosed)
            {
                MyReader.Close();
            }

            if (cn != null && cn.State == ConnectionState.Open)
            {
                cn.Close();
            }
        }

        if (cn.State == ConnectionState.Open)
        {
            cn.Close();
        }
        return ret;
    }

    public static int getNumberOfImagesToday(string fbid)
    {
        SqlDataReader MyReader = null;
        SqlConnection cn = GetConnection();
        int ret = 0;

        try
        {
            cn.Open();

            // monthly time frame  - get all scores
            string sql = "SELECT * FROM [users] where fbID ='" + fbid + "' and updateDay=" + DateTime.Now.Day + " and updateMonth=" + DateTime.Now.Month;

            SqlCommand cmd = new SqlCommand(sql, cn);

            MyReader = cmd.ExecuteReader();
            while (MyReader.Read())
            {
                ret++;
            }

            MyReader.Close();
            cmd.Dispose();
            cmd = null;
        }
        catch (Exception )
        {
            //throw ee;
        }
        finally
        {
            if (MyReader != null && !MyReader.IsClosed)
            {
                MyReader.Close();
            }

            if (cn != null && cn.State == ConnectionState.Open)
            {
                cn.Close();
            }
        }

        if (cn.State == ConnectionState.Open)
        {
            cn.Close();
        }
        return ret;
    }

    public static int getNumberOfVotes(string storyID)
    {
        SqlDataReader MyReader = null;
        SqlConnection cn = GetConnection();
        int ret = 0;

        try
        {
            cn.Open();

            // monthly time frame  - get all scores
            string sql = "SELECT * FROM [votes] where storyID =" + storyID;

            SqlCommand cmd = new SqlCommand(sql, cn);

            MyReader = cmd.ExecuteReader();
            while (MyReader.Read())
            {
                ret++;
            }

            MyReader.Close();
            cmd.Dispose();
            cmd = null;
        }
        catch (Exception )
        {
            //throw ee;
        }
        finally
        {
            if (MyReader != null && !MyReader.IsClosed)
            {
                MyReader.Close();
            }

            if (cn != null && cn.State == ConnectionState.Open)
            {
                cn.Close();
            }
        }

        if (cn.State == ConnectionState.Open)
        {
            cn.Close();
        }
        return ret;
    }
}