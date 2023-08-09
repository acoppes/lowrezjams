using Abduction101.Components;
using Gemserk.Leopotam.Ecs;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Abduction101.Systems
{
    public class LimitVerticalMovementSystem : BaseSystem, IEcsRunSystem
    {
        public float minZ = -0.5f;
        public float maxZ = 1.5f;
        
        readonly EcsFilterInject<Inc<PositionComponent, LimitMovementComponent>, Exc<DisabledComponent>> filter = default;
        
        public void Run(EcsSystems systems)
        {
            foreach (var entity in filter.Value)
            {
                ref var position = ref filter.Pools.Inc1.Get(entity);
                ref var limitMovement = ref filter.Pools.Inc2.Get(entity);

                var p = position.value;

                if (p.z > maxZ + limitMovement.maxZ)
                {
                    p.z = maxZ + limitMovement.maxZ;
                }
                
                if (p.z < minZ + limitMovement.minZ)
                {
                    p.z = minZ + limitMovement.minZ;
                }

                position.value = p;
            }
        }
    }
    
    
}