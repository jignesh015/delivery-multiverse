using System;
using UnityEngine;
using DG.Tweening;

namespace DeliveryMultiverse
{
    [RequireComponent(typeof(CanvasGroup))]
    public class DeliveryPointUI : MonoBehaviour
    {
        [SerializeField] private Transform pinPointTransform;
        [SerializeField] private GameObject stopIndicator;
        [SerializeField] private GameObject interactIndicator;
        
        [Header("Animation Settings")]
        [SerializeField] private float bounceHeight = 0.5f;
        [SerializeField] private float bounceDuration = 0.5f;
        [SerializeField] private float punchScaleFactor = 0.5f;
        [SerializeField] private float punchScaleDuration = 0.25f;
        [SerializeField] private Ease bounceEase = Ease.InOutSine;
        
        private DeliveryPoint m_DeliveryPoint;
        private CanvasGroup m_CanvasGroup;
        private Tween m_CurrentTween;
        
        private Vector3 m_OriginalPinPosition;
        private Vector3 m_OriginalInteractIndicatorScale;
        private bool m_IsOnScreen = false;
        
        private Canvas m_Canvas;
        private RectTransform m_RectTransform;
        private Camera m_Camera;

        private void Awake()
        {
            m_OriginalPinPosition = pinPointTransform.localPosition;
            m_OriginalInteractIndicatorScale = interactIndicator.transform.localScale;
            m_Canvas = GetComponentInParent<Canvas>();
            
            TryGetComponent(out m_RectTransform);
            TryGetComponent(out m_CanvasGroup);
            ToggleUIVisibility(false);
            TogglePinPointTween(false);
            ToggleStopIndicator(false);
            ToggleInteractIndicator(false);

            GameStatic.OnDeliveryPointAssigned += OnDeliveryPointAssigned;
            GameStatic.OnPlayerEnteredDeliveryPoint += OnPlayerEnteredDeliveryPoint;
            GameStatic.OnPlayerExitedDeliveryPoint += OnPlayerExitedDeliveryPoint;
            GameStatic.OnPlayerStoppedAtDeliveryPoint += OnPlayerStoppedAtDeliveryPoint;
            GameStatic.OnPlayerInteractedWithDeliveryPoint += OnPlayerInteractedWithDeliveryPoint;
            GameStatic.OnDayEnded += OnDayEnded;
        }

        private void OnDestroy()
        {
            GameStatic.OnDeliveryPointAssigned -= OnDeliveryPointAssigned;
            GameStatic.OnPlayerEnteredDeliveryPoint -= OnPlayerEnteredDeliveryPoint;
            GameStatic.OnPlayerExitedDeliveryPoint -= OnPlayerExitedDeliveryPoint;
            GameStatic.OnPlayerStoppedAtDeliveryPoint -= OnPlayerStoppedAtDeliveryPoint;
            GameStatic.OnPlayerInteractedWithDeliveryPoint -= OnPlayerInteractedWithDeliveryPoint;
            GameStatic.OnDayEnded -= OnDayEnded;
        }

        private void Start()
        {
            m_Camera = Camera.main;
            m_DeliveryPoint = GetComponentInParent<DeliveryPoint>();
        }

        private void OnDeliveryPointAssigned(DeliveryPoint deliveryPoint)
        {
            if (!m_DeliveryPoint)
            {
                m_DeliveryPoint = GetComponentInParent<DeliveryPoint>();
                if (!m_DeliveryPoint) return;
            }
            
            if (deliveryPoint != m_DeliveryPoint) return;
            
            TogglePinPointTween(true);
            ToggleUIVisibility(true);
        }

        private void OnPlayerEnteredDeliveryPoint(DeliveryPoint deliveryPoint)
        {
            if (deliveryPoint != m_DeliveryPoint) return;
            
           TogglePinPointTween(false);
           ToggleStopIndicator(true);
           ToggleInteractIndicator(false);
        }

        private void OnPlayerExitedDeliveryPoint(DeliveryPoint deliveryPoint)
        {
            if (deliveryPoint != m_DeliveryPoint) return;
            
            TogglePinPointTween(true);
            ToggleStopIndicator(false);
            ToggleInteractIndicator(false);
        }

        private void OnPlayerStoppedAtDeliveryPoint(DeliveryPoint deliveryPoint, bool hasStopped)
        {
            if (deliveryPoint != m_DeliveryPoint) return;
            
            TogglePinPointTween(false);
            ToggleStopIndicator(!hasStopped);
            ToggleInteractIndicator(hasStopped);
        }

        private void OnPlayerInteractedWithDeliveryPoint(DeliveryPoint deliveryPoint)
        {
            if (deliveryPoint != m_DeliveryPoint) return;
            
            TogglePinPointTween(false);
            ToggleStopIndicator(false);
            
            interactIndicator.transform.localScale = m_OriginalInteractIndicatorScale;
            interactIndicator.transform.DOPunchScale(m_OriginalInteractIndicatorScale * punchScaleFactor, punchScaleDuration, 1).OnComplete(() =>
            {
                ToggleInteractIndicator(false);
                ToggleUIVisibility(false);
                interactIndicator.transform.localScale = m_OriginalInteractIndicatorScale;
            });
        }

        private void OnDayEnded()
        {
            TogglePinPointTween(false);
            ToggleStopIndicator(false);
            ToggleInteractIndicator(false);
            ToggleUIVisibility(false);
        }
        
        
        private void TogglePinPointTween(bool activate)
        {
            if (m_CurrentTween != null && m_CurrentTween.IsActive())
                m_CurrentTween.Kill();
            
            if (activate)
            {
                pinPointTransform.localPosition = m_OriginalPinPosition;
                pinPointTransform.gameObject.SetActive(true);
                
                m_CurrentTween = pinPointTransform.DOLocalMoveY(bounceHeight, bounceDuration)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetEase(bounceEase);
            }
            else
            {
                pinPointTransform.gameObject.SetActive(false);
            }
        }

        private void ToggleUIVisibility(bool isVisible)
        {
            if (m_CanvasGroup)
                m_CanvasGroup.alpha = isVisible ? 1f : 0f;
        }
        
        private void ToggleStopIndicator(bool isVisible)
        {
            stopIndicator.SetActive(isVisible);

            if (!isVisible) return;
            stopIndicator.transform.localScale = Vector3.zero;
            stopIndicator.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
        }

        private void ToggleInteractIndicator(bool isVisible)
        {
            interactIndicator.SetActive(isVisible);

            if (!isVisible) return;
            interactIndicator.transform.localScale = Vector3.zero;
            interactIndicator.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
        }

        private void Update()
        {
            // Only check if UI is supposed to be visible
            if (!m_CanvasGroup || !Mathf.Approximately(m_CanvasGroup.alpha, 1f)) return;

            if (Mathf.Approximately(m_CanvasGroup.alpha, 1f) && GameStatic.CurrentDeliveryPoint != m_DeliveryPoint)
            {
                ToggleUIVisibility(false);
                return;
            }
            
            var isVisible = IsUIVisibleOnScreen();
            if (isVisible == m_IsOnScreen) return;
            
            m_IsOnScreen = isVisible;
            GameStatic.OnDeliveryPointVisibleOnScreen?.Invoke(m_DeliveryPoint, m_IsOnScreen);
        }

        private readonly Vector3[] m_CachedWorldCorners = new Vector3[4];
        private bool IsUIVisibleOnScreen()
        {
            if (!m_Canvas || !m_RectTransform)
                return false;

            m_RectTransform.GetWorldCorners(m_CachedWorldCorners);
            for (var i = 0; i < 4; i++)
            {
                var screenPoint = m_Camera ? m_Camera.WorldToScreenPoint(m_CachedWorldCorners[i]) : m_CachedWorldCorners[i];
                if (screenPoint.x >= 0 && screenPoint.x <= Screen.width &&
                    screenPoint.y >= 0 && screenPoint.y <= Screen.height)
                {
                    return true;
                }
            }
            return false;
        }
    }
}