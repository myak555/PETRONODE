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
    public partial class FileLoadControl : UserControl
    {
        private string m_FileName = "";

        public FileLoadControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Saving file
        /// </summary>
        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (!File.Exists(m_FileName))
            {
                buttonSaveAs_Click(sender, e);
                return;
            }
            enableControls(false);
            if (OnFileSaveReceived != null) OnFileSaveReceived(this);
            enableControls(true);
        }

        /// <summary>
        /// Saving file - new name
        /// </summary>
        private void buttonSaveAs_Click(object sender, EventArgs e)
        {
            if (File.Exists(m_FileName)) saveFileDialog1.FileName = m_FileName;
            if (saveFileDialog1.ShowDialog() != DialogResult.OK) return;
            enableControls(false);
            this.FileName = saveFileDialog1.FileName;
            if (OnFileSaveReceived != null) OnFileSaveReceived(this);
            enableControls(true);
        }

        /// <summary>
        /// Loads file from disk
        /// </summary>
        private void buttonOpen_Click(object sender, EventArgs e)
        {
            if (File.Exists(m_FileName)) openFileDialog1.FileName = m_FileName;
            if (openFileDialog1.ShowDialog() != DialogResult.OK) return;
            enableControls(false);
            //if (File.Exists(m_FileName) && OnFileSaveReceived != null) OnFileSaveReceived(this);
            m_FileName = openFileDialog1.FileName;
            if (OnFileLoadReceived != null) OnFileLoadReceived(this);
            enableControls(true);
        }

        /// <summary>
        /// Reloads file from disk
        /// </summary>
        private void buttonReload_Click(object sender, EventArgs e)
        {
            enableControls(false);
            if (OnFileLoadReceived != null) OnFileLoadReceived(this);
            enableControls(true);
        }

        /// <summary>
        /// Sets and retrieves the current name
        /// </summary>
        public string FileName
        {
            get { return m_FileName; }
            set
            { 
                m_FileName = value;
                buttonReload_Click(this, null);
            }
        }

        /// <summary>
        /// Sets and retrieves the filter
        /// </summary>
        public string Filter
        {
            get { return saveFileDialog1.Filter; }
            set
            {
                saveFileDialog1.Filter = (value.Length <= 0)?
                    "Definition files|*.def|Comma Separated Files|*.csv|All files|*.*"
                    : value;
                openFileDialog1.Filter = saveFileDialog1.Filter;
            }
        }

        /// <summary>
        /// Delegate is fired on the file selection change or refresh command
        /// </summary>
        public delegate void OnFileOperationDelegate(object sender);
        public OnFileOperationDelegate OnFileLoadReceived = null;
        public OnFileOperationDelegate OnFileSaveReceived = null;

        /// <summary>
        /// Delegate is fired on the mouse scroll outside the combo box
        /// </summary>
        public MouseEventHandler OnMouseScrollReceived = null;

        private void enableControls(bool state)
        {
            button1.Enabled = state;
            button2.Enabled = state;
            button3.Enabled = state;
        }
    }
}
