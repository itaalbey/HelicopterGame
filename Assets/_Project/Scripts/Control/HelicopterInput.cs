using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HelicopterGame
{
    public sealed class HelicopterInput : MonoBehaviour
    {
        [SerializeField] private InputActionReference _cyclicAction;
        [SerializeField] private InputActionReference _collectiveAction;
        [SerializeField] private InputActionReference _pedalsAction;
        [SerializeField] private InputActionReference _resetAction;

        public event Action ResetRequested;

        public Vector2 Cyclic => _cyclicAction.action.ReadValue<Vector2>();
        public float CollectivePitch => _collectiveAction.action.ReadValue<float>();
        public float Pedals => _pedalsAction.action.ReadValue<float>();

        private void OnEnable()
        {
            _cyclicAction.action.Enable();
            _collectiveAction.action.Enable();
            _pedalsAction.action.Enable();
            _resetAction.action.Enable();
            _resetAction.action.performed += OnResetPerformed;
        }

        private void OnDisable()
        {
            _resetAction.action.performed -= OnResetPerformed;
            _cyclicAction.action.Disable();
            _collectiveAction.action.Disable();
            _pedalsAction.action.Disable();
            _resetAction.action.Disable();
        }

        private void OnResetPerformed(InputAction.CallbackContext context)
        {
            ResetRequested?.Invoke();
        }
    }
}
