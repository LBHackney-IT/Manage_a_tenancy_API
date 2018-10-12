using System;
using System.Reflection;
using System.Collections;
using System.Configuration;
using System.Web.Http;

namespace LBH.Utils
{

    /// <summary>
    /// Summary description for Helpers
    /// </summary>
    public class Utils
    {
        public Utils()
        {
            //
            // TODO: Add constructor logic here
            //
        }


        public static string LongDate(string p)
        {
            if (p == "")
            {
                return "";
            }
            else
            {
                return Convert.ToDateTime(p).ToString("MMMM dd, yyyy");
            }
        }        


	//public static string GetApplicationPath()
	//{
 //       string strRoot = HttpContext.Current.Request.Url.ToString();
 //       string strFolder = HttpContext.Current.Request.ApplicationPath;

 //       strRoot = "http://" + HttpContext.Current.Request.ServerVariables["SERVER_NAME"] + ":" + HttpContext.Current.Request.ServerVariables["SERVER_PORT"];

 //       HttpContext.Current.Trace.Write(strRoot);
 //       HttpContext.Current.Trace.Write(strFolder);

 //       //string strPath = strRoot.Substring(1, strRoot.IndexOf("/"));

 //       return strRoot + strFolder;



	//}        

        //public static void HandleError(string strSource, string strMessage, string strExplanation)
        //{
        //    string strMsg = "Source: " + strSource + ", Msg: " + strMessage + "Explanation: " + strExplanation;

        //    WriteDebug("Error:", strMsg);

        //}


  
        public static int NullToInteger(object p)
        {
            if (p == null || p == "")
            {
                p = 0;
            }
            try
            {
                return int.Parse(p.ToString());
            }
            catch (Exception)
            {
                return 0;
            }

        }
        public static bool NullToBoolean(object p)
        {

            try
            {
                if (p == null | Convert.IsDBNull(p))
                {
                    p = false;
                }
            }
            catch (Exception)
            {
                p = false;
            }	

            return Convert.ToBoolean(p);

        }
        public static double NullToDouble(object p)
        {
            if (p == null || p == "")
            {
                p = 0;
            }
            try
            {
                return double.Parse(p.ToString());
            }
            catch (Exception Err)
            {
                return 0;
            }
        }
 
    public static string NullToString(object p)
        {
            if (p == null | Convert.IsDBNull(p))
            {
                p = "";
            }

            return p.ToString();

        }
       

        public static string CRToBR(string strBody)
        {
            return strBody.Replace(Environment.NewLine, "<br/>");

        }
        public static bool IsHTMLEmail(string strBody)
        {
            bool boolIsHTMLEmail = false;

            if (strBody.IndexOf("<html>") != -1)
            {
                boolIsHTMLEmail = true;
            }

            return boolIsHTMLEmail;

        }       


        public static string ShortDate(string p)
        {

            return Convert.ToDateTime(p).ToString("MMM dd, yyyy");
        }

        public static void AddIfThere(ref string strText, string strInput, string strDelim)
        {
            if (strInput.Trim().Length > 0)
            {
                strText += strInput + strDelim;
            }
        }

        internal static string CleanSql(string p)
        {
            return p.Replace("'", "''");
        }

        public static bool IsuserNameConflict(string value)
        {
            bool containsSearchResult = false;

            if (value.Contains("StatusCode: 409"))
            {
                containsSearchResult = true;
            }

            return containsSearchResult;
        }

        public static string ConverDateTimeToLocal(string value)
        {
            DateTime convertedDate = DateTime.Parse(value);

            return convertedDate.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}