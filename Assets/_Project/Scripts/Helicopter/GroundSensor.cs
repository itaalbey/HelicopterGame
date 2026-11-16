using UnityEngine;

namespace HelicopterGame
{
    [DefaultExecutionOrder(-20)]
    public sealed class GroundSensor : MonoBehaviour
    {
        [SerializeField] private Rigidbody _body;
        [SerializeField] private LayerMask _groundLayers = ~0;
        [SerializeField] private float _rayStartHeight = 0.5f;
        [SerializeField] private float _maxDistance = 500f;
        [SerializeField] private float _groundedThreshold = 0.15f;

        private readonly RaycastHit[] _hits = new RaycastHit[8];
        private Vector3 _localPosition;

        public float Altitude { get; private set; }
        public bool IsGrounded => Altitude <= _groundedThreshold;
        public Vector3 Position => _body.position + _body.rotation * _localPosition;

        private void Awake()
        {
            _localPosition = _body.transform.InverseTransformPoint(transform.position);
            Altitude = _maxDistance;
        }

        private void FixedUpdate()
        {
            Vector3 origin = Position + Vector3.up * _rayStartHeight;
            int hitCount = Physics.RaycastNonAlloc(origin, Vector3.down, _hits, _maxDistance, _groundLayers, QueryTriggerInteraction.Ignore);

            float closest = _maxDistance + _rayStartHeight;
            for (int i = 0; i < hitCount; i++)
            {
                if (_hits[i].collider.attachedRigidbody != _body)
                {
                    closest = Mathf.Min(closest, _hits[i].distance);
                }
            }

            Altitude = Mathf.Max(0f, closest - _rayStartHeight);
        }
    }
}
