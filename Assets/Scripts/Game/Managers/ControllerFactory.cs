using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
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

            IController controller;
            switch (Type)
            {
                case ControllerType.MIDI:
                    controller = gameObject.AddComponent(typeof(MidiController)) as MidiController;
                    break;
                case ControllerType.Visual:
                    controller = gameObject.AddComponent(typeof(VisualController)) as VisualController;
                    break;
                case ControllerType.KeyboardAndVisual:
                    controller = gameObject.AddComponent(typeof(MultipleController)) as MultipleController;
                    var keyboardController = gameObject.AddComponent(typeof(KeyboardController)) as KeyboardController;
                    var visualController = gameObject.AddComponent(typeof(VisualController)) as VisualController;
                    ((MultipleController)controller).InitializeController(keyboardController, visualController);
                    break;
                case ControllerType.KeyboardAndMidi:
                    controller = gameObject.AddComponent(typeof(MultipleController)) as MultipleController;
                    var keyboardController_2 = gameObject.AddComponent(typeof(KeyboardController)) as KeyboardController;
                    var midiController_2 = gameObject.AddComponent(typeof(MidiController)) as MidiController;
                    ((MultipleController)controller).InitializeController(keyboardController_2, midiController_2);
                    break;
                case ControllerType.KeyboardVisualMidi:
                    controller = gameObject.AddComponent(typeof(MultipleController)) as MultipleController;
                    var keyboardController_3 = gameObject.AddComponent(typeof(KeyboardController)) as KeyboardController;
                    var midiController_3 = gameObject.AddComponent(typeof(MidiController)) as MidiController;
                    var visualController_3 = gameObject.AddComponent(typeof(VisualController)) as VisualController;
                    ((MultipleController)controller).InitializeController(keyboardController_3, midiController_3, visualController_3);
                    break;
                case ControllerType.Keyboard:
                default:
                    controller = gameObject.AddComponent(typeof(KeyboardController)) as KeyboardController;
                    break;
            }

            _controllerInstance = controller;

            return controller;
        }

        public ControllerType GetCurrentType()
        {
            return Type;
        }
    }
}
