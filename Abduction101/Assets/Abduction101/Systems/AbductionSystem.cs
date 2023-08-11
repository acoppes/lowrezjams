using Abduction101.Components;
using Abduction101.Utilities;
using Game.Components;
using Gemserk.Leopotam.Ecs;
using Gemserk.Utilities;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Abduction101.Systems
{
    public class AbductionSystem : BaseSystem, IEcsRunSystem
    {
        readonly EcsFilterInject<Inc<PositionComponent, AbductionComponent>, Exc<DisabledComponent>> abductionFilter = default;
        readonly EcsFilterInject<Inc<PositionComponent, AbductionComponent, LookingDirection, VelocityComponent>, Exc<DisabledComponent>> filter = default;
        readonly EcsFilterInject<Inc<AbductionComponent, LookingDirection, PhysicsComponent>, Exc<DisabledComponent>> physicsFilter = default;

        public float rotationSpeed = 1;
        public bool resetSpeedOnStartAbduction = true;
        
        public void Run(EcsSystems systems)
        {
            foreach (var e in abductionFilter.Value)
            {
                ref var position = ref abductionFilter.Pools.Inc1.Get(e);
                ref var canBeAbducted = ref abductionFilter.Pools.Inc2.Get(e);

                if (canBeAbducted.isBeingAbducted && canBeAbducted.source.Exists())
                {
                    canBeAbducted.center = canBeAbducted.source.Get<PositionComponent>().value;
                    canBeAbducted.horizontal = (canBeAbducted.center - position.value).NoY();
                    canBeAbducted.vertical = (canBeAbducted.center - position.value).ToY();
                }
            }
            
            foreach (var e in filter.Value)
            {
                ref var position = ref filter.Pools.Inc1.Get(e);
                ref var abduction = ref filter.Pools.Inc2.Get(e);
                ref var lookingDirection = ref filter.Pools.Inc3.Get(e);
                ref var velocity = ref filter.Pools.Inc4.Get(e);

                if (!abduction.isBeingAbducted)
                {
                    continue;
                }

                var v = velocity.value;
                v.y = abduction.abductionSpeed;
                
                var p = position.value;
                p.y += v.y * dt;

                velocity.value = v;
                position.value = p;

                lookingDirection.value = Vector2.right.Rotate(abduction.targetAngle * Mathf.Deg2Rad);

                abduction.abductedTimeout--;
            }
            
            foreach (var e in physicsFilter.Value)
            {
                ref var abduction = ref physicsFilter.Pools.Inc1.Get(e);
                ref var lookingDirection = ref physicsFilter.Pools.Inc2.Get(e);
                ref var physics = ref physicsFilter.Pools.Inc3.Get(e);
                
                if (!abduction.isBeingAbducted)
                {
                    abduction.source = Entity.NullEntity;
                    abduction.wasBeingAbducted = abduction.isBeingAbducted;
                    continue;
                }

                if (resetSpeedOnStartAbduction)
                {
                    if (!abduction.wasBeingAbducted && abduction.isBeingAbducted)
                    {
                        physics.body.velocity = UnityEngine.Vector3.zero;
                    }
                }
                
                physics.body.AddForce(abduction.vertical * abduction.abductionForce, ForceMode.Acceleration);
                physics.body.AddForce(abduction.horizontal * abduction.abductionCenterForce, ForceMode.Acceleration);
                
                var angleDirection = abduction.targetAngle - abduction.currentAngle;
                var nextAngle = abduction.currentAngle + angleDirection * rotationSpeed * dt;
                if (nextAngle > abduction.targetAngle)
                {
                    nextAngle = abduction.targetAngle;
                }
                abduction.currentAngle = nextAngle;
                
                lookingDirection.value = Vector2.right.Rotate(abduction.currentAngle * Mathf.Deg2Rad);

                abduction.abductedTimeout--;
                
                abduction.abductionForce = 0;
                abduction.abductionCenterForce = 0;
                
                abduction.wasBeingAbducted = abduction.isBeingAbducted;
            }
        }
    }
}