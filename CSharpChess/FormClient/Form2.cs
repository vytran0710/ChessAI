using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FormClient
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if(Int32.Parse(textBox1.Text)>0)
                {
                    Form1.depth = Int32.Parse(textBox1.Text);
                    MessageBox.Show("Depth changed to " + Int32.Parse(textBox1.Text));
                }
                else
                {
                    MessageBox.Show("Depth can't be changed");
                }
            }
            catch
            {
                MessageBox.Show("Depth can't be changed");
            }
        }
    }
}
