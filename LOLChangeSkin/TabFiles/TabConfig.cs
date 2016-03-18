using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

public class TabConfigs
{
    static TabConfigs()
    {
        Read();
    }
    public static string lastGamePath = "";
    public static void Read()
    {
        try
        {
            FileStream fs = new FileStream("config.txt", FileMode.Open);
            if (fs != null)
            {
                StreamReader sr = new StreamReader(fs);
                string line = sr.ReadLine();
                if (line != null)
                {
                    lastGamePath = line;
                }
                sr.Close();
            }
        }
        catch
        { }
    }
    public static void Write(string txt)
    {
        if (string.IsNullOrEmpty(txt))
            return;

        FileStream fs = new FileStream("config.txt", FileMode.Create);
        if (fs != null)
        {
            StreamWriter sr = new StreamWriter(fs, Encoding.UTF8);
            sr.WriteLine(txt);
            sr.Close();
        }
    }
}
public class TabHeros
{
    public Int32 idx = -1;
    public string name;
    public string dir;
    public List<string> skingnames = new List<string>();

    public static List<TabHeros> lsTabConfig = new List<TabHeros>();

    public static void Read()
    {
        bool isok = false;
        TabReader tr = new TabReader("heros.txt", out isok);
        if (tr != null && isok)
        {
            for (int i = 0; i < tr.recordCount; i++)
            {
                TabHeros tc = new TabHeros();
                tc.idx = i;
                tc.name = tr.GetString(i, "name");
                tc.dir = tr.GetString(i, "dir");
                tc.skingnames = GetDirectoryFileNames(string.Format("Resource\\{0}", tc.dir));
                
                lsTabConfig.Add(tc);
            }
        }
    }
    static private List<string> GetDirectoryFileNames(string path)
    {
        List<string> ls = new List<string>();
        string[] fileNames = Directory.GetFiles(path);
        foreach (string file in fileNames)
        {
            if (file.Contains(".zip"))
            {
                string v1 = file.Replace(path, "");
                string v2 = v1.Replace(".zip", "");
                string v3 = v2.Replace("\\", "");
                ls.Add(v3);
            }
        }
        return ls;
    }
}