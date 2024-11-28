using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class LogInfos : INotifyPropertyChanged
    {
        public List<LogInfo> Items { get; set; }

        public object this[int index]
        {
            get => Items[index];
            set
            {
                Items[index] = (LogInfo)value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Items)));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }

    public class LogInfo
    {
        [Required]
        private int _id;

        [Required]
        private string _info;

        public int Id
        {
            get => _id;
            set
            {
                _id = value;
            }
        }

        public string Info
        {
            get => _info;
            set
            {
                _info = value;
            }
        }

        public LogInfo(int id, string info)
        {
            Id = id;
            Info = info;
        }
    }
}
