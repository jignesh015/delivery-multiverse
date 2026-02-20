using System;
using UnityEngine;

namespace DeliveryMultiverse
{
    public class VehicleEngineSfx : MonoBehaviour
    {
        [Header("ENGINE SOUND SETTINGS")]
        [SerializeField] private AudioSource engineIdleAudioSource;
        [SerializeField] private AudioSource engineRunningAudioSource;
        
        [Header("ENGINE PITCH SETTINGS")]
        [SerializeField] private float minPitch = 0.95f;
        [SerializeField] private float maxPitch = 1.1f;
        [SerializeField] private float pitchChangeSpeed = 1f;
        
        [Header("ENGINE VOLUME SETTINGS")]
        [SerializeField] private float minVolume = 0f;
        [SerializeField] private float maxVolume = 0.5f;
        [SerializeField] private float volumeChangeSpeed = 2.0f;
        
        public float debugCurrentSpeed;
        
        private float m_TargetPitch;
        private Rigidbody m_VehicleRigidbody;
        
        private const float PitchRetargetThreshold = 0.02f; // how close before choosing a new random target
        private const float MinNewTargetSeparation = 0.1f; // avoid tiny oscillations
        
        private void Awake()
        {
            TryGetComponent(out m_VehicleRigidbody);
            
            // Initialize pitch state
            if (engineRunningAudioSource)
            {
                engineRunningAudioSource.pitch = Mathf.Clamp(minPitch, 0.01f, maxPitch);
                m_TargetPitch = GetNextRandomPitch(engineRunningAudioSource.pitch);
            }
            
            GameStatic.OnNewDayStarted += OnNewDayStarted;
            GameStatic.OnDayEnded += OnDayEnded;
            GameStatic.OnPlayerInteractedWithDeliveryPoint += OnPlayerInteractedWithDeliveryPoint;
            GameStatic.OnDeliveryCompleted += OnDeliveryCompleted;
            GameStatic.OnVehicleDestroyed += OnDayEnded;
        }

        private void OnDestroy()
        {
            GameStatic.OnNewDayStarted -= OnNewDayStarted;
            GameStatic.OnDayEnded -= OnDayEnded;
            GameStatic.OnPlayerInteractedWithDeliveryPoint -= OnPlayerInteractedWithDeliveryPoint;
            GameStatic.OnDeliveryCompleted -= OnDeliveryCompleted;
            GameStatic.OnVehicleDestroyed -= OnDayEnded;
        }

        private void Update()
        {
            if(!GameStatic.IsDayActive) return;
            if(!engineIdleAudioSource || !engineRunningAudioSource) return;
            if(!GameStatic.CurrentVehicleConfig || !m_VehicleRigidbody) return;

            switch (Time.timeScale)
            {
                case 0 when engineRunningAudioSource.isPlaying:
                    engineRunningAudioSource.Pause();
                    break;
                case > 0 when !engineRunningAudioSource.isPlaying:
                    engineRunningAudioSource.UnPause();
                    break;
            }
            
            // Acquire current speed
            var speed = m_VehicleRigidbody.linearVelocity.magnitude;
            debugCurrentSpeed = speed;
            
            // Normalized speed 0..1
            var normalized = Mathf.Clamp01(speed / GameStatic.CurrentVehicleConfig.maxSpeed);

            // Target volumes (running increases with speed, idle decreases)
            var targetRunningVolume = Mathf.Lerp(minVolume, maxVolume, normalized);
            var targetIdleVolume = speed > 0.1f ? Mathf.Lerp(maxVolume, minVolume, normalized) : maxVolume; // avoid cutting idle too early at very low speeds

            // Smoothly move towards target volumes
            if (engineRunningAudioSource)
            {
                engineRunningAudioSource.volume = Mathf.MoveTowards(
                    engineRunningAudioSource.volume,
                    targetRunningVolume,
                    volumeChangeSpeed * Time.deltaTime);
            }
            if (engineIdleAudioSource)
            {
                engineIdleAudioSource.volume = Mathf.MoveTowards(
                    engineIdleAudioSource.volume,
                    targetIdleVolume,
                    volumeChangeSpeed * Time.deltaTime);
            }

            // --- Smooth random pitch wandering for running engine ---
            if (engineRunningAudioSource)
            {
                // Move current pitch toward target
                engineRunningAudioSource.pitch = Mathf.MoveTowards(
                    engineRunningAudioSource.pitch,
                    m_TargetPitch,
                    pitchChangeSpeed * Time.deltaTime);

                // If we've reached (or are very close to) the target, pick a new one
                if (Mathf.Abs(engineRunningAudioSource.pitch - m_TargetPitch) <= PitchRetargetThreshold)
                {
                    m_TargetPitch = GetNextRandomPitch(m_TargetPitch);
                }
            }
        }

        private float GetNextRandomPitch(float currentTarget)
        {
            // Ensure a noticeable change by retrying if too close
            for (var i = 0; i < 5; i++)
            {
                var candidate = UnityEngine.Random.Range(minPitch, maxPitch);
                if (Mathf.Abs(candidate - currentTarget) >= MinNewTargetSeparation)
                    return candidate;
            }
            // Fallback (accept even if close after several tries)
            return UnityEngine.Random.Range(minPitch, maxPitch);
        }

        private async void OnNewDayStarted()
        {
            // Ensure both relevant loops are playing so we can blend volumes
            if (engineIdleAudioSource && !engineIdleAudioSource.isPlaying)
                engineIdleAudioSource.Play();
            if (engineRunningAudioSource && !engineRunningAudioSource.isPlaying)
                engineRunningAudioSource.Play();

            // Reset pitching cycle when game actually starts
            if (engineRunningAudioSource)
            {
                engineRunningAudioSource.pitch = Mathf.Clamp(engineRunningAudioSource.pitch, minPitch, maxPitch);
                m_TargetPitch = GetNextRandomPitch(engineRunningAudioSource.pitch);
            }
        }

        private void OnDayEnded()
        {
            engineRunningAudioSource?.Stop();
            engineIdleAudioSource?.Stop();
        }

        private void OnDeliveryCompleted(DeliveryPoint arg0, int arg1)
        {
            engineRunningAudioSource?.UnPause();
            engineIdleAudioSource?.UnPause();
        }

        private void OnPlayerInteractedWithDeliveryPoint(DeliveryPoint arg0)
        {
            engineRunningAudioSource?.Pause();
            engineIdleAudioSource?.Pause();
        }
    }
}