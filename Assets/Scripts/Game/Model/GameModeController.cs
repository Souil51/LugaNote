using Assets.Scripts.Data;
using Assets.Scripts.Game.Model;
using Assets.Scripts.Game.Save;
using DataBinding.Core;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameModeController : ViewModelBase
{
    [SerializeField] private GameModeType GameModeType;
    [SerializeField] private IntervalMode IntervalMode;
    [SerializeField] private bool WithRandomAccidental;
    [SerializeField] private bool WithInversion;
    [SerializeField] private bool GuessName;
    [SerializeField] private Level Level;

    private GameModeData _gameModeData;

    private List<Score> _scoresOrdered => _gameModeData.Scores.OrderByDescending(x => x.Value).ToList();

    public int GoldScoreValue => _scoresOrdered.Count == 0 ? 0 : _scoresOrdered[0].Value;
    public string GoldScoreDate => _scoresOrdered.Count == 0 ? "" : _scoresOrdered[0].DateString;
    
    public int SilverScoreValue => _scoresOrdered.Count <= 1 ? 0 : _scoresOrdered[1].Value;
    public string SilverScoreDate => _scoresOrdered.Count <= 1 ? "" : _scoresOrdered[1].DateString;

    public int BronzeScoreValue => _scoresOrdered.Count <= 2 ? 0 : _scoresOrdered[2].Value;
    public string BronzeScoreDate => _scoresOrdered.Count <= 2 ? "" : _scoresOrdered[2].DateString;

    private void Awake()
    {
        _gameModeData = SaveManager.Save.GetGameModeData(GameModeType, IntervalMode, Level, GuessName, WithRandomAccidental, WithInversion);

        InitialiserNotifyPropertyChanged();
    }

    private void Start()
    {
        OnPropertyChanged("GoldScoreValue");
        OnPropertyChanged("GoldScoreDate");
        OnPropertyChanged("SilverScoreValue");
        OnPropertyChanged("SilverScoreDate");
        OnPropertyChanged("BronzeScoreValue");
        OnPropertyChanged("BronzeScoreDate");
    }
}
