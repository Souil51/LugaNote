using DataBinding.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.SmartFormat.Extensions;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UnityEngine.Localization.Tables;

public class TestController : ViewModelBase
{
    public LocalizeStringEvent localize;

    public string PlayerName => "AZERTY";
     
    private int _points = 0;
    public int PointsProp => _points;

    public int Points = 10;

    public string EntryTest => "MainScene/show_touch_keyboard";

    private void Awake()
    {
        InitialiserNotifyPropertyChanged();
    }

    // Start is called before the first frame update
    async void Start()
    {
        //var source = LocalizationSettings.StringDatabase.SmartFormatter.GetSourceExtension<PersistentVariablesSource>();
        //var test = source["global"];
        //StringVariable localizedSetTitleVariable = source["global"]["player-name"] as StringVariable;
        //localizedSetTitleVariable.Value = "qsdfghjklm";

        //ObjectVariable objectVariable = source["global"]["controller"] as ObjectVariable;
        //objectVariable.Value = this;
        // string playerName = LocalizationSettings.StringDatabase.GetGlobalVariable("player-name");

        _points = 10;

        //var collection = LocalizationEditorSettings.GetStringTableCollection("MainScene");
        //var data = collection.SharedData.GetId("hide_touch_keyboard");
        //var reference = collection.SharedData.GetEntry(data);

        //var locStr = new LocalizedString { TableReference = "MainazeScene", TableEntryReference = "show_touch_keyboard" };
        //localize.StringReference = locStr;

        var op = await LocalizationSettings.StringDatabase.GetLocalizedStringAsync("MainMenu", "menu_midi_61").Task;
        //if (op.IsDone)
        //    Debug.Log(op.Result);
        //else
        //    op.Completed += (op) => Debug.Log(op.Result);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
