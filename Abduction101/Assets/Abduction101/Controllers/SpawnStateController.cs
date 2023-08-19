using System;
using Game.Components;
using Game.Components.Abilities;
using Game.Controllers;
using Gemserk.Leopotam.Ecs;
using Gemserk.Leopotam.Ecs.Components;
using Gemserk.Leopotam.Ecs.Controllers;
using Gemserk.Leopotam.Ecs.Events;
using UnityEngine;

namespace Abduction101.Controllers
{
    public class SpawnStateController : ControllerBase, IUpdate, IActiveController
    {
        public void OnUpdate(World world, Entity entity, float dt)
        {
            ref var states = ref entity.Get<StatesComponent>();
            ref var activeController = ref entity.Get<ActiveControllerComponent>();
            ref var abilities = ref entity.Get<AbilitiesComponent>();
            var spawn = abilities.GetAbility("Spawn");
            ref var animations = ref entity.Get<AnimationComponent>();
            
            if (states.HasState("Spawning"))
            {
                if (animations.IsPlaying("Spawn") && animations.isCompleted)
                {
                    StopSpawning(entity);
                }
                
                // wait for animation to complete to move outside spawn...
                return;
            }

            if (spawn.pendingExecution && activeController.CanInterrupt(entity, this))
            {
                StartSpawning(entity);
            }
        }

        private void StartSpawning(Entity entity)
        {
            ref var states = ref entity.Get<StatesComponent>();
            ref var activeController = ref entity.Get<ActiveControllerComponent>();
            ref var abilities = ref entity.Get<AbilitiesComponent>();
            var spawn = abilities.GetAbility("Spawn");
            ref var animations = ref entity.Get<AnimationComponent>();
            
            activeController.TakeControl(entity, this);
            states.EnterState("Spawning");
            spawn.Start();
            
            if (entity.Has<MovementComponent>())
            {
                // cant move during spawn
                ref var movement = ref entity.Get<MovementComponent>();
                movement.speed = 0;
                movement.movingDirection = Vector3.zero;
            }
          
            animations.Play("Spawn", 1);
        }
        
        private void StopSpawning(Entity entity)
        {
            ref var states = ref entity.Get<StatesComponent>();
            ref var activeController = ref entity.Get<ActiveControllerComponent>();
            ref var abilities = ref entity.Get<AbilitiesComponent>();
            var spawn = abilities.GetAbility("Spawn");
            
            activeController.ReleaseControl(this);
            states.ExitState("Spawning");
            spawn.Stop(Ability.StopType.Completed);
            
            // not sure about the movement speed? recover it?
            
            if (entity.Has<MovementComponent>())
            {
                // cant move during spawn
                ref var movement = ref entity.Get<MovementComponent>();
                movement.speed = movement.baseSpeed;
            }
        }

        public bool CanBeInterrupted(Entity entity, IActiveController activeController)
        {
            return false;
        }

        public void OnInterrupt(Entity entity, IActiveController activeController)
        {
            throw new InvalidOperationException("Can't interrupt spawn");
        }
    }
}