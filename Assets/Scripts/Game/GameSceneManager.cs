using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    public delegate void SceneLoadedEventHandler(object sender, SceneEventArgs e);
    public event SceneLoadedEventHandler SceneLoaded;

    public static GameSceneManager Instance { get; private set; }
    private Dictionary<string, SceneSessionData> _datas = new Dictionary<string, SceneSessionData>();

    private void OnEnable()
    {
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
    }

    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        SceneLoaded?.Invoke(this, new SceneEventArgs(arg0, arg1));
    }

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

    public void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void SetValue(string key, object value)
    {
        if (!_datas.ContainsKey(key))
            _datas.Add(key, new SceneSessionData(key, value));
        else
            _datas[key] = new SceneSessionData(key, value);
    }

    public object GetValue(string key)
    {
        if (!_datas.ContainsKey(key))
            return null;

        return _datas[key].Value;
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