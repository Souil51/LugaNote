using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DataBinding.Core
{
    public class BasicUICustomElement : MonoBehaviour, IUICustomElement
    {
        /*
         * WHEN ADDING PROPERTY HERE DON'T FORGET TO HANDLE THE TYPE IN SimpleBinding.cs -> ChangeValueOfType
         */
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
                if (image != null)
                {
                    image.color = value;
                }
                _imageColor = value;
            }
        }

        private Sprite _imageSprite;
        public Sprite ImageSprite
        {
            get => _imageSprite;
            set
            {
                var image = GetComponent<Image>();
                if (image != null)
                {
                    image.sprite = value;
                }
                _imageSprite = value;
            }
        }
    }
}