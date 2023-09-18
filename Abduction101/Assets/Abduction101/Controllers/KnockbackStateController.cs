using Game.Components;
using Game.Controllers;
using Game.Systems;
using Gemserk.Leopotam.Ecs;
using Gemserk.Leopotam.Ecs.Components;
using Gemserk.Leopotam.Ecs.Controllers;
using Gemserk.Leopotam.Ecs.Events;
using UnityEngine;

namespace Abduction101.Controllers
{
    public class KnockbackStateController : ControllerBase, IUpdate, IActiveController, IDamagedEvent
    {
        public float knockbackImpulse = 100;
        
        public void OnUpdate(World world, Entity entity, float dt)
        {
            ref var states = ref entity.Get<StatesComponentV2>();
            
            if (states.TryGetState(PeopleStates.Knockback, out var state))
            {
                if (state.time > 0.1f)
                {
                    StopAction(entity);
                }
            }
        }

        private void StartAction(World world, Entity entity)
        {
            ref var states = ref entity.Get<StatesComponentV2>();
            ref var activeController = ref entity.Get<ActiveControllerComponent>();
            
            activeController.TakeControl(entity, this);
            
            if (entity.Has<MovementComponent>())
            {
                ref var movement = ref entity.Get<MovementComponent>();
                movement.speed = 0;
                movement.movingDirection = Vector3.zero;
            }
            
            states.Enter(PeopleStates.Knockback);
            
            ref var model = ref entity.Get<ModelComponent>();
            model.rotation = ModelComponent.RotationType.Rotate;
            
            if (entity.Has<PhysicsComponent>())
            {
                entity.Get<PhysicsComponent>().syncType = PhysicsComponent.SyncType.FromPhysics;
            }

            entity.Get<GravityComponent>().disabled = true;
        }
        
        private void StopAction(Entity entity)
        {
            ref var states = ref entity.Get<StatesComponentV2>();
            ref var activeController = ref entity.Get<ActiveControllerComponent>();
            
            activeController.ReleaseControl(this);
            states.Exit(PeopleStates.Knockback);
            
            entity.Get<GravityComponent>().disabled = false;

            // if (entity.Get<GravityComponent>().inContactWithGround)
            // {
            //     ref var model = ref entity.Get<ModelComponent>();
            //     model.rotation = ModelComponent.RotationType.FlipToLookingDirection;
            //
            //     if (entity.Has<PhysicsComponent>())
            //     {
            //         entity.Get<PhysicsComponent>().syncType = PhysicsComponent.SyncType.Both;
            //     }
            // }
        }

        public bool CanBeInterrupted(Entity entity, IActiveController activeController)
        {
            return false;
        }

        public void OnInterrupt(Entity entity, IActiveController activeController)
        {
            StopAction(entity);
        }

        public void OnDamaged(World world, Entity entity)
        {
            ref var activeController = ref entity.Get<ActiveControllerComponent>();
            var health = entity.Get<HealthComponent>();

            if (health.aliveType != HealthComponent.AliveType.Alive)
            {
                return;
            }

            foreach (var damage in health.processedDamages)
            {
                if (damage.knockback)
                {
                    if (activeController.CanInterrupt(entity, this))
                    {
                        StartAction(world, entity);

                        ref var physics = ref entity.Get<PhysicsComponent>();
                        var direction = entity.Get<PositionComponent>().value - damage.position;
                        direction += new Vector3(0, 0.25f, 0);

                        // var direction = Vector3.up;
                        physics.body.AddForce(direction.normalized * knockbackImpulse, ForceMode.Impulse);
                        
                        return;
                    }       
                }                
            }
        }
    }
}