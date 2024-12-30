namespace GUIHERO
{
    partial class GPS
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GPS));
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnHome = new System.Windows.Forms.Button();
            this.btnGPS = new System.Windows.Forms.Button();
            this.btnManuell = new System.Windows.Forms.Button();
            this.btnKommunikation = new System.Windows.Forms.Button();
            this.btnDebugg = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.SlateGray;
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Controls.Add(this.btnHome);
            this.panel1.Controls.Add(this.btnGPS);
            this.panel1.Controls.Add(this.btnManuell);
            this.panel1.Controls.Add(this.btnKommunikation);
            this.panel1.Controls.Add(this.btnDebugg);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(1104, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(160, 681);
            this.panel1.TabIndex = 12;
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
            // btnHome
            // 
            this.btnHome.BackColor = System.Drawing.Color.LightSlateGray;
            this.btnHome.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnHome.Location = new System.Drawing.Point(0, 181);
            this.btnHome.Name = "btnHome";
            this.btnHome.Size = new System.Drawing.Size(160, 100);
            this.btnHome.TabIndex = 4;
            this.btnHome.Text = "Home";
            this.btnHome.UseVisualStyleBackColor = false;
            // 
            // btnGPS
            // 
            this.btnGPS.BackColor = System.Drawing.Color.LightSlateGray;
            this.btnGPS.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnGPS.Location = new System.Drawing.Point(0, 281);
            this.btnGPS.Name = "btnGPS";
            this.btnGPS.Size = new System.Drawing.Size(160, 100);
            this.btnGPS.TabIndex = 3;
            this.btnGPS.Text = "GPS";
            this.btnGPS.UseVisualStyleBackColor = false;
            // 
            // btnManuell
            // 
            this.btnManuell.BackColor = System.Drawing.Color.LightSlateGray;
            this.btnManuell.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnManuell.Location = new System.Drawing.Point(0, 381);
            this.btnManuell.Name = "btnManuell";
            this.btnManuell.Size = new System.Drawing.Size(160, 100);
            this.btnManuell.TabIndex = 2;
            this.btnManuell.Text = "Manuell";
            this.btnManuell.UseVisualStyleBackColor = false;
            // 
            // btnKommunikation
            // 
            this.btnKommunikation.BackColor = System.Drawing.Color.LightSlateGray;
            this.btnKommunikation.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnKommunikation.Location = new System.Drawing.Point(0, 481);
            this.btnKommunikation.Name = "btnKommunikation";
            this.btnKommunikation.Size = new System.Drawing.Size(160, 100);
            this.btnKommunikation.TabIndex = 1;
            this.btnKommunikation.Text = "Kommunikation";
            this.btnKommunikation.UseVisualStyleBackColor = false;
            // 
            // btnDebugg
            // 
            this.btnDebugg.BackColor = System.Drawing.Color.LightSlateGray;
            this.btnDebugg.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnDebugg.Location = new System.Drawing.Point(0, 581);
            this.btnDebugg.Name = "btnDebugg";
            this.btnDebugg.Size = new System.Drawing.Size(160, 100);
            this.btnDebugg.TabIndex = 0;
            this.btnDebugg.Text = "Debuggen";
            this.btnDebugg.UseVisualStyleBackColor = false;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.White;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 48F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(318, 281);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(420, 150);
            this.label1.TabIndex = 15;
            this.label1.Text = "GPS";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // GPS
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(51)))), ((int)(((byte)(73)))));
            this.ClientSize = new System.Drawing.Size(1264, 681);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "GPS";
            this.Text = "GPS";
            this.Load += new System.EventHandler(this.GPS_Load);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btnHome;
        private System.Windows.Forms.Button btnGPS;
        private System.Windows.Forms.Button btnManuell;
        private System.Windows.Forms.Button btnKommunikation;
        private System.Windows.Forms.Button btnDebugg;
        private System.Windows.Forms.Label label1;

    }
}