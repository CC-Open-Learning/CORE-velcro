using UnityEngine;
using UnityEngine.Events;

namespace VARLab.Velcro.Demos
{
    public class SplashScreenDemoController : MonoBehaviour
    {
        public UnityEvent ProgressSplashScreen;

        void Start()
        {
            ProgressSplashScreen ??= new UnityEvent();

            ProgressSplashScreen?.Invoke();
        }
    }
}