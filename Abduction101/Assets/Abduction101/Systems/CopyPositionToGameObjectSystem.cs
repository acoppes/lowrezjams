using Abduction101.Components;
using Game.Components;
using Game.Systems;
using Gemserk.Leopotam.Ecs;
using Gemserk.Leopotam.Ecs.Components;
using Gemserk.Utilities;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Abduction101.Systems
{
    public class CopyPositionToGameObjectSystem : BaseSystem, IEcsRunSystem
    {
        readonly EcsFilterInject<Inc<PositionComponent, GameObjectComponent>, Exc<DisabledComponent>> filter = default;
        
        public void Run(EcsSystems systems)
        {
            foreach (var e in filter.Value)
            {
                ref var position = ref filter.Pools.Inc1.Get(e);
                ref var gameObjectComponent = ref filter.Pools.Inc2.Get(e);

                gameObjectComponent.gameObject.transform.position = GamePerspective.ConvertFromWorld(position.value);
            }
        }
    }
}