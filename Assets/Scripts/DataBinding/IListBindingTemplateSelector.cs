using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.DataBinding
{
    public interface IListBindingTemplateSelector
    {
        List<GameObject> Templates { get; }

        void InitSelector();
        GameObject GetTemplate(object param);
    }
}
