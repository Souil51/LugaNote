using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditorInternal.VersionControl;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

namespace Assets.Scripts.DataBinding
{
    public class ListBinding : MonoBehaviour
    {
        // Differents Path for ViewModel properties
        public List<BindingPath> Paths;
        // Item to clone
        public GameObject UIItem;

        private List<GameObject> _uiItems = new List<GameObject>();
        private Type _listType;
        private IList _list = null;

        private void InitListInfo(object value)
        {
            if (IsIList(value)) // Is the value a IList ?
            {
                _listType = GetListType(value); // What is the type ?
                _list = (IList)value;
            }
        }

        // If the list itself change, update all items
        public void ChangeValue(object value)
        {
            if (_list == null)
                InitListInfo(value);

            if (_list != null)
            {
                // here create the right amount of UI Items
                if(_uiItems.Count == 0) // first loading, no items so we create all
                {
                    int idx = 0;
                    foreach (var listItem in _list)
                    {
                        if (idx == 0 && UIItem.activeSelf) // If the Item to clone is active, use it for the first list object
                        {
                            _uiItems.Add(UIItem);
                        }
                        else // or clone the item
                        {
                            GameObject newItem = GameObject.Instantiate(UIItem);
                            newItem.transform.SetParent(this.transform);

                            _uiItems.Add(newItem);
                        }

                        idx++;
                    }
                }
                else if (_list.Count > _uiItems.Count) // else reuse ui items and add some
                {
                    int numberToAdd = _list.Count - _uiItems.Count;
                    for (int i = 0; i < numberToAdd; i++)
                    {
                        GameObject newItem = GameObject.Instantiate(UIItem);
                        newItem.transform.SetParent(this.transform);

                        _uiItems.Add(newItem);
                    }
                } 
                else if (_list.Count > _uiItems.Count) // or reuse° items and destroy some
                {
                    int numberToRemove = _uiItems.Count - _list.Count;
                    for (int i = 0; i < numberToRemove; i++)
                    {
                        Destroy(_uiItems[_uiItems.Count - 1]);
                        _uiItems.RemoveAt(_uiItems.Count - 1);
                    }
                }

                UpdateItems();
            }
        }

        private void UpdateItems()
        {
            for (int i = 0; i < _uiItems.Count; i++)
            {
                var uiItem = _uiItems[i];
                UpdateItem(uiItem, _list[i]);
            }
        }

        private void UpdateItem(GameObject item, object obj, string propertyName = null)
        {
            // Cast the item to the real type
            var typedItem = Convert.ChangeType(obj, _listType);

            if(typedItem is INotifyPropertyChanged npc)
            {
                npc.PropertyChanged += ListItem_PropertyChanged;
            }

            ListBindingItem listBindingItem = item.GetComponent<ListBindingItem>();
            if (listBindingItem != null) // If the  UIItem has a ListBindingItem (it's possible to have a ListBinding juste showing same element, with no bindings)
            {
                listBindingItem.SetItem(typedItem);
                List<SimpleBinding> itemBindings = new List<SimpleBinding>();
                listBindingItem.GetBindingsObjects().ForEach(x => itemBindings.AddRange(x.GetComponents<SimpleBinding>()));

                foreach (var binding in itemBindings) // For each SimpleBinding of the ListBindingItem
                {
                    foreach(var path in binding.Paths)
                    {
                        if (!String.IsNullOrEmpty(propertyName) && path.Name != propertyName) continue;

                        var property = _listType.GetProperties().FirstOrDefault(x => x.Name == path.Name);
                        if (property != null) // Get the property of the path for the type
                        {
                            if (property.IsStatic()) continue;

                            if (typeof(INotifyPropertyChanged).IsAssignableFrom(property.PropertyType)) // if the property implements INotifyPropertyChange
                            {
                                INotifyPropertyChanged currentValue = (INotifyPropertyChanged)_listType.GetProperty(property.Name).GetValue(typedItem, null);// Get the value of the right index
                                binding.ChangeValue(currentValue, path.Name); // Use the simple binding functionnalities
                            }
                            else if (property.PropertyType.IsPrimitive || property.PropertyType == typeof(string))// Primitive or string -> get the direct value
                            {
                                object currentValue = _listType.GetProperty(property.Name).GetValue(typedItem, null); // Get the value of the right index
                                binding.ChangeValue(currentValue, path.Name);// Use the simple binding functionnalities
                            }
                        }
                    }
                }
            }
        }

        private void ListItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Find the UI Item bound to the object that changed
            var typedItem = Convert.ChangeType(sender, _listType);
            var uiItem = _uiItems.FirstOrDefault(x => x.GetComponent<ListBindingItem>()?.Item == typedItem);
            if(uiItem != null)
            {
                // Update all the item, see to update only the good property
                UpdateItem(uiItem, typedItem, e.PropertyName);
            }
        }

        public static bool IsIList(object value)
        {
            Type genericType = typeof(IList<>);

            // Check if the value implements IList<T> for any T
            Type implementedType = value.GetType()
                .GetInterfaces()
                .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == genericType);

            if(implementedType != null)
            {
                var test = implementedType.GetGenericArguments()[0];
            }
            return implementedType != null;
        }

        public static Type GetListType(object value)
        {
            Type genericType = typeof(IList<>);

            // Check if the value implements IList<T> for any T
            Type implementedType = value.GetType()
                .GetInterfaces()
                .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == genericType);

            if (implementedType != null)
            {
                return implementedType.GetGenericArguments()[0];
            }
            return null;
        }
    }
}
