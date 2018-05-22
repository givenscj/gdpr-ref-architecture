using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Web.Configuration;
using Microsoft.SqlServer.Management.AlwaysEncrypted.AzureKeyVaultProvider;
using System.Data.SqlClient;
using Microsoft.Azure.KeyVault;
using System.Data.Entity.Core.EntityClient;
using CRM.DB;
using GDPR.Util.Messages;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;

namespace GDPR.Util
{
    public class Util
    {
        static KeyVaultClient kv;

        static string gdprSqlConnectionString;
        static string crmSqlConnectionString;
        static string eventHubConnectionString;
        static string eventHubConnectionStringWithPath;
        static string storageAccountKey;

        public static string GDPRSQLConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(gdprSqlConnectionString))
                {
                    string uri = WebConfigurationManager.AppSettings["keyVaultGDPRSQLConnectionStringUri"];
                    var Result = Task.Run(async () => { return await kv.GetSecretAsync(uri); }).Result;
                    gdprSqlConnectionString = Result.Value;
                }

                return gdprSqlConnectionString;
            }
        }

        internal static string DoPost(string url, string json, object p)
        {
            throw new NotImplementedException();
        }

        internal static string DoPut(string url, object p)
        {
            throw new NotImplementedException();
        }

        public static string CRMSQLConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(crmSqlConnectionString))
                {
                    string uri = WebConfigurationManager.AppSettings["keyVaultCRMSQLConnectionStringUri"];
                    var Result = Task.Run(async () => { return await kv.GetSecretAsync(uri); }).Result;
                    crmSqlConnectionString = Result.Value;
                }

                return crmSqlConnectionString;
            }
        }

        internal static string DoGet(string url, object p)
        {
            throw new NotImplementedException();
        }

        public static string EventHubConnectionStringWithPath
        {
            get
            {
                if (string.IsNullOrEmpty(eventHubConnectionStringWithPath))
                {
                    string uri = WebConfigurationManager.AppSettings["keyVaultEventHubConnectionStringUri"];
                    string eventHubName = WebConfigurationManager.AppSettings["eventHubName"];
                    var Result = Task.Run(async () => { return await kv.GetSecretAsync(uri); }).Result;
                    eventHubConnectionStringWithPath = Result.Value + ";EntityPath=" + eventHubName;
                }

                return eventHubConnectionStringWithPath;
            }
        }

        public static string EventHubConnectionString
        {
            get
            {
                if(string.IsNullOrEmpty(eventHubConnectionString))
                {
                    string uri = WebConfigurationManager.AppSettings["keyVaultEventHubConnectionStringUri"];
                    var Result = Task.Run(async () => { return await kv.GetSecretAsync(uri); }).Result;
                    eventHubConnectionString = Result.Value;
                }

                return eventHubConnectionString;
            }            
        }

        public static string StorageAccountKey
        {
            get
            {
                if (string.IsNullOrEmpty(storageAccountKey))
                {
                    string uri = WebConfigurationManager.AppSettings["keyVaultStorageAccountKeyUri"];
                    var Result = Task.Run(async () => { return await kv.GetSecretAsync(uri); }).Result;
                    storageAccountKey = Result.Value;
                }

                return storageAccountKey;
            }
        }        

        static Util()
        {
            //Adds the Azure Key Vault Provider to ensure the column is unencrypted when queryed upon
            //Reference: https://blogs.msdn.microsoft.com/sqlsecurity/2015/11/10/using-the-azure-key-vault-key-store-provider-for-always-encrypted/
            SqlColumnEncryptionAzureKeyVaultProvider azureKeyVaultProvider = new SqlColumnEncryptionAzureKeyVaultProvider(Util.GetToken);
            Dictionary<string, SqlColumnEncryptionKeyStoreProvider> providers = new Dictionary<string, SqlColumnEncryptionKeyStoreProvider>();
            providers.Add(SqlColumnEncryptionAzureKeyVaultProvider.ProviderName, azureKeyVaultProvider);
            SqlConnection.RegisterColumnEncryptionKeyStoreProviders(providers);

            //Get an access token for the Key Vault to get the secret out...
            kv = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(Util.GetToken));
        }

        public static string UploadBlob(Guid applicationId, string fileName)
        {
            FileInfo fi = new FileInfo(fileName);

            CloudStorageAccount storageAccount;

            string storageAccountName = WebConfigurationManager.AppSettings["storageAccountName"];
            string storageConnectionString = string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", storageAccountName, StorageAccountKey);

            try
            {
                if (CloudStorageAccount.TryParse(storageConnectionString, out storageAccount))
                {
                    CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();
                    CloudBlobContainer cloudBlobContainer = null;

                    try
                    {
                        // Create a container called 'quickstartblobs' and append a GUID value to it to make the name unique. 
                        cloudBlobContainer = cloudBlobClient.GetContainerReference(applicationId.ToString());

                        if (!cloudBlobContainer.Exists())
                            cloudBlobContainer.Create();
                    }
                    catch (Exception ex)
                    {                       
                    }

                    // Set the permissions so the blobs are public. 
                    BlobContainerPermissions permissions = new BlobContainerPermissions
                    {
                        PublicAccess = BlobContainerPublicAccessType.Blob
                    };

                    cloudBlobContainer.SetPermissions(permissions);

                    CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(fi.Name);
                    cloudBlockBlob.UploadFromFile(fileName,null, null, null);
                    return cloudBlockBlob.Uri.ToString();
                }
                else
                {
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return null;
        }

        public static GDPRDatabaseEntities GetGDPRDBContext(string connectionString)
        {
            const string metaData = "res://*/GDPRModel.csdl|res://*/GDPRModel.ssdl|res://*/GDPRModel.msl";
            const string appName = "EntityFramework";
            const string providerName = "System.Data.SqlClient";

            EntityConnectionStringBuilder efBuilder = new EntityConnectionStringBuilder();
            efBuilder.Metadata = metaData;
            efBuilder.Provider = providerName;
            efBuilder.ProviderConnectionString = connectionString;

            return new GDPRDatabaseEntities(efBuilder.ConnectionString);
        }

        public static CRMEntities GetCRMDBContext(string connectionString)
        {
            const string metaData = "res://*/CRM.csdl|res://*/CRM.ssdl|res://*/CRM.msl";
            const string appName = "EntityFramework";
            const string providerName = "System.Data.SqlClient";

            EntityConnectionStringBuilder efBuilder = new EntityConnectionStringBuilder();
            efBuilder.Metadata = metaData;
            efBuilder.Provider = providerName;
            efBuilder.ProviderConnectionString = connectionString;

            return new CRMEntities(efBuilder.ConnectionString);
        }

        static async public Task Configure()
        {
            //Adds the Azure Key Vault Provider to ensure the column is unencrypted when queryed upon
            //Reference: https://blogs.msdn.microsoft.com/sqlsecurity/2015/11/10/using-the-azure-key-vault-key-store-provider-for-always-encrypted/
            SqlColumnEncryptionAzureKeyVaultProvider azureKeyVaultProvider = new SqlColumnEncryptionAzureKeyVaultProvider(Util.GetToken);
            Dictionary<string, SqlColumnEncryptionKeyStoreProvider> providers = new Dictionary<string, SqlColumnEncryptionKeyStoreProvider>();
            providers.Add(SqlColumnEncryptionAzureKeyVaultProvider.ProviderName, azureKeyVaultProvider);
            SqlConnection.RegisterColumnEncryptionKeyStoreProviders(providers);

            //Get an access token for the Key Vault to get the secret out...
            kv = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(Util.GetToken));            
        }

        public static async Task<string> GetToken(string authority, string resource, string scope)
        {
            var authContext = new AuthenticationContext(authority);
            ClientCredential clientCred = new ClientCredential(WebConfigurationManager.AppSettings["ClientId"],
                        WebConfigurationManager.AppSettings["ClientSecret"]);
            AuthenticationResult result = await authContext.AcquireTokenAsync(resource, clientCred);

            if (result == null)
                throw new InvalidOperationException("Failed to obtain the JWT token");

            return result.AccessToken;
        }

        internal static void NotifyDataSubject(NotifyMessage nm)
        {
            if (!string.IsNullOrEmpty(nm.Subject.Email))
                Util.Notify_SendEmail(nm, nm.Subject.Email);

            if (!string.IsNullOrEmpty(nm.Subject.MobilePhone))
                Util.Notify_SendSMS(nm, nm.Subject.MobilePhone);

            if (!string.IsNullOrEmpty(nm.Subject.HomePhone))
                Util.Notify_CallPhone(nm, nm.Subject.HomePhone);

            if (!string.IsNullOrEmpty(nm.Subject.WorkPhone))
                Util.Notify_CallPhone(nm, nm.Subject.WorkPhone);
        }

        private static bool Notify_CallPhone(NotifyMessage nm, string homePhone)
        {
            Console.WriteLine("Calling phone...");
            return true;
        }        

        private static bool Notify_SendSMS(NotifyMessage nm, string mobilePhone)
        {
            Console.WriteLine("Sending SMS...");
            return true;
        }

        private static bool Notify_SendEmail(NotifyMessage nm, string emailAddress)
        {
            Console.WriteLine("Sending Email...");

            try
            {
                TokenManager tm = new TokenManager();
                EmailEngine ee = new EmailEngine();
                ee.Message.Body = tm.ReplaceTokens(nm.LongMessage);
                ee.Message.From = new System.Net.Mail.MailAddress(Constants.noReplyEmailAddress);
                ee.Message.IsBodyHtml = true;
                ee.Message.To.Add(emailAddress);
                ee.SendMail();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        internal static void LoginOffice365(string username, string password)
        {
            throw new NotImplementedException();
        }
    }
}
