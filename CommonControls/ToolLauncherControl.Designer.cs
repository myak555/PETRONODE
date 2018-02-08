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
            this.toolStripDigiPaint = new System.Windows.Forms.ToolStripButton();
            this.toolStripDigitizer = new System.Windows.Forms.ToolStripButton();
            this.toolStripDigiFitter = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonHelp,
            this.toolStripButtonColor,
            this.toolStripButtonFont,
            this.toolStripDigiPaint,
            this.toolStripDigitizer,
            this.toolStripDigiFitter});
            this.toolStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.VerticalStackWithOverflow;
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(61, 369);
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
            this.toolStripButtonHelp.Size = new System.Drawing.Size(59, 44);
            this.toolStripButtonHelp.Text = "Help";
            this.toolStripButtonHelp.ToolTipText = "About - Help";
            this.toolStripButtonHelp.Click += new System.EventHandler(this.toolStripButtonHelp_Click);
            // 
            // toolStripButtonColor
            // 
            this.toolStripButtonColor.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonColor.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonColor.Image")));
            this.toolStripButtonColor.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButtonColor.ImageTransparentColor = System.Drawing.Color.White;
            this.toolStripButtonColor.Name = "toolStripButtonColor";
            this.toolStripButtonColor.Size = new System.Drawing.Size(59, 44);
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
            this.toolStripButtonFont.Size = new System.Drawing.Size(59, 44);
            this.toolStripButtonFont.Text = "Font Picker";
            this.toolStripButtonFont.Click += new System.EventHandler(this.toolStripButtonFont_Click);
            // 
            // toolStripDigiPaint
            // 
            this.toolStripDigiPaint.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripDigiPaint.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDigiPaint.Image")));
            this.toolStripDigiPaint.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripDigiPaint.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.toolStripDigiPaint.Name = "toolStripDigiPaint";
            this.toolStripDigiPaint.Size = new System.Drawing.Size(59, 44);
            this.toolStripDigiPaint.Text = "toolStripButton1";
            this.toolStripDigiPaint.ToolTipText = "DigiPaint";
            this.toolStripDigiPaint.Click += new System.EventHandler(this.toolStripButtonDigiPaint_Click);
            // 
            // toolStripDigitizer
            // 
            this.toolStripDigitizer.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripDigitizer.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDigitizer.Image")));
            this.toolStripDigitizer.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripDigitizer.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.toolStripDigitizer.Name = "toolStripDigitizer";
            this.toolStripDigitizer.Size = new System.Drawing.Size(59, 44);
            this.toolStripDigitizer.Text = "toolStripButton1";
            this.toolStripDigitizer.ToolTipText = "Digitizer";
            this.toolStripDigitizer.Click += new System.EventHandler(this.toolStripButtonDigitizer_Click);
            // 
            // toolStripDigiFitter
            // 
            this.toolStripDigiFitter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripDigiFitter.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDigiFitter.Image")));
            this.toolStripDigiFitter.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripDigiFitter.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDigiFitter.Name = "toolStripDigiFitter";
            this.toolStripDigiFitter.Size = new System.Drawing.Size(59, 44);
            this.toolStripDigiFitter.Text = "toolStripButton1";
            this.toolStripDigiFitter.ToolTipText = "DigiFitter";
            this.toolStripDigiFitter.Click += new System.EventHandler(this.toolStripDigiFitter_Click);
            // 
            // ToolLauncherControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.toolStrip1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximumSize = new System.Drawing.Size(61, 12308);
            this.MinimumSize = new System.Drawing.Size(61, 57);
            this.Name = "ToolLauncherControl";
            this.Size = new System.Drawing.Size(61, 369);
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
        private System.Windows.Forms.ToolStripButton toolStripDigiPaint;
        private System.Windows.Forms.ToolStripButton toolStripDigitizer;
        private System.Windows.Forms.ToolStripButton toolStripDigiFitter;
    }
}
