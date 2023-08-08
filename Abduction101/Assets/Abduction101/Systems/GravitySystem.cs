using Abduction101.Components;
using Game.Components;
using Gemserk.Leopotam.Ecs;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Abduction101.Systems
{
    public class GravitySystem : BaseSystem, IEcsRunSystem
    {
        public float gravityAcceleration = -9.81f;
        
        readonly EcsFilterInject<Inc<PositionComponent, GravityComponent, VelocityComponent>, 
            Exc<DisabledComponent, PhysicsComponent, Physics2dComponent>> filter = default;
        
        public void Run(EcsSystems systems)
        {
            foreach (var entity in filter.Value)
            {
                ref var position = ref filter.Pools.Inc1.Get(entity);
                ref var gravity = ref filter.Pools.Inc2.Get(entity);
                ref var velocity = ref filter.Pools.Inc3.Get(entity);

                if (gravity.disabled)
                {
                    continue;
                }

                if (Mathf.Approximately(gravity.scale, 0))
                {
                    continue;
                }

                var v = velocity.value;
                v.y += gravity.scale * gravityAcceleration * dt;
                
                var p = position.value;
                p.y += v.y * dt;

                gravity.inContactWithGround = p.y <= 0;

                if (gravity.inContactWithGround)
                {
                    p.y = 0;
                    v.y = 0;
                    gravity.groundContactTime += dt;
                    gravity.timeSinceGroundContact = 0;
                }
                else
                {
                    gravity.groundContactTime = 0;
                    gravity.timeSinceGroundContact += dt;
                }

                position.value = p;
                velocity.value = v;
            }
        }
    }
}