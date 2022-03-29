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
    public class DataTreeNodeViewModel : INotifyPropertyChanged
    {
        private DataTreeNode _node;
        private DataTreeNodeViewModel? _parent;
        private ReadOnlyCollection<DataTreeNodeViewModel> _childs;

        private bool _isExpanded;
        private bool _isSelected;
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (value != _isExpanded)
                {
                    _isExpanded = value;
                }
            }
        }
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value != _isSelected)
                {
                    _isSelected = value;
                }
            }
        }
        public string Label { get => _node.Label; }
        public string Value { get => _node.Value; }
        public ReadOnlyCollection<DataTreeNodeViewModel> Childs { get => _childs; }
        public DataTreeNodeViewModel? Parent { get => _parent; }

        public DataTreeNodeViewModel(DataTreeNode node, DataTreeNodeViewModel? parent)
        {
            _node = node;
            _parent = parent;

            _childs = new ReadOnlyCollection<DataTreeNodeViewModel>(
                    (from child in _node.Childs
                     select new DataTreeNodeViewModel(child, this))
                     .ToList());
        }
        public DataTreeNodeViewModel(DataTreeNode node)
            : this(node, null)
        {
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
