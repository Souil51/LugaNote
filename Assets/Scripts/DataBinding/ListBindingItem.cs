using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.DataBinding
{
    public class ListBindingItem : MonoBehaviour
    {
        [SerializeField, TypeConstraint(typeof(SimpleBinding))]
        private List<GameObject> BindingsObjects;

        private object _item = null;
        public object Item => _item;

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
