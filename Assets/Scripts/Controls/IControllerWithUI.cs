using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Controls
{
    public interface IControllerWithUI : IController
    {
        void GenerateUI();
        void ChangeMode(VisualControllerMode mode);
    }
}
