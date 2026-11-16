using UnityEngine;

namespace HelicopterGame
{
    public sealed class HelicopterRespawner : MonoBehaviour
    {
        [SerializeField] private HelicopterInput _input;
        [SerializeField] private FlightController _flightController;
        [SerializeField] private Rigidbody _body;
        [SerializeField] private Transform _spawnPoint;

        private void OnEnable()
        {
            _input.ResetRequested += Respawn;
        }

        private void OnDisable()
        {
            _input.ResetRequested -= Respawn;
        }

        private void Respawn()
        {
            _body.linearVelocity = Vector3.zero;
            _body.angularVelocity = Vector3.zero;
            _body.position = _spawnPoint.position;
            _body.rotation = _spawnPoint.rotation;
            _body.transform.SetPositionAndRotation(_spawnPoint.position, _spawnPoint.rotation);
            _flightController.ResetState();
        }
    }
}
