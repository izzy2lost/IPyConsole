using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ControlLib.Utils
{
    internal class Player: INotifyPropertyChanged
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
                    OnPropertyChanged(nameof(Contents));
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
                OnPropertyChanged(nameof(Contents));
            }
        }

        public Player() { }

        public Player(int id, string name)
        {
            _id = id;
            Name = name;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged(string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }
}
