using System;
using Game.Components;
using MyBox;
using Object = UnityEngine.Object;

namespace Game.Utilities
{
    [Serializable]
    public struct TargetingFilter
    {
        public enum AreaType
        {
            Nothing = 0,
            HitBox = 1
        }
            
        public enum CheckDistanceType
        {
            Nothing = 0,
            InsideDistance = 1,
            InsideDistanceXZ = 2,
        }

        public HitBox area;
        public AreaType areaType;

        public CheckDistanceType distanceType;
        public MinMaxFloat range;

        public PlayerAllianceType playerAllianceType;

        public HealthComponent.AliveType aliveType;

        public MinMaxFloat angle;

        public float maxRangeSqr => range.Max * range.Max;
        public float minRangeSqr => range.Min * range.Min;

        public Object sorter;
    }
}