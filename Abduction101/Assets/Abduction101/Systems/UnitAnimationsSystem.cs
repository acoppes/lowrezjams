using Game;
using Game.Components;
using Gemserk.Leopotam.Ecs;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Abduction101.Systems
{
    public class UnitAnimationsSystem : BaseSystem, IEcsRunSystem
    {
        readonly EcsFilterInject<Inc<MovementComponent, AnimationComponent>, Exc<DisabledComponent>> animationFilter = default;
        readonly EcsFilterInject<Inc<MovementComponent, LookingDirection>, Exc<DisabledComponent>> lookingDirectionFilter = default;
        
        readonly EcsFilterInject<Inc<ActiveControllerComponent, AnimationComponent, MovementComponent>, Exc<DisabledComponent>> activeControllerFilter = default;
        readonly EcsFilterInject<Inc<AnimationComponent, MovementComponent>, Exc<DisabledComponent, ActiveControllerComponent>> notActiveControllerFilter = default;

        public void Run(EcsSystems systems)
        {
            foreach (var entity in animationFilter.Value)
            {
                ref var movement = ref animationFilter.Pools.Inc1.Get(entity);
                ref var animation = ref animationFilter.Pools.Inc2.Get(entity);

                if (movement.isMoving)
                {
                    var walkAnimation = 
                        animation.animationsAsset.GetDirectionalAnimation("Walk", 
                            movement.movingDirection);

                    if (!animation.IsPlaying(walkAnimation))
                    {
                        animation.Play(walkAnimation);
                    }
                }
            }
            
            foreach (var entity in lookingDirectionFilter.Value)
            {
                ref var movement = ref lookingDirectionFilter.Pools.Inc1.Get(entity);
                ref var lookingDirection = ref lookingDirectionFilter.Pools.Inc2.Get(entity);

                if (movement.isMoving)
                {
                    lookingDirection.value = movement.movingDirection.normalized;
                }
            }
            
            foreach (var entity in activeControllerFilter.Value)
            {
                ref var activeController = ref activeControllerFilter.Pools.Inc1.Get(entity);
                ref var animations = ref activeControllerFilter.Pools.Inc2.Get(entity);
                ref var movement = ref activeControllerFilter.Pools.Inc3.Get(entity);

                if (movement.isMoving)
                {
                    continue;
                }
                
                if (!activeController.IsControlled())
                {
                    if (!animations.IsPlaying("Idle"))
                    {
                        animations.Play("Idle");
                    }
                }
            }
            
            foreach (var entity in notActiveControllerFilter.Value)
            {
                ref var animations = ref notActiveControllerFilter.Pools.Inc1.Get(entity);
                ref var movement = ref notActiveControllerFilter.Pools.Inc2.Get(entity);

                if (movement.isMoving)
                {
                    continue;
                }
              
                if (!animations.IsPlaying("Idle"))
                {
                    animations.Play("Idle");
                }
            }
        }
    }
}