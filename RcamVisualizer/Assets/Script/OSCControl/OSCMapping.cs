using System.Collections.Generic;
using UnityEngine;

namespace Script.OSCControl
{
    [CreateAssetMenu(fileName = "DefaultMapping", menuName = "RcamOSCMapping", order = 0)]
    public class OSCMapping : ScriptableObject
    {
        public List<int> buttonsMap;
        public List<int> togglesMap;
        public List<int> knobsMap;
    }
}