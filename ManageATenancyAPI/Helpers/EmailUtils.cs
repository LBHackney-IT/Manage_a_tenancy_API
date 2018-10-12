using System;
using System.Data;
using System.Configuration;
using System.Web;

using System.Net.Mail;
using LBH.Utils;

namespace ManageATenancyAPI.Helpers
{
    /// <summary>
    /// Summary description for EmailUtils
    /// </summary>
    public class EmailUtils
    {
        public EmailUtils()
        {
        }


        private string _ErrMessage;

        public string ErrMessage
        {
            get
            {
                return _ErrMessage;
            }
            set
            {
                _ErrMessage = value;
            }
        }
        private string _SMTPServer;

        public string SMTPServer
        {
            get
            {
                return _SMTPServer;
            }
            set
            {
                _SMTPServer = value;
            }
        }
        private string _TestEmail;

        public string TestEmail
        {
            get
            {
                return _TestEmail;
            }
            set
            {
                _TestEmail = value;
            }
        }
        private bool _TestMode;

        public bool TestMode
        {
            get
            {
                return _TestMode;
            }
            set
            {
                _TestMode = value;
            }
        }

        public string EmailPreviewText(string strTo, string strFrom, string strSubject, string strBody, string strAttachment)
        {
            string strTemp = "";
            strTemp += "<table width='100%' cellpadding='3' cellspacing='1'>";
            strTemp += "<tr>";
            strTemp += "<td class='formleft'>To:</td>";
            strTemp += "<td class='formright'>" + strTo + "</td>";
            strTemp += "</tr>";
            strTemp += "<tr>";
            strTemp += "<td class='formleft'>From:</td>";
            strTemp += "<td class='formright'>" + strFrom + "</td>";
            strTemp += "</tr>";
            strTemp += "<tr>";
            strTemp += "<td class='formleft'>Subject:</td>";
            strTemp += "<td class='formright'>" + strSubject + "</td>";
            strTemp += "</tr>";
            strTemp += "<tr>";
            strTemp += "<td class='normal' colspan='2'>" + Utils.CRToBR(strBody) + "</td>";
            strTemp += "</tr>";
            strTemp += "</table>";
            return strTemp;
        }

        public bool SendEmail(string strTo, string strFrom, string strCC, string strSubject, string strBody)
        {
            SmtpClient smtpClient = new SmtpClient();

            MailMessage objMM;
            objMM = new MailMessage();

            string strSMTPServer = Utils.NullToString(SMTPServer);

//            if (strSMTPServer == "")
//            {
//                strSMTPServer = AppOptions.GetOption("SMTPServer");
//            }

            bool boolOK = false;


            objMM.IsBodyHtml = Utils.IsHTMLEmail(strBody);

            MailAddress fromAddress = new MailAddress(strFrom);
            MailAddress toAddress = new MailAddress(strTo);
            objMM.To.Add(toAddress);
            objMM.From = fromAddress;
            objMM.Priority = MailPriority.Normal;
            objMM.Subject = strSubject;
            objMM.Body = strBody;
            try
            {
                smtpClient.Host = strSMTPServer;
                smtpClient.Send(objMM);
                boolOK = true;
            }
            catch (Exception Err)
            {
                while (!((Err.InnerException == null)))
                {
                    ErrMessage += Err.InnerException.ToString() + "<br/><br/>";
                    Err = Err.InnerException;
                }
                boolOK = false;
            }
            return boolOK;
        }
        
        public static void SendEmail(string strTo, string strFrom, string strSubject, string strBody)
        {
            EmailUtils objEmail = new EmailUtils();
            objEmail.SendEmail(strTo, strFrom, "", strSubject, strBody);
        }

    }
}