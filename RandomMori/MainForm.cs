using System;
using System.Windows.Forms;

namespace RandomMori
{
    public partial class MainForm : Form
    {
        private MoriDriver core = MoriDriver.getInstance();

        public MainForm()
        {
            InitializeComponent();
            comboBox2.SelectedIndex = 0;
            comboBox1.SelectedIndex = 7;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(textBox6.Text) < 10)
            {
                textBox6.Text = "270";
                MessageBox.Show("必须大于10棵树！");
                return;
            }
            Base.CONSTA.init(
                Convert.ToInt32(textBox1.Text),
                Convert.ToInt32(textBox2.Text),
                Convert.ToInt32(textBox3.Text),
                Convert.ToInt32(textBox4.Text),
                Convert.ToInt32(textBox5.Text),
                Convert.ToInt32(textBox6.Text),
                Convert.ToInt32(comboBox1.Items[comboBox1.SelectedIndex]),
                Convert.ToDouble(textBox7.Text));
            Base.CONSTA.dt = (Base.DashType)(comboBox2.SelectedIndex);
            core.DashInPara();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(textBox6.Text) < 10)
            {
                textBox6.Text = "270";
                MessageBox.Show("必须大于10棵树！");
                return;
            }
            Base.CONSTA.init(
                Convert.ToInt32(textBox1.Text),
                Convert.ToInt32(textBox2.Text),
                Convert.ToInt32(textBox3.Text),
                Convert.ToInt32(textBox4.Text),
                Convert.ToInt32(textBox5.Text),
                Convert.ToInt32(textBox6.Text),
                Convert.ToInt32(comboBox1.Items[comboBox1.SelectedIndex]),
                Convert.ToDouble(textBox7.Text));
            Base.CONSTA.dt = (Base.DashType)(comboBox2.SelectedIndex);
            core.DashInSerial();
        }
    }
}
