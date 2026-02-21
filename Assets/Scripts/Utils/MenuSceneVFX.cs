using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace DeliveryMultiverse
{
    public class MenuSceneVFX : MonoBehaviour
    {
        [Header("Items")]
        [SerializeField] private List<Transform> itemsToAnimate;
        [SerializeField] private CanvasGroup inputInstructionCanvasGroup;

        [Header("Item Tween Settings")]
        [SerializeField] private float tweenDuration = 0.6f;
        [SerializeField] private float tweenStartDelay = 0.5f;
        [SerializeField] private float rowStaggerDelay = 0.15f;
        [SerializeField] private float itemSfxDelayFactor = 0.7f;
        [SerializeField] private Ease tweenEase = Ease.OutBounce;

        [Header("Layout Settings")]
        [SerializeField] private float zPosRange = 10f;
        [SerializeField] private float startPositionY = 40f;
        
        [Space(10)]
        [Header("Van Tween")]
        [SerializeField] private Rigidbody vanRigidbody;
        [SerializeField] private Transform vanVisualTransform;
        [SerializeField] private float vanStartYPos = 2f;
        [SerializeField] private float vanPopDuration = 0.35f;
        [SerializeField] private float vanOvershootScale = 1.25f;
        [SerializeField] private float vanSettleDuration = 0.15f;
        [SerializeField] private float vanTweenDelayFactor = 0.5f;
        [SerializeField] private Ease vanPopEase = Ease.OutBack;

        [Header("Audio")]
        [SerializeField] private AudioSource itemTouchDownSfx;
        [SerializeField] private AudioSource vanAppearSfx;

        // Runtime data
        private readonly List<float> _originalYPositions = new List<float>();

        private void Awake()
        {
            CacheAndPrepareItems();
            PrepareVan();
        }

        private void Start()
        {
            TweenItems();
        }

        /// <summary>
        /// Sorts itemsToAnimate by world Z, caches their original Y positions,
        /// then snaps each item to startPositionY.
        /// </summary>
        private void CacheAndPrepareItems()
        {
            if (itemsToAnimate == null || itemsToAnimate.Count == 0) return;
            
            if (inputInstructionCanvasGroup) inputInstructionCanvasGroup.alpha = 0f;

            // Sort ascending by world Z position
            itemsToAnimate.Sort((a, b) =>
                a.position.z.CompareTo(b.position.z));

            _originalYPositions.Clear();
            foreach (Transform item in itemsToAnimate)
            {
                _originalYPositions.Add(item.position.y);
                Vector3 pos = item.position;
                pos.y = startPositionY;
                item.position = pos;
            }
        }

        private void PrepareVan()
        {
            vanRigidbody.isKinematic = true;
            vanRigidbody.transform.position = new Vector3(vanRigidbody.transform.position.x, vanStartYPos, vanRigidbody.transform.position.z);
            vanVisualTransform.localScale = Vector3.zero;
        }

        /// <summary>
        /// Groups items into rows using zPosRange, then tweens each row down
        /// to its original Y position with a stagger delay per row.
        /// Once all items have landed, kicks off the van tween.
        /// </summary>
        private void TweenItems()
        {
            if (itemsToAnimate == null || itemsToAnimate.Count == 0) return;

            // Determine row index for each item based on its world Z position
            float minZ = itemsToAnimate[0].position.z; // list is sorted

            int rowIndex = 0;
            float rowStartZ = minZ;
            var itemRowIndices = new List<int>();

            for (int i = 0; i < itemsToAnimate.Count; i++)
            {
                float worldZ = itemsToAnimate[i].position.z;

                // Start a new row when the item falls outside the current row's Z band
                if (worldZ - rowStartZ > zPosRange)
                {
                    rowIndex++;
                    rowStartZ = worldZ;
                }

                itemRowIndices.Add(rowIndex);
            }

            // Build a single sequence for all item tweens
            Sequence itemSequence = DOTween.Sequence();

            // Initial delay before any row drops
            itemSequence.AppendInterval(tweenStartDelay);

            // Track which rows have already had their SFX scheduled
            var scheduledSfxRows = new HashSet<int>();

            for (int i = 0; i < itemsToAnimate.Count; i++)
            {
                Transform item = itemsToAnimate[i];
                float targetY = _originalYPositions[i];
                // Position in the sequence: each row's tween starts at rowIndex * rowStaggerDelay
                float insertTime = itemRowIndices[i] * rowStaggerDelay;

                itemSequence.Insert(insertTime, item.DOMoveY(targetY, tweenDuration).SetEase(tweenEase));

                // Schedule one SFX callback per row, at the moment that row's tween lands
                if (scheduledSfxRows.Add(itemRowIndices[i]))
                {
                    float sfxTime = insertTime + tweenDuration * itemSfxDelayFactor; 
                    itemSequence.InsertCallback(sfxTime, () =>
                    {
                        if (!itemTouchDownSfx) return;
                        itemTouchDownSfx.pitch = Random.Range(0.95f, 1.05f);
                        itemTouchDownSfx.Play();
                    });
                }
            }

            // Start the van tween after every item has landed
            var totalTweenTime = (itemRowIndices[^1] * rowStaggerDelay) + tweenDuration;
            itemSequence.InsertCallback(totalTweenTime * vanTweenDelayFactor, TweenVan);
        }

        /// <summary>
        /// Pops the van visual into existence dramatically, then enables physics once settled.
        /// </summary>
        private void TweenVan()
        {
            if (!vanVisualTransform) return;

            if (vanAppearSfx) vanAppearSfx.Play();

            DOTween.Sequence()
                .Append(vanVisualTransform.DOScale(vanOvershootScale, vanPopDuration).SetEase(vanPopEase))
                .Append(vanVisualTransform.DOScale(1f, vanSettleDuration).SetEase(Ease.InOutSine))
                .OnComplete(() =>
                {
                    if (vanRigidbody) vanRigidbody.isKinematic = false;
                    if (inputInstructionCanvasGroup)
                    {
                        inputInstructionCanvasGroup.DOFade(1f, 0.5f);
                    }
                });
        }
    }
}
