﻿using Game.Components;
using Game.Controllers;
using Gemserk.Leopotam.Ecs;
using Gemserk.Leopotam.Ecs.Components;
using Gemserk.Leopotam.Ecs.Controllers;
using Gemserk.Leopotam.Ecs.Events;
using MyBox;
using UnityEngine;

namespace Abduction101.Controllers
{
    public class WanderStateController : ControllerBase, IUpdate, IActiveController
    {
        public MinMaxFloat wanderTime;
        public MinMaxFloat idleTime;
        
        public void OnUpdate(World world, Entity entity, float dt)
        {
            ref var states = ref entity.Get<StatesComponent>();
            ref var activeController = ref entity.Get<ActiveControllerComponent>();

            if (states.HasState("Wandering"))
            {
                ref var movement = ref entity.Get<MovementComponent>();
                ref var input = ref entity.Get<InputComponent>();
                
                if (!states.HasState("Wandering.Timeout"))
                {
                    StopWandering(entity);
                }
                else
                {
                    if (movement.stationaryTime > 0.1f)
                    {
                        var randomDirection = UnityEngine.Random.insideUnitCircle;
                        input.direction().vector2 = randomDirection;
                    }
                }
                
                movement.movingDirection = input.direction3d();
                
                return;
            }
            
            if (!states.HasState("Wandering.Cooldown"))
            {
                if (activeController.CanInterrupt(entity, this))
                {
                    StartWandering(entity);
                }
            }
        }

        private void StartWandering(Entity entity)
        {
            ref var states = ref entity.Get<StatesComponent>();
            ref var activeController = ref entity.Get<ActiveControllerComponent>();
            
            activeController.TakeControl(entity, this);
            
            var randomDirection = UnityEngine.Random.insideUnitCircle;
                
            ref var movement = ref entity.Get<MovementComponent>();
            movement.speed = movement.baseSpeed;
            // movement.movingDirection = randomDirection;

            ref var input = ref entity.Get<InputComponent>();
            input.direction().vector2 = randomDirection;
                
            states.EnterState("Wandering");
            states.EnterState("Wandering.Timeout", wanderTime.RandomInRange());
            
            ref var model = ref entity.Get<ModelComponent>();
            model.rotation = ModelComponent.RotationType.FlipToLookingDirection;
        }
        
        private void StopWandering(Entity entity)
        {
            ref var states = ref entity.Get<StatesComponent>();
            ref var activeController = ref entity.Get<ActiveControllerComponent>();
            
            activeController.ReleaseControl(this);
            
            ref var input = ref entity.Get<InputComponent>();
            input.direction().vector2 = Vector2.zero;
            
            ref var movement = ref entity.Get<MovementComponent>();
            movement.movingDirection = Vector3.zero;
                
            states.ExitStatesAndSubStates("Wandering");
            
            states.EnterState("Wandering.Cooldown", idleTime.RandomInRange());
        }
        
        public bool CanBeInterrupted(Entity entity, IActiveController activeController)
        {
            return true;
        }

        public void OnInterrupt(Entity entity, IActiveController activeController)
        {
            StopWandering(entity);
        }
    }
}