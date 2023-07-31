using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.DataBinding
{
    /// <summary>
    /// Handles a list DataBinding
    /// Creating UIElement as needed and handle there updates
    /// </summary>
    public class ListBinding : MonoBehaviour
    {
        // Differents Path for ViewModel properties
        public List<BindingPath> Paths;
        // Item to clone
        public GameObject UIItem; // The base template if no TemplateSelector
        public MonoBehaviour TemplateSelector;
        public GameObject EmptyTemplate;

        private List<ListItemViewModel> _uiItems = new List<ListItemViewModel>();
        private Type _listType; // The unerlying type of the list (the type T of ILIst<T>)
        private IList _list = null; // The list itself
        private IListBindingTemplateSelector _templateSelector;

        private void InitListInfo(object value)
        {
            if (UIItem.activeSelf) UIItem.SetActive(false);

            // Init the template selector and hide template items
            if (TemplateSelector != null && !(TemplateSelector is IListBindingTemplateSelector))
            {
                throw new Exception("Template selector does not implements IListBindingTemplateSelector");
            }

            if (TemplateSelector != null)
            {
                _templateSelector = (IListBindingTemplateSelector)TemplateSelector;
                _templateSelector.InitSelector();
            }


            // First we hide all child, childs are normaly templates so we won't display them but clone them
            foreach (Transform child in gameObject.transform)
            {
                child.gameObject.SetActive(false);
            }

            if(EmptyTemplate != null)
                EmptyTemplate.SetActive(false);

            Type senderType = value.GetType();
            string[] propertySplit = Paths.First().Name.Split('.');// Always search for the last part of the path for the property
            PropertyInfo prop = value.GetType().GetProperty(propertySplit[propertySplit.Length - 1]);
            if (prop != null)
            {
                object typedValue = prop.GetValue(value);

                if (ListBinding.IsIList(typedValue)) // Is the value a IList ?
                {
                    _listType = ListBinding.GetListType(typedValue); // What is the type ?
                    _list = (IList)typedValue;
                }

                // If it is an observable collection, we can track changed event to update the UI automaticaly on add and remove
                if (ListBinding.IsObservableCollection(typedValue))
                {
                    if (typedValue is INotifyCollectionChanged notifyCollection)
                    {
                        notifyCollection.CollectionChanged += ObsCol_CollectionChanged;
                    }
                }
            }
        }

        // If the list itself change, update all items
        public void ChangeValue(object value)
        {
            if (_list == null)
                InitListInfo(value);

            string[] propertySplit = Paths.First().Name.Split('.');
            PropertyInfo prop = value.GetType().GetProperty(propertySplit[propertySplit.Length - 1]);
            if (prop != null)
            {
                object typedValue = prop.GetValue(value);

                if (ListBinding.IsIList(typedValue)) // Is the value a IList ?
                {
                    _list = (IList)typedValue;
                }
            }

            if (_list != null)
            {
                // here create the right amount of UI Items
                if (_uiItems.Count == 0) // first loading, no items so we create all
                {
                    for(int i = 0; i < _list.Count; i++)
                    {
                        // Get the template
                        var template = TemplateSelector != null ? _templateSelector.GetTemplate(i) : UIItem;

                        GameObject newItem = GameObject.Instantiate(template); // Clone the template
                        newItem.transform.SetParent(this.transform, false);
                        // newItem.transform.localScale = template.transform.localScale;
                        newItem.SetActive(true);

                        var viewModel = newItem.GetComponent<ListItemViewModel>();
                        if (viewModel == null)
                            throw new Exception("UI Item doesn't have ListItemViewModel component");

                        var viewManager = newItem.GetComponent<ViewManager>();
                        
                        _uiItems.Add(viewModel);
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
                        newItem.transform.SetParent(this.transform, false);
                        newItem.SetActive(true);

                        _uiItems.Add(newItem.GetComponent<ListItemViewModel>());
                    }
                }
                else if (_list.Count < _uiItems.Count) // or reuse° items and destroy some
                {
                    int numberToRemove = _uiItems.Count - _list.Count;
                    for (int i = 0; i < numberToRemove; i++)
                    {
                        Destroy(_uiItems[_uiItems.Count - 1].gameObject);
                        _uiItems.RemoveAt(_uiItems.Count - 1);
                    }
                }

                // We have the right count of UI Element cloned, we can assign items
                for(int i = 0; i < _uiItems.Count; i++) 
                {
                    var viewModel = _uiItems[i].GetComponent<ListItemViewModel>();
                    if (viewModel == null)
                        throw new Exception("UI Item doesn't have ListItemViewModel component");

                    var viewManager = _uiItems[i].GetComponent<ViewManager>();
                    // We need to : set the item, init de DataContext of the ViewManager and init the viewModel
                    // Else, the ViewManager Awake function is called after the ViewModel first Binding
                    viewModel.SetItem(_list[i]);
                    viewManager.InitialiserDataContext();
                    viewModel.InitialiserNotifyPropertyChanged(_list[i]);
                }

                if(EmptyTemplate != null)
                    EmptyTemplate.SetActive(_uiItems.Count == 0);
            }
        }


        // ObservableCollection delegate
        private void ObsCol_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                int newItemIndex = -1;
                foreach (var item in e.NewItems)
                {
                    // Get the template
                    var template = TemplateSelector != null ? _templateSelector.GetTemplate(e.NewStartingIndex) : UIItem;

                    GameObject newItem = GameObject.Instantiate(template); // Clone the template
                    newItem.transform.SetParent(this.transform, false);
                    newItem.SetActive(true);

                    if (newItemIndex < 0)
                        newItemIndex = transform.childCount - _list.Count;// Start from the end index to no count templates

                    newItem.transform.SetSiblingIndex(newItemIndex + e.NewStartingIndex);// We set the item in the right child index

                    var viewModel = newItem.GetComponent<ListItemViewModel>();
                    viewModel.SetItem(_list[e.NewStartingIndex]);

                    _uiItems.Add(viewModel);

                    newItemIndex++;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var item in e.OldItems)
                {
                    var itemToDelete = _uiItems.FirstOrDefault(x => x.Item == e.OldItems[0]);
                    _uiItems.Remove(itemToDelete);
                    Destroy(itemToDelete.gameObject);
                }
            }
        }

        public static bool IsIList(object value)
        {
            if (value == null) return false;

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
            if(obj == null) return false;

            Type objectType = obj.GetType();
            Type observableCollectionType = typeof(ObservableCollection<>);

            return objectType.IsGenericType && objectType.GetGenericTypeDefinition() == observableCollectionType;
        }

    }
}
