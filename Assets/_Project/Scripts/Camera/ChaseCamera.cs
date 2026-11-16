using UnityEngine;

namespace HelicopterGame
{
    // Follows the helicopter heading but ignores pitch and roll, so the tilt stays visible.
    public sealed class ChaseCamera : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private Vector3 _offset = new Vector3(0f, 6.5f, -22f);
        [SerializeField] private Vector3 _lookOffset = new Vector3(0f, 2.5f, 4f);
        [SerializeField] private float _positionSmoothTime = 0.2f;
        [SerializeField] private float _headingSharpness = 3f;
        [SerializeField] private float _rotationSharpness = 10f;
        [SerializeField] private float _minHeight = 0.5f;

        private Vector3 _velocity;
        private float _heading;

        private void Start()
        {
            _heading = _target.eulerAngles.y;
        }

        private void LateUpdate()
        {
            float deltaTime = Time.deltaTime;
            _heading = Mathf.LerpAngle(_heading, _target.eulerAngles.y, 1f - Mathf.Exp(-_headingSharpness * deltaTime));
            Quaternion headingRotation = Quaternion.Euler(0f, _heading, 0f);

            Vector3 position = Vector3.SmoothDamp(transform.position, _target.position + headingRotation * _offset, ref _velocity, _positionSmoothTime);
            position.y = Mathf.Max(position.y, _minHeight);
            transform.position = position;

            Quaternion lookRotation = Quaternion.LookRotation(_target.position + headingRotation * _lookOffset - position);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 1f - Mathf.Exp(-_rotationSharpness * deltaTime));
        }
    }
}
