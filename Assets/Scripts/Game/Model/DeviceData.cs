using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Game.Model
{
    [Serializable]
    public class DeviceData
    {
        public string Name  { get; set; }
        public bool AndroidPermissionRequested { get; set; }
        public bool AndroidPermission { get; set; }


        public DeviceData(string name)
        {
            Name = name;
        }
    }
}
