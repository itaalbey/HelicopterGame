using UnityEngine;

namespace HelicopterGame
{
    [DefaultExecutionOrder(-30)]
    public sealed class HelicopterEngine : MonoBehaviour
    {
        [SerializeField] private HelicopterConfig _config;

        // 0 - rotor stopped, 1 - nominal RPM
        public float RotorSpeed { get; private set; }

        private void FixedUpdate()
        {
            RotorSpeed = Mathf.MoveTowards(RotorSpeed, 1f, Time.fixedDeltaTime / _config.SpoolUpTime);
        }
    }
}
