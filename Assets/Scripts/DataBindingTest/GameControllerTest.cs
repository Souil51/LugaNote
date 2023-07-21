using Assets.Scripts.DataBinding;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Unity.VisualScripting;
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

    private ObservableCollection<Player> _players;
    public ObservableCollection<Player> PlayerList
    {
        get => _players;
        set 
        {
            _players = value;
            OnPropertyChanged();
        }
    }

    public ListBinding listBinding;

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
        _player = new Player(5, "Name");
        Player.GetDamaged(1);
        Points = 10;

        Player.SetFriend(new Player(5, "My friend"));

        /*PlayerList = new List<Player>();
        PlayerList.Add(new Player(10, "Player 1"));
        PlayerList[0].SetFriend(new Player(50, "My Friend"));
        PlayerList.Add(new Player(20, "Player 2"));
        PlayerList.Add(new Player(30, "Player 3"));*/

        PlayerList = new ObservableCollection<Player>();
        var player1 = new Player(10, "Player 1");
        player1.SetFriend(new Player(50, "My Friend"));

        PlayerList.Add(player1);
        PlayerList.Add(new Player(20, "Player 2"));
        PlayerList[1].SetFriend(new Player(10, "My 2nd Friend"));

        TestIsActive = true;

        InitialiserNotifyPropertyChanged();

        PlayerList[1].Friend.SetName("My 3rd Friend");
        Player.SetFriend(new Player(5, "My other friend"));
        // PlayerList[1].SetFriend(new Player(10, "My 2nd Frienc"));

        /*PlayerList[0].SetFriend(new Player(50, "My Friend"));
        PlayerList[0].Friend.SetName("My Friend changed");

        PlayerList.Add(new Player(30, "Player 3"));

        PlayerList.RemoveAt(1);

        var newList = new List<Player>();
        newList.Add(new Player(99, "T1"));
        newList.Add(new Player(99, "T2"));

        PlayerList.AddRange(newList);*/

        /*Player.GetDamaged(1);
        PlayerList[0].Friend.SetName("My Friend changed");
        PlayerList[0].GetDamaged(1);*/

        // For now, we have to recreate the list
        // Set to use observable list
        // PlayerList = new List<Player>(PlayerList);
    }
}
