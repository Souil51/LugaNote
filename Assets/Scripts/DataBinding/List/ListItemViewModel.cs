using DataBinding.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBinding.Core.Lists
{
    public class ListItemViewModel : ViewModelBase
    {
        private object _item;

        public object Item => _item;

        public void SetItem(object item)
        {
            _item = item;
            
            if(_item is INotifyPropertyChanged notifyObject)
            {
                notifyObject.PropertyChanged += NotifyObject_PropertyChanged;
            }

            InitialiserNotifyPropertyChanged(_item);

            // NotifyObject_PropertyChanged(item, new PropertyChangedEventArgs(null));
        }

        private void NotifyObject_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(sender, e.PropertyName);
        }
    }
}
