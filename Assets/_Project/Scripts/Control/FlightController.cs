using UnityEngine;

namespace HelicopterGame
{
    // Flight assist: turns pilot input into rotor commands. The body is moved only by rotor forces.
    [DefaultExecutionOrder(-10)]
    public sealed class FlightController : MonoBehaviour
    {
        private const float _verticalSpeedIntegralLimit = 3f;
        private const float _yawRateIntegralLimit = 1f;

        [SerializeField] private HelicopterConfig _config;
        [SerializeField] private HelicopterInput _input;
        [SerializeField] private Rigidbody _body;
        [SerializeField] private GroundSensor _groundSensor;
        [SerializeField] private MainRotor _mainRotor;
        [SerializeField] private TailRotor _tailRotor;

        private PiController _verticalSpeedController;
        private PiController _yawRateController;
        private float _collectivePitch;
        private float _holdAltitude;
        private Vector2 _targetTilt;

        public bool IsLanded { get; private set; }
        public bool IsHoldingAltitude { get; private set; }
        public Rigidbody Body => _body;
        public GroundSensor GroundSensor => _groundSensor;
        public MainRotor MainRotor => _mainRotor;
        public TailRotor TailRotor => _tailRotor;

        private float Weight => _body.mass * Physics.gravity.magnitude;

        private void Awake()
        {
            _verticalSpeedController = new PiController(_config.VerticalSpeedP, _config.VerticalSpeedI, _verticalSpeedIntegralLimit);
            _yawRateController = new PiController(_config.YawRateP, _config.YawRateI, _yawRateIntegralLimit);
        }

        public void ResetState()
        {
            IsLanded = false;
            IsHoldingAltitude = false;
            _collectivePitch = 0f;
            _targetTilt = Vector2.zero;
            _verticalSpeedController.Reset();
            _yawRateController.Reset();
        }

        private void FixedUpdate()
        {
            float deltaTime = Time.fixedDeltaTime;
            float collectiveInput = ApplyDeadZone(_input.CollectivePitch);
            IsLanded = _groundSensor.IsGrounded && collectiveInput <= 0f;

            UpdateCollectivePitch(collectiveInput, deltaTime);

            Vector3 angularAcceleration = Vector3.zero;
            if (IsLanded)
            {
                _targetTilt = Vector2.zero;
                _yawRateController.Reset();
            }
            else
            {
                Vector2 tilt = CalculateTiltAcceleration(deltaTime);
                angularAcceleration = new Vector3(tilt.x, CalculateYawAcceleration(deltaTime), tilt.y);
            }

            // torque = inertia * angular acceleration
            Vector3 torque = Vector3.Scale(_body.inertiaTensor, angularAcceleration);

            // Cyclic also cancels the roll moment of the tail rotor, the tail rotor cancels the main rotor reaction torque.
            Vector3 tailRotorTorque = Quaternion.Inverse(_body.rotation) * _tailRotor.AppliedTorque;
            _mainRotor.SetControls(_collectivePitch, new Vector2(torque.x - tailRotorTorque.x, torque.z - tailRotorTorque.z));
            _tailRotor.SetYawTorque(torque.y - _mainRotor.ReactionTorque);
        }

        private void UpdateCollectivePitch(float input, float deltaTime)
        {
            float target = 0f;

            if (IsLanded)
            {
                IsHoldingAltitude = false;
                _verticalSpeedController.Reset();
            }
            else
            {
                float verticalSpeed = _body.linearVelocity.y;
                float error = CalculateTargetVerticalSpeed(input, verticalSpeed) - verticalSpeed;
                target = CalculateHoverCollectivePitch() + _verticalSpeedController.Update(error, deltaTime);
            }

            _collectivePitch = Mathf.MoveTowards(_collectivePitch, Mathf.Clamp01(target), _config.CollectivePitchRate * deltaTime);
        }

        private float CalculateTargetVerticalSpeed(float input, float verticalSpeed)
        {
            // Descent slows down near the ground for a soft touchdown.
            float descentLimit = Mathf.Min(_config.MaxDescentRate, _config.TouchdownSpeed + _groundSensor.Altitude * _config.DescentAltitudeGain);

            if (input != 0f)
            {
                IsHoldingAltitude = false;
                return input > 0f ? input * _config.MaxClimbRate : input * descentLimit;
            }

            if (!IsHoldingAltitude)
            {
                // Stop first, then remember the altitude, otherwise it overshoots.
                if (Mathf.Abs(verticalSpeed) > _config.AltitudeCaptureSpeed)
                {
                    return 0f;
                }

                IsHoldingAltitude = true;
                _holdAltitude = _body.position.y;
            }

            float altitudeError = _holdAltitude - _body.position.y;
            return Mathf.Clamp(altitudeError * _config.AltitudeHoldGain, -descentLimit, _config.MaxClimbRate);
        }

        // Collective pitch at which the vertical part of the thrust equals the weight.
        private float CalculateHoverCollectivePitch()
        {
            float availableThrust = _mainRotor.AvailableThrust;
            if (availableThrust <= 0.01f)
            {
                return 1f;
            }

            float verticalPart = Mathf.Max(Vector3.Dot(_body.rotation * Vector3.up, Vector3.up), 0.5f);
            return Mathf.Clamp01(Weight / verticalPart / availableThrust);
        }

        // Returns angular acceleration for pitch (x) and roll (y), rad/s^2.
        private Vector2 CalculateTiltAcceleration(float deltaTime)
        {
            float heading = _body.rotation.eulerAngles.y;
            Vector3 velocity = Quaternion.Inverse(Quaternion.Euler(0f, heading, 0f)) * _body.linearVelocity;
            Vector2 input = new Vector2(ApplyDeadZone(_input.Cyclic.x), ApplyDeadZone(_input.Cyclic.y));

            // Stick sets the tilt angle; released stick tilts against the drift and stops the helicopter.
            float pitch = input.y != 0f ? input.y * _config.MaxTiltAngle : CalculateHoverHoldAngle(velocity.z);
            float bank = input.x != 0f ? input.x * _config.MaxTiltAngle : CalculateHoverHoldAngle(velocity.x);

            // Tail rotor pushes the helicopter sideways, a small opposite bank cancels it.
            bank -= Mathf.Atan2(_tailRotor.Thrust, Weight) * Mathf.Rad2Deg;

            _targetTilt = Vector2.MoveTowards(_targetTilt, new Vector2(pitch, bank), _config.TiltRate * deltaTime);

            Quaternion targetRotation = Quaternion.Euler(0f, heading, 0f) * Quaternion.Euler(_targetTilt.x, 0f, -_targetTilt.y);
            Vector3 error = CalculateTiltError(_body.rotation * Vector3.up, targetRotation * Vector3.up);
            Vector3 angularVelocity = Quaternion.Inverse(_body.rotation) * _body.angularVelocity;

            // Spring-damper: acceleration = w^2 * error - 2 * damping * w * angular velocity
            float frequency = _config.TiltFrequency;
            float damping = 2f * _config.TiltDampingRatio * frequency;
            float stiffness = frequency * frequency;

            return new Vector2(
                stiffness * error.x - damping * angularVelocity.x,
                stiffness * error.z - damping * angularVelocity.z);
        }

        private float CalculateHoverHoldAngle(float drift)
        {
            return Mathf.Clamp(-drift * _config.HoverHoldGain, -_config.MaxHoverHoldAngle, _config.MaxHoverHoldAngle);
        }

        // Local rotation (axis * angle in radians) that turns the current up vector into the target one.
        private Vector3 CalculateTiltError(Vector3 currentUp, Vector3 targetUp)
        {
            Vector3 axis = Vector3.Cross(currentUp, targetUp).normalized;
            float angle = Vector3.Angle(currentUp, targetUp) * Mathf.Deg2Rad;
            return Quaternion.Inverse(_body.rotation) * (axis * angle);
        }

        private float CalculateYawAcceleration(float deltaTime)
        {
            float targetRate = ApplyDeadZone(_input.Pedals) * _config.MaxYawRate * Mathf.Deg2Rad;
            float currentRate = (Quaternion.Inverse(_body.rotation) * _body.angularVelocity).y;
            return _yawRateController.Update(targetRate - currentRate, deltaTime);
        }

        private float ApplyDeadZone(float value)
        {
            return Mathf.Abs(value) > _config.InputDeadZone ? value : 0f;
        }
    }
}
