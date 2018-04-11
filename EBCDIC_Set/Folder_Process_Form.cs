using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace MY.Weatherford.EBCDIC_Set
{
    public partial class Folder_Process_Form : Form
    {
        DirectoryInfo di = null;
        byte[] EBCDICHeader = null;

        public Folder_Process_Form( string folder, byte[] EBCDIC)
        {
            InitializeComponent();
            EBCDICHeader = EBCDIC;
            string[] tmp = new string[2];
            tmp[1] = "false";
            di = new DirectoryInfo(folder);
            FileInfo[] fis = di.GetFiles("*.s*");
            foreach (FileInfo fi in fis)
            {
                string t = fi.Extension.ToUpper();
                if (!t.StartsWith(".SGY") && !t.StartsWith(".SEG")) continue;
                tmp[0] = fi.Name;
                dataGridView1.Rows.Add(tmp);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string message = "";
            for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
            {
                try
                {
                    string name = di.FullName + "\\" + dataGridView1.Rows[i].Cells[0].Value.ToString();
                    FileStream fs = File.Open(name, FileMode.Open, FileAccess.Write, FileShare.Read);
                    fs.Write(EBCDICHeader, 0, EBCDICHeader.Length);
                    fs.Close();
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                    continue;
                }
                dataGridView1.Rows[i].Cells[1].Value = true;
            }
            if (message.Length > 0)
            {
                MessageBox.Show("Some files have not been processed: " + message,
                    "Some files are not processed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            this.Close();
        }
    }
}