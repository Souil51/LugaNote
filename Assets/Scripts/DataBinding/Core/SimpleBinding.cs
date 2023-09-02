using Assets;
using DataBinding.Converter;
using DataBinding.Core;
using DataBinding.Core.Lists;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace DataBinding.Core
{
    public class SimpleBinding : MonoBehaviour
    {
        // The name of the Property of the ICanvasElement
        public string MemberName;
        // Differents Path for ViewModel properties
        public List<BindingPath> Paths;
        public MonoBehaviour Converter;
        // THe UI element
        private ICanvasElement _canvasElement;
        private IUICustomElement _customElement;
        private Selectable _selectableElement;
        // Used for the localization
        public LocalizeStringEvent localize;
        private IBindingConverter _converter;

        public bool LocalizationEntryBinding = false;

        private void Awake()
        {
            InitComponent();

            if (Converter != null)
            {
                if (!(Converter is IBindingConverter))
                    throw new Exception("Converter does not implement IBindingConverter");

                _converter = Converter.GetComponent<IBindingConverter>();
            }

            if (Paths.Where(x => x.LastValue != null).ToList().Count == 0)
            {
                object elementToChange = _canvasElement;
                var propInfo = elementToChange.GetType().GetProperty(MemberName);
                if (propInfo == null)
                {
                    elementToChange = _customElement;
                    propInfo = elementToChange.GetType().GetProperty(MemberName);

                    if (propInfo == null)
                    {
                        elementToChange = _selectableElement;
                        propInfo = elementToChange.GetType().GetProperty(MemberName);
                    }
                }

                if (propInfo != null)
                {
                    SetValueOfType(propInfo, elementToChange, propInfo.PropertyType.Default());
                }
            }
        }

        private void InitComponent()
        {
            if (_canvasElement == null)
                _canvasElement = GetComponent<ICanvasElement>();

            if (_customElement == null)
                _customElement = GetComponent<IUICustomElement>();

            if (_selectableElement == null)
                _selectableElement = GetComponent<Selectable>();
        }

        /// <summary>
        /// Update the value of the UI element
        /// </summary>
        /// <param name="value">the new value</param>
        /// <param name="propertyName">The ViewModel property name updated</param>
        public void ChangeValue(object value, string propertyName)
        {
            InitComponent();

            /* test : Possibility to bind inner property of UI Element ???
            string[] splitName = propertyName.Split('.');
            PropertyInfo propInfo;
            if (splitName.Length == 1)
            {
                propInfo = value.GetType().GetProperty(propertyName);
            }
            else if (splitName.Length == 2)
            {
                propInfo = value.GetType().GetProperty(splitName[0]);
                if(propInfo == null)
                    propInfo = _canvasElement.GetType().GetProperty("base"); // Testing common values

                if (propInfo != null)
                    propInfo.PropertyType.GetProperty(splitName[1]);
            }*/

            if (Converter != null && _converter == null)
            {
                if (!(Converter is IBindingConverter))
                    throw new Exception("Converter does not implement IBindingConverter in ChangedValue");

                _converter = Converter.GetComponent<IBindingConverter>();
            }

            if (Converter != null && _converter != null)
            {
                value = _converter.GetConvertedValue(value, null);
            }

            if (!ChangeValueOfType(_canvasElement, value, propertyName))
                if (!ChangeValueOfType(_customElement, value, propertyName))
                    ChangeValueOfType(_selectableElement, value, propertyName);
        }

        private bool ChangeValueOfType(object obj, object value, string propertyName)
        {
            bool result = false;

            if (obj == null)
                return result;

            var propInfo = obj.GetType().GetProperty(MemberName);

            if (propInfo != null)
            {
                result = true;

                // Find the path to update
                BindingPath path = Paths.Find(x => x.Name == propertyName);

                // Handle array index binding
                // The List property has changed and the index is in the propertyName after the key char "$" (passed by the viewmanager)
                if (path.Name.Contains('$')) 
                {
                    string[] arraySplit = propertyName.Split("$");
                    string arrayPropName = arraySplit[0];
                    int arrayIndex = -1;
                    int.TryParse(arraySplit[1], out arrayIndex);

                    if(arrayIndex >= 0 && value != null && ListBinding.IsIList(value))
                    {
                        IList typedList = (IList)value;
                        if(typedList.Count > arrayIndex)
                        {
                            value = typedList[arrayIndex];
                        }
                    }
                }

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
                    if (value is UnityEngine.Color)
                    {
                        convertedValue = (Color)value;
                    }
                    else if (value is Sprite)
                    {
                        convertedValue = (Sprite)value;
                    }
                    else if (value != null)
                    {
                        convertedValue = value.GetType().Default();
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
                        string concat = "";
                        var pathToConcat = Paths.Where(x => x.LastValue != null).ToList();
                        if (pathToConcat.Count > 1)
                        {
                            for (int i = 0; i < pathToConcat.Count; i++)
                            {
                                if (i == 0)
                                    concat += pathToConcat[i].LastValue.ToString() + pathToConcat[i].AfterConcatenationString;
                                else if (i == pathToConcat.Count - 1)
                                    concat += pathToConcat[i].BeforeConcatenationString + pathToConcat[i].LastValue.ToString();
                                else
                                    concat += pathToConcat[i].BeforeConcatenationString + pathToConcat[i].LastValue.ToString() + pathToConcat[i].AfterConcatenationString;
                            }
                        }
                        else if (pathToConcat.Count == 1)
                        {
                            concat = pathToConcat[0].LastValue.ToString();
                        }

                        SetValueOfType(propInfo, obj, concat);
                    }
                    else if(!LocalizationEntryBinding) // The binding will use the localize string, the value of the binding is a parameter of the localize string
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
                    else if(localize != null) // if LocalizationEntryBinding, the value of the Binding is the string entry key for the localization system
                    {
                        LocalizedString localizedString = null;

                        // Order by : we want the Entry Name first, then variables
                        foreach (BindingPath bindingPath in Paths.OrderBy(x => String.IsNullOrEmpty(x.LocalizeName) ? 0 : 1))
                        {
                            if (String.IsNullOrEmpty(bindingPath.LocalizeName))
                            {
                                if (bindingPath.LastValue != null)
                                {
                                    string[] localizeSplit = bindingPath.LastValue.ToString().Split('/');
                                    string tableRefName = localizeSplit[0];
                                    string entryRefName = String.Join("", localizeSplit.Skip(1));
                                    localizedString = new LocalizedString { TableReference = tableRefName, TableEntryReference = entryRefName };
                                }
                            }
                            else
                            {
                                if (bindingPath.LastValue != null && localizedString != null)
                                {
                                    localizedString.Add(bindingPath.LocalizeName, new StringVariable { Value = bindingPath.LastValue.ToString() });
                                }
                            }
                        }

                        localize.StringReference = localizedString;
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
            else if (obj is Selectable selectableElement)
                propInfo.SetValue(selectableElement, value);
        }
    }
}