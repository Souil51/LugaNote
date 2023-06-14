using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Game
{
    public class ControllerFactory
    {
        public static IController GetControllerForType(ControllerType type)
        {
            IController controller = null;

            switch (type)
            {
                case ControllerType.MIDI:
                    {
                        controller = GameController.Instance.gameObject.AddComponent(typeof(MidiController)) as MidiController;
                    }
                    break;
                case ControllerType.Visual:
                    controller = GameController.Instance.gameObject.AddComponent(typeof(VisualController)) as VisualController;
                    break;
                case ControllerType.Keyboard:
                default:
                    controller = GameController.Instance.gameObject.AddComponent(typeof(KeyboardController)) as KeyboardController;
                    break;
            }

            return controller;
        }
    }
}
