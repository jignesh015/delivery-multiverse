using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace DeliveryMultiverse
{
    [RequireComponent(typeof(CanvasGroup))]
    public class GameScreenUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TextMeshProUGUI dayText;
        [SerializeField] private TextMeshProUGUI timeTakenText;
        [SerializeField] private TextMeshProUGUI deliveriesCompletedText;
        [SerializeField] private TextMeshProUGUI tipsEarnedText;
        [SerializeField] private Color textHighlightColor = Color.green;
        
        private CanvasGroup m_CanvasGroup;
        private string m_TextHighlightColorHex;

        private void Awake()
        {
            TryGetComponent(out m_CanvasGroup);
            ToggleCanvasGroup(false);
            m_TextHighlightColorHex = ColorUtility.ToHtmlStringRGB(textHighlightColor);
            
            GameStatic.OnNewDayStarted += OnNewDayStarted;
            GameStatic.OnDayEnded += OnDayEnded;
            GameStatic.OnDeliveryCompleted += OnDeliveryCompleted;
        }

        private void OnDestroy()
        {
            GameStatic.OnNewDayStarted -= OnNewDayStarted;
            GameStatic.OnDayEnded -= OnDayEnded;
            GameStatic.OnDeliveryCompleted -= OnDeliveryCompleted;
        }

        private void Update()
        {
            if(m_CanvasGroup.alpha == 0f)
                return;
            
            UpdateTimer();
        }

        private void UpdateTimer()
        {
            var totalSeconds = Mathf.CeilToInt(GameStatic.TotalTimeTaken);
            var minutes = totalSeconds / 60;
            var seconds = totalSeconds % 60;
            timeTakenText.text = $"{minutes}:{seconds:D2}";
        }
        
        private IEnumerator UpdateUI()
        {
            for (var i = 0; i < 3; i++)
            {
                dayText.text = $"{GameStatic.CurrentDayNumber}";
                deliveriesCompletedText.text = $"{GameStatic.DeliveriesCompletedToday}/<color=#{m_TextHighlightColorHex}>{GameStatic.DeliveriesToCompleteToday}</color>";
                tipsEarnedText.text = $"${GameStatic.TotalTipsEarnedToday}";
                yield return null;
            }
        }

        private void OnNewDayStarted()
        {
            StartCoroutine(UpdateUI());
            ToggleCanvasGroup(true);
        }

        private void OnDeliveryCompleted(DeliveryPoint deliveryPoint, int tipAmount)
        {
            StartCoroutine(UpdateUI());
        }

        private void OnDayEnded()
        {
            ToggleCanvasGroup(false);
        }
        
        private void ToggleCanvasGroup(bool isVisible)
        {
            if (isVisible)
            {
                m_CanvasGroup.DOFade(1f, 0.5f).SetUpdate(true);
                m_CanvasGroup.interactable = true;
                m_CanvasGroup.blocksRaycasts = true;
            }
            else
            {
                m_CanvasGroup.DOFade(0f, 0.5f).SetUpdate(true);
                m_CanvasGroup.interactable = false;
                m_CanvasGroup.blocksRaycasts = false;
            }
        }
    }
}