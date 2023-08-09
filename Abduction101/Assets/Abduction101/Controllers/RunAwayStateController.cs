using Abduction101.Components;
using Game.Components;
using Game.Components.Abilities;
using Game.Controllers;
using Gemserk.Leopotam.Ecs;
using Gemserk.Leopotam.Ecs.Components;
using Gemserk.Leopotam.Ecs.Controllers;
using Gemserk.Leopotam.Ecs.Events;
using Gemserk.Utilities;
using MyBox;
using UnityEngine;

namespace Abduction101.Controllers
{
    public class RunAwayStateController : ControllerBase, IUpdate, IActiveController
    {
        // public MinMaxFloat distance;

        public Object sfxDefinition;
        
        public void OnUpdate(World world, Entity entity, float dt)
        {
            ref var states = ref entity.Get<StatesComponent>();
            ref var activeController = ref entity.Get<ActiveControllerComponent>();
            ref var abilities = ref entity.Get<AbilitiesComponent>();

            var runAway = abilities.GetAbility("RunAway");

            if (runAway.isExecuting)
            {
                ref var movement = ref entity.Get<MovementComponent>();
                ref var input = ref entity.Get<InputComponent>();

                var mainTarget = runAway.abilityTargets[0];
                var direction = (entity.Get<PositionComponent>().value - mainTarget.position).XZ().normalized;

                input.direction().vector2 = direction;
                
                movement.movingDirection = input.direction3d();

                if (!mainTarget.valid)
                {
                    StopRunAway(entity);
                }
            }
            
            if (runAway.isReady && runAway.hasTargets && activeController.CanInterrupt(entity, this))
            {
                if (activeController.CanInterrupt(entity, this))
                {
                    StartRunAway(world, entity);
                }
            }
            
        }

        private void StartRunAway(World world, Entity entity)
        {
            ref var states = ref entity.Get<StatesComponent>();
            ref var activeController = ref entity.Get<ActiveControllerComponent>();
            ref var abilities = ref entity.Get<AbilitiesComponent>();

            activeController.TakeControl(entity, this);
            
            var runAway = abilities.GetAbility("RunAway");
            runAway.Start();
            runAway.targetsLocked = true;

            var mainTarget = runAway.abilityTargets[0];
            var direction = (entity.Get<PositionComponent>().value - mainTarget.position).XZ().normalized;
                
            ref var movement = ref entity.Get<MovementComponent>();
            movement.speed = entity.Get<RunAwayComponent>().speed;
            
            // movement.movingDirection = randomDirection;

            ref var input = ref entity.Get<InputComponent>();
            input.direction().vector2 = direction;
                
            states.EnterState("RunAway");
            
            ref var model = ref entity.Get<ModelComponent>();
            model.rotation = ModelComponent.RotationType.FlipToLookingDirection;

            if (sfxDefinition != null)
            {
                var sfxEntity = world.CreateEntity(sfxDefinition);
                sfxEntity.Get<PositionComponent>().value = entity.Get<PositionComponent>().value;
            }
        }
        
        private void StopRunAway(Entity entity)
        {
            ref var states = ref entity.Get<StatesComponent>();
            ref var activeController = ref entity.Get<ActiveControllerComponent>();
            ref var abilities = ref entity.Get<AbilitiesComponent>();
            
            activeController.ReleaseControl(this);
            
            var runAway = abilities.GetAbility("RunAway");
            runAway.Stop(Ability.StopType.Completed);
            runAway.targetsLocked = false;
            
            ref var input = ref entity.Get<InputComponent>();
            input.direction().vector2 = Vector2.zero;
            
            ref var movement = ref entity.Get<MovementComponent>();
            movement.movingDirection = Vector3.zero;
                
            states.ExitStatesAndSubStates("RunAway");
        }
        
        public bool CanBeInterrupted(Entity entity, IActiveController activeController)
        {
            if (activeController is FallActiveController)
            {
                return true;
            }

            if (activeController is AbductedActiveController)
            {
                return true;
            }
            
            // cant be interrupted by wander
            return false;
        }

        public void OnInterrupt(Entity entity, IActiveController activeController)
        {
            StopRunAway(entity);
        }
    }
}