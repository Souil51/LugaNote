using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSessionManager : MonoBehaviour
{
    public static SceneSessionManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private GameMode _gameMode;
    public GameMode GameMode
    {
        get => _gameMode;
        private set
        {
            _gameMode = value;
        }
    }

    public void SetGameMode(GameMode gameMode)
    {
        GameMode = gameMode;
    }
}
