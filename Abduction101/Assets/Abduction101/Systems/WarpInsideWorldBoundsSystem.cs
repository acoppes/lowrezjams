using Abduction101.Components;
using Gemserk.Leopotam.Ecs;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Abduction101.Systems
{
    public class WarpInsideWorldBoundsSystem : BaseSystem, IEcsRunSystem
    {
        readonly EcsFilterInject<Inc<PositionComponent, WorldBoundsComponent>, Exc<DisabledComponent>> worldBoundsFilter = default;
        readonly EcsFilterInject<Inc<PositionComponent, WarpInsideWorldBoundsComponent>, Exc<DisabledComponent>> filter = default;

        public void Run(EcsSystems systems)
        {
            foreach (var worldBoundsEntity in worldBoundsFilter.Value)
            {
                ref var worldBoundsPosition = ref worldBoundsFilter.Pools.Inc1.Get(worldBoundsEntity);
                ref var worldBoundsComponent = ref worldBoundsFilter.Pools.Inc2.Get(worldBoundsEntity);

                var worldBounds = worldBoundsComponent.bounds;
                worldBounds.center = worldBoundsPosition.value;
                worldBoundsComponent.bounds = worldBounds;
                
                foreach (var e in filter.Value)
                {
                    ref var position = ref filter.Pools.Inc1.Get(e);
                    ref var warpInsideWorld = ref filter.Pools.Inc2.Get(e);

                    var bounds = warpInsideWorld.bounds;
                    bounds.center = position.value;

                    if (worldBounds.Intersects(bounds))
                    {
                        continue;
                    }

                    if (position.value.x < worldBounds.min.x)
                    {
                        position.value.x += worldBounds.size.x;
                    }
                    
                    if (position.value.x > worldBounds.max.x)
                    {
                        position.value.x -= worldBounds.size.x;
                    }
                }
            }

        }
    }
}