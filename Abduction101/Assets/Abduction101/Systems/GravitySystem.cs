﻿using Abduction101.Components;
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
        
        readonly EcsFilterInject<Inc<PositionComponent, GravityComponent, VelocityComponent>, Exc<DisabledComponent>> filter = default;
        
        public void Run(EcsSystems systems)
        {
            foreach (var entity in filter.Value)
            {
                ref var position = ref filter.Pools.Inc1.Get(entity);
                var gravity = filter.Pools.Inc2.Get(entity);
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
                p.y += v.y;

                gravity.inContactWithGround = p.y <= 0;

                if (gravity.inContactWithGround)
                {
                    p.y = 0;
                    v.y = 0;
                }

                position.value = p;
                velocity.value = v;
            }
        }
    }
}