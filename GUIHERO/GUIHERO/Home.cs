using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
 

namespace GUIHERO
{
    public partial class GUI : Form
    {
        public GUI()
        {
            InitializeComponent();
        }

       
        private void Home_Load(object sender, EventArgs e)
        {
        }


        //movement 
        private void btnFront_Click(object sender, EventArgs e)
        {

        }

        private void btnLeft_Click(object sender, EventArgs e)
        {

        }

        private void btnRight_Click(object sender, EventArgs e)
        {

        }

        private void btnBack_Click(object sender, EventArgs e)
        {

        }



        //Kameras 
 
        private void backCAM_Click(object sender, EventArgs e)
        {

        }

        private void frontCAM_Click(object sender, EventArgs e)
        {

        }

        // Design mit LOGO und seiten panel

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {

        }
        private void pctLogo_Click(object sender, EventArgs e)
        {

        }


        //Buttons zum switchen zwieschen den panels 
        private void btnKommunikation_Click(object sender, EventArgs e)
        {
            this.Hide();
            Kommunikation formK = new Kommunikation();
            formK.Show();
        }

        private void btnGPS_Click(object sender, EventArgs e)
        {
            this.Hide();
            GPS formG = new GPS();
            formG.Show();
        }

        private void btnManuell_Click(object sender, EventArgs e)
        {
            this.Hide();
            Manuelle_Funktionen formMF = new Manuelle_Funktionen();
            formMF.Show();
        }

        private void btnDebugg_Click(object sender, EventArgs e)
        {
            this.Hide();
            Debuggen formD = new Debuggen();
            formD.Show();
        }

        private void btnHome_Click(object sender, EventArgs e)
        {

        }
    }
}
