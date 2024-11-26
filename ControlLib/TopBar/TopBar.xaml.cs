using System.Windows;
using System.Windows.Controls;

namespace ControlLib.TopBar;

public partial class TopBar : UserControl
{
    public TopBar()
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
}