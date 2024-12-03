namespace wfaSearchCity
{
    public partial class Form1 : Form
    {
        private readonly string[] cities;

        public Form1()
        {
            InitializeComponent();

            cities = Properties.Resources.goroda.Split('\n');

            textBox1.TextChanged += TextBox1_TextChanged;
        }

        private void TextBox1_TextChanged(object? sender, EventArgs e)
        {
            var r = cities.Where(i => i.ToUpper().Contains(textBox1.Text.ToUpper()))
                .Where(i => ! i.Contains("Список городов России в формате *.ече")
                .ToArray();
            textBox2.Text = string.Join(Environment.NewLine, r);
            this.Text = $"(textBox1.Text)";
        }
    }
}
