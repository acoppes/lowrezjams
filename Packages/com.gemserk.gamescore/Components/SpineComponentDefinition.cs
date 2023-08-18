using System;
using Gemserk.Leopotam.Ecs;
using Spine;
using Spine.Unity;
using UnityEngine.Serialization;

namespace Game.Components
{
    public struct SpineComponent : IEntityComponent
    {
        public SkeletonAnimation animation;
        public SkeletonDataAsset skeletonDataAsset;
        // animation values

        public string initialSkin;
        
        public int currentAnimation;
        public float currentPlayingTime;
    }
    
    public struct SpineChangeSkinComponent : IEntityComponent
    {
        public string skin;
    }
    
    public class SpineComponentDefinition : ComponentDefinitionBase
    {
        public SkeletonDataAsset skeletonDataAsset;
        public string initialSkin = "default";
        
        public override string GetComponentName()
        {
            return nameof(SpineComponent);
        }

        public override void Apply(World world, Entity entity)
        {
            // var spine = new SpineComponent();
            // spine.animation.AnimationState.set
            world.AddComponent(entity, new SpineComponent()
            {
                skeletonDataAsset = skeletonDataAsset,
                currentAnimation = -1, 
                initialSkin = initialSkin
            });
        }
    }

    public static class SkeletonAnimationExtensions
    {
        public static bool IsPlaying(this SkeletonAnimation skeletonAnimation, string animationName, int trackIndex = 0)
        {
            var current = skeletonAnimation.AnimationState.GetCurrent(trackIndex);
            
            if (current == null)
            {
                return false;
            }

            if (current.Animation == null)
            {
                return false;
            }

            return current.Animation.Name.Equals(animationName, StringComparison.OrdinalIgnoreCase);
        }

        public static TrackEntry Play(this SkeletonAnimation skeletonAnimation, string animationName, bool loop = true, 
            bool clear = true, int trackIndex = 0)
        {
            // if (clear)
            // {
            //     skeletonAnimation.AnimationState.SetEmptyAnimation(trackIndex, 0);
            //     // trackEntry.MixBlend = MixBlend.Setup;
            // }

            var t = skeletonAnimation.AnimationState.SetAnimation(trackIndex, animationName, loop);
            if (clear)
            {
                t.MixBlend = MixBlend.Setup;
            }
            return t;
        }
    }
}