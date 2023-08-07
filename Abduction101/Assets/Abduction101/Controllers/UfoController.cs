using Game.Components;
using Game.Components.Abilities;
using Game.Systems;
using Gemserk.Leopotam.Ecs;
using Gemserk.Leopotam.Ecs.Components;
using Gemserk.Leopotam.Ecs.Controllers;
using Gemserk.Leopotam.Ecs.Events;
using UnityEngine;

namespace Abduction101.Controllers
{
    public class UfoController : ControllerBase, IUpdate, IInit
    {
        public float abductionSpeedMultiplier = 0.25f;

        public GameObject particlesPrefab;
        
        [EntityDefinition]
        [SerializeField]
        private Object abductEffectDefinition;

        private ParticleSystem particles;
        private Entity abductEffect;
        
        public void OnInit(World world, Entity entity)
        {
            particles = GameObject.Instantiate(particlesPrefab).GetComponent<ParticleSystem>();
            particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
        
        public void OnUpdate(World world, Entity entity, float dt)
        {
            ref var movementComponent = ref entity.Get<MovementComponent>();
            // movementComponent.speed = 0;
            movementComponent.speed = movementComponent.baseSpeed;
            
            // TODO: acceleration...
            
            ref var input = ref entity.Get<InputComponent>();
            // ref var bufferedInput = ref entity.Get<BufferedInputComponent>();
            ref var states = ref entity.Get<StatesComponent>();
            var position = entity.Get<PositionComponent>();

            var abilities = entity.Get<AbilitiesComponent>();
            var abductAbility = abilities.GetAbility("Abduct");

            if (!abductAbility.isExecuting)
            {
                if (input.button1().isPressed)
                {
                    abductAbility.Start();
                    states.EnterState("Abducting");

                    abductEffect = world.CreateEntity(abductEffectDefinition);
                    abductEffect.Get<PositionComponent>().value = new Vector3(position.value.x, 0, position.value.z);
                    
                    particles.Play();
                    particles.transform.position =
                        GamePerspective.ConvertFromWorld(new Vector3(position.value.x, 0, position.value.z));
                    
                    // spawn definition etc
                    return;
                }
                
                // movementComponent.speed = movementComponent.baseSpeed;
            }
            else
            {
                // input.direction().vector2 = Vector2.zero;
                movementComponent.speed = movementComponent.baseSpeed * abductionSpeedMultiplier;
                
                // update abduct effect position to my ground position...
                abductEffect.Get<PositionComponent>().value = new Vector3(position.value.x, 0, position.value.z);
                particles.transform.position =
                    GamePerspective.ConvertFromWorld(new Vector3(position.value.x, 0, position.value.z));
                
                if (!input.button1().isPressed)
                {
                    abductAbility.Stop(Ability.StopType.Completed);
                    states.ExitState("Abducting");

                    abductEffect.Get<DestroyableComponent>().destroy = true;
                    abductEffect = Entity.NullEntity;
                    
                    particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                    
                    // destroy abduct effect
                    return;
                }
            }
        }


    }
}
