using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicUICustomElement : MonoBehaviour, IUICustomElement
{
    private bool _isActive;
    public bool IsActive 
    {
        get => _isActive;
        set
        {
            gameObject.SetActive(value);
            _isActive = value;
        }
    }
}
