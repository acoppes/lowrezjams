using Abduction101.Components;
using Abduction101.Utilities;
using Game.Components;
using Gemserk.Leopotam.Ecs;
using Gemserk.Utilities;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using MyBox;
using UnityEngine;
using Vector3 = System.Numerics.Vector3;

namespace Abduction101.Systems
{
    public class AbductionSystem : BaseSystem, IEcsRunSystem
    {
        readonly EcsFilterInject<Inc<PositionComponent, CanBeAbductedComponent>, Exc<DisabledComponent>> abductionFilter = default;
        readonly EcsFilterInject<Inc<PositionComponent, CanBeAbductedComponent, LookingDirection, VelocityComponent>, Exc<DisabledComponent>> filter = default;
        readonly EcsFilterInject<Inc<CanBeAbductedComponent, LookingDirection, PhysicsComponent>, Exc<DisabledComponent>> physicsFilter = default;

        public float centerForce = 1000;
        public float abductionAngle = 23;
        
        public void Run(EcsSystems systems)
        {
            foreach (var e in abductionFilter.Value)
            {
                ref var position = ref abductionFilter.Pools.Inc1.Get(e);
                ref var canBeAbducted = ref abductionFilter.Pools.Inc2.Get(e);

                if (canBeAbducted.isBeingAbducted && canBeAbducted.source.Exists())
                {
                    canBeAbducted.center = canBeAbducted.source.Get<PositionComponent>().value;
                    canBeAbducted.horizontal = canBeAbducted.center.XZ() - position.value.XZ();
                    canBeAbducted.vertical = (canBeAbducted.center - position.value).ToY();
                }
            }
            
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

                canBeAbducted.abductedTimeout--;
            }
            
            foreach (var e in physicsFilter.Value)
            {
                ref var canBeAbducted = ref physicsFilter.Pools.Inc1.Get(e);
                ref var lookingDirection = ref physicsFilter.Pools.Inc2.Get(e);
                ref var physics = ref physicsFilter.Pools.Inc3.Get(e);
                
                if (!canBeAbducted.isBeingAbducted)
                {
                    canBeAbducted.source = Entity.NullEntity;
                    continue;
                }
                
                physics.body.AddForce(canBeAbducted.vertical * canBeAbducted.abductionForce);
                physics.body.AddForce(canBeAbducted.horizontal * centerForce);
                
                lookingDirection.value = Vector2.right.Rotate(abductionAngle * Mathf.Deg2Rad);

                canBeAbducted.abductedTimeout--;
                canBeAbducted.abductionForce = 0;
            }
        }
    }
}