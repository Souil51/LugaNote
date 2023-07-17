using Assets.Scripts.DataBinding;
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

    public List<Player> PlayerList;

    public ListBinding listBinding;

    // Start is called before the first frame update
    void Start()
    {
        var PlayerList = new List<Player>();
        PlayerList.Add(new Player(10, "Player 1"));
        PlayerList.Add(new Player(20, "Player 2"));
        listBinding.ChangeValue(PlayerList);

        PlayerList[0].GetDamaged(1);
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
