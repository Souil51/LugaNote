using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameViewModel : ViewModelBase
{
    public delegate void PlayAgainEventHandler(object sender, EventArgs e);
    public event PlayAgainEventHandler PlayAgain;

    public delegate void NavigateToMenuEventHandler(object sender, EventArgs e);
    public event NavigateToMenuEventHandler NavigateToMenu;

    public delegate void ResumeEventHandler(object sender, EventArgs e);
    public event ResumeEventHandler Resume;

    public delegate void OpenMenuEventHandler(object sender, EventArgs e);
    public event OpenMenuEventHandler OpenMenu;

    public delegate void ToggleVisualKeysEventHandler(object sender, EventArgs e);
    public event ToggleVisualKeysEventHandler ToggleVisualKeysVisibility;

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

    private string _playAgainString = "Play again";
    public string PlayAgainString
    {
        get => _playAgainString;
        set
        {
            _playAgainString = value;
            OnPropertyChanged();
        }
    }

    private bool _isPaused;

    public bool IsPaused
    {
        get { return _isPaused; }
        private set 
        { 
            _isPaused = value;
            OnPropertyChanged();
        }
    }

    private bool _visualKeysVisibilityVisibility = true;

    private string _visualKeysVisibilityText;
    public string VisualKeysVisibilityText
    {
        get => _visualKeysVisibilityText;
        set
        {
            _visualKeysVisibilityText = value;
            OnPropertyChanged();
        }
    }

    private bool _uIButtonsVisible;
    public bool UIButtonVisible
    {
        get { return _uIButtonsVisible; }
        set 
        { 
            _uIButtonsVisible = value;
            OnPropertyChanged();
        }
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
        UpdateVisualKeysVisibilityText();
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
        NavigateToMenu?.Invoke(this, EventArgs.Empty);
    }

    public void Button_EndPanelResume()
    {
        Resume?.Invoke(this, EventArgs.Empty);
    }

    public void Button_ReturnToMenu_Click()
    {
        OpenMenu?.Invoke(this, EventArgs.Empty);
    }

    public void Button_ToggleVisualKeysVisibility()
    {
        ToggleVisualKeysVisibility?.Invoke(this, EventArgs.Empty);

        _visualKeysVisibilityVisibility = !_visualKeysVisibilityVisibility;
        UpdateVisualKeysVisibilityText();
    }

    private void UpdateVisualKeysVisibilityText()
    {
        if (_visualKeysVisibilityVisibility)
            VisualKeysVisibilityText = "Hide visuals keys";
        else
            VisualKeysVisibilityText = "Show visuals keys";
    }

    #endregion
}
