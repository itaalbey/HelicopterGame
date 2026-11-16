using UnityEngine;

namespace HelicopterGame
{
    [CreateAssetMenu(menuName = "Helicopter/Helicopter Config", fileName = "HelicopterConfig")]
    public sealed class HelicopterConfig : ScriptableObject
    {
        [Header("Airframe")]
        [SerializeField] private float _mass = 5500f;
        [SerializeField] private Vector3 _centerOfMass = new Vector3(0f, 1.7f, 0f);
        [Tooltip("Moments of inertia around X (pitch), Y (yaw), Z (roll), kg*m^2")]
        [SerializeField] private Vector3 _inertiaTensor = new Vector3(21500f, 21000f, 4800f);

        [Header("Aerodynamics")]
        [SerializeField] private float _airDensity = 1.225f;
        [Tooltip("Drag coeff multiplied by reference area for X (side), Y (vertical), Z (front)")]
        [SerializeField] private Vector3 _dragAreas = new Vector3(25f, 40f, 8f);
        [SerializeField] private Vector3 _centerOfPressure = new Vector3(0f, 1.7f, -1.2f);
        [SerializeField] private Vector3 _angularDamping = new Vector3(13000f, 12600f, 3700f);

        [Header("Engine")]
        [Tooltip("Time while rotor turn on")]
        [SerializeField] private float _spoolUpTime = 4f;

        [Header("Main rotor")]
        [SerializeField] private float _maxThrust = 89000f;
        [SerializeField] private float _rotorRadius = 6.55f;
        [SerializeField] private float _maxGroundEffect = 1.3f;
        [Tooltip("Reaction torque on the fuselage per Newton of thrust")]
        [SerializeField] private float _reactionTorquePerThrust = 0.8f;
        [Tooltip("Max cyclic torque for pitch (X) and roll (Y)")]
        [SerializeField] private Vector2 _maxCyclicTorque = new Vector2(150000f, 40000f);

        [Header("Tail rotor")]
        [SerializeField] private float _maxTailThrust = 12000f;

        [Header("Flight assist")]
        [SerializeField] private float _inputDeadZone = 0.05f;
        [SerializeField] private float _maxClimbRate = 8f;
        [SerializeField] private float _maxDescentRate = 6f;
        [SerializeField] private float _touchdownSpeed = 0.6f;
        [SerializeField] private float _descentAltitudeGain = 0.8f;
        [SerializeField] private float _altitudeHoldGain = 1.2f;
        [SerializeField] private float _altitudeCaptureSpeed = 0.5f;
        [SerializeField] private float _collectivePitchRate = 1.5f;
        [SerializeField] private float _verticalSpeedP = 0.12f;
        [SerializeField] private float _verticalSpeedI = 0.08f;
        [SerializeField] private float _maxTiltAngle = 25f;
        [SerializeField] private float _tiltRate = 60f;
        [SerializeField] private float _tiltFrequency = 4f;
        [SerializeField] private float _tiltDampingRatio = 0.9f;
        [SerializeField] private float _hoverHoldGain = 2.5f;
        [SerializeField] private float _maxHoverHoldAngle = 18f;
        [SerializeField] private float _maxYawRate = 23.55f;
        [SerializeField] private float _yawRateP = 3f;
        [SerializeField] private float _yawRateI = 1.5f;

        public float Mass => _mass;
        public Vector3 CenterOfMass => _centerOfMass;
        public Vector3 InertiaTensor => _inertiaTensor;

        public float AirDensity => _airDensity;
        public Vector3 DragAreas => _dragAreas;
        public Vector3 CenterOfPressure => _centerOfPressure;
        public Vector3 AngularDamping => _angularDamping;

        public float SpoolUpTime => _spoolUpTime;

        public float MaxThrust => _maxThrust;
        public float RotorRadius => _rotorRadius;
        public float MaxGroundEffect => _maxGroundEffect;
        public float ReactionTorquePerThrust => _reactionTorquePerThrust;
        public Vector2 MaxCyclicTorque => _maxCyclicTorque;

        public float MaxTailThrust => _maxTailThrust;

        public float InputDeadZone => _inputDeadZone;
        public float MaxClimbRate => _maxClimbRate;
        public float MaxDescentRate => _maxDescentRate;
        public float TouchdownSpeed => _touchdownSpeed;
        public float DescentAltitudeGain => _descentAltitudeGain;
        public float AltitudeHoldGain => _altitudeHoldGain;
        public float AltitudeCaptureSpeed => _altitudeCaptureSpeed;
        public float CollectivePitchRate => _collectivePitchRate;
        public float VerticalSpeedP => _verticalSpeedP;
        public float VerticalSpeedI => _verticalSpeedI;
        public float MaxTiltAngle => _maxTiltAngle;
        public float TiltRate => _tiltRate;
        public float TiltFrequency => _tiltFrequency;
        public float TiltDampingRatio => _tiltDampingRatio;
        public float HoverHoldGain => _hoverHoldGain;
        public float MaxHoverHoldAngle => _maxHoverHoldAngle;
        public float MaxYawRate => _maxYawRate;
        public float YawRateP => _yawRateP;
        public float YawRateI => _yawRateI;
    }
}
