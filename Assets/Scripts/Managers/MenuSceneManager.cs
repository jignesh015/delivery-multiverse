using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DeliveryMultiverse
{
    public class MenuSceneManager : MonoBehaviour
    {
        private InputAction _anyInputAction;
        private bool _cursorLocked = false;

        private void Awake()
        {
            GameStatic.IsInMenuScene = true;

            // For WebGL, cursor can only be locked after user input
            #if UNITY_WEBGL
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            _anyInputAction = new InputAction(type: InputActionType.PassThrough);
            _anyInputAction.AddBinding("<Keyboard>/anyKey");
            _anyInputAction.AddBinding("<Mouse>/leftButton");
            _anyInputAction.AddBinding("<Mouse>/rightButton");
            _anyInputAction.AddBinding("<Mouse>/middleButton");
            _anyInputAction.AddBinding("<Mouse>/delta");
            _anyInputAction.AddBinding("<Gamepad>/buttonSouth");
            _anyInputAction.AddBinding("<Gamepad>/buttonNorth");
            _anyInputAction.AddBinding("<Gamepad>/buttonEast");
            _anyInputAction.AddBinding("<Gamepad>/buttonWest");
            _anyInputAction.performed += OnAnyInput;
            _anyInputAction.Enable();
            #else
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            #endif
        }

        private void OnAnyInput(InputAction.CallbackContext ctx)
        {
            if (_cursorLocked) return;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            _cursorLocked = true;
            _anyInputAction.Disable();
        }

        private void OnDestroy()
        {
            #if UNITY_WEBGL
            if (_anyInputAction != null)
                _anyInputAction.Dispose();
            #endif
        }

        private void Start()
        {
            GameStatic.OnBiomeChanged?.Invoke(BiomeType.Normal);
        }
    }
}