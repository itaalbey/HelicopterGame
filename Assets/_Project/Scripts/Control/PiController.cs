using UnityEngine;

namespace HelicopterGame
{
    public sealed class PiController
    {
        private readonly float _proportionalGain;
        private readonly float _integralGain;
        private readonly float _integralLimit;

        private float _integral;

        public PiController(float proportionalGain, float integralGain, float integralLimit)
        {
            _proportionalGain = proportionalGain;
            _integralGain = integralGain;
            _integralLimit = integralLimit;
        }

        public float Update(float error, float deltaTime)
        {
            _integral = Mathf.Clamp(_integral + error * deltaTime, -_integralLimit, _integralLimit);
            return _proportionalGain * error + _integralGain * _integral;
        }

        public void Reset()
        {
            _integral = 0f;
        }
    }
}
