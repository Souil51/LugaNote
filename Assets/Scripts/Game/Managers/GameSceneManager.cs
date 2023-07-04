using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager
{
    public delegate void SceneLoadedEventHandler(object sender, SceneEventArgs e);
    public event SceneLoadedEventHandler SceneLoaded;

    public static GameSceneManager Instance { get; private set; }
    private Dictionary<string, SceneSessionData> _sessionDatas = new Dictionary<string, SceneSessionData>();

    private static GameSceneManager _instance;
    public static GameSceneManager Instance 
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameSceneManager();
            }

            return _instance;
        }
        private set
        {
            _instance = value;
        }
    }

    private GameSceneManager()
    {
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    }

    ~GameSceneManager()
    {
        SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
    }

    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        SceneLoaded?.Invoke(this, new SceneEventArgs(arg0, arg1));
    }

    public void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void SetValue(string key, object value)
    {
        if (!_sessionDatas.ContainsKey(key))
            _sessionDatas.Add(key, new SceneSessionData(key, value));
        else
            _sessionDatas[key] = new SceneSessionData(key, value);
    }

    public object GetValue(string key)
    {
        if (!_sessionDatas.ContainsKey(key))
            return null;

        return _sessionDatas[key].Value;
    }

    public T GetValue<T>(string key)
    {
        if (!_sessionDatas.ContainsKey(key))
            return default(T);

        try
        {
            return (T)_sessionDatas[key].Value;
        }
        catch
        {
            return default(T);
        }
    }
}

public class SceneSessionData
{
    private Type _type;
    public Type Type => _type;

    private string _key;
    public string Key => _key;

    private object _value;
    public object Value => _value;

    public SceneSessionData(string key, object value)
    {
        _key = key;
        _value = value;
        _type = value.GetType();
    }
}

public class SceneEventArgs : EventArgs
{
    private Scene _scene;
    public Scene Scene => _scene;

    private LoadSceneMode _loadSceneMode;
    public LoadSceneMode LoadSceneMode => _loadSceneMode;

    public SceneEventArgs(Scene scene, LoadSceneMode loadSceneMode)
    {
        _scene = scene;
        _loadSceneMode = loadSceneMode;
    }
}