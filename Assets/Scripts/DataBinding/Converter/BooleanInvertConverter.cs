using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.DataBinding.Converter
{
    public class BooleanInvertConverter : MonoBehaviour, IBindingConverter
    {
        public object GetConvertedValue(object value, object param)
        {
            if(value is bool convertedValue)
            {
                return !convertedValue;
            }

            return value;
        }
    }
}
