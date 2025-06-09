using UnityEngine;

namespace VARLab.Velcro.Demos
{
    public class DisableObject : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                gameObject.SetActive(false);
            }
        }
    }
}