using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace DeliveryMultiverse
{
    [RequireComponent(typeof(Button))]
    public class ButtonClickInput : MonoBehaviour
    {
        [SerializeField] private InputActionReference clickAction;
        [SerializeField] private AudioSource clickAudioSource;
        
        private Button m_Button;
        
        private void Awake()
        {
            TryGetComponent(out m_Button);
        }
        
        private void OnEnable()
        {
            clickAction.action.Enable();
            clickAction.action.performed += OnClickPerformed;
        }
        
        private void OnDisable()
        {
            clickAction.action.performed -= OnClickPerformed;
            clickAction.action.Disable();
        }

        private void OnClickPerformed(InputAction.CallbackContext obj)
        {
            if (!m_Button.interactable)
                return;

            // Check parent CanvasGroups for interactable state
            var current = m_Button.transform;
            while (current)
            {
                if (current.TryGetComponent(out CanvasGroup cg) && (!cg.interactable || cg.alpha <= 0f))
                    return;
                current = current.parent;
            }
            
            clickAudioSource?.Play();
            m_Button.onClick.Invoke();
        }
    }
}