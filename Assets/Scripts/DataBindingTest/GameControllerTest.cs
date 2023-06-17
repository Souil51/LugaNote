using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControllerTest : ViewModelBase
{
    private int _points;
    public int Points
    {
        get => _points;
        private set
        {
            _points = value;
            OnPropertyChanged();
        }
    }

    private Player _player;
    public Player Player => _player;

    public TMPro.TextMeshProUGUI test;

    private bool _testIsActive = true;
    public bool TestIsActive
    {
        get => _testIsActive;
        set
        {
            _testIsActive = value;
            OnPropertyChanged();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TestIsActive = !TestIsActive;
        }
    }

    private void Awake()
    {
        _player = new Player(5);
        Player.GetDamaged(1);
        Points = 10;
        
        InitialiserNotifyPropertyChanged();
    }
}
