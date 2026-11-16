using UnityEngine;

namespace HelicopterGame
{
    public sealed class RotorSpinner : MonoBehaviour
    {
        [SerializeField] private HelicopterEngine _engine;
        [SerializeField] private Vector3 _localAxis = Vector3.up;
        [SerializeField] private float _maxAngularSpeed = 1440f;

        private void Update()
        {
            transform.Rotate(_localAxis, _maxAngularSpeed * _engine.RotorSpeed * Time.deltaTime, Space.Self);
        }
    }
}
