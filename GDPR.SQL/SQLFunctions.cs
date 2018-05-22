using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Text;
using System.Net;


public class SQLFunctions
{
    //GDPR Hub Endpoint...
    static string gdprHubEndpoint = "";

    [Microsoft.SqlServer.Server.SqlFunction(DataAccess = DataAccessKind.Read)]
    public static SqlString SubjectCreate(SqlString applicationId, SqlString subjectId, SqlString email)
    {
        string url = string.Format("{0}/api/GDPR/SubjectCreate?applicationId={1}&subjectId={2}&email={3}", gdprHubEndpoint, applicationId, subjectId, email);
        return DoGet(url);
    }

    public static SqlString SubjectDelete(SqlString applicationId, SqlString subjectId, SqlString email)
    {
        string url = string.Format("{0}/api/GDPR/SubjectDelete?applicationId={1}&subjectId={2}&email={3}", gdprHubEndpoint, applicationId, subjectId, email);
        return DoGet(url);
    }

    public static SqlString DoGet(string url)
    {
        SqlString document = SqlString.Null;

        WebRequest req = WebRequest.Create(Convert.ToString(url));
        try
        {
            using (WebResponse resp = req.GetResponse())
            {
                using (Stream dataStream = resp.GetResponseStream())
                {
                    using (StreamReader rdr = new StreamReader(dataStream))
                    {
                        document = (SqlString)rdr.ReadToEnd();
                    }
                }
            }
        }
        catch (Exception ex)
        { }
        return (document);
    }

    [Microsoft.SqlServer.Server.SqlFunction(DataAccess = DataAccessKind.Read)]
    public static SqlString HttpGetFromSQL(SqlString uri)
    {
        SqlString document = SqlString.Null;
        WebRequest req = WebRequest.Create(Convert.ToString(uri));
        try
        {
            using (WebResponse resp = req.GetResponse())
            {
                using (Stream dataStream = resp.GetResponseStream())
                {
                    using (StreamReader rdr = new StreamReader(dataStream))
                    {
                        document = (SqlString)rdr.ReadToEnd();
                    }
                }
            }
        }
        catch (Exception ex)
        { }
        return (document);
    }

    [Microsoft.SqlServer.Server.SqlFunction(DataAccess = DataAccessKind.Read)]
    public static SqlString HttpPostFromSQL(SqlString uri, SqlString postData)
    {
        SqlString document = SqlString.Null;
        try
        {
            byte[] postByteArray = Encoding.UTF8.GetBytes(Convert.ToString(postData));
            WebRequest req = WebRequest.Create(Convert.ToString(uri));
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            using (Stream dataStream = req.GetRequestStream())
            {
                dataStream.Write(postByteArray, 0, postByteArray.Length);
            }

            using (WebResponse resp = req.GetResponse())
            {
                Stream dataStream = resp.GetResponseStream();
                using (StreamReader rdr = new StreamReader(dataStream))
                {
                    document = (SqlString)rdr.ReadToEnd();
                }
            }
        }
        catch (Exception ex)
        { }
        return (document);
    }
}