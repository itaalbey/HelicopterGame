using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace HelicopterGame
{
    public sealed class HelicopterHud : MonoBehaviour
    {
        private const float _refreshInterval = 0.1f;
        private const float _metersPerSecondToKilometersPerHour = 3.6f;

        private const string _format =
            "<b>Status:</b>  {0}\n" +
            "<b>Altitude:</b>  {1:0.0} m\n" +
            "<b>Vertical speed:</b>  {2:+0.0;-0.0;0.0} m/s\n" +
            "<b>Airspeed:</b>  {3:0} km/h\n" +
            "<b>Heading:</b>  {4:000}°\n" +
            "<b>Pitch / Bank:</b>  {5:+0;-0;0}° / {6:+0;-0;0}°\n" +
            "<b>Rotor RPM:</b>  {7:0} %\n" +
            "<b>Collective pitch:</b>  {8:0} %\n" +
            "<b>Thrust / Weight:</b>  {9:0.00}\n" +
            "<b>Ground effect:</b>  x{10:0.00}\n" +
            "<b>Tail rotor:</b>  {11:+0;-0;0} N";

        [SerializeField] private FlightController _flightController;
        [SerializeField] private Text _label;

        private float _nextRefreshTime;

        private void Update()
        {
            if (Time.unscaledTime < _nextRefreshTime)
            {
                return;
            }

            _nextRefreshTime = Time.unscaledTime + _refreshInterval;
            _label.text = BuildText();
        }

        private string BuildText()
        {
            Rigidbody body = _flightController.Body;
            MainRotor mainRotor = _flightController.MainRotor;
            Vector3 forward = body.rotation * Vector3.forward;
            Vector3 right = body.rotation * Vector3.right;

            return string.Format(CultureInfo.InvariantCulture, _format,
                GetStatus(),
                _flightController.GroundSensor.Altitude,
                body.linearVelocity.y,
                body.linearVelocity.magnitude * _metersPerSecondToKilometersPerHour,
                body.rotation.eulerAngles.y,
                Mathf.Asin(forward.y) * Mathf.Rad2Deg,
                -Mathf.Asin(right.y) * Mathf.Rad2Deg,
                mainRotor.RotorSpeed * 100f,
                mainRotor.CollectivePitch * 100f,
                mainRotor.Thrust / (body.mass * Physics.gravity.magnitude),
                mainRotor.GroundEffect,
                _flightController.TailRotor.Thrust);
        }

        private string GetStatus()
        {
            if (_flightController.IsLanded)
            {
                return "LANDED";
            }

            return _flightController.IsHoldingAltitude ? "HOVER / ALT HOLD" : "FLYING";
        }
    }
}
