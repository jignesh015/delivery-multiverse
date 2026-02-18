using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace DeliveryMultiverse
{
    [RequireComponent(typeof(CanvasGroup))]
    public class GameOverUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform popUpTransform;
        
        [Space(20)]
        [SerializeField] private Button resignButton;
        [SerializeField] private Button restartDayButton;
        
        private CanvasGroup m_CanvasGroup;
        
        private void Awake()
        {
            TryGetComponent(out m_CanvasGroup);
            ToggleCanvasGroup(false);
            resignButton.onClick.AddListener(OnResignButtonPressed);
            restartDayButton.onClick.AddListener(OnRestartDayButtonPressed);
            
            GameStatic.OnVehicleDestroyed += OnVehicleDestroyed;
        }
        
        private void OnDestroy()
        {
            GameStatic.OnVehicleDestroyed -= OnVehicleDestroyed;
            
            if (resignButton)
                resignButton.onClick.RemoveListener(OnResignButtonPressed);
            if (restartDayButton)                
                restartDayButton.onClick.RemoveListener(OnRestartDayButtonPressed);
        }

        private void OnVehicleDestroyed()
        {
            ToggleCanvasGroup(true);
            popUpTransform.localScale = Vector3.zero;
            popUpTransform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
        }

        private void ToggleCanvasGroup(bool isVisible)
        {
            m_CanvasGroup.DOFade(isVisible ? 1f : 0f, 0.5f);
            m_CanvasGroup.interactable = isVisible;
            m_CanvasGroup.blocksRaycasts = isVisible;
        }
        
        private void OnResignButtonPressed()
        {
            m_CanvasGroup.interactable = false;
            popUpTransform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
            {
                GameStatic.OnResignButtonPressed?.Invoke();
            });
        }

        private void OnRestartDayButtonPressed()
        {
            m_CanvasGroup.interactable = false;
            popUpTransform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
            {
                ToggleCanvasGroup(false);
                GameStatic.OnRestartDayButtonPressed?.Invoke();
            });
        }
    }
}