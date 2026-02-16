using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DeliveryMultiverse
{
    public static class GameStatic
    {
        #region REALTIME DATA
        public static DeliveryPoint CurrentDeliveryPoint;
        #endregion

        #region EVENTS
        public static UnityAction OnDeliveryPointsRequested;
        public static UnityAction<Queue<DeliveryPoint>> OnDeliveryPointsQueued;
        public static UnityAction<DeliveryPoint> OnDeliveryPointAssigned;
        #endregion
    }
}