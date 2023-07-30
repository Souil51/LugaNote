using Assets.Scripts.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadioButtonsGroupViewModel : ViewModelBase
{
    public delegate void SelectedButtonChangedEventHandler(object sender, IntEventArgs e);
    public event SelectedButtonChangedEventHandler SelectedButtonChanged;

    [SerializeField] private List<GameObject> Buttons = new List<GameObject>();
    [SerializeField] private Color SelectedColor;
    [SerializeField] private Color NeutralColor;
    [SerializeField] private Color SelectedSecondaryColor;
    [SerializeField] private Color NeutralSecondaryColor;
    [SerializeField] private Color SelectedTextColor;
    [SerializeField] private Color NeutralTextColor;
    [SerializeField] private Sprite SelectedSprite;
    [SerializeField] private Sprite NeutralSprite;

    [SerializeField] private List<Color> SelectedColorsList;
    [SerializeField] private List<Color> NeutralColorsList;
    [SerializeField] private List<Color> SelectedSecondaryColorsList;
    [SerializeField] private List<Color> NeutralSecondaryColorsList;
    [SerializeField] private List<Color> SelectedTextColorsList;
    [SerializeField] private List<Color> NeutralTextColorsList;
    [SerializeField] private List<Sprite> SelectedSpritesList;
    [SerializeField] private List<Sprite> NeutralSpritesList;

    private int _selectedIndex = 0;
    public int SelectedIndex => _selectedIndex;

    private List<Color> _buttonsColor = new List<Color>();
    public List<Color> ButtonsColor
    {
        get => _buttonsColor;
        set
        {
            _buttonsColor = value;
            OnPropertyChanged();
        }
    }

    private List<Color> _buttonsSecondaryColor = new List<Color>();
    public List<Color> ButtonsSecondaryColor
    {
        get => _buttonsSecondaryColor;
        set
        {
            _buttonsSecondaryColor = value;
            OnPropertyChanged();
        }
    }

    private List<Color> _buttonsTextColor = new List<Color>();
    public List<Color> ButtonsTextColor
    {
        get => _buttonsTextColor;
        set
        {
            _buttonsTextColor = value;
            OnPropertyChanged();
        }
    }

    private List<Sprite> _buttonsSprite = new List<Sprite>();
    public List<Sprite> ButtonsSprite
    {
        get => _buttonsSprite;
        set
        {
            _buttonsSprite = value;
            OnPropertyChanged();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        InitLists();

        InitialiserNotifyPropertyChanged();


        UpdateButtons();
    }

    private void InitLists()
    {
        ButtonsColor.Clear();
        ButtonsSecondaryColor.Clear();
        ButtonsTextColor.Clear();
        ButtonsSprite.Clear();

        for (int i = 0; i < Buttons.Count; i++)
        {
            ButtonsColor.Add(NeutralColor);
            ButtonsSecondaryColor.Add(NeutralSecondaryColor);
            ButtonsTextColor.Add(NeutralTextColor);
            ButtonsSprite.Add(NeutralSprite);
        }
    }

    private void UpdateButtons()
    {
        for (int i = 0; i < Buttons.Count; i++)
        {
            if (i == _selectedIndex)
            {
                if(SelectedColorsList != null && SelectedColorsList.Count > 0)
                {
                    ButtonsColor[i] = SelectedColorsList[i % SelectedColorsList.Count];
                }
                else
                {
                    ButtonsColor[i] = SelectedColor;
                }

                if (SelectedSecondaryColorsList != null && SelectedSecondaryColorsList.Count > 0)
                {
                    ButtonsSecondaryColor[i] = SelectedSecondaryColorsList[i % SelectedSecondaryColorsList.Count];
                }
                else
                {
                    ButtonsSecondaryColor[i] = SelectedSecondaryColor;
                }

                if (SelectedTextColorsList != null && SelectedTextColorsList.Count > 0)
                {
                    ButtonsTextColor[i] = SelectedTextColorsList[i % SelectedTextColorsList.Count];
                }
                else
                {
                    ButtonsTextColor[i] = SelectedTextColor;
                }

                if(SelectedSpritesList != null && SelectedSpritesList.Count > 0) 
                {
                    ButtonsSprite[i] = SelectedSpritesList[i % SelectedSpritesList.Count];
                }
                else
                {
                    ButtonsSprite[i] = SelectedSprite;
                }
            }
            else
            {
                if (NeutralColorsList != null && NeutralColorsList.Count > 0)
                {
                    ButtonsColor[i] = NeutralColorsList[i % NeutralColorsList.Count];
                }
                else
                {
                    ButtonsColor[i] = NeutralColor;
                }

                if (NeutralSecondaryColorsList != null && NeutralSecondaryColorsList.Count > 0)
                {
                    ButtonsSecondaryColor[i] = NeutralSecondaryColorsList[i % NeutralSecondaryColorsList.Count];
                }
                else
                {
                    ButtonsSecondaryColor[i] = NeutralSecondaryColor;
                }

                if (NeutralTextColorsList != null && NeutralTextColorsList.Count > 0)
                {
                    ButtonsTextColor[i] = NeutralTextColorsList[i % NeutralTextColorsList.Count];
                }
                else
                {
                    ButtonsTextColor[i] = NeutralTextColor;
                }

                if(NeutralSpritesList != null && NeutralSpritesList.Count > 0)
                {
                    ButtonsSprite[i] = NeutralSpritesList[i % NeutralSpritesList.Count];
                }
                else
                {
                    ButtonsSprite[i] = NeutralSprite;
                }
            }

            OnPropertyChanged(nameof(ButtonsColor) + "$" + i);
            OnPropertyChanged(nameof(ButtonsSecondaryColor) + "$" + i);
            OnPropertyChanged(nameof(ButtonsTextColor) + "$" + i);
            OnPropertyChanged(nameof(ButtonsSprite) + "$" + i);
        }
    }

    public void SelectButton(int index)
    {
        if (ButtonsColor == null || ButtonsColor.Count == 0)
            InitLists();

        if (Buttons.Count == 0 || index >= Buttons.Count) return;

        _selectedIndex = index;
        UpdateButtons();

        SelectedButtonChanged?.Invoke(this, new IntEventArgs(SelectedIndex));
    }
}