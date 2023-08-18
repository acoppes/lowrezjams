using System.Runtime.CompilerServices;
using Game.Components;
using Gemserk.Leopotam.Ecs;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game.Systems
{
    public class AnimationSystem : BaseSystem, IEcsRunSystem
    {
        readonly EcsFilterInject<Inc<AnimationComponent, StartingAnimationComponent>, Exc<DisabledComponent>> startingAnimationFilter = default;
        readonly EcsFilterInject<Inc<AnimationComponent>, Exc<DisabledComponent>> animationFilter = default;
        
        public void Run(EcsSystems systems)
        {
            var animationDt = dt;      
            
            foreach (var entity in startingAnimationFilter.Value)
            {
                var worldEntity = world.GetEntity(entity);
                
                ref var animationComponent = ref startingAnimationFilter.Pools.Inc1.Get(entity);
                ref var startingAnimationComponent = ref startingAnimationFilter.Pools.Inc2.Get(entity);

                var animation = 0;
                
                if (startingAnimationComponent.startingAnimationType == StartingAnimationComponent.StartingAnimationType.Name)
                {
                    animation = animationComponent.animationsAsset.GetAnimationIndexByName(startingAnimationComponent.name);
                } else if (startingAnimationComponent.startingAnimationType == StartingAnimationComponent.StartingAnimationType.Random)
                {
                    animation = UnityEngine.Random.Range(0, animationComponent.animationsAsset.animations.Count);
                }

                var startingFrame = 0;
                
                var animationDefinition = animationComponent.animationsAsset.animations[animation];

                if (startingAnimationComponent.randomizeStartFrame)
                {
                    startingFrame = UnityEngine.Random.Range(0, animationDefinition.TotalFrames);
                }

                animationComponent.Play(animation, startingFrame, startingAnimationComponent.loop);
                
                if (startingAnimationComponent.randomizeStartFrame)
                {
                    // also randomize current frame time, in case frames are bigger than 1fps.
                    animationComponent.currentTime =
                        UnityEngine.Random.Range(0, animationDefinition.frames[startingFrame].time);
                    animationComponent.playingTime = animationComponent.currentTime;
                }
                
                world.RemoveComponent<StartingAnimationComponent>(worldEntity);
            }
            
            foreach (var entity in animationFilter.Value)
            {
                ref var animationComponent = ref animationFilter.Pools.Inc1.Get(entity);
                UpdateAnimation(ref animationComponent, animationDt);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UpdateAnimation(ref AnimationComponent animationComponent, float dt)
        {
            if (animationComponent.paused)
            {
                return;
            }
            
            var animationDt = dt * animationComponent.speed;
            
            if (animationComponent.state == AnimationComponent.State.Playing)
            {
                animationComponent.totalPlayingTime += animationDt;
            }

            if (animationComponent.pauseTime > 0)
            {
                animationComponent.pauseTime -= animationDt;
                return;
            }

            if (animationComponent.state == AnimationComponent.State.Playing)
            {
                // if (animationComponent.onStartEventPending)
                // {
                //     animationComponent.OnStart();
                //     animationComponent.onStartEventPending = false;
                // }

                var currentAnimation = animationComponent.animationsAsset.animations[animationComponent.currentAnimation];

                animationComponent.currentTime += animationDt;
                animationComponent.playingTime += animationDt;

                var currentFrame = currentAnimation.frames[animationComponent.currentFrame];
                
                Assert.AreNotApproximatelyEqual(currentFrame.time, 0.0f, "Invalid frame duration");

                while (animationComponent.currentTime >= currentFrame.time)
                {
                    currentFrame = currentAnimation.frames[animationComponent.currentFrame];
                    
                    Assert.AreNotApproximatelyEqual(currentFrame.time, 0.0f, "Invalid frame duration");
                    
                    // if (definition.frames != null && definition.frames.Count > 0 && definition.frames[animationComponent.currentFrame].HasEvents)
                    // {
                    //     animationComponent.OnEvent();
                    // }

                    animationComponent.currentTime -= currentFrame.time;
                    animationComponent.currentFrame++;

                    if (animationComponent.currentFrame >= currentAnimation.TotalFrames)
                    {
                        if (animationComponent.loops > 0)
                        {
                            animationComponent.loops -= 1;
                        }

                        // if (animationComponent.loops == -1)
                        // {
                        //     animationComponent.OnCompletedLoop();
                        // }

                        if (animationComponent.loops == 0)
                        {
                            animationComponent.state = AnimationComponent.State.Completed;
                            animationComponent.currentFrame = currentAnimation.TotalFrames - 1;
                            // animationComponent.OnComplete();
                            break;
                        }

                        animationComponent.currentFrame = 0;
                    }
                }
            }
        }
    }
}