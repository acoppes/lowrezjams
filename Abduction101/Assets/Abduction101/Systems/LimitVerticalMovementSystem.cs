using Abduction101.Components;
using Gemserk.Leopotam.Ecs;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Abduction101.Systems
{
    public class LimitVerticalMovementSystem : BaseSystem, IEcsRunSystem
    {
        readonly EcsFilterInject<Inc<PositionComponent, LimitMovementComponent>, Exc<DisabledComponent>> filter = default;
        
        public void Run(EcsSystems systems)
        {
            foreach (var entity in filter.Value)
            {
                ref var position = ref filter.Pools.Inc1.Get(entity);
                ref var limitMovement = ref filter.Pools.Inc2.Get(entity);

                var p = position.value;

                if (p.z > limitMovement.maxZ)
                {
                    p.z = limitMovement.maxZ;
                }
                
                if (p.z < limitMovement.minZ)
                {
                    p.z = limitMovement.minZ;
                }

                position.value = p;
            }
        }
    }
    
    
}