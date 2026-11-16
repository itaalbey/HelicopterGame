using UnityEngine;

namespace HelicopterGame
{
    public sealed class MainRotor : MonoBehaviour
    {
        [SerializeField] private HelicopterConfig _config;
        [SerializeField] private Rigidbody _body;
        [SerializeField] private HelicopterEngine _engine;
        [SerializeField] private GroundSensor _groundSensor;

        private Vector3 _localHubPosition;
        private float _collectivePitch;
        private Vector2 _cyclicTorque;

        public float CollectivePitch => _collectivePitch;
        public float RotorSpeed => _engine.RotorSpeed;
        public float Thrust { get; private set; }
        public float ReactionTorque { get; private set; }
        public float GroundEffect { get; private set; } = 1f;

        // Thrust and control power grow with the square of the rotor speed.
        public float Power => _engine.RotorSpeed * _engine.RotorSpeed;
        public float AvailableThrust => _config.MaxThrust * Power * GroundEffect;

        private void Awake()
        {
            _localHubPosition = _body.transform.InverseTransformPoint(transform.position);
        }

        public void SetControls(float collectivePitch, Vector2 cyclicTorque)
        {
            _collectivePitch = Mathf.Clamp01(collectivePitch);
            _cyclicTorque = cyclicTorque;
        }

        private void FixedUpdate()
        {
            Vector3 hub = _body.position + _body.rotation * _localHubPosition;
            Vector3 up = _body.rotation * Vector3.up;

            GroundEffect = CalculateGroundEffect(hub.y - _groundSensor.Position.y + _groundSensor.Altitude);
            Thrust = _collectivePitch * AvailableThrust;
            _body.AddForceAtPosition(up * Thrust, hub);

            // The blades turn counter-clockwise, so the fuselage is pushed clockwise.
            ReactionTorque = Thrust * _config.ReactionTorquePerThrust;
            _body.AddTorque(up * ReactionTorque);

            Vector2 limit = _config.MaxCyclicTorque * Power;
            float pitchTorque = Mathf.Clamp(_cyclicTorque.x, -limit.x, limit.x);
            float rollTorque = Mathf.Clamp(_cyclicTorque.y, -limit.y, limit.y);
            _body.AddRelativeTorque(new Vector3(pitchTorque, 0f, rollTorque));
        }

        // Near the ground thrust grows: T / T_free = 1 / (1 - (R / 4h)^2)
        private float CalculateGroundEffect(float rotorHeight)
        {
            float ratio = _config.RotorRadius / (4f * Mathf.Max(rotorHeight, 0.01f));
            if (ratio >= 1f)
            {
                return _config.MaxGroundEffect;
            }

            return Mathf.Min(1f / (1f - ratio * ratio), _config.MaxGroundEffect);
        }
    }
}
