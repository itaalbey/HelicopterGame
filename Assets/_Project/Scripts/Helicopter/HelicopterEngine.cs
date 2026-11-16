using UnityEngine;

namespace HelicopterGame
{
    [DefaultExecutionOrder(-30)]
    public sealed class HelicopterEngine : MonoBehaviour
    {
        [SerializeField] private HelicopterConfig _config;

        public float RotorSpeed { get; private set; }

        private void FixedUpdate()
        {
            RotorSpeed = Mathf.MoveTowards(RotorSpeed, 1f, Time.fixedDeltaTime / _config.SpoolUpTime);
        }
    }
}
