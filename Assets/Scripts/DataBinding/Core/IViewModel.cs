using System;
using System.ComponentModel;
using UnityEngine;

namespace DataBinding.Core
{
    public interface IViewModel : INotifyPropertyChanged 
    {
        delegate void GameObjectCreatedEventHandler(object sender, GameObjectEventArgs e);
        event GameObjectCreatedEventHandler GameObjectCreated;

        delegate void GameObjectDestroyedEventHandler(object sender, GameObjectEventArgs e);
        event GameObjectDestroyedEventHandler GameObjectDestroyed;
    }

    public class GameObjectEventArgs : EventArgs
    {
        private GameObject _gameObject;
        public GameObject GameObject => _gameObject;

        public GameObjectEventArgs(GameObject gameObject)
        {
            _gameObject = gameObject;
        }
    }
}
