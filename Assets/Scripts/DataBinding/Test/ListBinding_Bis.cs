using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.DataBinding
{
    public class ListBinding_Bis : MonoBehaviour
    {
        // Differents Path for ViewModel properties
        public List<BindingPath> Paths;
        // Item to clone
        public GameObject UIItem; // The base template if no TemplateSelector
        public MonoBehaviour TemplateSelector;

        private List<GameObject> _uiItems = new List<GameObject>();
        private Type _listType; // The unerlying type of the list (the type T of ILIst<T>)
        private IList _list = null; // The list itself
        private IListBindingTemplateSelector _templateSelector;

        private void InitListInfo(object value)
        {
            if (UIItem.activeSelf) UIItem.SetActive(false);

            // Init the template selector and hide template items
            if (!(TemplateSelector is IListBindingTemplateSelector))
            {
                throw new Exception("Template selector does not implements IListBindingTemplateSelector");
            }

            if (TemplateSelector != null)
            {
                _templateSelector = (IListBindingTemplateSelector)TemplateSelector;
                _templateSelector.InitSelector();

                if (TemplateSelector != null)
                {
                    foreach (var template in _templateSelector.Templates)
                    {
                        template.SetActive(false);
                    }
                }
            }

            Type senderType = value.GetType();
            string[] propertySplit = Paths.First().Name.Split('.');
            PropertyInfo prop = value.GetType().GetProperty(propertySplit[propertySplit.Length - 1]);
            if (prop != null)
            {
                object typedValue = prop.GetValue(value);

                if (IsIList(typedValue)) // Is the value a IList ?
                {
                    _listType = GetListType(typedValue); // What is the type ?
                    _list = (IList)typedValue;
                }

                // If it is an observable collection, we can track changed event to update the UI automaticaly
                if (IsObservableCollection(typedValue))
                {
                    // As we don't know (do we ?) the generic type of ObservableCollection
                    // Get the event with reflection
                    EventInfo collectionChangedEvent = typedValue.GetType().GetEvent("CollectionChanged");

                    // Get the delegate in this type
                    MethodInfo handlerMethod = this.GetType().GetMethod("ObsCol_CollectionChanged");

                    // Subscribe to the event
                    // (how to unsubscribe if needed ?)
                    collectionChangedEvent.AddEventHandler(typedValue, Delegate.CreateDelegate(collectionChangedEvent.EventHandlerType, handlerMethod));
                }
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
                if (_uiItems.Count == 0) // first loading, no items so we create all
                {
                    int idx = 0;
                    foreach (var listItem in _list)
                    {
                        // Get the template
                        var template = TemplateSelector != null ? _templateSelector.GetTemplate(idx) : UIItem;

                        GameObject newItem = GameObject.Instantiate(template); // Clone the template
                        newItem.transform.SetParent(this.transform);
                        newItem.SetActive(true);

                        _uiItems.Add(newItem);

                        idx++;
                    }
                }
                else if (_list.Count > _uiItems.Count) // else reuse ui items and add some
                {
                    int numberToAdd = _list.Count - _uiItems.Count;
                    for (int i = 0; i < numberToAdd; i++)
                    {
                        // Get the template
                        var template = TemplateSelector != null ? _templateSelector.GetTemplate(_uiItems.Count) : UIItem;

                        GameObject newItem = GameObject.Instantiate(template); // Clone the template
                        newItem.transform.SetParent(this.transform);
                        newItem.SetActive(true);

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

            if (typedItem is INotifyPropertyChanged npc) // Here we track Item change
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
                    foreach (var path in binding.Paths) // For each path
                    {
                        if (!String.IsNullOrEmpty(propertyName) && path.Name != propertyName) continue;

                        var splitPathName = path.Name.Split('.');
                        if (splitPathName.Length == 1)
                        {
                            ChangeObjectValue(binding, path.Name, typedItem);
                        }
                        else // For inner property, we get first the inner property and get value from this inner property
                        {
                            var property = _listType.GetProperties().FirstOrDefault(x => x.Name == splitPathName[0]);
                            if (property != null && !property.PropertyType.IsPrimitive && property.PropertyType != typeof(string))
                            {
                                var objectFromProperty = property.PropertyType.GetProperty(splitPathName[0]).GetValue(typedItem, null);

                                ChangeObjectValue(binding, path.Name, objectFromProperty);
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
            if (uiItem != null)
            {
                // Update all the item, see to update only the good property
                UpdateItem(uiItem, typedItem, e.PropertyName);
            }
        }

        /// <summary>
        /// Copy the value of an object  to the right property of an UI Element
        /// </summary>
        /// <param name="binding">The simple binding which contains the path</param>
        /// <param name="fullPathName">The full name of the property to get (if inner property it contains the full path with the dot, else it equals innerPathName)</param>
        /// <param name="sourceObject">The object from which we want to get the value</param>
        private void ChangeObjectValue(SimpleBinding binding, string fullPathName, object sourceObject)
        {
            if (sourceObject == null)
            {
                binding.ChangeValue(null, fullPathName);
            }
            else
            {
                // The property from which the value come from
                string innerPathName = fullPathName.Split('.').Last();

                // Get the property info to copy from
                var innerProperty = sourceObject.GetType().GetProperties().FirstOrDefault(x => x.Name == innerPathName);
                if (innerProperty != null && !innerProperty.IsStatic())
                {
                    if (typeof(INotifyPropertyChanged).IsAssignableFrom(innerProperty.PropertyType)
                        || innerProperty.PropertyType.IsPrimitive
                        || innerProperty.PropertyType == typeof(string))
                    {
                        object currentValue = sourceObject.GetType().GetProperty(innerProperty.Name).GetValue(sourceObject, null); // Get the value of the source object
                        binding.ChangeValue(currentValue, fullPathName);// Use the simple binding functionnalities
                    }
                }
            }
        }

        private void ObsCol_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        public static bool IsIList(object value)
        {
            return IsIList(value.GetType());
        }

        public static bool IsIList(Type type)
        {
            Type genericType = typeof(IList<>);

            // Check if the value implements IList<T> for any T
            Type implementedType = type
                .GetInterfaces()
                .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == genericType);

            if (implementedType != null)
            {
                var test = implementedType.GetGenericArguments()[0];
            }
            return implementedType != null;
        }

        public static Type GetListType(object value)
        {
            return GetListType(value.GetType());
        }

        public static Type GetListType(Type type)
        {
            Type genericType = typeof(IList<>);

            // Check if the value implements IList<T> for any T
            Type implementedType = type
                .GetInterfaces()
                .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == genericType);

            if (implementedType != null)
            {
                return implementedType.GetGenericArguments()[0];
            }
            return null;
        }

        public static bool IsObservableCollection(object obj)
        {
            Type objectType = obj.GetType();
            Type observableCollectionType = typeof(ObservableCollection<>);

            return objectType.IsGenericType && objectType.GetGenericTypeDefinition() == observableCollectionType;
        }
    }
}
