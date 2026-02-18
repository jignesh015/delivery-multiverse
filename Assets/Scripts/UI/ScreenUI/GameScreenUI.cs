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
        
        [Header("Health Bar")]
        [SerializeField] private Image healthBarFillImage;
        [SerializeField] private Slider healthBarSlider;
        [SerializeField] private Color healthBarNormalColor = Color.green;
        [SerializeField] private Color healthBarLowColor = Color.red;
        [SerializeField] private float lowHealthThreshold = 0.3f;
        [SerializeField] private Image healthBarIcon;
        [SerializeField] private float healthBarFillChangeDuration = 0.3f;
        [SerializeField] private float healthBarIconTweenDuration = 0.3f;
        [SerializeField] private float healthBarIconTweenScale = 1.1f;
        [SerializeField] private Transform healthBarContainer;
        [SerializeField] private float healthBarShakeDuration = 0.5f;
        [SerializeField] private float healthBarShakeStrength = 10f;
        [SerializeField] private int healthBarShakeVibrato = 10;
        [SerializeField] private float healthBarShakeRandomness = 90f;
        
        private CanvasGroup m_CanvasGroup;
        private string m_TextHighlightColorHex;
        private Vector3 m_OriginalHealthBarIconScale;
        private Vector3 m_OriginalHealthBarContainerPosition;

        private void Awake()
        {
            TryGetComponent(out m_CanvasGroup);
            ToggleCanvasGroup(false);
            m_TextHighlightColorHex = ColorUtility.ToHtmlStringRGB(textHighlightColor);
            m_OriginalHealthBarIconScale = healthBarIcon.transform.localScale;
            m_OriginalHealthBarContainerPosition = healthBarContainer.localPosition;
            
            GameStatic.OnNewDayStarted += OnNewDayStarted;
            GameStatic.OnDayEnded += OnDayEnded;
            GameStatic.OnDeliveryCompleted += OnDeliveryCompleted;
            GameStatic.OnVehicleCollidedWithObstacle += UpdateHealthBar;
            GameStatic.OnVehicleCollectedRepairKit += UpdateHealthBar;
        }

        private void OnDestroy()
        {
            GameStatic.OnNewDayStarted -= OnNewDayStarted;
            GameStatic.OnDayEnded -= OnDayEnded;
            GameStatic.OnDeliveryCompleted -= OnDeliveryCompleted;
            GameStatic.OnVehicleCollidedWithObstacle -= UpdateHealthBar;
            GameStatic.OnVehicleCollectedRepairKit -= UpdateHealthBar;
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
        
        private void UpdateHealthBar()
        {
            var healthPercent = GameStatic.VehicleHealth;
            var targetFillAmount = Mathf.Clamp01(healthPercent);
            healthBarSlider.DOValue(targetFillAmount, healthBarFillChangeDuration).SetEase(Ease.OutQuad);
            
            var targetColor = targetFillAmount <= lowHealthThreshold ? healthBarLowColor : healthBarNormalColor;
            healthBarFillImage.DOColor(targetColor, healthBarFillChangeDuration).SetEase(Ease.OutQuad);
            healthBarIcon.DOColor(targetColor, healthBarFillChangeDuration).SetEase(Ease.OutQuad);
            
            healthBarContainer.DOKill();
            healthBarContainer.localPosition = m_OriginalHealthBarContainerPosition;
            // Shake the health bar container
            healthBarContainer.DOShakePosition(healthBarShakeDuration, healthBarShakeStrength, healthBarShakeVibrato, healthBarShakeRandomness)
                .SetEase(Ease.OutQuad);
            
            healthBarIcon.transform.DOKill();
            healthBarIcon.transform.localScale = m_OriginalHealthBarIconScale;

            if (targetFillAmount > lowHealthThreshold) return;
            healthBarIcon.transform.DOScale(Vector3.one * healthBarIconTweenScale, healthBarIconTweenDuration)
                .SetEase(Ease.OutBack).SetLoops(-1, LoopType.Yoyo);
            
        }

        private void OnNewDayStarted()
        {
            StartCoroutine(UpdateUI());
            UpdateHealthBar();
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