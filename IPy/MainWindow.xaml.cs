using System.ComponentModel;
using Core;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Mono.Unix.Native;

namespace IPy
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // should close window
        private bool _closeWindow = true;
        
        public static readonly DependencyProperty IsTopWindowProperty =
            DependencyProperty.Register(nameof(IsTopWindow), typeof(bool), typeof(MainWindow), new PropertyMetadata(false));
        
        public static readonly DependencyProperty WindowSizeProperty =
            DependencyProperty.Register(nameof(WindowSize), typeof(Enum), typeof(MainWindow), new PropertyMetadata(WindowState.Normal));
        
        // top window
        public bool IsTopWindow
        {
            get { return (bool)GetValue(IsTopWindowProperty); }
            set { SetValue(IsTopWindowProperty, value); }
        }
        
        // window state
        public WindowState WindowSize
        {
            get { return (WindowState)GetValue(WindowSizeProperty); }
            set { SetValue(WindowSizeProperty, value); }
        }
        
        public MainWindow()
        {
            InitializeComponent();
            
            WindowSize = WindowState.Normal;
            IsTopWindow = false;
        }
        
        private void Window_Activated(object sender, EventArgs e)
        {
            // if window is activated
            WindowsActiveIcon.Text = "✔";
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            // if window is deactivated
            WindowsActiveIcon.Text = "×";
        }

        private void Window_Closing(object? sender, CancelEventArgs e)
        {
            this.Closed += (s, args) => {MessageBox.Show("Close Window");};
            
            if (!_closeWindow)
            {
                e.Cancel = true;
            }
        }
    }
}