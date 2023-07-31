using Assets;
using Assets.Scripts.DataBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// The base of a viemodel for DataBinding
/// It initializes the binding by looping in all public Property of the object type
/// The databinding updated is then done by calling OnPropertyChanged
/// </summary>
public class ViewModelBase : MonoBehaviour, INotifyPropertyChanged, IViewModel
{
    public event PropertyChangedEventHandler PropertyChanged;
    public event IViewModel.GameObjectCreatedEventHandler GameObjectCreated;
    public event IViewModel.GameObjectDestroyedEventHandler GameObjectDestroyed;

    private Dictionary<object, string> _properties = new Dictionary<object, string>();

    protected virtual void OnGameObjectCreated(object sender, GameObjectEventArgs e)
    {
        GameObjectCreated?.Invoke(sender, e);
    }

    protected virtual void OnGameObjectDestroyed(object sender, GameObjectEventArgs e)
    {
        GameObjectDestroyed?.Invoke(sender, e);
    }

    // Add binding on all public property of the GameController/ViewModel
    public void InitialiserNotifyPropertyChanged(object viewModel = null)
    {
        if(viewModel == null)
            viewModel = this;

        Type type = viewModel.GetType();

        foreach (PropertyInfo property in type.GetProperties()) // For each property in this
        {
            if (property.IsStatic()) continue;

            if (ListBinding.IsIList(property.PropertyType))
            {
                /*var listType = ListBinding.GetListType(property.PropertyType);
                object obj = type.GetProperty(property.Name).GetValue(this, null);*/
                BoundProperty_PropertyChanged(viewModel, new PropertyChangedEventArgs(property.Name));
            }

            if (typeof(INotifyPropertyChanged).IsAssignableFrom(property.PropertyType)) // if the property implements INotifyPropertyChange
            {
                BindInnerObjectProperty(viewModel, property);
                // Get the object that implements  INotifyPropertyChanged from the property
                /*INotifyPropertyChanged obj = (INotifyPropertyChanged)type.GetProperty(property.Name).GetValue(viewModel, null);

                if (obj != null)
                {
                    // Add binding GameController properties that implements INotifyPropertyChanged
                    // This will notify the DataBinding process if a inner property changes in a GameController object property
                    obj.PropertyChanged += BoundProperty_PropertyChanged;

                    // Keep tracking of non primitive properties the know later if a property is bound to a object (and get the value in this object, not directly in the GameController)
                    if (!obj.GetType().IsPrimitive && !_properties.ContainsKey(obj))
                        _properties.Add(obj, property.Name);

                    // This is for initialization at launch
                    // For each properties of the GameController inner object
                    foreach (PropertyInfo propertyChild in obj.GetType().GetProperties())
                    {
                        BoundProperty_PropertyChanged(obj, new PropertyChangedEventArgs(propertyChild.Name));
                    }
                }*/
            }
            else if (property.PropertyType.IsPrimitive || property.PropertyType == typeof(string))// Primitive or string -> get the direct value
            {
                // This is for initialization at launch
                // object obj = type.GetProperty(property.Name).GetValue(this, null);

                BoundProperty_PropertyChanged(viewModel, new PropertyChangedEventArgs(property.Name));
            }
        }
    }

    // Property change handle
    private void BoundProperty_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (sender == null) return;

        string propertyName = "";
        _properties.TryGetValue(sender, out propertyName);

        // If the value to get comes from a non primitive property, the update will have to check the inner properties from this non primitive
        if (!String.IsNullOrEmpty(propertyName)) propertyName += ".";

        // Notify change
         OnPropertyChanged(sender, propertyName + e.PropertyName);
    }

    private void BindInnerObjectProperty(object target, PropertyInfo property)
    {
        if (typeof(INotifyPropertyChanged).IsAssignableFrom(property.PropertyType)) // if the property implements INotifyPropertyChange
        {
            // Get the object that implements  INotifyPropertyChanged from the property
            INotifyPropertyChanged obj = (INotifyPropertyChanged)target.GetType().GetProperty(property.Name).GetValue(target, null);

            if (obj != null)
            {
                // Add binding GameController properties that implements INotifyPropertyChanged
                // This will notify the DataBinding process if a inner property changes in a GameController object property
                if(!obj.GetType().IsPrimitive && !_properties.ContainsKey(obj))
                {
                    obj.PropertyChanged += BoundProperty_PropertyChanged;
                    // Keep tracking of non primitive properties the know later if a property is bound to a object (and get the value in this object, not directly in the GameController)
                    _properties.Add(obj, property.Name);
                }
                
                // This is for initialization at launch
                // For each properties of the GameController inner object
                foreach (PropertyInfo propertyChild in obj.GetType().GetProperties())
                {
                    BoundProperty_PropertyChanged(obj, new PropertyChangedEventArgs(propertyChild.Name));
                }
            }
        }
    }

    // Bound property change
    protected void OnPropertyChanged(object sender, [CallerMemberName] string name = null)
    {
        if (!_properties.ContainsValue(name))
        {
            var property = sender.GetType().GetProperties().Where(x => x.Name == name).FirstOrDefault();
            if (property != null && !property.PropertyType.IsPrimitive)
            {
                BindInnerObjectProperty(sender, property);
            }
        }

        PropertyChanged?.Invoke(sender, new PropertyChangedEventArgs(name));
    }

    // ViewModel/GameController Property change
    protected void OnPropertyChanged([CallerMemberName] string name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}