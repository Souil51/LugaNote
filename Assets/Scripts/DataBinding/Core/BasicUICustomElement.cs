using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    private Color _imageColor;
    public Color ImageColor
    {
        get => _imageColor;
        set
        {
            var image = GetComponent<Image>();
            if(image != null)
            {
                image.color = value;
            }
            _imageColor = value;
        }
    }
}
