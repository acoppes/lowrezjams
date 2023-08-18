using System.Collections.Generic;
using Game.Components;
using Game.Definitions;
using Gemserk.Leopotam.Ecs;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Spine.Unity;
using UnityEngine;

namespace Game.Systems
{
    public class SpineAnimationSystem : BaseSystem, IEcsRunSystem, IEntityCreatedHandler
    {
        readonly EcsFilterInject<Inc<AnimationComponent, SpineComponent>, Exc<DisabledComponent>> spineAnimationFilter = default;
        
        public void OnEntityCreated(World world, Entity entity)
        {
            if (world.HasComponent<SpineComponent>(entity) &&
                world.HasComponent<AnimationComponent>(entity))
            {
                ref var animationComponent = ref world.GetComponent<AnimationComponent>(entity);
                var spineComponent = world.GetComponent<SpineComponent>(entity);

                var skeletonData = spineComponent.skeletonDataAsset.GetSkeletonData(true);
                spineComponent.animation.skeleton.SetSkin(spineComponent.initialSkin);
                spineComponent.animation.skeleton.SetSlotsToSetupPose();

                var animations = skeletonData.Animations;
                var animationsAsset = ScriptableObject.CreateInstance<AnimationsAsset>();

                animationsAsset.animations = new List<AnimationDefinition>();

                foreach (var animation in animations)
                {
                    var animationDuration = animation.Duration;

                    animationDuration = Mathf.Max(Time.fixedDeltaTime, animationDuration);
                    
                    animationsAsset.animations.Add(new AnimationDefinition()
                    {
                        name = animation.Name,
                        frames = new List<AnimationFrame>()
                        {
                            new AnimationFrame()
                            {
                                sprite = null,
                                time = animationDuration
                            }
                        }
                    });
                }
                
                animationComponent.animationsAsset = animationsAsset;
            }
        }
        
        public void Run(EcsSystems systems)
        {
            foreach (var entity in spineAnimationFilter.Value)
            {
                var animationComponent = spineAnimationFilter.Pools.Inc1.Get(entity);
                ref var spineComponent = ref spineAnimationFilter.Pools.Inc2.Get(entity);

                spineComponent.animation.UpdateTiming = UpdateTiming.ManualUpdate;

                // check for animation changes to play new animation, default track 0

                if (spineComponent.currentAnimation != animationComponent.currentAnimation)
                {
                    spineComponent.currentPlayingTime = 0;
                    spineComponent.currentAnimation = animationComponent.currentAnimation;

                    spineComponent.animation.Play(
                        animationComponent.animationsAsset.animations[animationComponent.currentAnimation].name,
                        animationComponent.loops < 0);
                }

                var animationDeltaTime = animationComponent.playingTime - spineComponent.currentPlayingTime;
                spineComponent.animation.Update(animationDeltaTime);
                spineComponent.currentPlayingTime = animationComponent.playingTime;
            }
        }


    }
}