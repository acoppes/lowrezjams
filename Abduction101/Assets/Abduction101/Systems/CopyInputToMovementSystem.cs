using Abduction101.Components;
using Game.Components;
using Gemserk.Leopotam.Ecs;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Abduction101.Systems
{
    public class CopyInputToMovementSystem : BaseSystem, IEcsRunSystem
    {
        readonly EcsFilterInject<Inc<InputComponent, MovementComponent, CopyInputToMovementComponent>, Exc<DisabledComponent>> filter = default;
        
        public void Run(EcsSystems systems)
        {
            foreach (var e in filter.Value)
            {
                var input = filter.Pools.Inc1.Get(e);
                ref var movement = ref filter.Pools.Inc2.Get(e);
                
                // TODO: maybe disable this with a config or something?
                movement.movingDirection = input.direction3d();
            }
            
        }
    }
}