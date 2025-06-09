using System.Collections.Generic;
using UnityEngine;

namespace VARLab.Velcro
{
    [CreateAssetMenu(fileName = "ToolbarSO", menuName = "ScriptableObjects/ToolbarSO")]
    public class ToolbarSO : ScriptableObject
    {
        public List<ToolbarButton> Buttons;
    }
}
