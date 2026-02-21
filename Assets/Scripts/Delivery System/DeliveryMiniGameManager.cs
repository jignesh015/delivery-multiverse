using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using MoreMountains.Feedbacks;
using UnityEngine.InputSystem;
using TMPro;
using Random = UnityEngine.Random;

namespace DeliveryMultiverse
{
    public class DeliveryMiniGameManager : MonoBehaviour
    {
        [Header("Mini-Game UI References")]
        [SerializeField] private GameObject miniGamePopup;
        [SerializeField] private float popupDelay = 0.3f;
        
        [Header("Delivering Screen")]
        [SerializeField] private GameObject deliveringScreen;
        [SerializeField] private RectTransform barContainer;
        [SerializeField] private RectTransform safeZone;
        [SerializeField] private RectTransform needle;
        [SerializeField] private Image safeZoneImage;
        [SerializeField] private Image needleImage;
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private TextMeshProUGUI liveBalanceScoreText;
        
        [Space(10)]
        [Header("Delivered Screen")]
        [SerializeField] private GameObject deliveredScreen;
        [SerializeField] private TextMeshProUGUI finalBalanceScoreText;
        [SerializeField] private TextMeshProUGUI tipEarnedText;
        [SerializeField] private Color tipAmountColor = Color.green;
        
        [Header("Mini-Game Input")] 
        [SerializeField] private InputActionReference leftAction;
        [SerializeField] private InputActionReference rightAction;
        
        [Header("Game Settings")]
        [SerializeField] private float gameDuration = 15f;
        [SerializeField] private float delayBeforeEnd = 2f;
        
        [Header("Feedbacks")]
        [SerializeField] private MMF_Player deliveringScreenFeedback;
        [SerializeField] private MMF_Player deliveredScreenFeedback;
        [SerializeField] private MMF_Player deliveryFailedFeedback;
        
        [Header("Audio")]
        [SerializeField] private AudioSource popupSfx;
        [SerializeField] private AudioSource coinSfx;
        [SerializeField] private int coinSfxInterval = 5;
        [SerializeField] private Vector2 coinSfxPitchRange = new Vector2(0.8f, 2f);
        
        [Serializable]
        private class BiomeSpecificSettings
        {
            public BiomeType biomeType;
            public float safeZoneMinWidth = 200f;
            public float safeZoneMaxWidth = 400f;
            public float wobbleSpeed = 20f;
            public float wobbleAmplitude = 50f;
            public float wobbleRandomness = 0.5f;
            
            [Header("Needle Movement Settings")]
            [Space(5)]
            public float needleMoveSpeed = 300f;
            public float needleMomentumFactor = 0.2f;
            public float needleDrag = 0.5f;
        }
        
        [Header("Safe Zone Settings")]
        [SerializeField] private List<BiomeSpecificSettings> biomeSpecificSettings;
        
        [Header("Visual Feedback")]
        [SerializeField] private Color safeColor = Color.green;
        [SerializeField] private Color needleInSafeZoneColor = Color.yellow;
        [SerializeField] private Color needleOutSafeZoneColor = Color.white;
        
        [Header("Tip Settings")]
        [SerializeField] private int minTipAmount = 5;
        [SerializeField] private int maxTipAmount = 40;
        [SerializeField] private float tipAmountTweenDuration = 1f;

        [Header("Failure Feedback Settings")] [SerializeField]
        private int deliveryCompleteCount = 2;
        [SerializeField] private GameObject deliveryFailedScreen;
        
        // Private variables
        private DeliveryPoint m_CurrentDeliveryPoint;
        private BiomeSpecificSettings m_CurrentBiomeSpecificSettings;
        private bool m_IsGameActive;
        private float m_GameTimer;
        private float m_TimeInSafeZone;
        private float m_NeedleVelocity;
        private float m_BarWidth;
        private float m_WobbleOffset;
        private float m_CurrentSafeZoneWidth;
        private Coroutine m_GameCoroutine;
        private CanvasGroup m_CanvasGroup;
        private bool m_PlayDeliveryFailedFeedback;
        
        private void Awake()
        {
            TryGetComponent(out m_CanvasGroup);
            GameStatic.OnPlayerInteractedWithDeliveryPoint += OnPlayerInteractedWithDeliveryPoint;
            ToggleCanvasGroup(false);
            
            if (miniGamePopup != null)
                miniGamePopup.SetActive(false);
            
            GameStatic.IsPlayingMinigame = false;
        }
        
        private void OnDestroy()
        {
            GameStatic.OnPlayerInteractedWithDeliveryPoint -= OnPlayerInteractedWithDeliveryPoint;
        }

        private void OnEnable()
        {
            if (leftAction)
                leftAction.action.Enable();
            if (rightAction)
                rightAction.action.Enable();
        }

        private void OnDisable()
        {
            if (leftAction)
                leftAction.action.Disable();
            if (rightAction)
                rightAction.action.Disable();
        }

        private void Update()
        {
            if (!m_IsGameActive) return;
            
            UpdateNeedleMovement();
            UpdateSafeZoneWobble();
            CheckNeedleInSafeZone();
        }

        private void OnPlayerInteractedWithDeliveryPoint(DeliveryPoint deliveryPoint)
        {
            m_CurrentDeliveryPoint = deliveryPoint;
            m_PlayDeliveryFailedFeedback = GameStatic.CurrentDayNumber == 1 && GameStatic.DeliveriesCompletedToday == deliveryCompleteCount;
            
            StartMiniGame();
        }

        private void StartMiniGame()
        {
            if (m_GameCoroutine != null)
                StopCoroutine(m_GameCoroutine);
            
            m_GameCoroutine = StartCoroutine(MiniGameCoroutine());
        }

        private IEnumerator MiniGameCoroutine()
        {
            // Initialize the mini-game
            InitializeMiniGame();
            yield return new WaitForSeconds(popupDelay);
            
            popupSfx?.Play();
            ToggleCanvasGroup(true);
            
            // Show popup with animation
            miniGamePopup.SetActive(true);
            miniGamePopup.transform.localScale = Vector3.zero;
            miniGamePopup.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
            
            yield return new WaitForSeconds(0.5f);
            
            // Start the game
            m_IsGameActive = true;
            m_GameTimer = gameDuration * (m_PlayDeliveryFailedFeedback ? 0.5f : 1f);
            
            if (deliveringScreenFeedback)
                deliveringScreenFeedback.PlayFeedbacks();
            
            // Game loop
            while (m_GameTimer > 0f)
            {
                m_GameTimer -= Time.deltaTime;
                UpdateUI();
                yield return null;
            }
            
            // End the game
            m_IsGameActive = false;
            EndMiniGame();
        }

        private void InitializeMiniGame()
        {
            m_TimeInSafeZone = 0f;
            m_NeedleVelocity = 0f;
            m_WobbleOffset = 0f;
            GameStatic.IsPlayingMinigame = true;
            
            m_CurrentBiomeSpecificSettings = biomeSpecificSettings.Find(s => s.biomeType == GameStatic.CurrentBiome);
            var safeZoneMinWidth = m_CurrentBiomeSpecificSettings.safeZoneMinWidth;
            var safeZoneMaxWidth = m_CurrentBiomeSpecificSettings.safeZoneMaxWidth;
            
            // Randomly select safe zone width from range
            m_CurrentSafeZoneWidth = Random.Range(safeZoneMinWidth, safeZoneMaxWidth);
            
            // Set initial UI states
            deliveredScreen.SetActive(false);
            deliveringScreen.SetActive(true);
            deliveryFailedScreen.SetActive(false);
            
            // Get bar width
            if (barContainer)
                m_BarWidth = barContainer.rect.width;
            
            // Set safe zone width
            if (safeZone)
                safeZone.sizeDelta = new Vector2(m_CurrentSafeZoneWidth, safeZone.sizeDelta.y);
            
            // Reset needle position to center
            if (needle)
                needle.anchoredPosition = Vector2.zero;
            
            // Reset safe zone position
            if (safeZone)
                safeZone.anchoredPosition = Vector2.zero;
            
            // Set initial colors
            if (safeZoneImage)
                safeZoneImage.color = safeColor;
            if (needleImage)
                needleImage.color = needleOutSafeZoneColor;
        }

        private void UpdateNeedleMovement()
        {
            if (!needle || m_CurrentBiomeSpecificSettings == null) return;
            
            var input = 0f;
            
            // Get input from actions
            if (leftAction && leftAction.action.IsPressed())
                input -= 1f;
            if (rightAction && rightAction.action.IsPressed())
                input += 1f;
            
            // Apply momentum
            m_NeedleVelocity += input * m_CurrentBiomeSpecificSettings.needleMoveSpeed * Time.deltaTime;
            m_NeedleVelocity *= Mathf.Pow(m_CurrentBiomeSpecificSettings.needleMomentumFactor, Time.deltaTime * m_CurrentBiomeSpecificSettings.needleDrag);
            
            // Update position
            var currentPos = needle.anchoredPosition;
            currentPos.x += m_NeedleVelocity * Time.deltaTime;
            
            // Clamp to bar bounds
            var halfBarWidth = m_BarWidth * 0.5f;
            currentPos.x = Mathf.Clamp(currentPos.x, -halfBarWidth, halfBarWidth);
            
            // Apply damping at edges
            if (Mathf.Abs(currentPos.x) >= halfBarWidth - 10f)
            {
                m_NeedleVelocity *= 0.5f;
            }
            
            needle.anchoredPosition = currentPos;
        }

        private void UpdateSafeZoneWobble()
        {
            if (!safeZone) return;
            
            var wobbleSpeed = m_CurrentBiomeSpecificSettings.wobbleSpeed;
            var wobbleAmplitude = m_CurrentBiomeSpecificSettings.wobbleAmplitude;
            var wobbleRandomness = m_CurrentBiomeSpecificSettings.wobbleRandomness;
            
            // Add random variation to wobble
            var randomFactor = Mathf.PerlinNoise(Time.time * wobbleRandomness, 0f) * 2f - 1f;
            m_WobbleOffset += Time.deltaTime * wobbleSpeed * (1f + randomFactor * wobbleRandomness);
            
            // Calculate wobble position
            var wobbleX = Mathf.Sin(m_WobbleOffset) * wobbleAmplitude;
            
            // Clamp safe zone to stay within bar
            var halfBarWidth = m_BarWidth * 0.5f;
            var halfSafeZoneWidth = m_CurrentSafeZoneWidth * 0.5f;
            wobbleX = Mathf.Clamp(wobbleX, -halfBarWidth + halfSafeZoneWidth, halfBarWidth - halfSafeZoneWidth);
            
            safeZone.anchoredPosition = new Vector2(wobbleX, 0f);
        }

        private void CheckNeedleInSafeZone()
        {
            if (!needle || !safeZone) return;
            
            var needleX = needle.anchoredPosition.x;
            var safeZoneX = safeZone.anchoredPosition.x;
            var halfSafeZoneWidth = m_CurrentSafeZoneWidth * 0.5f;
            
            var isInSafeZone = Mathf.Abs(needleX - safeZoneX) <= halfSafeZoneWidth;
            
            if (isInSafeZone)
            {
                m_TimeInSafeZone += Time.deltaTime;
                
                // Visual feedback
                if (needleImage)
                    needleImage.color = needleInSafeZoneColor;
            }
            else
            {
                if (needleImage)
                    needleImage.color = needleOutSafeZoneColor;
            }
        }
        
        private void ToggleCanvasGroup(bool enable)
        {
            if (m_CanvasGroup)
            {
                m_CanvasGroup.DOFade(enable ? 1 : 0, 0.2f);
            }
        }

        private void UpdateUI()
        {
            // Update timer
            if (timerText)
                timerText.text = $"Time Left: {m_GameTimer:F1}s";
            
            // Update score
            if (liveBalanceScoreText)
            {
                var currentScore = GetNormalizedScore();
                liveBalanceScoreText.text = $"Balance: {(currentScore * 100f):F0}%";
            }
        }

        private float GetNormalizedScore()
        {
            if (gameDuration <= 0f) return 0f;
            return Mathf.Clamp01(m_TimeInSafeZone / gameDuration);
        }

        private void EndMiniGame()
        {
            if (deliveringScreenFeedback)
                deliveringScreenFeedback.StopFeedbacks();

            if (m_PlayDeliveryFailedFeedback && deliveryFailedFeedback)
            {
                deliveryFailedFeedback.PlayFeedbacks();
            }
            else
            {
                var finalScore = GetNormalizedScore();
            
                deliveringScreen.SetActive(false);
                deliveredScreen.SetActive(true);

                // Set final balance score
                if (finalBalanceScoreText)
                    finalBalanceScoreText.text = $"Balance : {(finalScore * 100f):F0}%";

                // Calculate tip amount (scaled by score)
                var tipAmount = Mathf.RoundToInt(Mathf.Lerp(minTipAmount, maxTipAmount, finalScore));

                // Start coroutine to animate tip and close popup
                StartCoroutine(ShowTipAndCloseCoroutine(tipAmount));
                
            }
        }

        private IEnumerator ShowTipAndCloseCoroutine(int tipAmount)
        {
            var elapsed = 0f;
            var tipAmountColorHtml = ColorUtility.ToHtmlStringRGB(tipAmountColor);
            var lastPlayedTip = 0;
            while (elapsed < tipAmountTweenDuration)
            {
                elapsed += Time.deltaTime;
                var t = Mathf.Clamp01(elapsed / tipAmountTweenDuration);
                var displayedTip = Mathf.RoundToInt(Mathf.Lerp(0, tipAmount, t));
                if (tipEarnedText)
                    tipEarnedText.text = $"Tips Earned: <color=#{tipAmountColorHtml}>${displayedTip}</color>";
                // Play coinSfx for every coinSfxInterval increment, varying pitch
                if (coinSfx && coinSfxInterval > 0 && displayedTip > lastPlayedTip)
                {
                    var start = lastPlayedTip + 1;
                    for (var i = start; i <= displayedTip; i++)
                    {
                        if (i % coinSfxInterval != 0) continue;
                        var progress = tipAmount > 1 ? (float)i / tipAmount : 0f;
                        var pitch = Mathf.Lerp(coinSfxPitchRange.x, coinSfxPitchRange.y, progress);
                        coinSfx.pitch = pitch;
                        coinSfx.Play();
                    }
                    lastPlayedTip = displayedTip;
                }
                yield return null;
            }
            // Ensure final value is set
            if (tipEarnedText)
                tipEarnedText.text = $"Tips Earned: <color=#{tipAmountColorHtml}>${tipAmount}</color>";
            
            yield return new WaitForSeconds(0.1f);
            
            deliveredScreenFeedback?.PlayFeedbacks();

            // Wait for delayBeforeEnd
            yield return new WaitForSeconds(delayBeforeEnd);

            // Hide popup with animation, then invoke event
            miniGamePopup.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack).OnComplete(() =>
            {
                miniGamePopup.SetActive(false);
                ToggleCanvasGroup(false);
                GameStatic.IsPlayingMinigame = false;
                GameStatic.OnDeliveryCompleted?.Invoke(m_CurrentDeliveryPoint, tipAmount);
            });
        }
        
        public void OnFailedDeliveryFeedbackComplete()
        {
            // Hide popup with animation, then invoke event
            miniGamePopup.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack).OnComplete(() =>
            {
                miniGamePopup.SetActive(false);
                ToggleCanvasGroup(false);
                GameStatic.IsPlayingMinigame = false;
                GameStatic.CanSwitchBiome = true;
                GameStatic.OnDeliveryCompleted?.Invoke(m_CurrentDeliveryPoint, 0);
            });
        }
    }
}