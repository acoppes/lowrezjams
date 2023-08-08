using Abduction101.Components;
using Game.Components;
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
        readonly EcsFilterInject<Inc<PositionComponent, CanBeAbductedComponent, LookingDirection, VelocityComponent>, Exc<DisabledComponent>> filter = default;
        readonly EcsFilterInject<Inc<CanBeAbductedComponent, LookingDirection, PhysicsComponent>, Exc<DisabledComponent>> physicsFilter = default;

        public float abductionAngle = 23;
        
        public void Run(EcsSystems systems)
        {
            foreach (var e in filter.Value)
            {
                ref var position = ref filter.Pools.Inc1.Get(e);
                ref var canBeAbducted = ref filter.Pools.Inc2.Get(e);
                ref var lookingDirection = ref filter.Pools.Inc3.Get(e);
                ref var velocity = ref filter.Pools.Inc4.Get(e);

                if (!canBeAbducted.isBeingAbducted)
                {
                    continue;
                }

                var v = velocity.value;
                v.y = canBeAbducted.abductionSpeed;
                
                var p = position.value;
                p.y += v.y * dt;

                velocity.value = v;
                position.value = p;

                lookingDirection.value = Vector2.right.Rotate(abductionAngle * Mathf.Deg2Rad);

                canBeAbducted.isBeingAbducted = false;
            }
            
            foreach (var e in physicsFilter.Value)
            {
                ref var canBeAbducted = ref physicsFilter.Pools.Inc1.Get(e);
                ref var lookingDirection = ref physicsFilter.Pools.Inc2.Get(e);
                ref var physics = ref physicsFilter.Pools.Inc3.Get(e);
                
                if (!canBeAbducted.isBeingAbducted)
                {
                    continue;
                }
                
                physics.body.AddForce(UnityEngine.Vector3.up * canBeAbducted.abductionForce);
                lookingDirection.value = Vector2.right.Rotate(abductionAngle * Mathf.Deg2Rad);

                canBeAbducted.isBeingAbducted = false;
                canBeAbducted.abductionForce = 0;
            }
        }
    }
}