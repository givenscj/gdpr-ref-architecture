using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace GDPR.Util
{
    public class EmailEngine
    {
        private MailMessage message = new MailMessage();
        private Hashtable tokenHashTable = new Hashtable();

        const string Ns = "http://schemas.microsoft.com/cdo/configuration/";
        const string FieldSmtpAuthenticate = Ns + "smtpauthenticate";
        const string FieldSendUsername = Ns + "sendusername";
        const string FieldSendPassword = Ns + "sendpassword";

        public EmailEngine()
        {
        }

        public MailMessage Message
        {
            get
            { return (this.message); }
            set
            { this.message = value; }
        }

        /// <summary>
        /// Loads a default message into the subject and body of the message to be sent
        /// </summary>
        /// <param name="fileLocation">the location of the text file containing the default message</param>
        public bool LoadDefaultMessage(string fileLocation)
        {
            if (System.IO.File.Exists(fileLocation))
            {
                StreamReader strReader = System.IO.File.OpenText(fileLocation);
                this.message.Subject = strReader.ReadLine();
                this.message.Body = strReader.ReadToEnd();
                strReader.Close();
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Sends the associated message
        /// </summary>
        /// <returns>true if mail succeeds, false if it fails, or if there is no To address</returns>
        public bool SendMail()
        {
            bool _returnValue = false;

            //if (this.message.To != "")
            if (this.message.To[0].Address != "")
            {
                string _temp = WebConfigurationManager.AppSettings["MailServer"];
                //string _temp = _appSettings.GetValue("MailServer");

                if (_temp.Length > 0)
                {
                    System.Net.Mail.SmtpClient client = new SmtpClient();
                    client.Host = _temp;                    

                    string _port = WebConfigurationManager.AppSettings["MailServerPort"];
                    if (_port.Length > 0)
                    {
                        client.Port = int.Parse(_port);
                        client.EnableSsl = true;
                    }

                    //SmtpMail.SmtpServer = _temp;

                    if (bool.Parse(WebConfigurationManager.AppSettings["UseSecurity"]))
                    {
                        client.Credentials = new System.Net.NetworkCredential(WebConfigurationManager.AppSettings["mailServerUserName"], WebConfigurationManager.AppSettings["mailServerPassword"]);

                        //this.message.Fields.Add(FieldSmtpAuthenticate, _appSettings.GetValue("mailServerAuthentication"));
                        //this.message.Fields.Add(FieldSendUsername, _appSettings.GetValue("mailServerUserName"));
                        //this.message.Fields.Add(FieldSendPassword, _appSettings.GetValue("mailServerPassword"));
                    }

                    //SmtpMail.Send(this.message);
                    client.Send(this.message);

                    _returnValue = true;
                }
            }
            else
                _returnValue = false;

            return (_returnValue);
        }

    }
}
