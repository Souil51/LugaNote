using Assets.Scripts.Data;
using Assets.Scripts.Game.Model;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModeController : ViewModelBase
{
    private GameModeData _gameModeData;

    public int GoldScoreValue => _gameModeData?.Scores.Count == 0 ? 0 : _gameModeData.Scores[0].Value;
    public string GoldScoreDate => _gameModeData?.Scores.Count == 0 ? "" : _gameModeData.Scores[0].DateString;
    
    public int SilverScoreValue => _gameModeData?.Scores.Count <= 1 ? 0 : _gameModeData.Scores[1].Value;
    public string SilverScoreDate => _gameModeData?.Scores.Count <= 1 ? "" : _gameModeData.Scores[1].DateString;

    public int BronzeScoreValue => _gameModeData?.Scores.Count <= 2 ? 0 : _gameModeData.Scores[2].Value;
    public string BronzeScoreDate => _gameModeData?.Scores.Count <= 2 ? "" : _gameModeData.Scores[1].DateString;

    private void Awake()
    {
        _gameModeData = SaveManager.Save.GetGameModeData(0);

        OnPropertyChanged("GoldScoreValue");
        OnPropertyChanged("GoldScoreDate");
        OnPropertyChanged("SilverScoreValue");
        OnPropertyChanged("SilverScoreDate");
        OnPropertyChanged("BronzeScoreValue");
        OnPropertyChanged("BronzeScoreDate");
    }
}
