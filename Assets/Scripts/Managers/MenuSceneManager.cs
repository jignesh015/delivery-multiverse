using System;
using UnityEngine;

namespace DeliveryMultiverse
{
    public class MenuSceneManager : MonoBehaviour
    {
        private void Awake()
        {
            GameStatic.IsInMenuScene = true;
            
            //Disable mouse pointer 
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Start()
        {
            GameStatic.OnBiomeChanged?.Invoke(BiomeType.Normal);
        }
    }
}