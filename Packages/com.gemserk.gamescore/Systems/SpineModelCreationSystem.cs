using Game.Components;
using Gemserk.Leopotam.Ecs;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Spine.Unity;

namespace Game.Systems
{
    public class SpineModelCreationSystem : BaseSystem, IEntityCreatedHandler, IEntityDestroyedHandler, IEcsRunSystem
    {
        readonly EcsFilterInject<Inc<SpineComponent, SpineChangeSkinComponent>, Exc<DisabledComponent>> 
            spineChangeSkinFilter = default;
        
        public void OnEntityCreated(World world, Entity entity)
        {
            if (world.HasComponent<SpineComponent>(entity) && 
                world.HasComponent<ModelComponent>(entity))
            {
                ref var spineComponent = ref world.GetComponent<SpineComponent>(entity);
                var modelComponent = world.GetComponent<ModelComponent>(entity);

                spineComponent.animation = modelComponent.instance.GetComponentInChildren<SkeletonAnimation>();
                
                if (spineComponent.animation.skeletonDataAsset != spineComponent.skeletonDataAsset)
                {
                    spineComponent.animation.skeletonDataAsset = spineComponent.skeletonDataAsset;
                    spineComponent.animation.Initialize(true);
                }
                // SpineEditorUtilities.ReloadSkeletonDataAssetAndComponent(c as SkeletonGraphic);
            }
        }

        public void OnEntityDestroyed(World world, Entity entity)
        {
            if (world.HasComponent<SpineComponent>(entity))
            {
                ref var spineComponent = ref world.GetComponent<SpineComponent>(entity);

                // spineComponent.animation.skeletonDataAsset = null;
                spineComponent.animation = null;
                spineComponent.currentAnimation = -1;
                spineComponent.currentPlayingTime = 0;
            }
        }

        public void Run(EcsSystems systems)
        {
            foreach (var entity in spineChangeSkinFilter.Value)
            {
                var spineComponent = spineChangeSkinFilter.Pools.Inc1.Get(entity);
                var spineChangeSkinComponent = spineChangeSkinFilter.Pools.Inc2.Get(entity);
                
                spineComponent.animation.skeleton.SetSkin(spineChangeSkinComponent.skin);
                spineComponent.animation.skeleton.SetSlotsToSetupPose();
                
                // spineComponent.animation.Initialize(true);
                
                world.RemoveComponent<SpineChangeSkinComponent>(world.GetEntity(entity));
            }
        }
    }
}