using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DeliveryMultiverse
{
    public class GameScreenInputInstructions : MonoBehaviour
    {
        [SerializeField] private InputActionReference toggleInstructionsAction;
        [SerializeField] private CanvasGroup instructionsPanel;
        [SerializeField] private CanvasGroup showInstructionsPanel;

        private bool m_IsInstructionsVisible;
        
        private void Awake()
        {
            m_IsInstructionsVisible = true;
            instructionsPanel.alpha = 1f;
            showInstructionsPanel.alpha = 0f;
            Invoke(nameof(ToggleInstructions), 5f);
        }

        private void OnEnable()
        {
            toggleInstructionsAction.action.Enable();
            toggleInstructionsAction.action.performed += OnToggleInstructionsPerformed;
        }

        private void OnDisable()
        {
            toggleInstructionsAction.action.performed -= OnToggleInstructionsPerformed;
            toggleInstructionsAction.action.Disable();
        }

        private void OnToggleInstructionsPerformed(InputAction.CallbackContext obj)
        {
            ToggleInstructions();
        }

        private void ToggleInstructions()
        {
            m_IsInstructionsVisible = !m_IsInstructionsVisible;

            instructionsPanel.DOKill();
            showInstructionsPanel.DOKill();
            
            instructionsPanel.alpha = 0;
            showInstructionsPanel.alpha = 0;

            if (m_IsInstructionsVisible)
                instructionsPanel.DOFade(1f, 0.5f);
            else
                showInstructionsPanel.DOFade(1f, 0.5f);
        }
    }
}