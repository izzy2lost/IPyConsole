using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace ControlLib.TopBar.TopBtnBar;

public partial class TopBtnBar : UserControl
{
    public TopBtnBar()
    {
        InitializeComponent();
        this.DataContextChanged += TopBar_DataContextChanged;
    }

    private void TopBar_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        // if (e.NewValue is YourDataSourceType dataSource)
        // {
        //     // 假设YourDataSourceType是包含Sources属性的类型，这里根据实际情况修改
        //     // 进行数据绑定或者其他操作，比如更新UI控件的绑定
        //     if (listViewTopBar!= null)
        //     {
        //         listViewTopBar.ItemsSource = dataSource.Sources;
        //     }
        // }
    }

    public delegate bool Predicate<in T>(T obj);

    private void Btn_File_Click(object sender, RoutedEventArgs e)
    {

        Popup_File.IsOpen = !Popup_File.IsOpen;

        
        //e.Handled = false;
    }

    private void Btn_Edit_Click(object sender, RoutedEventArgs e)
    {
        //e.Handled = true;
    }

    private void Btn_View_Click(object sender, RoutedEventArgs e)
    {

    }

    private void Btn_Tool_Click(object sender, RoutedEventArgs e)
    {

    }

    private void Btn_File_OpenFolder_Click(object sender, RoutedEventArgs e)
    {
        string currentDirectory = Directory.GetCurrentDirectory();

        Process.Start("explorer.exe", currentDirectory);

    }

    private void Btn_File_OpenFile_Click(object sender, RoutedEventArgs e)
    {

    }

    private void Btn_File_CloseFile_Click(object sender, RoutedEventArgs e)
    {

    }

    private CustomPopupPlacement[] Popup_PlacementCallback(Size popupSize, Size targetSize, Point offset)
    {
        // 获取窗口的位置
        Point windowPosition = this.PointToScreen(new Point(0, 0));
        // 计算Popup相对于窗口的新位置
        double newX = windowPosition.X + Btn_File.PointToScreen(new Point(0, 0)).X - windowPosition.X;
        double newY = windowPosition.Y + Btn_File.PointToScreen(new Point(0, 0)).Y - windowPosition.Y + Btn_File.ActualHeight;
        // 创建并返回一个包含新位置的CustomPopupPlacement数组
        return new CustomPopupPlacement[] { new CustomPopupPlacement(new Point(newX, newY), PopupPrimaryAxis.None) };
    }
}