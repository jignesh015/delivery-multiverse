using System;
using UnityEngine;

namespace DeliveryMultiverse
{
    public class DeliveryPoint : MonoBehaviour
    {
        private bool hasVehicleStoppedHere = false;

        private void Awake()
        {
            GameStatic.OnPlayerPressedInteract += OnPlayerPressedInteract;
        }

        private void OnDestroy()
        {
            GameStatic.OnPlayerPressedInteract -= OnPlayerPressedInteract;
        }

        private void OnPlayerPressedInteract()
        {
            if(GameStatic.CurrentDeliveryPoint != this) return;
            if(!hasVehicleStoppedHere) return;
            
            GameStatic.OnPlayerInteractedWithDeliveryPoint?.Invoke(this);
        }

        private void OnTriggerEnter(Collider other)
        {
            if(!other.CompareTag(GameStatic.VehicleTag)) return;
            if(GameStatic.CurrentDeliveryPoint != this) return;
            
            hasVehicleStoppedHere = false;
            GameStatic.OnPlayerEnteredDeliveryPoint?.Invoke(this);
        }
        
        private void OnTriggerExit(Collider other)
        {
            if(!other.CompareTag(GameStatic.VehicleTag)) return;
            if(GameStatic.CurrentDeliveryPoint != this) return;
            
            hasVehicleStoppedHere = false;
            GameStatic.OnPlayerExitedDeliveryPoint?.Invoke(this);
        }

        private void OnTriggerStay(Collider other)
        {
            if(!other.CompareTag(GameStatic.VehicleTag)) return;
            if(GameStatic.CurrentDeliveryPoint != this) return;
            
            if(!hasVehicleStoppedHere && other.attachedRigidbody.linearVelocity.magnitude < 0.1f)
            {
                hasVehicleStoppedHere = true;
                GameStatic.OnPlayerStoppedAtDeliveryPoint?.Invoke(this, true);
            }
            else if (hasVehicleStoppedHere && other.attachedRigidbody.linearVelocity.magnitude >= 0.1f)
            {
                hasVehicleStoppedHere = false;
                GameStatic.OnPlayerStoppedAtDeliveryPoint?.Invoke(this, false);
            }
        }
    }
}