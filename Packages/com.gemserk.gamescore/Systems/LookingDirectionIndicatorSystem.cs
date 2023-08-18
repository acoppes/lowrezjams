using Game.Components;
using Gemserk.Leopotam.Ecs;
using Gemserk.Utilities;
using Gemserk.Utilities.Pooling;
using Leopotam.EcsLite;
using UnityEngine;

namespace Game.Systems
{
    public class LookingDirectionIndicatorSystem : BaseSystem, IEcsRunSystem, IEntityCreatedHandler, IEntityDestroyedHandler,
        IEcsInitSystem
    {
        [SerializeField]
        protected GameObject indicatorPrefab;

        private GameObjectPool pool;
        
        public void Init(EcsSystems systems)
        {
            pool = new GameObjectPool(indicatorPrefab, "~LookingDirectionIndicator");
        }
        
        public void OnEntityCreated(World world, Entity entity)
        {
            var lookingDirectionIndicators = world.GetComponents<HasLookingDirectionIndicatorComponent>();
            if (lookingDirectionIndicators.Has(entity))
            {
                ref var indicatorComponent = ref lookingDirectionIndicators.Get(entity);
                indicatorComponent.instance = pool.Get();
                indicatorComponent.pivot = indicatorComponent.instance.transform.Find("Pivot");
            }
        }

        public void OnEntityDestroyed(World world, Entity entity)
        {
            var indicators = world.GetComponents<HasLookingDirectionIndicatorComponent>();
            if (indicators.Has(entity))
            {
                ref var indicatorComponent = ref indicators.Get(entity);
                if (indicatorComponent.instance != null)
                {
                    pool.Release(indicatorComponent.instance);
                }
                indicatorComponent.instance = null;
            }
        }
        
        public void Run(EcsSystems systems)
        {
            var indicators = world.GetComponents<HasLookingDirectionIndicatorComponent>();
            var lookingDirectionComponents = world.GetComponents<LookingDirection>();
            var positions = world.GetComponents<PositionComponent>();

            foreach (var entity in world.GetFilter<HasLookingDirectionIndicatorComponent>().Inc<LookingDirection>().End())
            {
                var indicatorComponent = indicators.Get(entity);
                var lookingDirection = lookingDirectionComponents.Get(entity);
                
                var pivot = indicatorComponent.pivot;

                var eulerAngles = pivot.localEulerAngles;
                var lookingDirection2d = GamePerspective.ProjectFromWorld(lookingDirection.value);
                eulerAngles.z = Vector2.SignedAngle(Vector2.right, lookingDirection2d);
                pivot.localEulerAngles = eulerAngles;
            }
            
            foreach (var entity in world.GetFilter<HasLookingDirectionIndicatorComponent>().Inc<PositionComponent>().End())
            {
                var indicatorComponent = indicators.Get(entity);
                var positionComponent = positions.Get(entity);

                var indicatorInstance = indicatorComponent.instance;
                indicatorInstance.transform.position = GamePerspective.ConvertFromWorld(positionComponent.value);
            }
        }
    }
}