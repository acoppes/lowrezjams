using Abduction101.Components;
using Gemserk.Leopotam.Ecs;
using Gemserk.Utilities;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using Vector3 = System.Numerics.Vector3;

namespace Abduction101.Systems
{
    public class AbductionSystem : BaseSystem, IEcsRunSystem
    {
        readonly EcsFilterInject<Inc<PositionComponent, CanBeAbductedComponent, LookingDirection>, Exc<DisabledComponent>> filter = default;

        public float abductionAngle = 23;
        
        public void Run(EcsSystems systems)
        {
            foreach (var entity in filter.Value)
            {
                ref var position = ref filter.Pools.Inc1.Get(entity);
                ref var canBeAbducted = ref filter.Pools.Inc2.Get(entity);
                ref var lookingDirection = ref filter.Pools.Inc3.Get(entity);

                if (!canBeAbducted.isBeingAbducted)
                {
                    continue;
                }
                
                var p = position.value;
                p.y += canBeAbducted.abductionSpeed * dt;
                position.value = p;

                lookingDirection.value = Vector2.right.Rotate(abductionAngle * Mathf.Deg2Rad);

                canBeAbducted.isBeingAbducted = false;
            }
        }
    }
}