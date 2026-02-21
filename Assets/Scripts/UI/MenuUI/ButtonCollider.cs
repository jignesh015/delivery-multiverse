using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DeliveryMultiverse
{
    [RequireComponent(typeof(Collider), typeof(Button))]
    public class ButtonCollider : MonoBehaviour
    {
        [SerializeField] private  float timeToTrigger = 0.75f;
        [SerializeField] private Slider progressSlider;
        [SerializeField] private Transform buttonVisual;
        
        [Header("SFX")]
        [SerializeField] private AudioSource progressAudioSource;
        [SerializeField] private Vector2 progressAudioPitchRange = new Vector2(0.8f, 1.5f);
        
        [Header("Tweens")]
        [SerializeField] private float punchScaleAmount = 0.2f;
        [SerializeField] private float punchDuration = 0.3f;
        [SerializeField] private Ease punchEase = Ease.OutBack;
        [SerializeField] private int punchVibrato = 10;
        
        public UnityEvent onButtonEntered;
        public UnityEvent onButtonExited;
        public UnityEvent onButtonPressed;
        
        private Button m_Button;
        private float m_TimeInside;
        private bool m_IsPressed;

        private void Awake()
        {
            progressSlider.value = 0;
            TryGetComponent(out m_Button);
        }

        private void Update()
        {
            if (m_IsPressed)
            {
                progressSlider.value = 1;
                if (progressAudioSource && progressAudioSource.isPlaying)
                    progressAudioSource.Stop();
            }
            else
            {
                var progress = Mathf.Clamp01(m_TimeInside / timeToTrigger);
                progressSlider.value = progress;
                if (progressAudioSource)
                {
                    if (progress == 0f && progressAudioSource.isPlaying)
                        progressAudioSource.Stop();
                    progressAudioSource.pitch = Mathf.Lerp(progressAudioPitchRange.x, progressAudioPitchRange.y, progress);
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(GameStatic.VehicleTag)) return;
            m_TimeInside = 0f;
            m_IsPressed = false;
            progressSlider.value = 0;
            if (progressAudioSource != null)
            {
                progressAudioSource.pitch = progressAudioPitchRange.x;
                progressAudioSource.Play();
            }
            onButtonEntered?.Invoke();
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag(GameStatic.VehicleTag)) return;
            m_TimeInside = 0f;
            m_IsPressed = false;
            if (progressAudioSource != null)
            {
                progressAudioSource.pitch = progressAudioPitchRange.x;
                progressAudioSource.Stop();
            }
            onButtonExited?.Invoke();
        }

        private void OnTriggerStay(Collider other)
        {
            if (!other.CompareTag(GameStatic.VehicleTag)) return;
            
            m_TimeInside += Time.deltaTime;
            if (m_IsPressed || !(m_TimeInside >= timeToTrigger)) return;
            m_IsPressed = true;
            buttonVisual.DOKill();
            buttonVisual.DOPunchScale(Vector3.one * punchScaleAmount, punchDuration, punchVibrato).SetEase(punchEase);
            
            onButtonPressed?.Invoke();
            m_Button.onClick.Invoke(); // Trigger the button's onClick event
        }
    }
}