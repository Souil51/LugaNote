using Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UnityEngine.U2D;
using UnityEngine.UI;

public class SimpleBinding : MonoBehaviour
{
    // The name of the Property of the ICanvasElement
    public string MemberName;
    // Differents Path for ViewModel properties
    public List<BindingPath> Paths;
    // THe UI element
    private ICanvasElement _canvasElement;
    private IUICustomElement _customElement;
    // Used for the localization
    public LocalizeStringEvent localize;

    private void Awake()
    {
        InitComponent();
    }

    private void InitComponent()
    {
        if (_canvasElement == null)
            _canvasElement = GetComponent<ICanvasElement>();

        if (_customElement == null)
            _customElement = GetComponent<IUICustomElement>();
    }

    /// <summary>
    /// Update the value of the UI element
    /// </summary>
    /// <param name="value">the new value</param>
    /// <param name="propertyName">The ViewModel property name updated</param>
    public void ChangeValue(object value, string propertyName)
    {
        InitComponent();

        /*string[] splitName = MemberName.Split('.');
        if (splitName.Length == 1)
        {
            propInfo = _canvasElement.GetType().GetProperty(MemberName);
        }
        else if (splitName.Length == 2)
        {
            propInfo = _canvasElement.GetType().GetProperty(splitName[0]);
            if(propInfo == null)
                propInfo = _canvasElement.GetType().GetProperty("base"); // Testing common values

            if (propInfo != null)
                propInfo.PropertyType.GetProperty(splitName[1]);
        }*/

        if (!ChangeValueOfType(_canvasElement, value, propertyName))
            ChangeValueOfType(_customElement, value, propertyName);
    }

    private bool ChangeValueOfType(object obj, object value, string propertyName)
    {
        bool result = false;

        var propInfo = obj.GetType().GetProperty(MemberName);

        if (propInfo != null)
        {
            result = true;

            // Find the path to update
            BindingPath path = Paths.Find(x => x.Name == propertyName);

            object convertedValue = null;
            if (value is IConvertible)
            {
                // Get the type of the property to update
                Enum.TryParse(propInfo.PropertyType.Name, true, out TypeCode enumValue);
                // Convert the new value to the same type of the UI element property
                convertedValue = Convert.ChangeType(value, enumValue);
            }
            else
            {
                if(value is UnityEngine.Color)
                {
                    convertedValue = (UnityEngine.Color) value;
                }
            }
            
            path.LastValue = convertedValue;

            // For all type but not string, update the value
            if (propInfo.PropertyType != typeof(string))
            {
                SetValueOfType(propInfo, obj, convertedValue);
            }
            else // for the strings, perhaps it has to be localize or perhaps the value is a concatenation of multiples paths
            {
                if (localize == null)
                {
                    string concat = String.Join("", Paths.Where(x => x.LastValue != null).Select(x => x.LastValue));
                    SetValueOfType(propInfo, obj, concat);
                }
                else
                {
                    LocalizedString localizedStrings = new LocalizedString(localize.StringReference.TableReference, localize.StringReference.TableEntryReference);

                    foreach (BindingPath bindingPath in Paths)
                    {
                        if (bindingPath.LastValue != null)
                        {
                            localizedStrings.Add(bindingPath.LocalizeName, new StringVariable { Value = bindingPath.LastValue.ToString() });
                        }
                    }

                    localize.StringReference = localizedStrings;
                    localize.StringReference.RefreshString();
                }
            }
        }

        return result;
    }

    private void SetValueOfType(PropertyInfo propInfo, object obj, object value)
    {
        if (obj is ICanvasElement canvasElement)
            propInfo.SetValue(canvasElement, value);
        else if (obj is IUICustomElement customElement)
            propInfo.SetValue(customElement, value);
    }
}
