using UnityEngine;
using UnityEngine.InputSystem;

namespace DeliveryMultiverse
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] private InputActionAsset actions;
        [SerializeField] private string actionMapName = "Player";
        [SerializeField] private string accelerateActionName = "Accelerate";
        [SerializeField] private string brakeActionName = "Brake";
        [SerializeField] private string steerActionName = "Steer";
        [SerializeField] private string jumpActionName = "Jump";
        [SerializeField] private string interactActionName = "Interact";

        private InputAction m_AccelerateAction;
        private InputAction m_BrakeAction;
        private InputAction m_SteerAction;
        private InputAction m_JumpAction;
        private InputAction m_InteractAction;

        private float m_AccelerateInput;
        private float m_BrakeInput;
        private float m_SteerInput;
        private bool m_JumpQueued;
        private bool m_InteractQueued;
        private bool m_LoggedMissing;

        public float AccelerateInput => m_AccelerateInput;
        public float BrakeInput => m_BrakeInput;
        public float SteerInput => m_SteerInput;

        public bool ConsumeJumpPressed()
        {
            // Disable jump for now
            return false;

            if (!m_JumpQueued)
            {
                return false;
            }

            m_JumpQueued = false;
            return true;
        }
        
        public bool ConsumeInteractPressed()
        {
            if (!m_InteractQueued)
            {
                return false;
            }

            m_InteractQueued = false;
            return true;
        }

        private void OnEnable()
        {
            BindActions();
        }

        private void OnDisable()
        {
            m_AccelerateAction?.Disable();
            m_BrakeAction?.Disable();
            m_SteerAction?.Disable();
            m_JumpAction?.Disable();
            m_InteractAction?.Disable();
        }

        private void Update()
        {
            if (m_AccelerateAction != null)
            {
                m_AccelerateInput = m_AccelerateAction.ReadValue<float>();
            }

            if (m_BrakeAction != null)
            {
                m_BrakeInput = m_BrakeAction.ReadValue<float>();
            }

            if (m_SteerAction != null)
            {
                m_SteerInput = m_SteerAction.ReadValue<float>();
            }

            if (m_JumpAction is { triggered: true })
            {
                m_JumpQueued = true;
            }
            
            if (m_InteractAction is { triggered: true })
            {
                m_InteractQueued = true;
            }
        }

        private void BindActions()
        {
            if (!actions)
            {
                LogMissing("InputManager is missing an InputActionAsset reference.");
                return;
            }

            var map = actions.FindActionMap(actionMapName, true);
            if (map == null)
            {
                LogMissing("InputManager could not find action map: " + actionMapName);
                return;
            }

            m_AccelerateAction = map.FindAction(accelerateActionName, true);
            m_BrakeAction = map.FindAction(brakeActionName, true);
            m_SteerAction = map.FindAction(steerActionName, true);
            m_JumpAction = map.FindAction(jumpActionName, true);
            m_InteractAction = map.FindAction(interactActionName, true);

            if (m_AccelerateAction == null || m_BrakeAction == null || m_SteerAction == null)
            {
                LogMissing("InputManager could not find Accelerate, Brake, or Steer actions.");
                return;
            }

            m_AccelerateAction.Enable();
            m_BrakeAction.Enable();
            m_SteerAction.Enable();

            m_JumpAction?.Enable();
            m_InteractAction?.Enable();
        }

        private void LogMissing(string message)
        {
            if (m_LoggedMissing)
            {
                return;
            }

            m_LoggedMissing = true;
            Debug.LogWarning(message, this);
        }
    }
}
