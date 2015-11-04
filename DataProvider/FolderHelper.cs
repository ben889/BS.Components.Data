namespace BS.Components.Data.DataProvider
{
    using System;
    using System.IO;

    public class FolderHelper
    {
        public static void Delete(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
        }

        public static bool IsEmpty(string path)
        {
            return (!Directory.Exists(path) || LoopCheckIsEmpty(path));
        }

        private static bool LoopCheckIsEmpty(string path)
        {
            string[] files = Directory.GetFiles(path);
            foreach (string str in files)
            {
                FileInfo info = new FileInfo(str);
                if (info.Length > 0L)
                {
                    return false;
                }
            }
            string[] directories = Directory.GetDirectories(path);
            int index = 0;
            while (index < directories.Length)
            {
                string str2 = directories[index];
                return LoopCheckIsEmpty(str2);
            }
            return true;
        }
    }
}

