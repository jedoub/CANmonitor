using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kv_DeviceDemo
{
    public partial class OSSForm : Form
    {
        public OSSForm()
        {
            InitializeComponent();

            richTextBox1.ScrollBars = RichTextBoxScrollBars.Vertical;
            richTextBox1.Font = new Font("Lucida Console", 8);
        }

        private void OKbtn_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void OSSForm_Load(object sender, EventArgs e)
        {
            richTextBox1.Text = "Copyright(c) 2011 - 2017 GitHub Inc.";
            richTextBox1.Text += Environment.NewLine;
            richTextBox1.Text += Environment.NewLine;
            richTextBox1.Text += "Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files(the \"Software\")," +
                " to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/ or sell copies of the Software," +
                " and to permit persons to whom the Software is furnished to do so, subject to the following conditions:";
            richTextBox1.Text += Environment.NewLine;
            richTextBox1.Text += Environment.NewLine;
            richTextBox1.Text += "The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.";
            richTextBox1.Text += Environment.NewLine;
            richTextBox1.Text += Environment.NewLine;
            richTextBox1.Text += "THE SOFTWARE IS PROVIDED \"AS IS\", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY," +
                " FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM," +
                " DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.";            
        }
    }
}
