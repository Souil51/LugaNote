using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControllerTest : GameControllerBase
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

    public TMPro.TextMeshProUGUI test;

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
        InitialiserNotifyPropertyChanged();

        Points = 10;
    }
}
