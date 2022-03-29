using HEXEH.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEXEH.WPF.ViewModel
{
    internal class DataTreeViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<DataTreeNodeViewModel> _head;
        public ObservableCollection<DataTreeNodeViewModel> Head { get => _head; }

        public DataTreeViewModel(DataTree dt)
        {
            _head = new ObservableCollection<DataTreeNodeViewModel>();
            var dth = new DataTreeNodeViewModel(dt.Head);
            _head.Add(dth);
        }
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
