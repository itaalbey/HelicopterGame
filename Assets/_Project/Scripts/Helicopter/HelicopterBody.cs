using UnityEngine;

namespace HelicopterGame
{
    [RequireComponent(typeof(Rigidbody))]
    public sealed class HelicopterBody : MonoBehaviour
    {
        [SerializeField] private HelicopterConfig _config;

        private Rigidbody _body;

        private void Awake()
        {
            _body = GetComponent<Rigidbody>();
            _body.mass = _config.Mass;
            _body.centerOfMass = _config.CenterOfMass;
            _body.inertiaTensor = _config.InertiaTensor;
            _body.inertiaTensorRotation = Quaternion.identity;
            _body.linearDamping = 0f;
            _body.angularDamping = 0f;
            _body.interpolation = RigidbodyInterpolation.Interpolate;
            _body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }

        private void FixedUpdate()
        {
            ApplyDrag();
            ApplyAngularDamping();
        }

        // Quadratic drag for every local axis: F = -0.5 * rho * CdA * v * |v|
        private void ApplyDrag()
        {
            Vector3 velocity = Quaternion.Inverse(_body.rotation) * _body.linearVelocity;
            Vector3 speed = new Vector3(Mathf.Abs(velocity.x), Mathf.Abs(velocity.y), Mathf.Abs(velocity.z));
            Vector3 drag = -0.5f * _config.AirDensity * Vector3.Scale(_config.DragAreas, Vector3.Scale(velocity, speed));

            Vector3 pressurePoint = _body.position + _body.rotation * _config.CenterOfPressure;
            _body.AddForceAtPosition(_body.rotation * drag, pressurePoint);
        }

        private void ApplyAngularDamping()
        {
            Vector3 angularVelocity = Quaternion.Inverse(_body.rotation) * _body.angularVelocity;
            _body.AddRelativeTorque(-Vector3.Scale(_config.AngularDamping, angularVelocity));
        }
    }
}
