using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewSanofi.ClassHelper
{
    public static class TextFileProcess
    {
        public static void AppendFile(String text, string line)
        {
            string filename = text + ".txt";
            string path = Path.Combine(MainWindow.currentDirectory, @"Data\", filename);
            if (!File.Exists(path))
            {
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine(line);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine(line);
                }
            }
        }

        public static void DeleteFile(String text)
        {
            string filename = text + ".txt";
            string path = Path.Combine(MainWindow.currentDirectory, @"Data\", filename);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public static void DeleteFile1(String text)
        {
            string path = Path.Combine(MainWindow.currentDirectory, text);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        static public List<string> ReadFile(String text)
        {
            List<string> lsString = new List<string>();
            string line;
            string filename = text + ".txt";
            string path = Path.Combine(MainWindow.currentDirectory, @"Data\", filename);
            using (StreamReader sr = new StreamReader(path))
            {
                line = sr.ReadLine();
                while (line != null)
                {
                    lsString.Add(line);
                    line = sr.ReadLine();
                }
                sr.Close();
            }
            return lsString;
        }

        static public List<string> ReadFileExtent(String path)
        {
            List<string> lsString = new List<string>();
            string line;
            
            using (StreamReader sr = new StreamReader(path))
            {
                line = sr.ReadLine();
                while (line != null)
                {
                    lsString.Add(line);
                    line = sr.ReadLine();
                }
                sr.Close();
            }
            return lsString;
        }

        static public void WriteFile(string fileName, List<string> content)
        {
            string filename = fileName + ".txt";
            string tempPath = Path.Combine(MainWindow.currentDirectory, @"Data\");
            string path = Path.Combine(Environment.CurrentDirectory, @"Data\", filename);
            bool exists = System.IO.Directory.Exists(tempPath);
            if (exists == false)
            {
                System.IO.Directory.CreateDirectory(tempPath);
            }
            try
            {
                StreamWriter sw = new StreamWriter(path);
                foreach (string c in content)
                {
                    sw.WriteLine(c);
                }
                sw.Close();
            }
            catch
            {
                return;
            }
        }


        static public void WriteFile(string fileName, List<string> content,string path1)
        {
            string filename = fileName + ".txt";
            string path = Path.Combine(path1, filename);
            bool exists = System.IO.Directory.Exists(path1);
            if (exists == false)
            {
                System.IO.Directory.CreateDirectory(path1);
            }
            try
            {
                StreamWriter sw = new StreamWriter(path);
                foreach (string c in content)
                {
                    sw.WriteLine(c);
                }
                sw.Close();
            }
            catch
            {
                return;
            }
        }

    }
}
