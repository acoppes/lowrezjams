using Game.Components;
using Game.Controllers;
using Gemserk.Leopotam.Ecs;
using Gemserk.Leopotam.Ecs.Components;
using Gemserk.Leopotam.Ecs.Controllers;
using Gemserk.Leopotam.Ecs.Events;
using UnityEngine;

namespace Abduction101.Controllers
{
    public class FallActiveController : ControllerBase, IUpdate, IActiveController
    {
        public void OnUpdate(World world, Entity entity, float dt)
        {
            ref var states = ref entity.Get<StatesComponent>();
            
            ref var activeController = ref entity.Get<ActiveControllerComponent>();
            var position = entity.Get<PositionComponent>();
            var gravity = entity.Get<GravityComponent>();
            
            if (states.HasState("Falling"))
            {
                if (gravity.inContactWithGround)
                {
                    StopFalling(entity);
                }
                
                return;
            }
            
            if (position.value.y > 0 && !gravity.inContactWithGround && !gravity.disabled && activeController.CanInterrupt(entity, this))
            {
                StartFalling(entity);
            }
        }

        private void StartFalling(Entity entity)
        {
            ref var states = ref entity.Get<StatesComponent>();
            ref var activeController = ref entity.Get<ActiveControllerComponent>();
            
            activeController.TakeControl(entity, this);
            
            ref var movement = ref entity.Get<MovementComponent>();
            movement.speed = 0;
            movement.movingDirection = Vector3.zero;
            
            states.EnterState("Falling");
            
            ref var model = ref entity.Get<ModelComponent>();
            model.rotation = ModelComponent.RotationType.Rotate;
        }
        
        private void StopFalling(Entity entity)
        {
            ref var states = ref entity.Get<StatesComponent>();
            ref var activeController = ref entity.Get<ActiveControllerComponent>();
            
            activeController.ReleaseControl(this);
            states.ExitState("Falling");
            
            ref var model = ref entity.Get<ModelComponent>();
            model.rotation = ModelComponent.RotationType.FlipToLookingDirection;

            if (entity.Has<PhysicsComponent>())
            {
                entity.Get<PhysicsComponent>().syncType = PhysicsComponent.SyncType.Both;
            }
            
            // ref var lookingDirection = ref entity.Get<LookingDirection>();
            // lookingDirection.value = Vector3.right;
        }

        public bool CanBeInterrupted(Entity entity, IActiveController activeController)
        {
            if (activeController is AbductedActiveController)
            {
                return true;
            }
            
            return false;
        }

        public void OnInterrupt(Entity entity, IActiveController activeController)
        {
            StopFalling(entity);
        }
    }
}