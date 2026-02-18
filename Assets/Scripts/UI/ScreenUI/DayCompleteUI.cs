using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace DeliveryMultiverse
{
    [RequireComponent(typeof(CanvasGroup))]
    public class DayCompleteUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TextMeshProUGUI dayCompleteText;
        [SerializeField] private TextMeshProUGUI deliveriesCompletedText;
        [SerializeField] private TextMeshProUGUI tipsEarnedText;
        [SerializeField] private Transform popUpTransform;
        [SerializeField] private Color textHighlightColor = Color.green;
        
        [Space(20)]
        [SerializeField] private Button resignButton;
        [SerializeField] private Button nextDayButton;
        
        private CanvasGroup m_CanvasGroup;
        private string m_TextHighlightColorHex;

        private void Awake()
        {
            TryGetComponent(out m_CanvasGroup);
            ToggleCanvasGroup(false);
            m_TextHighlightColorHex = ColorUtility.ToHtmlStringRGB(textHighlightColor);
            resignButton.onClick.AddListener(OnReginButtonPressed);
            nextDayButton.onClick.AddListener(OnNextDayButtonPressed);
            
            GameStatic.OnDayEnded += OnDayEnded;
        }
        
        private void OnDestroy()
        {
            GameStatic.OnDayEnded -= OnDayEnded;
            
            if (resignButton)
                resignButton.onClick.RemoveListener(OnReginButtonPressed);
            if (nextDayButton)                
                nextDayButton.onClick.RemoveListener(OnNextDayButtonPressed);
        }
        
        private void OnDayEnded()
        {
            dayCompleteText.text = $"Day {GameStatic.CurrentDayNumber} Complete!";
            deliveriesCompletedText.text = $"Packages Delivered: <color=#{m_TextHighlightColorHex}>{GameStatic.DeliveriesCompletedToday}</color>";
            tipsEarnedText.text = $"Total Tips Earned: <color=#{m_TextHighlightColorHex}>${GameStatic.TotalTipsEarnedToday}</color>";
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
        
        private void OnReginButtonPressed()
        {
            m_CanvasGroup.interactable = false;
            popUpTransform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
            {
                // TODO: Imeplement menu scene and load it here
            });
        }

        private void OnNextDayButtonPressed()
        {
            m_CanvasGroup.interactable = false;
            popUpTransform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
            {
                ToggleCanvasGroup(false);
                GameStatic.OnNextDayButtonPressed?.Invoke();
            });
        }

    }
}