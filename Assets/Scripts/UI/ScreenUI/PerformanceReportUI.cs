using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;


namespace DeliveryMultiverse
{
    [RequireComponent(typeof(CanvasGroup))]
    public class PerformanceReportUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform popUpTransform;
        [SerializeField] private DeliveryScoreListItemUI deliveryScoreListItemPrefab;
        [SerializeField] private Transform scoreListContentTransform;
        [SerializeField] private ScrollRect scoreListScrollRect;
        
        [Space(20)]
        [SerializeField] private Button resignButton;
        [SerializeField] private Button nextDayButton;
        
        private CanvasGroup m_CanvasGroup;
        
        private void Awake()
        {
            TryGetComponent(out m_CanvasGroup);
            ToggleCanvasGroup(false);
            resignButton.onClick.AddListener(OnResignButtonPressed);
            nextDayButton.onClick.AddListener(OnNextDayButtonPressed);
            
            GameStatic.OnDayEnded += OnDayEnded;
        }
        
        private void OnDestroy()
        {
            GameStatic.OnDayEnded -= OnDayEnded;
            
            if (resignButton)
                resignButton.onClick.RemoveListener(OnResignButtonPressed);
            if (nextDayButton)                
                nextDayButton.onClick.RemoveListener(OnNextDayButtonPressed);
        }
        
        private void OnDayEnded()
        {
            ToggleCanvasGroup(true);
            popUpTransform.localScale = Vector3.zero;
            popUpTransform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
            StartCoroutine(UpdateScoreList());
        }
        
        private IEnumerator UpdateScoreList()
        {
            // Clear previous list items
            foreach (Transform child in scoreListContentTransform)
            {
                Destroy(child.gameObject);
            }
            
            // This can be used to animate the score list if needed
            yield return null;
            
            // Load delivery scores
            var scoreInfoList = GameStatic.LoadDeliveryScores();
            var scores = scoreInfoList.scores;
            var count = scores.Count;
            for (var i = count - 1; i >= 0; i--)
            {
                var score = scores[i];
                var item = Instantiate(deliveryScoreListItemPrefab, scoreListContentTransform);
                var isCurrentDay = (i == count - 1); // last in list is current day
                item.SetScoreInfo(i + 1, score.totalTimeTaken, score.totalTipsEarned, isCurrentDay);
            }
            scoreListScrollRect.DOVerticalNormalizedPos(1, 0f, false).SetEase(Ease.OutCubic);
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