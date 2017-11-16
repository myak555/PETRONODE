using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace Petronode.CommonControls
{
    public partial class ImageLoadControl : UserControl
    {
        private string m_FileName = "";

        public ImageLoadControl()
        {
            InitializeComponent();
            comboBox1.MouseWheel += new MouseEventHandler(ComboWheelMove);
            button1.MouseWheel += new MouseEventHandler(ComboWheelMove);
            button2.MouseWheel += new MouseEventHandler(ComboWheelMove);
        }

        /// <summary>
        /// Loads image file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonLoad_Click(object sender, EventArgs e)
        {
            string fn = "";
            if (comboBox1.Items.Count >= 1) fn = (string)comboBox1.SelectedItem;
            if (File.Exists(fn)) openFileDialog1.FileName = fn;
            if (openFileDialog1.ShowDialog() != DialogResult.OK) return;
            this.FileName = openFileDialog1.FileName;
        }

        /// <summary>
        /// Performs the picture reload
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            enableControls( false);
            if (OnFileLoadReceived != null) OnFileLoadReceived(this);
            enableControls(true);
        }

        /// <summary>
        /// Delegate is fired on the file selection change or refresh command
        /// </summary>
        public delegate void OnFileLoadDelegate(object sender);
        public OnFileLoadDelegate OnFileLoadReceived = null;

        /// <summary>
        /// Delegate is fired on the mouse scroll outside the combo box
        /// </summary>
        public MouseEventHandler OnMouseScrollReceived = null;

        /// <summary>
        /// Sets and retrieves the current name
        /// </summary>
        public string FileName
        {
            get { return m_FileName; }
            set
            {
                if (value.Length == 0) return;
                if (value == "none") return;
                enableControls(false);
                this.m_FileName = value;
                for (int i = 0; i < comboBox1.Items.Count; i++)
                {
                    if ((string)comboBox1.Items[i] != value) continue;
                    comboBox1.Items.RemoveAt(i);
                    break;
                }
                ClearHistory();
                while (comboBox1.Items.Count > 20) comboBox1.Items.RemoveAt(20);
                comboBox1.Items.Insert(0, value);
                comboBox1.SelectedIndex = 0;
                if (OnFileLoadReceived != null) OnFileLoadReceived(this);
                enableControls(true);
            }
        }

        /// <summary>
        /// Deletes all names in the history
        /// </summary>
        public void ClearHistory()
        {
            for (int i = comboBox1.Items.Count - 1; i >= 0; i--)
            {
                string s = (string)comboBox1.Items[i]; 
                if (s.Length > 0 || s != "none") continue;
                comboBox1.Items.RemoveAt(i);
            }
        }

        private void enableControls(bool state)
        {
            button1.Enabled = state;
            button2.Enabled = state;
        }

        private void ComboWheelMove(object sender, MouseEventArgs e)
        {
            if (OnMouseScrollReceived == null) return;
            if (0 < e.X && e.X < comboBox1.Width && 0 < e.Y && e.Y < comboBox1.Height) return;
            OnMouseScrollReceived(this, e);
        }

        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            if (comboBox1.Items.Count >= 1 && comboBox1.SelectedIndex >= 0)
                comboBox1.Items[comboBox1.SelectedIndex] = comboBox1.Text;
            m_FileName = comboBox1.Text;
            if (OnFileLoadReceived != null) OnFileLoadReceived(this);
        }
    }
}
