using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.DataBinding
{
    public class LeaderboardTemplateSelector : MonoBehaviour, IListBindingTemplateSelector
    {
        [SerializeField] private GameObject FirstTemplate;
        [SerializeField] private GameObject SecondTemplate;
        [SerializeField] private GameObject ThirdTemplate;
        [SerializeField] private GameObject InitialTemplate;

        private List<GameObject> _templates = new List<GameObject>();
        public List<GameObject> Templates => _templates;

        public void InitSelector()
        {
            if (Templates.Count == 0)
            {
                _templates.Add(FirstTemplate);
                _templates.Add(SecondTemplate);
                _templates.Add(ThirdTemplate);
                _templates.Add(InitialTemplate);
            }
        }

        public GameObject GetTemplate(object param)
        {
            GameObject template = InitialTemplate;
            if(param is int index)
            {
                switch (index)
                {
                    case 0: template = FirstTemplate; break;
                    case 1: template = SecondTemplate; break;
                    case 2: template = ThirdTemplate; break;
                    default: template = InitialTemplate; break;
                }
            }
            
            return template;
        }
    }
}
