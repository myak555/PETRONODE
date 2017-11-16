namespace Petronode.CommonControls
{
    partial class ToolLauncherControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ToolLauncherControl));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonHelp = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonColor = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonFont = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonHelp,
            this.toolStripButtonColor,
            this.toolStripButtonFont});
            this.toolStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.VerticalStackWithOverflow;
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(46, 300);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButtonHelp
            // 
            this.toolStripButtonHelp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonHelp.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonHelp.Image")));
            this.toolStripButtonHelp.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButtonHelp.ImageTransparentColor = System.Drawing.Color.White;
            this.toolStripButtonHelp.Name = "toolStripButtonHelp";
            this.toolStripButtonHelp.Size = new System.Drawing.Size(44, 44);
            this.toolStripButtonHelp.Text = "Help";
            this.toolStripButtonHelp.Click += new System.EventHandler(this.toolStripButtonHelp_Click);
            // 
            // toolStripButtonColor
            // 
            this.toolStripButtonColor.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonColor.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonColor.Image")));
            this.toolStripButtonColor.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButtonColor.ImageTransparentColor = System.Drawing.Color.White;
            this.toolStripButtonColor.Name = "toolStripButtonColor";
            this.toolStripButtonColor.Size = new System.Drawing.Size(44, 44);
            this.toolStripButtonColor.Text = "Color Picker";
            this.toolStripButtonColor.Click += new System.EventHandler(this.toolStripButtonColor_Click);
            // 
            // toolStripButtonFont
            // 
            this.toolStripButtonFont.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonFont.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonFont.Image")));
            this.toolStripButtonFont.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButtonFont.ImageTransparentColor = System.Drawing.Color.White;
            this.toolStripButtonFont.Name = "toolStripButtonFont";
            this.toolStripButtonFont.Size = new System.Drawing.Size(44, 44);
            this.toolStripButtonFont.Text = "Font Picker";
            this.toolStripButtonFont.Click += new System.EventHandler(this.toolStripButtonFont_Click);
            // 
            // ToolLauncherControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.toolStrip1);
            this.MaximumSize = new System.Drawing.Size(46, 10000);
            this.MinimumSize = new System.Drawing.Size(46, 46);
            this.Name = "ToolLauncherControl";
            this.Size = new System.Drawing.Size(46, 300);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButtonHelp;
        private System.Windows.Forms.ToolStripButton toolStripButtonColor;
        private System.Windows.Forms.ToolStripButton toolStripButtonFont;
    }
}
