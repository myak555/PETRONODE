using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Petronode.CommonData;

namespace Petronode.CommonControls
{
    public partial class ToolLauncherControl : UserControl
    {
        string m_HelpLocation = "";
        string[] m_Args = null;

        public ToolLauncherControl()
        {
            InitializeComponent();
        }

        public string HelpLocation
        {
            get { return m_HelpLocation; }
            set { m_HelpLocation = value; }
        }

        private void toolStripButtonHelp_Click(object sender, EventArgs e)
        {
            if (m_HelpLocation == "") ToolLauncher.LaunchBrowser();
            else ToolLauncher.LaunchLocalHelp(m_HelpLocation);
        }

        private void toolStripButtonColor_Click(object sender, EventArgs e)
        {
            ToolLauncher.LaunchPetronodeUtility("Petronode.ColorPicker");
        }

        private void toolStripButtonFont_Click(object sender, EventArgs e)
        {
            FontHelpForm fhf = new FontHelpForm();
            fhf.Show();
        }

        private void toolStripButtonDigiPaint_Click(object sender, EventArgs e)
        {
            if (OnGetArgs != null) m_Args = OnGetArgs();
            ToolLauncher.LaunchPetronodeUtility("Petronode.DigiPaint");
        }

        private void toolStripButtonDigitizer_Click(object sender, EventArgs e)
        {
            if (OnGetArgs != null) m_Args = OnGetArgs();
            ToolLauncher.LaunchPetronodeUtility("Petronode.Digitizer");
        }

        private void toolStripDigiFitter_Click(object sender, EventArgs e)
        {
            if (OnGetArgs != null) m_Args = OnGetArgs();
            ToolLauncher.LaunchPetronodeUtility("Petronode.DigiFitter");
        }

        public delegate string[] GetArgsDelegate();
        public GetArgsDelegate OnGetArgs = null;
    }
}
