using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Tools
{
    public class RotatingItem : MonoBehaviour
    {
        [SerializeField] private Vector3 angles;

        private void Update()
        {
            transform.Rotate(angles * Time.unscaledDeltaTime);
        }
    }
}