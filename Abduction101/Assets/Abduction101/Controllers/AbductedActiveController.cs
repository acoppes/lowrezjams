using Abduction101.Components;
using Game.Components;
using Game.Controllers;
using Gemserk.Leopotam.Ecs;
using Gemserk.Leopotam.Ecs.Components;
using Gemserk.Leopotam.Ecs.Controllers;
using Gemserk.Leopotam.Ecs.Events;
using UnityEngine;

namespace Abduction101.Controllers
{
    public class AbductedActiveController : ControllerBase, IUpdate, IActiveController
    {
        public void OnUpdate(World world, Entity entity, float dt)
        {
            ref var states = ref entity.Get<StatesComponent>();
            ref var canBeAbducted = ref entity.Get<CanBeAbductedComponent>();
            ref var activeController = ref entity.Get<ActiveControllerComponent>();
            
            if (states.HasState("IsBeingAbducted"))
            {
                if (!canBeAbducted.isBeingAbducted)
                {
                    StopAbduction(entity);
                }
                
                return;
            }
            
            if (canBeAbducted.isBeingAbducted && activeController.CanInterrupt(entity, this))
            {
                StartAbduction(entity);
            }
        }

        private void StartAbduction(Entity entity)
        {
            ref var states = ref entity.Get<StatesComponent>();
            ref var activeController = ref entity.Get<ActiveControllerComponent>();
            ref var animations = ref entity.Get<AnimationComponent>();
            
            activeController.TakeControl(entity, this);
            
            ref var movement = ref entity.Get<MovementComponent>();
            movement.speed = 0;
            movement.movingDirection = Vector3.zero;
            
            states.EnterState("IsBeingAbducted");
            
            ref var model = ref entity.Get<ModelComponent>();
            model.rotation = ModelComponent.RotationType.Rotate;

            entity.Get<GravityComponent>().disabled = true;
            
            if (entity.Has<PhysicsComponent>())
            {
                entity.Get<PhysicsComponent>().syncType = PhysicsComponent.SyncType.FromPhysics;
            }
            
            animations.Play("Idle");
        }
        
        private void StopAbduction(Entity entity)
        {
            ref var states = ref entity.Get<StatesComponent>();
            ref var activeController = ref entity.Get<ActiveControllerComponent>();
            
            activeController.ReleaseControl(this);
            states.ExitState("IsBeingAbducted");
            
            // ref var model = ref entity.Get<ModelComponent>();
            // model.rotation = ModelComponent.RotationType.FlipToLookingDirection;

            entity.Get<GravityComponent>().disabled = false;
        }

        public bool CanBeInterrupted(Entity entity, IActiveController activeController)
        {
            return false;
        }

        public void OnInterrupt(Entity entity, IActiveController activeController)
        {
            throw new System.NotImplementedException();
        }
    }
}