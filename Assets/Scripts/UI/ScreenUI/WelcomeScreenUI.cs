using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace DeliveryMultiverse
{
    [RequireComponent(typeof(CanvasGroup))]
    public class WelcomeScreenUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform popUpTransform;
        [SerializeField] private float popupDelay = 1f;
        [SerializeField] private CanvasGroup screen1;
        [SerializeField] private CanvasGroup screen2;
        
        [Space(20)]
        [SerializeField] private Button continueButton;
        [SerializeField] private Button beginButton;
        
        [Space(20)]
        [SerializeField] private AudioSource popupSfx;
        
        private CanvasGroup m_CanvasGroup;
        
        private void Awake()
        {
            TryGetComponent(out m_CanvasGroup);
            ToggleCanvasGroup(m_CanvasGroup,false);
            
            continueButton.onClick.AddListener(OnContinueButtonPressed);
            beginButton.onClick.AddListener(OnBeginButtonPressed);

            GameStatic.OnWelcomeScreenRequested += OnWelcomeScreenRequested;
        }
        
        private void OnDestroy()
        {
            if (continueButton)                
                continueButton.onClick.RemoveListener(OnContinueButtonPressed);
            if (beginButton)
                beginButton.onClick.RemoveListener(OnBeginButtonPressed);
            
            GameStatic.OnWelcomeScreenRequested -= OnWelcomeScreenRequested;
        }

        private void OnWelcomeScreenRequested()
        {
            StartCoroutine(ShowPopupAfterDelay());
        }
        
        private IEnumerator ShowPopupAfterDelay()
        {
            yield return new WaitForSeconds(popupDelay);
            ToggleCanvasGroup(screen1,true, 0);
            ToggleCanvasGroup(screen2,false, 0);
            ToggleCanvasGroup(m_CanvasGroup,true);
            
            popUpTransform.localScale = Vector3.zero;
            popUpTransform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
            if (popupSfx) popupSfx.Play();
        }

        private void ToggleCanvasGroup(CanvasGroup cg, bool isVisible, float fadeDuration = 0.5f)
        {
            cg.DOFade(isVisible ? 1f : 0f, fadeDuration).SetUpdate(true);
            cg.interactable = isVisible;
            cg.blocksRaycasts = isVisible;
        }
        
        private void OnContinueButtonPressed()
        {
            ToggleCanvasGroup(screen1,false, 0);
            ToggleCanvasGroup(screen2,true, 0.2f);
        }
        
        private void OnBeginButtonPressed()
        {
            m_CanvasGroup.interactable = false;
            popUpTransform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
            {
                ToggleCanvasGroup(m_CanvasGroup, false);
                GameStatic.OnBeginButtonPressed?.Invoke();
            });
        }
    }
}