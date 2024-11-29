using System.Configuration;
using System.Data;
using System.Net.Http;
using System.Windows;

namespace IPy
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        // private void Application_Startup(object sender, StartupEventArgs e)
        // {
        //     // Create the window
        //     MainWindow window = new MainWindow();
        //
        //     // Open the window
        //     window.ShowDialog();
        // }
        
        /// <summary>
        /// Application is active or not 
        /// </summary>
        public bool IsApplicationActive;

        void App_Activated(object sender, EventArgs e)
        {
            // Application activated
            this.IsApplicationActive = true;
        }

        void App_Deactivated(object sender, EventArgs e)
        {
            // Application deactivated
            this.IsApplicationActive = false;
        }

        private readonly HttpClient _httpClient;

        public HttpClient HttpClient { get => _httpClient; }

        public App()
        {
            _httpClient = new HttpClient();
        }
    }

}
