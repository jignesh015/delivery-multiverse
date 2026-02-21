using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace DeliveryMultiverse
{
    [RequireComponent(typeof(CanvasGroup))]
    public class PauseGameUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform popUpTransform;
        
        [Space(20)]
        [SerializeField] private Button resignButton;
        [SerializeField] private Button resumeButton;
        [SerializeField] private InputActionReference pauseToggleAction;
        
        private CanvasGroup m_CanvasGroup;
        
        private void Awake()
        {
            TryGetComponent(out m_CanvasGroup);
            ToggleCanvasGroup(false);
            resignButton.onClick.AddListener(OnResignButtonPressed);
            resumeButton.onClick.AddListener(OnResumeButtonPressed);
            
            pauseToggleAction.action.Enable();
            pauseToggleAction.action.performed += OnPauseTogglePerformed;
        }
        
        private void OnDestroy()
        {
            if (resignButton)
                resignButton.onClick.RemoveListener(OnResignButtonPressed);
            if (resumeButton)                
                resumeButton.onClick.RemoveListener(OnResumeButtonPressed);
            
            pauseToggleAction.action.performed -= OnPauseTogglePerformed;
            pauseToggleAction.action.Disable();
        }

        private void OnPauseTogglePerformed(InputAction.CallbackContext obj)
        {
            if(!GameStatic.IsDayActive) return;
            
            if(m_CanvasGroup.interactable) return;
            
            ToggleCanvasGroup(true);
            TogglePauseState(true);
            
            popUpTransform.localScale = Vector3.zero;
            popUpTransform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).SetUpdate(true);
        }


        private void ToggleCanvasGroup(bool isVisible)
        {
            m_CanvasGroup.DOFade(isVisible ? 1f : 0f, 0.5f).SetUpdate(true);
            m_CanvasGroup.interactable = isVisible;
            m_CanvasGroup.blocksRaycasts = isVisible;
        }
        
        private void OnResignButtonPressed()
        {
            TogglePauseState(false);
            m_CanvasGroup.interactable = false;
            popUpTransform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
            {
                ToggleCanvasGroup(false);
                GameStatic.OnResignButtonPressed?.Invoke();
            });
        }

        private void OnResumeButtonPressed()
        {
            TogglePauseState(false);
            m_CanvasGroup.interactable = false;
            popUpTransform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
            {
                ToggleCanvasGroup(false);
            });
        }
        
        private void TogglePauseState(bool isPaused)
        {
            Time.timeScale = isPaused ? 0f : 1f;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
        }
    }
}