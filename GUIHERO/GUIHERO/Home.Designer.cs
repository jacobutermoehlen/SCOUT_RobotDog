using System;
using System.Windows.Forms;

namespace GUIHERO
{
    partial class GUI
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GUI));
            this.btnFront = new System.Windows.Forms.Button();
            this.btnLeft = new System.Windows.Forms.Button();
            this.btnRight = new System.Windows.Forms.Button();
            this.btnBack = new System.Windows.Forms.Button();
            this.FrontCAM = new System.Windows.Forms.PictureBox();
            this.BackCAM = new System.Windows.Forms.PictureBox();
            this.AKKU = new System.Windows.Forms.ProgressBar();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pctLogo = new System.Windows.Forms.PictureBox();
            this.btnHome = new System.Windows.Forms.Button();
            this.btnGPS = new System.Windows.Forms.Button();
            this.btnManuell = new System.Windows.Forms.Button();
            this.btnKommunikation = new System.Windows.Forms.Button();
            this.btnDebugg = new System.Windows.Forms.Button();
            this.lblFrontCAM = new System.Windows.Forms.Label();
            this.lblBackCAM = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.FrontCAM)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BackCAM)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pctLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // btnFront
            // 
            this.btnFront.BackColor = System.Drawing.Color.LightSlateGray;
            this.btnFront.Location = new System.Drawing.Point(84, 428);
            this.btnFront.Name = "btnFront";
            this.btnFront.Size = new System.Drawing.Size(60, 55);
            this.btnFront.TabIndex = 0;
            this.btnFront.Text = "w";
            this.btnFront.UseVisualStyleBackColor = false;
            this.btnFront.Click += new System.EventHandler(this.btnFront_Click);
            // 
            // btnLeft
            // 
            this.btnLeft.BackColor = System.Drawing.Color.LightSlateGray;
            this.btnLeft.Location = new System.Drawing.Point(18, 480);
            this.btnLeft.Name = "btnLeft";
            this.btnLeft.Size = new System.Drawing.Size(60, 55);
            this.btnLeft.TabIndex = 1;
            this.btnLeft.Text = "a";
            this.btnLeft.UseVisualStyleBackColor = false;
            this.btnLeft.Click += new System.EventHandler(this.btnLeft_Click);
            // 
            // btnRight
            // 
            this.btnRight.BackColor = System.Drawing.Color.LightSlateGray;
            this.btnRight.Location = new System.Drawing.Point(150, 480);
            this.btnRight.Name = "btnRight";
            this.btnRight.Size = new System.Drawing.Size(60, 55);
            this.btnRight.TabIndex = 2;
            this.btnRight.Text = "d";
            this.btnRight.UseVisualStyleBackColor = false;
            this.btnRight.Click += new System.EventHandler(this.btnRight_Click);
            // 
            // btnBack
            // 
            this.btnBack.BackColor = System.Drawing.Color.LightSlateGray;
            this.btnBack.Location = new System.Drawing.Point(84, 537);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(60, 55);
            this.btnBack.TabIndex = 3;
            this.btnBack.Text = "s";
            this.btnBack.UseVisualStyleBackColor = false;
            // 
            // FrontCAM
            // 
            this.FrontCAM.Location = new System.Drawing.Point(18, 12);
            this.FrontCAM.Name = "FrontCAM";
            this.FrontCAM.Size = new System.Drawing.Size(450, 300);
            this.FrontCAM.TabIndex = 5;
            this.FrontCAM.TabStop = false;
            this.FrontCAM.Click += new System.EventHandler(this.pictureBox2_Click);
            // 
            // BackCAM
            // 
            this.BackCAM.Location = new System.Drawing.Point(648, 12);
            this.BackCAM.Name = "BackCAM";
            this.BackCAM.Size = new System.Drawing.Size(450, 300);
            this.BackCAM.TabIndex = 6;
            this.BackCAM.TabStop = false;
            this.BackCAM.Click += new System.EventHandler(this.pictureBox3_Click);
            // 
            // AKKU
            // 
            this.AKKU.BackColor = System.Drawing.Color.White;
            this.AKKU.Location = new System.Drawing.Point(953, 627);
            this.AKKU.Name = "AKKU";
            this.AKKU.Size = new System.Drawing.Size(100, 23);
            this.AKKU.TabIndex = 9;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.SlateGray;
            this.panel1.Controls.Add(this.pctLogo);
            this.panel1.Controls.Add(this.btnHome);
            this.panel1.Controls.Add(this.btnGPS);
            this.panel1.Controls.Add(this.btnManuell);
            this.panel1.Controls.Add(this.btnKommunikation);
            this.panel1.Controls.Add(this.btnDebugg);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(1104, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(160, 681);
            this.panel1.TabIndex = 10;
            // 
            // pctLogo
            // 
            this.pctLogo.Dock = System.Windows.Forms.DockStyle.Top;
            this.pctLogo.Image = ((System.Drawing.Image)(resources.GetObject("pctLogo.Image")));
            this.pctLogo.Location = new System.Drawing.Point(0, 0);
            this.pctLogo.Name = "pctLogo";
            this.pctLogo.Size = new System.Drawing.Size(160, 160);
            this.pctLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pctLogo.TabIndex = 5;
            this.pctLogo.TabStop = false;
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
            this.btnHome.Click += new System.EventHandler(this.btnHome_Click);
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
            this.btnGPS.Click += new System.EventHandler(this.btnGPS_Click);
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
            this.btnManuell.Click += new System.EventHandler(this.btnManuell_Click);
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
            this.btnKommunikation.Click += new System.EventHandler(this.btnKommunikation_Click);
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
            this.btnDebugg.Click += new System.EventHandler(this.btnDebugg_Click);
            // 
            // lblFrontCAM
            // 
            this.lblFrontCAM.BackColor = System.Drawing.Color.LightSteelBlue;
            this.lblFrontCAM.Location = new System.Drawing.Point(172, 315);
            this.lblFrontCAM.Name = "lblFrontCAM";
            this.lblFrontCAM.Size = new System.Drawing.Size(100, 25);
            this.lblFrontCAM.TabIndex = 11;
            this.lblFrontCAM.Text = "FrontCAM";
            this.lblFrontCAM.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblBackCAM
            // 
            this.lblBackCAM.BackColor = System.Drawing.Color.LightSteelBlue;
            this.lblBackCAM.Location = new System.Drawing.Point(848, 315);
            this.lblBackCAM.Name = "lblBackCAM";
            this.lblBackCAM.Size = new System.Drawing.Size(100, 25);
            this.lblBackCAM.TabIndex = 12;
            this.lblBackCAM.Text = "BackCAM";
            this.lblBackCAM.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // GUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(51)))), ((int)(((byte)(73)))));
            this.ClientSize = new System.Drawing.Size(1264, 681);
            this.Controls.Add(this.lblBackCAM);
            this.Controls.Add(this.lblFrontCAM);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.AKKU);
            this.Controls.Add(this.BackCAM);
            this.Controls.Add(this.FrontCAM);
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.btnRight);
            this.Controls.Add(this.btnLeft);
            this.Controls.Add(this.btnFront);
            this.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "GUI";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Home_Load);
            ((System.ComponentModel.ISupportInitialize)(this.FrontCAM)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BackCAM)).EndInit();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pctLogo)).EndInit();
            this.ResumeLayout(false);

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void GUI_Paint(object sender, PaintEventArgs e)
        {
            throw new NotSupportedException();
        }

        #endregion

        private System.Windows.Forms.Button btnFront;
        private System.Windows.Forms.Button btnLeft;
        private System.Windows.Forms.Button btnRight;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.PictureBox FrontCAM;
        private System.Windows.Forms.PictureBox BackCAM;
        private System.Windows.Forms.ProgressBar AKKU;
        private Panel panel1;
        private PictureBox pctLogo;
        private Button btnHome;
        private Button btnGPS;
        private Button btnManuell;
        private Button btnKommunikation;
        private Button btnDebugg;
        private Label lblFrontCAM;
        private Label lblBackCAM;
    }
}

