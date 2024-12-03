namespace wfSender
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            button1.Click += UltraSuperUltimateUniversalControlClickHandlerProMaxPlusSlim2024Edition;
            button2.Click += UltraSuperUltimateUniversalControlClickHandlerProMaxPlusSlim2024Edition;
            button3.Click += UltraSuperUltimateUniversalControlClickHandlerProMaxPlusSlim2024Edition;
            checkBox1.Click += UltraSuperUltimateUniversalControlClickHandlerProMaxPlusSlim2024Edition;
            label1.Click += UltraSuperUltimateUniversalControlClickHandlerProMaxPlusSlim2024Edition;
            this.Click += UltraSuperUltimateUniversalControlClickHandlerProMaxPlusSlim2024Edition;
        }

        private void UltraSuperUltimateUniversalControlClickHandlerProMaxPlusSlim2024Edition(object? sender, EventArgs e)
        {
            if (sender is Control c)
                MessageBox.Show(c.Text);
        }
    }
}
