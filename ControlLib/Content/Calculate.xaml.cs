using System.Windows;
using System.Windows.Controls;
using Model;

namespace ControlLib.Content
{
    /// <summary>
    /// Calculate.xaml 的交互逻辑
    /// </summary>
    public partial class Calculate : UserControl
    {
        internal IDictionary<int, Player> Players { get; set; } = new Dictionary<int, Player>();

        internal Player player { get; set; }

        public Calculate()
        {
            InitializeComponent();

            player = new Player(0, "cc");

            player.Contents[0] = new string("mm");

            this.DataContext = player;
        }

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            this.tb_1.Text = "enter";

            player.Name = "dd";

            player[0] = new string("nn");
        }


        public void Add_Player()
        {
            Player player = new Player(0, "aa");

            Players.Values.Add(player);
        }
    }
}
