using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Model
{
    public class Player: INotifyPropertyChanged
    {
        private readonly int _id;

        private string _name;

        private object[] _contents = new object[10];

        public int Id => _id;

        public string Name
        {
            get { return _name; }
            set 
            { 
                _name = value; 
                OnPropertyChanged(nameof(Name));
            }
        }

        public object[] Contents
        {
            get { return _contents; }
            set
            {
                if (value.Length == 10)
                {
                    _contents = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Contents 索引器
        /// </summary>
        public object this[int index]
        {
            get 
            {
                return Contents[index];
            }
            set 
            {
                Contents[index] = value;
                OnPropertyChanged();
            }
        }

        public Player() { }

        public Player(int id, string name)
        {
            _id = id;
            Name = name;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }
}
