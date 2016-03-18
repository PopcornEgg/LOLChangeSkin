using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace LOLChangeSkin
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Init();
        }
        void Init()
        {

            TabHeros.Read();

            textBox2.Text = TabConfigs.lastGamePath;

            for (int i = 0; i < TabHeros.lsTabConfig.Count; i++)
            {
                TabHeros tc = TabHeros.lsTabConfig[i];
                listBox1.Items.Add(tc.name);
            }

            listView1.LargeImageList = imageList1;
            radioButton1.Tag = "c:\\";
            radioButton2.Tag = "d:\\";
            radioButton3.Tag = "e:\\";
            radioButton4.Tag = "f:\\";
            radioButton5.Tag = "g:\\";
            radioButton6.Tag = "h:\\";
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            pictureBox1.Image = null;
            int idx = listBox1.SelectedIndex;
            if (idx < 0 || idx >= TabHeros.lsTabConfig.Count)
                return;

            TabHeros tc = TabHeros.lsTabConfig[idx];
            label1.Text = tc.name;
            imageList1.Images.Clear();
            listView1.Items.Clear();
            for (int i = 0; i < tc.skingnames.Count; i++)
            {
                imageList1.ImageSize = new Size(190, 240);
                imageList1.Images.Add(Image.FromFile(String.Format("Resource\\{0}\\{1}.jpg", tc.dir, tc.skingnames[i]), true));

                listView1.Items.Add(tc.skingnames[i]);
                listView1.Items[i].ImageIndex = i;
            }

            listView1.Tag = tc;
            //listView1.Items.ob
        }
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabHeros tc = listView1.Tag as TabHeros;
            if(tc != null && listView1.SelectedItems.Count == 1)
            {
                int idx = listView1.SelectedItems[0].ImageIndex;
                pictureBox1.Image = Image.FromFile(String.Format("Resource\\{0}\\{1}.jpg", tc.dir, tc.skingnames[idx]));
                label2.Text = tc.skingnames[idx];
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox2.Text))
            {
                textBox1.Text = "未设置游戏路径！";
                return;
            }
                
            TabHeros tc = listView1.Tag as TabHeros;
            if (tc != null && listView1.SelectedItems.Count == 1)
            {
                int idx = listView1.SelectedItems[0].ImageIndex;
                string sourPath = String.Format("Resource\\{0}\\{1}.zip", tc.dir, tc.skingnames[idx]);
                string destPath = String.Format("{0}\\Game\\{1}.zip", textBox2.Text, tc.dir);
                System.IO.File.Copy(sourPath, destPath, true);
                SetClientZipsTxt(tc.dir + ".zip");
                textBox1.Text = "设置成功！";
            }
            else
            {
                textBox1.Text = "未选择皮肤！";
                return;
            }
        }

        void SetClientZipsTxt(string text)
        {
            string destPath = String.Format("{0}\\Game\\ClientZips.txt", textBox2.Text);

            bool ishas = false;
            bool isxxx_xx_x = false;
            String xxx_xx_x = "xxx_xx_x";
            FileStream fs = new FileStream(destPath, FileMode.Open);
            if (fs != null)
            {
                StreamReader sr = new StreamReader(fs);
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if(line.Contains(text))
                    {
                        ishas = true;
                    }
                    else if (line.Contains(xxx_xx_x))
                    {
                        isxxx_xx_x = true;
                    }
                }
                sr.Close();
            }

            //先把原来的备份
            if (!isxxx_xx_x)
            {
                System.IO.File.Copy(destPath, String.Format("{0}\\Game\\ClientZips_old.txt", textBox2.Text), true);

                FileStream fs2 = new FileStream(destPath, FileMode.Append);
                if (fs != null)
                {
                    StreamWriter sr = new StreamWriter(fs2);
                    //sr.WriteLine("\r" + xxx_xx_x);
                    sr.WriteLine(xxx_xx_x);
                    sr.Close();
                }
            }

            if (!ishas)
            {
                FileStream fs2 = new FileStream(destPath, FileMode.Append);
                if (fs != null)
                {
                    StreamWriter sr = new StreamWriter(fs2);
                    sr.WriteLine(text);
                    sr.Close();
                }
            }
        }

        string searchRoot = "";
        Thread threadDirectories = null;
        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            searchRoot = rb.Tag.ToString();

            StopThreadDirectories();
            threadDirectories = new Thread(new ThreadStart(StartSearch));
            threadDirectories.Start();
        }

//         private void getxxxDirectories(string path)
//         {
//             string[] fileNames = Directory.GetFiles(path);
//             string[] directories = Directory.GetDirectories(path);
//             foreach (string file in fileNames)
//             {
//                 Console.WriteLine("Filename:{0}", file);
//             }
//             foreach (string dir in directories)
//             {
//                 Console.WriteLine("Directoriesname:{0}", dir);
//                 getxxxDirectories(dir);
//             }
//         }


        delegate void SetTextCallback(string text);
        void StartSearch()
        {
            string istag = "";
            GetLOLDirectory(searchRoot, ref istag);
            SetTextBox2Text(istag);
        }

        private void SetTextBox2Text(string text)
        {
            if (string.IsNullOrEmpty(text))
                text = "未找到游戏目录！";

            if (this.textBox2.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetTextBox2Text);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.textBox2.Text = text;
            }
        }
        private void SetTextBox1Text(string text)
        {
            if (string.IsNullOrEmpty(text))
                text = "未找到游戏目录！";

            if (this.textBox1.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetTextBox1Text);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.textBox1.Text = text;
            }
        }
        
        private void GetLOLDirectory(string path, ref string istag)
        {
            try{
                string[] directories = Directory.GetDirectories(path);
                foreach (string dir in directories)
                {
                    if (istag.Length > 0)
                        return;

                    SetTextBox1Text("搜索目录：" + dir);

                    if (dir.Contains("英雄联盟"))
                    {
                        if (CheckLOLDirectory(dir))
                        {
                            istag = dir;
                            return;
                        }
                    }
                    else
                        GetLOLDirectory(dir, ref istag);
                }
            }
            catch(Exception exc)
            {
                SetTextBox1Text(exc.Message);
            }
            
        }
        private bool CheckLOLDirectory(string path)
        {
            try
            {
                int idx = 0;
                string[] directories = Directory.GetDirectories(path);
                foreach (string dir in directories)
                {
                    if (dir.Contains("Game"))
                    {
                        idx++;
                    }
                    else if (dir.Contains("TCLS"))
                    {
                        idx++;
                    }
                }
                return idx > 1;
            }
            catch (Exception exc)
            {
                SetTextBox1Text(exc.Message);
                return false;
            }
        }

        public void StopThreadDirectories()
        {
            if(threadDirectories != null)
            {
                threadDirectories.Abort();
            }
        }

        private void Form1_Closed(object sender, EventArgs e)
        {
            StopThreadDirectories();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            StopThreadDirectories();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            TabConfigs.Write(this.textBox2.Text);
        }
    }
}
