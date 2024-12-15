using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
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
        public new static App Instance => (App)Application.Current;
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

        /// <summary>
        /// 获取存放应用服务的容器
        /// </summary>
        public IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// 配置应用服务
        /// </summary>
        /// <returns></returns>
        public static IServiceProvider ConfigureServices()
        {
            var serviceCollection = new ServiceCollection()
                .AddSingleton<MainWindow>();

            return serviceCollection.BuildServiceProvider();
        }

        public App()
        {
            _httpClient = new HttpClient();
            ServiceProvider = ConfigureServices();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // MainWindow 类通过服务注册的方式进行实例化
            // 删除 App.xaml 中的 StartupUri="MainWindow.xaml"
            var main_Window = ServiceProvider.GetRequiredService<MainWindow>();
            main_Window.Show();
        }
    }

}
