using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Xps.Packaging;
using Model;
using Expression = System.Linq.Expressions.Expression;

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
        
        public ImgPaths ImgPaths { get; set; } = new ImgPaths();
        
        public MainWindow()
        {
            InitializeComponent();
            
            WindowSize = WindowState.Normal;
            IsTopWindow = false;
            
            ImgPaths.Sources.Add("AAAAA");
        }
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private async void Window_Activated(object sender, EventArgs e)
        {
            // if window is activated
            WindowsActiveIcon.Text = "✔";

            // --- 测试功能 ---
            //string progID = "VisionLabViewer.Application";
            //Type? type = Type.GetTypeFromProgID(progID);
            //if (type != null)
            //{
            //      // 使用VisionLabViewer该程序，打开xmp文件
            //}

            TempTest tt = new TempTest();

            // 与 ASP.NET Core API项目进行通信

            //var app = Application.Current as App;

            //if (app != null)
            //{
            //    var httpClient = app.HttpClient;

            //    var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:7216/products");
            //    var content = new StringContent("{\r\n    \"IsActive\": true\r\n}", null, "application/json");
            //    request.Content = content;
            //    var response = await httpClient.SendAsync(request);

            //    response.EnsureSuccessStatusCode();
                
            //    Console.WriteLine(await response.Content.ReadAsStringAsync());
            //}


            // --- 测试功能 ---
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            // if window is deactivated
            WindowsActiveIcon.Text = "×";
        }

        private void Window_Closing(object? sender, CancelEventArgs e)
        {
            //this.Closed += (s, args) => {MessageBox.Show("Close Window");};
            
            if (!_closeWindow)
            {
                e.Cancel = true;
            }
        }

        private void Btn_Print(object sender, RoutedEventArgs routedEventArgs)
        {
            string filePath = "";
            bool hideDialog = false;
            if (PrintWholeDocument(filePath, hideDialog))
            {
                MessageBox.Show("print successfully");
            }
        }
        
        /// <summary>
        /// Print all pages of an XPS document.
        /// Optionally, hide the print dialog window.
        /// </summary>
        /// <param name="xpsFilePath">Path to source XPS file</param>
        /// <param name="hidePrintDialog">Whether to hide the print dialog window (shown by default)</param>
        /// <returns>Whether the document printed</returns>
        public static bool PrintWholeDocument(string xpsFilePath, bool hidePrintDialog = false)
        {
            // Create the print dialog object and set options.
            PrintDialog printDialog = new();

            if (!hidePrintDialog)
            {
                // Display the dialog. This returns true if the user presses the Print button.
                bool? isPrinted = printDialog.ShowDialog();
                if (isPrinted != true)
                    return false;
            }

            // Print the whole document.
            try
            {
                // Open the selected document.
                XpsDocument xpsDocument = new(xpsFilePath, FileAccess.Read);

                // Get a fixed document sequence for the selected document.
                FixedDocumentSequence fixedDocSeq = xpsDocument.GetFixedDocumentSequence();

                // Create a paginator for all pages in the selected document.
                DocumentPaginator docPaginator = fixedDocSeq.DocumentPaginator;

                // Print to a new file.
                printDialog.PrintDocument(docPaginator, $"Printing {Path.GetFileName(xpsFilePath)}");

                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);

                return false;
            }
        }
        
        
    }
}