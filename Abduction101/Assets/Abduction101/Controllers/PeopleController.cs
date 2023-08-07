using Game.Components;
using Gemserk.Leopotam.Ecs;
using Gemserk.Leopotam.Ecs.Components;
using Gemserk.Leopotam.Ecs.Controllers;
using Gemserk.Leopotam.Ecs.Events;
using MyBox;
using UnityEngine;

namespace Abduction101.Controllers
{
    public class PeopleController : ControllerBase, IUpdate, IStateChanged
    {
        public MinMaxFloat wanderTime;
        public MinMaxFloat idleTime;
        
        public void OnUpdate(World world, Entity entity, float dt)
        {
            ref var states = ref entity.Get<StatesComponent>();

            if (states.HasState("Behaviour.Wander"))
            {
                return;
            }

            if (states.HasState("Behaviour.Idle"))
            {
                return;
            }
            
            states.EnterState("Behaviour.Idle", idleTime.RandomInRange());
            
            ref var input = ref entity.Get<InputComponent>();
            input.direction().vector2 = Vector2.zero;
        }

        public void OnEnterState(World world, Entity entity)
        {
            
        }

        public void OnExitState(World world, Entity entity)
        {
            ref var states = ref entity.Get<StatesComponent>();
            
            if (states.statesExited.Contains("Behaviour.Idle"))
            {
                var randomDirection = UnityEngine.Random.insideUnitCircle;
                
                ref var movementComponent = ref entity.Get<MovementComponent>();
                movementComponent.speed = movementComponent.baseSpeed;

                ref var input = ref entity.Get<InputComponent>();
                input.direction().vector2 = randomDirection;
                
                // movementComponent.movingDirection = new Vector3(randomDirection.x, 0, randomDirection.y);
                
                states.EnterState("Behaviour.Wander", wanderTime.RandomInRange());
            }
        }
    }
}