using Assets;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using UnityEngine;

public class ViewManager : MonoBehaviour
{
    [SerializeField, TypeConstraint(typeof(IViewModel))]
    private GameObject DataContextContainer;

    [SerializeField]
    private List<SimpleBinding> Bindings;

    private IViewModel DataContext;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Awake()
    {
        DataContext = DataContextContainer.GetComponent<IViewModel>();
        InitialiserDataContext();
    }

    public void DamagePlayer_Click()
    {
        // ((GameController)DataContext).DamagePlayer();
    }

    /// <summary>
    /// Get the DataContext change sevents
    /// </summary>
    private void InitialiserDataContext()
    {
        DataContext.PropertyChanged += BoundProperty_PropertyChanged;
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

            PropertyInfo prop = sender.GetType().GetProperty(propertySplit[propertySplit.Length - 1]);

            if (prop != null)
            {
                object value = prop.GetValue(sender);

                simpleBinding.ChangeValue(value, propertyName);
            }
        }
    }
}
