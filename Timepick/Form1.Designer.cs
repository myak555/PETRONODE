namespace Petronode.Timepick
{
    partial class Form1
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.button1 = new System.Windows.Forms.Button();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.shot_Directory_Comparison1 = new Petronode.SeismicUIComponents.Shot_Directory_Comparison();
            this.surface_Shot_View1 = new Petronode.SeismicUIComponents.Surface_Shot_View();
            this.downhole_Shot_View1 = new Petronode.SeismicUIComponents.Downhole_Shot_View();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Interval = 15000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.shot_Directory_Comparison1);
            this.splitContainer1.Panel1.Controls.Add(this.button1);
            this.splitContainer1.Panel1MinSize = 350;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(1182, 753);
            this.splitContainer1.SplitterDistance = 450;
            this.splitContainer1.TabIndex = 22;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(0, 0);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(3, 3);
            this.button1.TabIndex = 23;
            this.button1.Text = "fake";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            // 
            // splitContainer2
            // 
            this.splitContainer2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.surface_Shot_View1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.downhole_Shot_View1);
            this.splitContainer2.Size = new System.Drawing.Size(728, 753);
            this.splitContainer2.SplitterDistance = 261;
            this.splitContainer2.TabIndex = 0;
            // 
            // shot_Directory_Comparison1
            // 
            this.shot_Directory_Comparison1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.shot_Directory_Comparison1.Location = new System.Drawing.Point(0, 0);
            this.shot_Directory_Comparison1.Name = "shot_Directory_Comparison1";
            this.shot_Directory_Comparison1.Raw_Shot_Directory = "";
            this.shot_Directory_Comparison1.Search_Pattern = "*.s*";
            this.shot_Directory_Comparison1.Selected_Shot_Directory = "";
            this.shot_Directory_Comparison1.Size = new System.Drawing.Size(448, 751);
            this.shot_Directory_Comparison1.TabIndex = 24;
            // 
            // surface_Shot_View1
            // 
            this.surface_Shot_View1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.surface_Shot_View1.Location = new System.Drawing.Point(0, 0);
            this.surface_Shot_View1.Name = "surface_Shot_View1";
            this.surface_Shot_View1.Size = new System.Drawing.Size(726, 259);
            this.surface_Shot_View1.SNR = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.surface_Shot_View1.TabIndex = 0;
            this.surface_Shot_View1.Time_Max = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.surface_Shot_View1.Time_Min = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.surface_Shot_View1.TuningType = -1;
            this.surface_Shot_View1.Vertical_Scale = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.surface_Shot_View1.Window_Size = new decimal(new int[] {
            500,
            0,
            0,
            0});
            // 
            // downhole_Shot_View1
            // 
            this.downhole_Shot_View1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.downhole_Shot_View1.Location = new System.Drawing.Point(0, 0);
            this.downhole_Shot_View1.Name = "downhole_Shot_View1";
            this.downhole_Shot_View1.Pick_Component = -1;
            this.downhole_Shot_View1.Show_Component = -1;
            this.downhole_Shot_View1.Size = new System.Drawing.Size(726, 486);
            this.downhole_Shot_View1.SNR = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.downhole_Shot_View1.TabIndex = 0;
            this.downhole_Shot_View1.Time_Max = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.downhole_Shot_View1.Time_Min = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.downhole_Shot_View1.TuningType = -1;
            this.downhole_Shot_View1.Vertical_Scale = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.downhole_Shot_View1.Window_Size = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.downhole_Shot_View1.Window_Start = new decimal(new int[] {
            500,
            0,
            0,
            0});
            // 
            // Form1
            // 
            this.AcceptButton = this.button1;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1182, 753);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MinimumSize = new System.Drawing.Size(750, 500);
            this.Name = "Form1";
            this.Text = "PETRONODE Timepick";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Button button1;
        private Petronode.SeismicUIComponents.Shot_Directory_Comparison shot_Directory_Comparison1;
        private Petronode.SeismicUIComponents.Surface_Shot_View surface_Shot_View1;
        private Petronode.SeismicUIComponents.Downhole_Shot_View downhole_Shot_View1;
    }
}

