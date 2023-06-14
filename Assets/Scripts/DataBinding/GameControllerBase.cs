using Assets;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class GameControllerBase : MonoBehaviour, INotifyPropertyChanged, IViewModel
{
    private Dictionary<object, string> _properties = new Dictionary<object, string>();

    // Add binding on all public property of the GameController/ViewModel
    protected void InitialiserNotifyPropertyChanged()
    {
        Type type = this.GetType();

        foreach (PropertyInfo property in type.GetProperties()) // For each property in this
        {
            if (property.IsStatic()) continue;

            if (typeof(INotifyPropertyChanged).IsAssignableFrom(property.PropertyType)) // if the property implements INotifyPropertyChange
            {
                // Get the object that implements  INotifyPropertyChanged from the property
                INotifyPropertyChanged obj = (INotifyPropertyChanged)type.GetProperty(property.Name).GetValue(this, null);

                if (obj != null)
                {
                    // Add binding GameController properties that implements INotifyPropertyChanged
                    // This will notify the DataBinding process if a inner property changes in a GameController object property
                    obj.PropertyChanged += BoundProperty_PropertyChanged;

                    // Keep tracking of non primitive properties the know later if a property is bound to a object (and get the value in this object, not directly in the GameController)
                    if (!obj.GetType().IsPrimitive)
                        _properties.Add(obj, property.Name);

                    // This is for initialization a launch
                    // For each properties of the GameController inner object
                    foreach (PropertyInfo propertyChild in obj.GetType().GetProperties())
                    {
                        BoundProperty_PropertyChanged(obj, new PropertyChangedEventArgs(propertyChild.Name));
                    }
                }
            }
            else if (property.PropertyType.IsPrimitive || property.PropertyType == typeof(string))// Primitive or string -> get the direct value
            {
                // This is for initialization at launch
                object obj = type.GetProperty(property.Name).GetValue(this, null);

                BoundProperty_PropertyChanged(obj, new PropertyChangedEventArgs(property.Name));
            }
        }
    }

    // Property change handle
    private void BoundProperty_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        string propertyName = "";
        _properties.TryGetValue(sender, out propertyName);

        // If the value to get comes from a non primitive property, the update will have to check the inner properties from this non primitive
        if (!String.IsNullOrEmpty(propertyName)) propertyName += ".";

        // Notify change
        OnPropertyChanged(sender, propertyName + e.PropertyName);
    }

    public event PropertyChangedEventHandler PropertyChanged;

    // Bound property change
    protected void OnPropertyChanged(object sender, [CallerMemberName] string name = null)
    {
        PropertyChanged?.Invoke(sender, new PropertyChangedEventArgs(name));
    }

    // ViewModel/GameController Property change
    protected void OnPropertyChanged([CallerMemberName] string name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
