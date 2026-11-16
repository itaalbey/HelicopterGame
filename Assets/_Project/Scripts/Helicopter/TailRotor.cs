using UnityEngine;

namespace HelicopterGame
{
    public sealed class TailRotor : MonoBehaviour
    {
        [SerializeField] private HelicopterConfig _config;
        [SerializeField] private Rigidbody _body;
        [SerializeField] private HelicopterEngine _engine;

        private float _yawTorque;
        private Vector3 _localPosition;

        public float Thrust { get; private set; }
        public Vector3 AppliedTorque { get; private set; }

        private void Awake()
        {
            _localPosition = _body.transform.InverseTransformPoint(transform.position);
        }

        public void SetYawTorque(float torque)
        {
            _yawTorque = torque;
        }

        private void FixedUpdate()
        {
            Vector3 position = _body.position + _body.rotation * _localPosition;
            Vector3 direction = _body.rotation * Vector3.right;
            Vector3 leverArm = position - _body.worldCenterOfMass;

            float yawTorquePerNewton = Vector3.Dot(Vector3.Cross(leverArm, direction), _body.rotation * Vector3.up);
            float maxThrust = _config.MaxTailThrust * _engine.RotorSpeed * _engine.RotorSpeed;
            Thrust = Mathf.Clamp(_yawTorque / yawTorquePerNewton, -maxThrust, maxThrust);

            Vector3 force = direction * Thrust;
            AppliedTorque = Vector3.Cross(leverArm, force);
            _body.AddForceAtPosition(force, position);
        }
    }
}
