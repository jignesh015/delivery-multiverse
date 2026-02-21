using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

namespace DeliveryMultiverse
{
    public class MusicToggler : MonoBehaviour
    {
        [SerializeField] private InputActionReference musicToggleAction;
        [SerializeField] private AudioMixerGroup musicToggleGroup;
        
        private void OnEnable()
        {
            musicToggleAction.action.Enable();
            musicToggleAction.action.performed += OnMusicTogglePerformed;
        }
        
        private void OnDisable()
        {
            musicToggleAction.action.performed -= OnMusicTogglePerformed;
            musicToggleAction.action.Disable();
        }
        
        private void OnMusicTogglePerformed(InputAction.CallbackContext obj)
        {
            musicToggleGroup.audioMixer.GetFloat("MusicVolume", out float currentVolume);
            if (currentVolume > -40f) // Assuming -80dB is silence and 0dB is full volume
            {
                musicToggleGroup.audioMixer.SetFloat("MusicVolume", -80f); // Mute
            }
            else
            {
                musicToggleGroup.audioMixer.SetFloat("MusicVolume", 0f); // Unmute
            }
        }
    }
}