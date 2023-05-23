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
    // Used for the localization
    public LocalizeStringEvent localize;

    private void Awake()
    {
        _canvasElement = GetComponent<ICanvasElement>();
    }

    /// <summary>
    /// Update the value of the UI element
    /// </summary>
    /// <param name="value">the new value</param>
    /// <param name="propertyName">The ViewModel property name updated</param>
    public void ChangeValue(object value, string propertyName)
    {
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

        // Get the property from the UI element
        PropertyInfo propInfo = _canvasElement.GetType().GetProperty(MemberName);

        if (propInfo != null)
        {
            // Find the path to update
            BindingPath path = Paths.Find(x => x.Name == propertyName);

            // Get the type of the property to update
            Enum.TryParse(propInfo.PropertyType.Name, true, out TypeCode enumValue);
            // Convert the new value to the same type of the UI element property
            var convertedValue = Convert.ChangeType(value, enumValue);

            path.LastValue = convertedValue;

            // For all type but not string, update the value
            if (propInfo.PropertyType != typeof(string))
            {
                propInfo.SetValue(_canvasElement, convertedValue);
            }
            else // for the strings, perhaps it has to be localize or perhaps the value is a concatenation of multiples paths
            {
                if (localize == null)
                {
                    string concat = String.Join("", Paths.Where(x => x.LastValue != null).Select(x => x.LastValue));
                    propInfo.SetValue(_canvasElement, concat);
                }
                else
                {
                    LocalizedString localizedStrings = new LocalizedString(localize.StringReference.TableReference, localize.StringReference.TableEntryReference);

                    foreach (BindingPath bindingPath in Paths)
                    {
                        if(bindingPath.LastValue != null)
                        {
                            localizedStrings.Add(bindingPath.LocalizeName, new StringVariable { Value = bindingPath.LastValue.ToString() });
                        }
                    }

                    localize.StringReference = localizedStrings;
                    localize.StringReference.RefreshString();
                }
            }
        }
    }
}
