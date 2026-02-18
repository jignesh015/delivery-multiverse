using System;
using System.Collections.Generic;

namespace DeliveryMultiverse
{
    [Serializable]
    public class DeliveryScoreInfo
    {
        public float totalTimeTaken;
        public int totalTipsEarned;
    }

    [Serializable]
    public class DeliveryScoreInfoList
    {
        public List<DeliveryScoreInfo> scores = new List<DeliveryScoreInfo>();
    }
}