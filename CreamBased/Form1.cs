using Cream;
using System;
using System.Text;


namespace CreamBased
{
    public partial class Form1 : Form
    {
        TextWriter _writer = null;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string code = textBox1.Text;
            var compiler = new CreamCompiler(code);
            compiler.Run();
            Console.SetOut(new ControlWriter(textBox2));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox2.Text = "";
        }
    }

    public class ControlWriter : TextWriter
    {
        private Control textbox;
        public ControlWriter(Control textbox)
        {
            this.textbox = textbox;
        }

        public override void Write(char value)
        {
            textbox.Text += value;
        }

        public override void Write(string value)
        {
            textbox.Text += value;
        }

        public override Encoding Encoding
        {
            get { return Encoding.ASCII; }
        }
    }
}