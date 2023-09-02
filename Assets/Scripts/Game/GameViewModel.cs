using DataBinding.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GameViewModel : ViewModelBase
{
    public event EventHandler PlayAgain;
    public event EventHandler NavigateToMenu;
    public event EventHandler Resume;
    public event EventHandler OpenMenu;
    public event EventHandler ToggleVisualKeysVisibility;

    #region Properties
    private int _points = 0;
    public int Points
    {
        get => _points;
        set => SetProperty(ref _points, value);
    }

    private string _timeLeft;
    public string TimeLeft
    {
        get => _timeLeft;
        set => SetProperty(ref _timeLeft, value);
    }

    private bool _endPanelVisible = false;
    public bool EndPanelVisible
    {
        get => _endPanelVisible;
        set => SetProperty(ref _endPanelVisible, value);
    }

    private bool _uiVisible = false;
    public bool UIVisible
    {
        get => _uiVisible;
        set => SetProperty(ref _uiVisible, value);
    }

    private string _playAgainString = "Play again";
    public string PlayAgainString
    {
        get => _playAgainString;
        set => SetProperty(ref _playAgainString, value);
    }

    private string _resumeString = "Resume";
    public string ResumeString
    {
        get => _resumeString;
        set => SetProperty(ref _resumeString, value);
    }

    private bool _isPaused;

    public bool IsPaused
    {
        get { return _isPaused; }
        private set => SetProperty(ref _isPaused, value);
    }

    private bool _visualKeysVisibilityVisibility = true;
    public bool VisualKeysVisibilityVisibility
    {
        get => _visualKeysVisibilityVisibility;
        set => SetProperty(ref _visualKeysVisibilityVisibility, value);
    }

    private string _visualKeysVisibilityText;
    public string VisualKeysVisibilityText
    {
        get => _visualKeysVisibilityText;
        set => SetProperty(ref _visualKeysVisibilityText, value);
    }

    private bool _uIButtonsVisible;
    public bool UIButtonVisible
    {
        get { return _uIButtonsVisible; }
        set => SetProperty(ref _uIButtonsVisible, value);
    }

    #endregion

    private void Awake()
    {
        GameController.Instance.PropertyChanged += Instance_PropertyChanged;
    }

    public void InitializeViewModel()
    {
        InitialiserNotifyPropertyChanged();

        UIButtonVisible = GameController.Instance.HasControllerUI;
        // UpdateVisualKeysVisibilityText();
    }

    private void Instance_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if(e.PropertyName == nameof(GameController.Instance.Points))
        {
            Points = GameController.Instance.Points;
        }
        else if(e.PropertyName == nameof(GameController.Instance.TimeLeft))
        {
            if(GameController.Instance.TimeLeft == 0)
                TimeLeft = (((int)(GameController.Instance.TimeLeft))).ToString();
            else
                TimeLeft = (((int)(GameController.Instance.TimeLeft)) + 1).ToString();
        }
        else if (e.PropertyName == nameof(GameController.Instance.State))
        {
            bool panelVisible = false;
            if (GameController.Instance.State == GameState.Ended || GameController.Instance.State == GameState.Paused)
            {
                if (!EndPanelVisible)
                    panelVisible = true;
            }

            IsPaused = GameController.Instance.State == GameState.Paused;
            EndPanelVisible = panelVisible;
        }
    }

    #region Methods
    public void HideAll()
    {
        UIVisible = false;
        EndPanelVisible = false;
    }

    public void ShowAll() => UIVisible = true;

    public void Button_EndPanelPlayAgain() => PlayAgain?.Invoke(this, EventArgs.Empty);

    public void Button_EndPanelBackToMenu() => NavigateToMenu?.Invoke(this, EventArgs.Empty);

    public void Button_EndPanelResume() => Resume?.Invoke(this, EventArgs.Empty);

    public void Button_ReturnToMenu_Click() => OpenMenu?.Invoke(this, EventArgs.Empty);

    public void Button_ToggleVisualKeysVisibility()
    {
        ToggleVisualKeysVisibility?.Invoke(this, EventArgs.Empty);

        VisualKeysVisibilityVisibility = !VisualKeysVisibilityVisibility;
        // UpdateVisualKeysVisibilityText();
    }

    private void UpdateVisualKeysVisibilityText()
    {
        if (_visualKeysVisibilityVisibility)
            VisualKeysVisibilityText = "Hide visuals keys";
        else
            VisualKeysVisibilityText = "Show visuals keys";
    }

    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        if (!EqualityComparer<T>.Default.Equals(field, value))
        {
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        return false;
    }

    #endregion
}
