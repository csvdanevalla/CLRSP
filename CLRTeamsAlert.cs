using System;
using System.Data.SqlTypes;
using System.Net;
using System.Text;
using Microsoft.SqlServer.Server;

public partial class StoredProcedures
{

    [Microsoft.SqlServer.Server.SqlProcedure]
    public static void SendAlertMessageToTeams (
        SqlString webHookURL, 
        SqlString errorAlertColor,
        SqlString title,
        SqlString messageResult)
    {
        // Put your code here
        string webhookUrl = webHookURL.ToString();

        string jsonData = "{\"type\":\"MessageCard\"," +
               "\"context\":\"https://schema.org/extensions\"," +
               "\"summary\":\"MVR App Alert\"," +
               "\"themeColor\":\"" + errorAlertColor.ToString() + "\"," +
               "\"title\":\"" + title.ToString() + "\"," +
               "\"sections\":[{\"facts\":[" +
               "{\"name\":\"Event At:\",\"value\":\"" + DateTime.Now.ToString("MM/dd/yyyy HH:mm") + "\"}," +
               "\"text\":\"" + messageResult.ToString() + "\"}]}";
        try
        {
            var request = (HttpWebRequest)HttpWebRequest.Create(webhookUrl);

            request.ContentType = "application/json";

            request.Method = "POST";

            var data = Encoding.ASCII.GetBytes(jsonData);

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();
            
            SqlContext.Pipe.Send("Sending Alert Completed!");
        }
        catch (Exception ex)
        {
            SqlContext.Pipe.Send("Sending Alert Failed! Error: " + ex.Message);
        }
    }
}
