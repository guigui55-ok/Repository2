using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MenuStripSample
{
    public partial class MenuStripSampleForm : Form
    {
        protected ErrorManager.ErrorManager _err;
        public MenuStripSampleForm()
        {
            InitializeComponent();
            _err = new ErrorManager.ErrorManager(1);
            MenuStripManager menuStripManager = new MenuStripManager(_err, this);
            MenuStripSampleEvents menuStripSampleEvents = new MenuStripSampleEvents(_err, menuStripManager);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
