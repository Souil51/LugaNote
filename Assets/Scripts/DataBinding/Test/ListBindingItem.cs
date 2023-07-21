using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.DataBinding
{
    /// <summary>
    /// Use on item list templates
    /// Contains references to the item (the DataContext of the item)
    /// And list of all object containing Binding (like the ViewManager)
    /// </summary>
    public class ListBindingItem : MonoBehaviour
    {
        [SerializeField, TypeConstraint(typeof(SimpleBinding))]
        private List<GameObject> BindingsObjects;

        private object _item = null;
        public object Item => _item; // The DataContext of the line

        public List<GameObject> GetBindingsObjects()
        {
            return BindingsObjects;
        }

        public void SetItem(object item)
        {
            _item = item;
        }
    }
}
