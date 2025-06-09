using System.Collections.Generic;
using UnityEngine;

namespace VARLab.Velcro
{
    [CreateAssetMenu(fileName = "SplashScreenSO", menuName = "ScriptableObjects/SplashScreenSO")]
    public class SplashScreenSO : ScriptableObject
    {
        [Header("Content"), Space(5)]
        [TextArea(1, 5), Tooltip("The [Intro] label of the splash screen. Above Conestoga's logo")]
        public string IntroText = "Developed by";

        [TextArea(1, 5), Tooltip("The [Organization] label of the splash screen. To the right of Conestoga's logo")]
        public string OrganizationText = "CENTRE FOR VIRTUAL\nREALITY INNOVATION";

        [TextArea(1, 10), Tooltip("The [Message] label of the splash screen. All messages that will be displayed during scene setup")]
        public List<string> Messages;
    }
}