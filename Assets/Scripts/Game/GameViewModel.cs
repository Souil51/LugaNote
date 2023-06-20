using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameViewModel : ViewModelBase
{
    public delegate void PlayAgainEventHandler(object sender, EventArgs e);
    public event PlayAgainEventHandler PlayAgain;

    public delegate void BackToMenuEventHandler(object sender, EventArgs e);
    public event BackToMenuEventHandler BackToMenu;

    #region Properties
    private int _points = 0;
    public int Points
    {
        get => _points;
        set
        {
            _points = value;
            OnPropertyChanged();
        }
    }

    private string _timeLeft;
    public string TimeLeft
    {
        get => _timeLeft;
        set
        {
            _timeLeft = value;
            OnPropertyChanged();
        }
    }

    private bool _endPanelVisible = false;
    public bool EndPanelVisible
    {
        get => _endPanelVisible;
        set
        {
            _endPanelVisible = value;
            OnPropertyChanged();
        }
    }

    private bool _uiVisible = false;
    public bool UIVisible
    {
        get => _uiVisible;
        set
        {
            _uiVisible = value;
            OnPropertyChanged();
        }
    }
    #endregion

    private void Awake()
    {
        GameController.Instance.PropertyChanged += Instance_PropertyChanged;
    }

    private void Update()
    {
        if(GameController.Instance.IsGameEnded && !EndPanelVisible) 
        {
            EndPanelVisible = true;
        }
    }

    public void InitializeViewModel()
    {
        InitialiserNotifyPropertyChanged();
    }

    private void Instance_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if(e.PropertyName == nameof(GameController.Instance.Points))
        {
            Points = GameController.Instance.Points;
        }
        
    }

    #region Methods
    public void HideAll()
    {
        UIVisible = false;
    }

    public void ShowAll()
    {
        UIVisible = true;
    }

    public void Button_EndPanelPlayAgain()
    {
        PlayAgain?.Invoke(this, EventArgs.Empty);
    }

    public void Button_EndPanelBackToMenu()
    {
        BackToMenu?.Invoke(this, EventArgs.Empty);
    }
    #endregion
}
