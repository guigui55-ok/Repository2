﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExcelCellsManager.ExcelCellsManager.SettingsForm
{
    public partial class EcmSettingsForm : Form
    {
        public EcmSettingsForm()
        {
            InitializeComponent();
        }

        private void Ecm_SettingsForm_Load(object sender, EventArgs e)
        {

        }

        private void Button_SettingsClose_Click(object sender, EventArgs e)
        {
            this.Visible = false;
        }

        private void Button_SettingsApply_Click(object sender, EventArgs e)
        {
            
        }
    }
}
