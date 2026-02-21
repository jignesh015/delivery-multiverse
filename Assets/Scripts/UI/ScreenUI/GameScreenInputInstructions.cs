using System;
using System.Collections;
using System.Collections.Generic;
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
            m_IsInstructionsVisible = false;
            instructionsPanel.alpha = 0f;
            showInstructionsPanel.alpha = 0f;
            
            GameStatic.OnNewDayStarted += OnNewDayStarted;
        }

        private void OnDestroy()
        {
            GameStatic.OnNewDayStarted -= OnNewDayStarted;
        }

        private void OnNewDayStarted()
        {
            ToggleInstructions();
            StartCoroutine(AutoHideInstructions());
        }
        
        private IEnumerator AutoHideInstructions()
        {
            yield return new WaitForSeconds(10f);
            if (m_IsInstructionsVisible)
                ToggleInstructions();
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