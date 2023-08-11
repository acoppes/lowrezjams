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
    public class FireAttackStateController : ControllerBase, IUpdate, IActiveController
    {
        public Object projectileDefinition;
        
        public void OnUpdate(World world, Entity entity, float dt)
        {
            ref var states = ref entity.Get<StatesComponent>();
            ref var activeController = ref entity.Get<ActiveControllerComponent>();
            ref var abilities = ref entity.Get<AbilitiesComponent>();
            ref var animations = ref entity.Get<AnimationComponent>();
            
            var attack = abilities.GetAbility("Attack");
            
            if (states.HasState("Attack"))
            {
                if (animations.IsPlaying("AttackLoop") && attack.executionTime > attack.startTime.Total)
                {
                    animations.Play("Attack", 1);
                    
                    // SPAWN PROJECTILE

                    var projectileEntity = world.CreateEntity(projectileDefinition);
                    projectileEntity.Get<PlayerComponent>().player = entity.Get<PlayerComponent>().player;

                    projectileEntity.Get<PositionComponent>().value =
                        entity.Get<AttachPointsComponent>().attachPoints["weapon"].position;

                    ref var projectile = ref projectileEntity.Get<ProjectileComponent>();
                    projectile.source = entity;
                    projectile.initialVelocity = entity.Get<LookingDirection>().value;
                    
                    return;
                }
                
                if (animations.IsPlaying("Attack") && animations.isCompleted)
                {
                    StopAttack(entity);
                    return;
                }
                
                return;
            }
            
            if (attack.isReady && attack.hasTargets && activeController.CanInterrupt(entity, this))
            {
                StartAttack(entity);
            }
        }

        private void StartAttack(Entity entity)
        {
            ref var states = ref entity.Get<StatesComponent>();
            ref var activeController = ref entity.Get<ActiveControllerComponent>();
            ref var abilities = ref entity.Get<AbilitiesComponent>();
            ref var animations = ref entity.Get<AnimationComponent>();
            ref var lookingDirection = ref entity.Get<LookingDirection>();
            
            activeController.TakeControl(entity, this);
            
            var attack = abilities.GetAbility("Attack");
            attack.targetsLocked = true;
            attack.Start();
            
            // LOOK TO TARGET!

            lookingDirection.value = (attack.abilityTargets[0].targetPosition - entity.Get<PositionComponent>().value)
                .normalized;
            
            if (entity.Has<MovementComponent>())
            {
                ref var movement = ref entity.Get<MovementComponent>();
                movement.speed = 0;
                movement.movingDirection = Vector3.zero;
            }
            
            states.EnterState("Attack");
            
            animations.Play("AttackLoop");
        }
        
        private void StopAttack(Entity entity)
        {
            ref var states = ref entity.Get<StatesComponent>();
            ref var activeController = ref entity.Get<ActiveControllerComponent>();
            ref var abilities = ref entity.Get<AbilitiesComponent>();
            ref var animations = ref entity.Get<AnimationComponent>();
            
            var attack = abilities.GetAbility("Attack");
            attack.targetsLocked = false;
            attack.Stop(Ability.StopType.Completed);
            
            activeController.ReleaseControl(this);
            states.ExitState("Attack");
            
            if (entity.Has<MovementComponent>())
            {
                ref var movement = ref entity.Get<MovementComponent>();
                movement.speed = movement.baseSpeed;
            }
            
            animations.Play("Idle");
        }

        public bool CanBeInterrupted(Entity entity, IActiveController activeController)
        {
            return true;
        }

        public void OnInterrupt(Entity entity, IActiveController activeController)
        {
            StopAttack(entity);
        }
    }
}