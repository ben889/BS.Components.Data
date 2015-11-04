namespace BS.Components.Data.DataProvider
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;
    using System.Web;

    public class FileHelper
    {
        public static string CheckFilename(string fullname, string filetype)
        {
            int num = 0;
            string directoryName = Path.GetDirectoryName(fullname);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fullname);
            string str3 = fileNameWithoutExtension;
            string extension = Path.GetExtension(fullname);
            while (true)
            {
                if (Directory.GetFiles(directoryName, str3 + "." + filetype).Length <= 0)
                {
                    return Path.Combine(directoryName, str3 + extension);
                }
                num++;
                str3 = string.Concat(new object[] { fileNameWithoutExtension, "(", num, ")" });
            }
        }

        public static string CreatFileName(string fileExt)
        {
            string str = DateTime.Now.ToString("yyyy-MM-dd,hh-mm-ss,");
            Random random = new Random();
            for (int i = 0; i < 5; i++)
            {
                str = str + random.Next(9);
            }
            return (str + fileExt);
        }

        public static bool Delete(string fullname)
        {
            if (File.Exists(fullname))
            {
                FileInfo info = new FileInfo(fullname);
                if (info.IsReadOnly)
                {
                    info.IsReadOnly = false;
                }
                try
                {
                    File.Delete(fullname);
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }

        public static void FlushFileToWeb(string displayName, string fileName, bool isDeleteFile)
        {
            if (File.Exists(fileName))
            {
                FileInfo info = new FileInfo(fileName);
                string str = info.Extension.ToLower();
                int startIndex = displayName.LastIndexOf('.');
                if ((startIndex == -1) || (displayName.ToLower().Substring(startIndex) != str))
                {
                    displayName = displayName + str;
                }
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.ClearHeaders();
                HttpContext.Current.Response.Buffer = false;
                HttpContext.Current.Response.ContentType = "application/octet-stream";
                HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(displayName, Encoding.UTF8).Replace("+", " "));
                HttpContext.Current.Response.AppendHeader("Content-Length", info.Length.ToString());
                HttpContext.Current.Response.WriteFile(fileName);
                HttpContext.Current.Response.Flush();
                if (isDeleteFile)
                {
                    File.Delete(fileName);
                }
            }
        }

        public static string GetFileMd5(string fullfile)
        {
            FileStream inputStream = new FileStream(fullfile, FileMode.Open, FileAccess.Read, FileShare.Read);
            MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();
            string str = BitConverter.ToString(provider.ComputeHash(inputStream));
            provider.Clear();
            inputStream.Dispose();
            return str;
        }

        public static bool Move(string orgfile, string descfile, bool overwrite)
        {
            if (!File.Exists(orgfile))
            {
                return false;
            }
            FileInfo info = new FileInfo(orgfile);
            if (info.IsReadOnly)
            {
                info.IsReadOnly = false;
            }
            if (File.Exists(descfile))
            {
                if (overwrite)
                {
                    if (!Delete(descfile))
                    {
                        return false;
                    }
                }
                else
                {
                    int num = 0;
                    int length = descfile.LastIndexOf(".") - 1;
                    string format = descfile.Substring(0, length) + "({0})" + descfile.Substring(length + 1);
                    while (true)
                    {
                        num++;
                        string path = string.Format(format, num);
                        if (!File.Exists(path))
                        {
                            descfile = path;
                            break;
                        }
                    }
                }
            }
            try
            {
                File.Move(orgfile, descfile);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static string ReadFileByFullname(string fullname)
        {
            if (!File.Exists(fullname))
            {
                return "";
            }
            StreamReader reader = new StreamReader(fullname, Encoding.UTF8);
            string str = reader.ReadToEnd();
            reader.Close();
            return str;
        }

        public static bool Rename(string orgfile, ref string newfile, bool overwrite)
        {
            if (!File.Exists(orgfile))
            {
                return false;
            }
            FileInfo info = new FileInfo(orgfile);
            if (info.IsReadOnly)
            {
                info.IsReadOnly = false;
            }
            if (File.Exists(newfile))
            {
                if (overwrite)
                {
                    if (!Delete(newfile))
                    {
                        return false;
                    }
                }
                else
                {
                    int num = 0;
                    int length = newfile.LastIndexOf(".") - 1;
                    string format = newfile.Substring(0, length) + "({0})" + newfile.Substring(length + 1);
                    while (true)
                    {
                        num++;
                        string path = string.Format(format, num);
                        if (!File.Exists(path))
                        {
                            newfile = path;
                            break;
                        }
                    }
                }
            }
            try
            {
                File.Move(orgfile, newfile);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static void WriteFileByFullname(string fullname, string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            FileStream stream = new FileStream(fullname, FileMode.Create, FileAccess.Write, FileShare.Write);
            stream.Write(bytes, 0, bytes.Length);
            stream.Flush();
            stream.Close();
        }

        public static void WriteFileByFullname(string fullname, byte[] data)
        {
            FileStream stream = new FileStream(fullname, FileMode.Create, FileAccess.Write, FileShare.Write);
            if (data != null)
            {
                stream.Write(data, 0, data.Length);
                stream.Flush();
            }
            stream.Close();
        }
    }
}

