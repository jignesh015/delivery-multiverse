using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DeliveryMultiverse
{
    [RequireComponent(typeof(CanvasGroup))]
    public class DeliveryScoreListItemUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI dayText;
        [SerializeField] private TextMeshProUGUI timeTakenText;
        [SerializeField] private TextMeshProUGUI tipsEarnedText;
        [SerializeField] private Image backgroundHighlightImage;
        [SerializeField] private Color currentDayHighlightColor = Color.yellow;
        [SerializeField] private Color valueHighlightColor = Color.yellow;
        [SerializeField] private CanvasGroup canvasGroup;
        
        private bool m_IsCurrentDay;
        private string m_ValueHighlightColorHex;

        public void SetScoreInfo(int dayNumber, float totalTimeTaken, int totalTipsEarned, bool isCurrentDay = false)
        {
            m_ValueHighlightColorHex = ColorUtility.ToHtmlStringRGB(valueHighlightColor);
            m_IsCurrentDay = isCurrentDay;
            dayText.text = $"Day {dayNumber}";
            var minutes = Mathf.CeilToInt(totalTimeTaken) / 60;
            var seconds = Mathf.CeilToInt(totalTimeTaken) % 60;
            timeTakenText.text = $"Time: <color=#{m_ValueHighlightColorHex}>{minutes}:{seconds:D2}</color>";
            tipsEarnedText.text = $"Tips: <color=#{m_ValueHighlightColorHex}>${totalTipsEarned}</color>";
           
            if(canvasGroup)
                canvasGroup.alpha = isCurrentDay ? 1f : 0.5f;
            
            if (!m_IsCurrentDay) return;
            backgroundHighlightImage.color = currentDayHighlightColor;
        }
    }
}