using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DataBinding.Core.Lists
{
    public interface IListBindingTemplateSelector
    {
        List<GameObject> Templates { get; }

        void InitSelector();
        GameObject GetTemplate(object param);
    }
}
