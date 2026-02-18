using System;
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
        [SerializeField] private TextMeshProUGUI timeRemainingText;
        [SerializeField] private TextMeshProUGUI deliveriesCompletedText;
        [SerializeField] private TextMeshProUGUI tipsEarnedText;
        
        [Space(20)]
        [SerializeField] private float lowTimeThreshold = 10f;
        [SerializeField] private Color timeRemainingNormalColor = Color.yellow;
        [SerializeField] private Color timeRemainingLowColor = Color.red;
        [SerializeField] private float timeTextPunchScale = 0.3f;
        [SerializeField] private float timeTextPunchScaleDuration = 0.25f;
        [SerializeField] private int timeTextPunchScaleVibrato = 1;
        
        private CanvasGroup m_CanvasGroup;
        private Vector3 m_OriginalTimeTextScale;
        private bool m_IsLowTimeWarningActive = false;
        private Tween m_TimeTextTween;

        private void Awake()
        {
            m_OriginalTimeTextScale = timeRemainingText.transform.localScale;
            TryGetComponent(out m_CanvasGroup);
            ToggleCanvasGroup(false);
            
            GameStatic.OnNewDayStarted += OnNewDayStarted;
            GameStatic.OnDayEnded += OnDayEnded;
        }

        private void OnDestroy()
        {
            GameStatic.OnNewDayStarted -= OnNewDayStarted;
            GameStatic.OnDayEnded -= OnDayEnded;
        }

        private void Update()
        {
            if(m_CanvasGroup.alpha == 0f)
                return;
            
            UpdateUI();
        }
        
        private void UpdateUI()
        {
            dayText.text = $"{GameStatic.CurrentDayNumber}";
            var totalSeconds = Mathf.CeilToInt(GameStatic.TimeRemainingInDay);
            var minutes = totalSeconds / 60;
            var seconds = totalSeconds % 60;
            timeRemainingText.text = $"{minutes}:{seconds:D2}";
            deliveriesCompletedText.text = $"{GameStatic.DeliveriesCompletedToday}";
            tipsEarnedText.text = $"${GameStatic.TotalTipsEarnedToday}";

            // Low time color and tween logic
            if (GameStatic.TimeRemainingInDay <= lowTimeThreshold)
            {
                if (!m_IsLowTimeWarningActive)
                {
                    m_IsLowTimeWarningActive = true;
                    timeRemainingText.color = timeRemainingLowColor;
                    // Kill any running tween
                    m_TimeTextTween?.Kill();
                    // Punch scale tween
                    m_TimeTextTween = timeRemainingText.transform.DOPunchScale(
                        Vector3.one * timeTextPunchScale,
                        timeTextPunchScaleDuration,
                        timeTextPunchScaleVibrato,
                        0f
                    );
                }
            }
            else
            {
                if (m_IsLowTimeWarningActive)
                {
                    m_IsLowTimeWarningActive = false;
                    timeRemainingText.color = timeRemainingNormalColor;
                    timeRemainingText.transform.localScale = m_OriginalTimeTextScale;
                    m_TimeTextTween?.Kill();
                }
            }
        }

        private void OnNewDayStarted()
        {
            timeRemainingText.color = timeRemainingNormalColor;
            timeRemainingText.transform.localScale = m_OriginalTimeTextScale;
            m_IsLowTimeWarningActive = false;
            m_TimeTextTween?.Kill();
            
            ToggleCanvasGroup(true);
        }

        private void OnDayEnded()
        {
            if (m_TimeTextTween != null && m_TimeTextTween.IsActive())
            {
                m_TimeTextTween.Kill();
            }
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