﻿using Abduction101.Components;
using Game.Components;
using Game.Controllers;
using Gemserk.Leopotam.Ecs;
using Gemserk.Leopotam.Ecs.Components;
using Gemserk.Leopotam.Ecs.Controllers;
using Gemserk.Leopotam.Ecs.Events;
using UnityEngine;

namespace Abduction101.Controllers
{
    public class AbductedStateController : ControllerBase, IUpdate, IActiveController
    {
        public Object sfxDefinition;
        
        public void OnUpdate(World world, Entity entity, float dt)
        {
            ref var states = ref entity.Get<StatesComponentV2>();
            ref var abduction = ref entity.Get<AbductionComponent>();
            ref var activeController = ref entity.Get<ActiveControllerComponent>();
            
            if (states.HasState(PeopleStates.Abducting))
            {
                if (!abduction.isBeingAbducted)
                {
                    StopAbduction(world, entity);
                }
                
                return;
            }
            
            if (abduction.isBeingAbducted && activeController.CanInterrupt(entity, this))
            {
                StartAbduction(world, entity);
            }
        }

        private void StartAbduction(World world, Entity entity)
        {
            ref var states = ref entity.Get<StatesComponentV2>();
            ref var activeController = ref entity.Get<ActiveControllerComponent>();
            ref var animations = ref entity.Get<AnimationComponent>();
            ref var abduction = ref entity.Get<AbductionComponent>();
            ref var gravityComponent = ref entity.Get<GravityComponent>();
            
            activeController.TakeControl(entity, this);

            if (gravityComponent.inContactWithGround)
            {
                abduction.currentAngle = 0;
            }
            
            if (entity.Has<MovementComponent>())
            {
                ref var movement = ref entity.Get<MovementComponent>();
                movement.speed = 0;
                movement.movingDirection = Vector3.zero;
            }
            
            states.Enter(PeopleStates.Abducting);
            
            ref var model = ref entity.Get<ModelComponent>();
            model.rotation = ModelComponent.RotationType.Rotate;
            
            gravityComponent.disabled = true;
            
            if (entity.Has<PhysicsComponent>())
            {
                entity.Get<PhysicsComponent>().syncType = PhysicsComponent.SyncType.FromPhysics;
            }
            
            animations.Play("Idle");
            
            if (sfxDefinition != null && gravityComponent.inContactWithGround)
            {
                var sfxEntity = world.CreateEntity(sfxDefinition);
                sfxEntity.Get<PositionComponent>().value = entity.Get<PositionComponent>().value;
            }
        }
        
        private void StopAbduction(World world, Entity entity)
        {
            ref var states = ref entity.Get<StatesComponentV2>();
            ref var activeController = ref entity.Get<ActiveControllerComponent>();
            ref var gravityComponent = ref entity.Get<GravityComponent>();
            
            activeController.ReleaseControl(this);
            states.Exit(PeopleStates.Abducting);
            
            // ref var model = ref entity.Get<ModelComponent>();
            // model.rotation = ModelComponent.RotationType.FlipToLookingDirection;
            
            gravityComponent.disabled = false;

            if (gravityComponent.inContactWithGround)
            {
                ref var model = ref entity.Get<ModelComponent>();
                model.rotation = ModelComponent.RotationType.FlipToLookingDirection;
            }
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