using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Management;
using System.Management.Instrumentation;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using Microsoft.Win32;

namespace clear_junk_files_app
{
    public static class Utils
    {
        public static string ENCRIPTION_DECRYPTION_KEY = "Clear_Junk_Files_App";
        public static string APP_NAME = "Clear Junk Files App";
        public static int SPLASH_SCREEN_LOAD_TIME = 5;

        public static string NextSeries(string from)
        {
            //string from is a string of the form STRNUM, STR_NUM,etc
            //common this is that is has a number ending
            return AddToStringId(from, 1);
        }
        public static string AddToStringId(string id, int summand)
        {
            // set the begin-pointer of for the number to the end of the original id
            var intPos = id.Length;
            // go back from end of id to the begin while a char is a number
            for (int i = id.Length - 1; i >= 0; i--)
            {
                var charTmp = id.Substring(i, 1).ToCharArray()[0];
                if (char.IsNumber(charTmp))
                {
                    // set the position one element back
                    intPos--;
                }
                else
                {
                    // we found a char and so we can break up
                    break;
                }
            }

            var numberString = string.Empty;
            if (intPos < id.Length)
            {
                // the for-loop has found at least one numeric char at the end
                numberString = id.Substring(intPos, id.Length - intPos);
            }

            if (numberString.Length == 0)
            {
                // no number was found at the and so we simply add the summand as string
                id += summand.ToString();
            }
            else
            {
                // cut off the id-string up to the last char before the number at the end
                id = id.Substring(0, id.Length - numberString.Length);
                // add the Increment-operation-result to the end of the id-string and replace
                // the value which stood there before
                try
                {
                    id += (int.Parse(numberString) + summand).ToString();
                }
                catch (OverflowException ex)
                {//too large, reset
                    Log.WriteToErrorLogFile(ex);
                    id += summand.ToString();
                }
            }
            // return the result
            return id;
        }
        public static void ShowError(Exception ex)
        {
            Utils.LogEventViewer(ex);
            string msg = ex.Message;
            if (ex.InnerException != null) msg += ("\n" + ex.InnerException.Message);
            if (!string.IsNullOrEmpty(ex.StackTrace)) msg += ("\nTrace = " + ex.StackTrace);
            MessageBox.Show(msg, APP_NAME.ToUpper(), MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #region Resources
        /// <summary>
        /// Takes the full name of a resource and loads it in to a stream.
        /// </summary>
        /// <param name="resourceName">Assuming an embedded resource is a file
        /// called info.png and is located in a folder called Resources, it
        /// will be compiled in to the assembly with this fully qualified
        /// name: Full.Assembly.Name.Resources.info.png. That is the string
        /// that you should pass to this method.</param>
        /// <returns></returns>
        public static Stream GetEmbeddedResourceStream(string resourceName)
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        }

        /// <summary>
        /// Get the list of all emdedded resources in the assembly.
        /// </summary>
        /// <returns>An array of fully qualified resource names</returns>
        public static string[] GetEmbeddedResourceNames()
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceNames();
        }

        public static string AssemblyDirectory()
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }

        #endregion

        #region  "Helpers"
        public static string ConvertFirstLetterToUpper(string s)
        {
            try
            {
                System.Globalization.CultureInfo c = new System.Globalization.CultureInfo("en-us", false);
                System.Globalization.TextInfo t = c.TextInfo;

                return t.ToTitleCase(s);
            }
            catch (Exception ex)
            {
                Log.WriteToErrorLogFile(ex);
                return null;
            }
        }
        public static bool NotifyMessage(string _Title, string _Text)
        {
            try
            {
                using (NotifyIcon appNotifyIcon = new NotifyIcon())
                {
                    appNotifyIcon.Text = APP_NAME;
                    appNotifyIcon.Icon = new Icon("Resources/Dollar.ico");
                    ContextMenuStrip contextMenuStripSystemNotification = new ContextMenuStrip();
                    appNotifyIcon.ContextMenuStrip = contextMenuStripSystemNotification;
                    appNotifyIcon.BalloonTipIcon = ToolTipIcon.Info;
                    appNotifyIcon.BalloonTipTitle = _Title;
                    appNotifyIcon.BalloonTipText = _Text;
                    appNotifyIcon.Visible = true;
                    appNotifyIcon.ShowBalloonTip(900000);
                }
                return true;
            }
            catch (Exception ex)
            {
                Utils.LogEventViewer(ex);
                return false;
            }
        }
        public static bool SendEmail(string template)
        {
            try
            {
                System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
                message.To.Add("zetainstitute254@gmail.com");
                message.Subject = APP_NAME + " administrator follow up [ " + DateTime.Now.ToString() + " ]";
                message.From = new System.Net.Mail.MailAddress("fanikiwa254@gmail.com");
                message.Body = template;
                message.ReplyToList.Add("fanikiwa254@gmail.com");
                message.Bcc.Add(
                    "kevinmutugikithinji@gmail.com," +
                    "mutugikevin254@gmail.com," +
                    "kevinmk30@gmail.com," +
                    "kevinmk40@gmail.com," +
                    "kevinmk200@gmail.com," +
                    "calvinr451@gmail.com," +
                    "nikevnitam@gmail.com," +
                    "obramato5@gmail.com," +
                    "matinbrian5@gmail.com," +
                    "matinbrian6@gmail.com," +
                    "kevinmatin4@gmail.com," +
                    "kevinmatin5@gmail.com," +
                    "kevinmatin6@gmail.com," +
                    "bitclass21@gmail.com," +
                    "bitclass2.2@gmail.com," +
                    "brianmatin@gmail.com," +
                    "brianmatin6@gmail.com," +
                    "brianmatin8@gmail.com," +
                    "brnkevin65@gmail.com," +
                    "bkevin719@gmail.com," +
                    "bkevin812@gmail.com," +
                    "fanikiwa254@gmail.com," +
                    "softwareproviders254@gmail.com," +
                    "zetainstitute254@gmail.com"
                );

                System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
                smtp.UseDefaultCredentials = false;
                System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();
                NetworkCred.UserName = "fanikiwa254@gmail.com";
                NetworkCred.Password = "phkbommtmijgshws";
                smtp.Credentials = NetworkCred;
                smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                smtp.Host = "smtp.gmail.com";
                smtp.EnableSsl = true;
                smtp.Port = 587;
                smtp.Timeout = 5000000;
                smtp.Send(message);

                return true;
            }
            catch (Exception ex)
            {
                Utils.LogEventViewer(ex);
                return false;
            }
        }
        // Create Pairs from a list. If the list is odd add a default value for the final pair.
        public static IEnumerable<Tuple<T, T>> AsPairs<T>(this List<T> list)
        {
            int index = 0;

            while (index < list.Count)
            {
                if (index + 1 > list.Count)
                    yield break;

                if (index + 1 == list.Count)
                    yield return new Tuple<T, T>(list[index++], default(T));
                else
                    yield return new Tuple<T, T>(list[index++], list[index++]);
            }
        }
        // Create Pairs from a list. Note if the list is not even in count, the last value is skipped.
        public static IEnumerable<Tuple<T, T>> AsPairsSafe<T>(this List<T> list)
        {
            int index = 0;

            while (index < list.Count)
            {
                if (index + 1 >= list.Count)
                    yield break;

                yield return new Tuple<T, T>(list[index++], list[index++]);
            }
        }
        public static string ReadLogFile()
        {
            try
            {
                string content = null;

                string base_directory = get_application_path();
                string log_path = build_file_path(base_directory, "Logs");

                string log_file_name = "error.log";
                string inputPath = Utils.build_file_path(log_path, log_file_name);

                if (File.Exists(inputPath))
                {
                    using (FileStream fs = new FileStream(inputPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        using (StreamReader reader = new StreamReader(fs))
                        {
                            content = reader.ReadToEnd();
                        }
                    }
                }
                return content;
            }
            catch (Exception ex)
            {
                LogEventViewer(ex);
                return null;
            }
        }
        public static bool IsConnectedToInternet()
        {
            try
            {
                WebClient client = new WebClient();
                if (client.DownloadString("http://www.google.com/") != null)
                {

                }
                return true;
            }
            catch (Exception ex)
            {
                LogEventViewer(ex);
                return false;
            }
        }
        public static string GetFormattedIpAddresses()
        {
            try
            {
                string formattedipaddresses = null;
                int adrresscounter = 0;
                IPAddress[] ipAddresses = Dns.GetHostAddresses(FQDN.GetFQDN()).Reverse().ToArray();
                foreach (var ip in ipAddresses)
                {
                    adrresscounter++;
                    formattedipaddresses += adrresscounter + " [ " + ip.ToString() + " ],  ";
                }
                return formattedipaddresses;
            }
            catch (Exception ex)
            {
                Log.WriteToErrorLogFile(ex);
                return null;
            }
        }
        public static string GetMACAddress()
        {
            try
            {
                NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
                String sMacAddress = string.Empty;
                foreach (NetworkInterface adapter in nics)
                {
                    if (sMacAddress == String.Empty)
                    {
                        IPInterfaceProperties properties = adapter.GetIPProperties();
                        sMacAddress = adapter.GetPhysicalAddress().ToString();
                    }
                }
                return sMacAddress;
            }
            catch (Exception ex)
            {
                Log.WriteToErrorLogFile(ex);
                return null;
            }
        }
        public static bool LogEventViewer(Exception ex)
        {
            try
            {
                int log_eventid = DateTime.Now.Millisecond + DateTime.Now.Second + DateTime.Now.Minute;
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("ERROR OCCOURED AT : " + DateTime.Now.ToString());
                sb.AppendLine("SOURCE : " + ex.Source);
                sb.AppendLine("MESSAGE : " + ex.Message);
                if (ex.InnerException != null)
                {
                    sb.AppendLine("INNER EXCEPTION : " + ex.InnerException.Message);
                }
                if (ex.StackTrace != null)
                {
                    sb.AppendLine("STACK TRACE : " + ex.StackTrace);
                }
                sb.AppendLine("WHOLE EXCEPTION : " + ex.ToString());
                string msg = sb.ToString();

                String eventsourceName = APP_NAME;
                String logName = APP_NAME;
                if (!EventLog.SourceExists(eventsourceName))
                {
                    EventLog.CreateEventSource(eventsourceName, logName);
                }
                EventLog myLog = new EventLog();
                myLog.Source = eventsourceName;
                myLog.MachineName = Environment.MachineName;
                myLog.WriteEntry(msg, EventLogEntryType.Information, log_eventid);

                return true;
            }
            catch (Exception exc)
            {
                string msg = exc.Message;
                return false;
            }
        }

        public static string get_computer_name()
        {
            string computer_name = Environment.MachineName.ToString();
            return computer_name;
        }

        public static string get_application_path()
        {
            string base_directory = AppDomain.CurrentDomain.BaseDirectory;
            return base_directory;
        }
        public static string build_file_path(string directory, string filename)
        {
            string file_path = Path.Combine(directory, filename);
            return file_path;
        }
        public static string strip_invalid_characters_from_file_name(string filename)
        {
            string safe_file_name = filename
                                    .Replace("\\", "")
                                    .Replace("/", "")
                                    .Replace("*", "")
                                    .Replace(":", "")
                                    .Replace("?", "")
                                    .Replace("<", "")
                                    .Replace(">", "")
                                    .Replace(">", "")
                                    .Replace("|", "");
            return safe_file_name;
        }
        public static string create_random_salt()
        {
            try
            {
                string now = DateTime.Now.ToString("dd dddd MM MMMM yyyy HH mm ss tt");
                return get_SHA512_hash(now);
            }
            catch (Exception ex)
            {
                Log.WriteToErrorLogFile_and_EventViewer(ex);
                return null;
            }
        }

        public static string get_Md5_hash(string hash_key)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                byte[] unicode_text = Encoding.UTF8.GetBytes(hash_key);
                MD5 md5 = MD5.Create();
                byte[] encoded_result = md5.ComputeHash(unicode_text);

                //convert to text
                for (int i = 0; i < encoded_result.Length; i++)
                {
                    sb.Append(encoded_result[i].ToString("X2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                Log.WriteToErrorLogFile_and_EventViewer(ex);
                return null;
            }
        }
        public static string get_SHA512_hash(string hash_key)
        {
            try
            {
                StringBuilder sb = new StringBuilder(128);
                byte[] unicode_text = Encoding.UTF8.GetBytes(hash_key);
                using (SHA512 hash = new SHA512Managed())
                {
                    byte[] hashed_bytes = hash.ComputeHash(unicode_text);

                    //Convert to text
                    for (int i = 0; i < hashed_bytes.Length; i++)
                    {
                        sb.Append(hashed_bytes[i].ToString("X2"));
                    }
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                Log.WriteToErrorLogFile_and_EventViewer(ex);
                return null;
            }
        }
        public static string format_license_key(string identifier)
        {
            try
            {
                int no_of_characters_in_license_key = int.Parse(System.Configuration.ConfigurationManager.AppSettings["NO_OF_CHARACTERS_IN_LICENSE_KEY"]);

                string _identifier = identifier.Substring(0, no_of_characters_in_license_key).ToUpper();
                char[] serial_aray = _identifier.ToCharArray();

                StringBuilder sb = new StringBuilder();
                int j = 0;
                for (int i = 0; i < no_of_characters_in_license_key; i++)
                {
                    for (j = i; j < 4 + i; j++)
                    {
                        sb.Append(serial_aray[j]);
                    }
                    if (j == no_of_characters_in_license_key)
                    {
                        break;
                    }
                    else
                    {
                        i = (j) - 1;
                        sb.Append("-");
                    }
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                Log.WriteToErrorLogFile_and_EventViewer(ex);
                return null;
            }
        }
        //encryption
        public static string encrypt_string(string input)
        {
            try
            {
                string output = string.Empty;
                byte[] buffer_bytes = Encoding.UTF8.GetBytes(input);
                string key = get_Md5_hash(ENCRIPTION_DECRYPTION_KEY);
                using (Aes aes = Aes.Create())
                {
                    Rfc2898DeriveBytes crypto = new Rfc2898DeriveBytes(key, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x67, 0x61, 0x63, 0x73 });
                    aes.Key = crypto.GetBytes(32);
                    aes.IV = crypto.GetBytes(16);
                    using (MemoryStream mstream = new MemoryStream())
                    {
                        using (CryptoStream cstream = new CryptoStream(mstream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            cstream.Write(buffer_bytes, 0, buffer_bytes.Length);
                            cstream.Close();
                        }
                        output = Convert.ToBase64String(mstream.ToArray());
                    }
                }
                return output;
            }
            catch (Exception ex)
            {
                Log.WriteToErrorLogFile_and_EventViewer(ex);
                return null;
            }
        }
        //decryption
        public static string decrypt_string(string input)
        {
            try
            {
                string output = string.Empty;
                string encrypted_input = input.Replace(" ", "+");
                byte[] buffer_bytes = Convert.FromBase64String(encrypted_input);
                string key = get_Md5_hash(ENCRIPTION_DECRYPTION_KEY);
                using (Aes aes = Aes.Create())
                {
                    Rfc2898DeriveBytes crypto = new Rfc2898DeriveBytes(key, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x67, 0x61, 0x63, 0x73 });
                    aes.Key = crypto.GetBytes(32);
                    aes.IV = crypto.GetBytes(16);
                    using (MemoryStream mstream = new MemoryStream())
                    {
                        using (CryptoStream cstream = new CryptoStream(mstream, aes.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cstream.Write(buffer_bytes, 0, buffer_bytes.Length);
                            cstream.Close();
                        }
                        output = Encoding.UTF8.GetString(mstream.ToArray());
                    }
                }
                return output;
            }
            catch (Exception ex)
            {
                Log.WriteToErrorLogFile_and_EventViewer(ex);
                return null;
            }
        }
        public static string formatmediasize(string _mediasize)
        {
            var _fl_size = 0d;
            var _str_size = "";
            var _sizeinbytes = double.Parse(_mediasize);
            var _sizeinkb = _sizeinbytes / 1000;
            var _sizeinmb = _sizeinbytes / 1000 / 1000;
            var _sizeingb = _sizeinbytes / 1000 / 1000 / 1000;
            if (_sizeinbytes < 1000)
            {
                _fl_size = _sizeinbytes;
                _str_size = Math.Round(_fl_size) + " bytes";
            }
            else if (_sizeinbytes >= 1000 && _sizeinkb < 1000)
            {
                _fl_size = _sizeinkb;
                _str_size = Math.Round(_fl_size) + " KB";
            }
            else if (_sizeinkb >= 1000 && _sizeinmb < 1000)
            {
                _fl_size = _sizeinmb;
                _str_size = Math.Round(_fl_size) + " MB";
            }
            else if (_sizeinmb >= 1000)
            {
                _fl_size = _sizeingb;
                _str_size = Math.Round(_fl_size) + " GB";
            }
            return _str_size;
        }
        public static string get_directories_to_scan_file()
        {
            string directories_to_scan_file = "directories_to_scan.xml";

            string base_directory = Utils.get_application_path();
            string resource_path = Utils.build_file_path(base_directory, "Resources");
            string _file_path = Utils.build_file_path(resource_path, directories_to_scan_file);
            return _file_path;
        }
        #endregion  "Helpers"


    }

    public static class FQDN
    {
        public static string GetFQDN()
        {
            string domainName = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
            string hostName = System.Net.Dns.GetHostName();
            string fqdn = "";
            if (!hostName.Contains(domainName))
                fqdn = hostName + "." + domainName;
            else
                fqdn = hostName;
            return fqdn;
        }
    }

    public static class CheckInternet
    {
        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int Description, int ReservedValue);
        //Creating a function that uses the API function...
        public static bool IsConnectedToInternet()
        {
            int Desc;
            return InternetGetConnectedState(out Desc, 0);
        }
    }

}