namespace BS.Components.Data.DataProvider
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    public class SerializationHelper
    {
        private static Dictionary<int, XmlSerializer> serializer_dict = new Dictionary<int, XmlSerializer>();

        public static object DeSerialize(Type type, string s)
        {
            object obj2;
            byte[] bytes = Encoding.UTF8.GetBytes(s);
            try
            {
                obj2 = GetSerializer(type).Deserialize(new MemoryStream(bytes));
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return obj2;
        }

        public static XmlSerializer GetSerializer(Type t)
        {
            int hashCode = t.GetHashCode();
            if (!serializer_dict.ContainsKey(hashCode))
            {
                serializer_dict.Add(hashCode, new XmlSerializer(t));
            }
            return serializer_dict[hashCode];
        }

        public static object Load(Type type, string fullfile)
        {
            FileStream stream = null;
            object obj2;
            try
            {
                stream = new FileStream(fullfile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                obj2 = new XmlSerializer(type).Deserialize(stream);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }
            return obj2;
        }

        public static bool Save(object obj, string fullfile)
        {
            bool flag = false;
            FileStream stream = null;
            try
            {
                stream = new FileStream(fullfile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                new XmlSerializer(obj.GetType()).Serialize((Stream) stream, obj);
                flag = true;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }
            return flag;
        }

        public static string Serialize(object obj)
        {
            string str = "";
            XmlSerializer serializer = GetSerializer(obj.GetType());
            MemoryStream w = new MemoryStream();
            XmlTextWriter writer = null;
            StreamReader reader = null;
            try
            {
                writer = new XmlTextWriter(w, Encoding.UTF8) {
                    Formatting = Formatting.Indented
                };
                serializer.Serialize((XmlWriter) writer, obj);
                w.Seek(0L, SeekOrigin.Begin);
                reader = new StreamReader(w);
                str = reader.ReadToEnd();
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                }
                if (reader != null)
                {
                    reader.Close();
                }
                w.Close();
            }
            return str;
        }
    }
}

