namespace GUIHERO
{
    partial class Debuggen
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Debuggen));
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.HomeButton = new System.Windows.Forms.Button();
            this.GPSButton = new System.Windows.Forms.Button();
            this.ManuellButton = new System.Windows.Forms.Button();
            this.KommunikationButton = new System.Windows.Forms.Button();
            this.DebuggButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.SlateGray;
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Controls.Add(this.HomeButton);
            this.panel1.Controls.Add(this.GPSButton);
            this.panel1.Controls.Add(this.ManuellButton);
            this.panel1.Controls.Add(this.KommunikationButton);
            this.panel1.Controls.Add(this.DebuggButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(1104, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(160, 681);
            this.panel1.TabIndex = 13;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(160, 160);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 5;
            this.pictureBox1.TabStop = false;
            // 
            // HomeButton
            // 
            this.HomeButton.BackColor = System.Drawing.Color.LightSlateGray;
            this.HomeButton.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.HomeButton.Location = new System.Drawing.Point(0, 181);
            this.HomeButton.Name = "HomeButton";
            this.HomeButton.Size = new System.Drawing.Size(160, 100);
            this.HomeButton.TabIndex = 4;
            this.HomeButton.Text = "Home";
            this.HomeButton.UseVisualStyleBackColor = false;
            // 
            // GPSButton
            // 
            this.GPSButton.BackColor = System.Drawing.Color.LightSlateGray;
            this.GPSButton.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.GPSButton.Location = new System.Drawing.Point(0, 281);
            this.GPSButton.Name = "GPSButton";
            this.GPSButton.Size = new System.Drawing.Size(160, 100);
            this.GPSButton.TabIndex = 3;
            this.GPSButton.Text = "GPS";
            this.GPSButton.UseVisualStyleBackColor = false;
            // 
            // ManuellButton
            // 
            this.ManuellButton.BackColor = System.Drawing.Color.LightSlateGray;
            this.ManuellButton.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ManuellButton.Location = new System.Drawing.Point(0, 381);
            this.ManuellButton.Name = "ManuellButton";
            this.ManuellButton.Size = new System.Drawing.Size(160, 100);
            this.ManuellButton.TabIndex = 2;
            this.ManuellButton.Text = "Manuell";
            this.ManuellButton.UseVisualStyleBackColor = false;
            // 
            // KommunikationButton
            // 
            this.KommunikationButton.BackColor = System.Drawing.Color.LightSlateGray;
            this.KommunikationButton.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.KommunikationButton.Location = new System.Drawing.Point(0, 481);
            this.KommunikationButton.Name = "KommunikationButton";
            this.KommunikationButton.Size = new System.Drawing.Size(160, 100);
            this.KommunikationButton.TabIndex = 1;
            this.KommunikationButton.Text = "Kommunikation";
            this.KommunikationButton.UseVisualStyleBackColor = false;
            // 
            // DebuggButton
            // 
            this.DebuggButton.BackColor = System.Drawing.Color.LightSlateGray;
            this.DebuggButton.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.DebuggButton.Location = new System.Drawing.Point(0, 581);
            this.DebuggButton.Name = "DebuggButton";
            this.DebuggButton.Size = new System.Drawing.Size(160, 100);
            this.DebuggButton.TabIndex = 0;
            this.DebuggButton.Text = "Debuggen";
            this.DebuggButton.UseVisualStyleBackColor = false;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.White;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 48F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(337, 301);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(440, 150);
            this.label1.TabIndex = 14;
            this.label1.Text = "Debugging\r\n";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Debuggen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(51)))), ((int)(((byte)(73)))));
            this.ClientSize = new System.Drawing.Size(1264, 681);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.Name = "Debuggen";
            this.Text = "Debuggen";
            this.Load += new System.EventHandler(this.Debuggen_Load);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button HomeButton;
        private System.Windows.Forms.Button GPSButton;
        private System.Windows.Forms.Button ManuellButton;
        private System.Windows.Forms.Button KommunikationButton;
        private System.Windows.Forms.Button DebuggButton;
        private System.Windows.Forms.Label label1;
    }
}