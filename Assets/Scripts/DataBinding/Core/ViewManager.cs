using Assets;
using DataBinding.Core.Lists;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace DataBinding.Core
{
    /// <summary>
    /// Handle the data binding from a datacontext (IViewModel) to some items (the list of SimpleBinding)
    /// </summary>
    public class ViewManager : MonoBehaviour
    {
        [SerializeField, TypeConstraint(typeof(IViewModel))]
        private GameObject DataContextContainer;

        [SerializeField, TypeConstraint(typeof(SimpleBinding))]
        private List<GameObject> BindingsObjects;

        [SerializeField, TypeConstraint(typeof(ListBinding))]
        private List<GameObject> ListBindingsObjects;

        private List<SimpleBinding> Bindings;
        private List<ListBinding> ListBindings;

        private IViewModel DataContext;

        private void Awake()
        {
            InitialiserDataContext();
        }

        public void DamagePlayer_Click()
        {
            // ((GameController)DataContext).DamagePlayer();
        }

        /// <summary>
        /// Get the DataContext change sevents
        /// </summary>
        public void InitialiserDataContext()
        {
            var allSimpleBindings = GetComponentsInChildren<SimpleBinding>(true).Select(x => x.gameObject).Distinct().ToList();
            var allListBindings = GetComponentsInChildren<ListBinding>(true).Select(x => x.gameObject).Distinct().ToList();

            Bindings = new List<SimpleBinding>();
            ListBindings = new List<ListBinding>();

            allSimpleBindings.ForEach(x => Bindings.AddRange(x.GetComponents<SimpleBinding>()));
            allListBindings.ForEach(x => ListBindings.AddRange(x.GetComponents<ListBinding>()));

            DataContext = DataContextContainer.GetComponent<IViewModel>();

            DataContext.PropertyChanged += BoundProperty_PropertyChanged;
            DataContext.GameObjectCreated += DataContext_GameObjectCreated;
            DataContext.GameObjectDestroyed += DataContext_GameObjectDestroyed; ;

            /*foreach(var binding in Bindings)
            {
                foreach(var path in binding.Paths)
                {
                    if(DataContext is ListItemViewModel listitem)
                        BoundProperty_PropertyChanged(listitem.Item, new PropertyChangedEventArgs(path.Name));
                    else
                        BoundProperty_PropertyChanged(DataContext, new PropertyChangedEventArgs(path.Name));
                }
            }*/
        }

        private void DataContext_GameObjectCreated(object sender, GameObjectEventArgs e)
        {
            var canvasElement = e.GameObject.GetComponent<ICanvasElement>();
            var customElement = e.GameObject.GetComponent<BasicUICustomElement>();
            var instantiableElement = e.GameObject.GetComponent<IInstantiableUIElement>();

            if (canvasElement != null || customElement != null || instantiableElement != null)
            {
                e.GameObject.transform.SetParent(gameObject.transform);

                var simpleBinding = e.GameObject.GetComponent<SimpleBinding>();
                if (simpleBinding != null)
                {
                    Bindings.Add(simpleBinding);
                    if (simpleBinding.Paths != null && simpleBinding.Paths.Count > 0)
                        BoundProperty_PropertyChanged(sender, new PropertyChangedEventArgs(simpleBinding.Paths.First().Name));
                }
            }
        }

        private void DataContext_GameObjectDestroyed(object sender, GameObjectEventArgs e)
        {
            var canvasElement = e.GameObject.GetComponent<ICanvasElement>();
            var customElement = e.GameObject.GetComponent<BasicUICustomElement>();
            var instantiableElement = e.GameObject.GetComponent<IInstantiableUIElement>();

            if (canvasElement != null || customElement != null || instantiableElement != null)
            {
                var simpleBinding = e.GameObject.GetComponent<SimpleBinding>();
                if (simpleBinding != null)
                {
                    Bindings.Remove(simpleBinding);
                }
            }
        }

        // Get the property and update it
        private void BoundProperty_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            FindBindingAndChangeProperty(sender, e.PropertyName);
        }

        // Search for all UI element bound to the pchanged property
        private void FindBindingAndChangeProperty(object sender, string propertyName)
        {
            List<SimpleBinding> foundBindings = Bindings.FindAll(x => x.Paths.Find(y => y.Name == propertyName) != null);

            foreach (SimpleBinding sb in foundBindings)
            {
                ChangeProperty(sb, sender, propertyName);
            }

            List<ListBinding> foundListBindings = ListBindings.FindAll(x => x.Paths.Find(y => y.Name == propertyName) != null);

            foreach (ListBinding lb in foundListBindings)
            {
                ChangeProperty(lb, sender, propertyName);
            }
        }

        // Update the value
        // For now, it work for public property in the ViewModel or for public property in a public property of the ViewModel
        private void ChangeProperty(SimpleBinding simpleBinding, object sender, string propertyName)
        {
            Type senderType = sender.GetType();

            // if it is a direct bind (a primitive or a string in the ViewModel)
            if (senderType.IsPrimitive || senderType == typeof(string))
            {
                simpleBinding.ChangeValue(sender, propertyName);
            }
            else // Else it is a property of a ViewModel property
            {
                string[] propertySplit = propertyName.Split('.');
                string propertyToGet = propertySplit[propertySplit.Length - 1];

                // The symbol $ is used to bind an array index
                // This way we can handle Arrays binding without using list
                if (propertyToGet.Contains("$"))
                {
                    string[] arraySplit = propertyToGet.Split("$");
                    string arrayPropName = arraySplit[0];
                    int arrayIndex = -1;
                    int.TryParse(arraySplit[1], out arrayIndex);

                    PropertyInfo prop = senderType.GetProperty(arrayPropName);

                    if (prop != null && arrayIndex >= 0) // The prop exists et the index is a positive int
                    {
                        object value = prop.GetValue(sender);
                        if (ListBinding.IsIList(value))
                        {
                            var list = (IList)value;
                            if (list.Count > arrayIndex) // ChangeValue using the value in the index of the array
                            {
                                object arrayValue = list[arrayIndex];
                                simpleBinding.ChangeValue(arrayValue, propertyName);
                            }
                        }
                    }
                }
                else
                {
                    PropertyInfo prop = sender.GetType().GetProperty(propertyToGet);

                    if (prop != null)
                    {
                        object value = prop.GetValue(sender);

                        simpleBinding.ChangeValue(value, propertyName);
                    }
                }
            }
        }

        private void ChangeProperty(ListBinding listBinding, object sender, string propertyName)
        {
            Type senderType = sender.GetType();

            string[] propertySplit = propertyName.Split('.');

            PropertyInfo prop = sender.GetType().GetProperty(propertySplit[propertySplit.Length - 1]);

            if (prop != null)
            {
                object value = prop.GetValue(sender);

                listBinding.ChangeValue(sender);
            }
        }
    }
}