using System.Collections.ObjectModel;
using System.ComponentModel;
namespace Model;

public class ImgPaths : INotifyPropertyChanged
{
    public ImgPaths()
    {
        _sources = new List<string>();
    }

    private List<string> _sources;

    private ObservableCollection<string> _sourcesCollection;
    
    // 获取和设置图片源列表
    public List<string> Sources
    {
        get => _sources;
        set
        {
            _sources = value;
            OnPropertyChanged(nameof(Sources));
        }
    }

    // 索引器，通过索引获取或设置单个元素
    public string this[int index]
    {
        get => Sources[index];
    }

    // 添加图片源
    public void AddSource(string source)
    {
        Sources.Add(source);
        OnPropertyChanged(nameof(Sources));
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
