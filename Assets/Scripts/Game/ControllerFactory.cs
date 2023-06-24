using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Game
{
    public class ControllerFactory : MonoBehaviour
    {
        [SerializeField] private ControllerType Type;

        private IController _controllerInstance;

        public static ControllerFactory Instance { get; private set; }

        private void OnEnable()
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

        public IController GetController()
        {
            if (_controllerInstance != null)
                return _controllerInstance;

            IController controller = null;

            switch (Type)
            {
                case ControllerType.MIDI:
                    controller = gameObject.AddComponent(typeof(MidiController)) as MidiController;
                    break;
                case ControllerType.Visual:
                    controller = gameObject.AddComponent(typeof(VisualController)) as VisualController;
                    break;
                case ControllerType.Keyboard:
                default:
                    controller = gameObject.AddComponent(typeof(KeyboardController)) as KeyboardController;
                    break;
            }

            _controllerInstance = controller;

            return controller;
        }
    }
}
