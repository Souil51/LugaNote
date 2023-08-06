using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBinding.Converter
{
    public interface IBindingConverter
    {
        object GetConvertedValue(object value, object param);
    }
}
