using Abduction101.Components;
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
        public float abductionForce = 100;

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
            ref var movement = ref entity.Get<MovementComponent>();
            // movementComponent.speed = 0;
            movement.speed = movement.baseSpeed;
            
            // TODO: acceleration...
            
            ref var input = ref entity.Get<InputComponent>();
            // ref var bufferedInput = ref entity.Get<BufferedInputComponent>();
            ref var states = ref entity.Get<StatesComponent>();
            
            movement.movingDirection = input.direction3d();
           
            // ref var hasShadow = ref entity.Get<HasShadowComponent>();
            
            var position = entity.Get<PositionComponent>();

            var abilities = entity.Get<AbilitiesComponent>();
            var abductAbility = abilities.GetAbility("Abduct");
            var consumeAbility = abilities.GetAbility("Consume");

            if (!abductAbility.isExecuting)
            {
                if (input.button1().isPressed)
                {
                    abductAbility.Start();
                    states.EnterState("Abducting");

                    abductEffect = world.CreateEntity(abductEffectDefinition);
                    particles.Play();
                    
                    // abductEffect.Get<PositionComponent>().value = new Vector3(position.value.x, 0, position.value.z);
                    // particles.transform.position =
                    //     GamePerspective.ConvertFromWorld(new Vector3(position.value.x, 0, position.value.z));
                    
                    // spawn definition etc
                    // return;
                }
                
                // movementComponent.speed = movementComponent.baseSpeed;
            }
            
            if (abductAbility.isExecuting)
            {
                // input.direction().vector2 = Vector2.zero;
                movement.speed = movement.baseSpeed * abductionSpeedMultiplier;
                
                // update abduct effect position to my ground position...
                abductEffect.Get<PositionComponent>().value = new Vector3(position.value.x, 0, position.value.z);
                particles.transform.position =
                    GamePerspective.ConvertFromWorld(new Vector3(position.value.x, 0, position.value.z));

                foreach (var target in abductAbility.abilityTargets)
                {
                    if (target.valid && target.target.entity.Exists())
                    {
                        if (target.target.entity.Has<CanBeAbductedComponent>())
                        {
                            ref var abductedComponent = ref target.target.entity.Get<CanBeAbductedComponent>();
                            abductedComponent.abductedTimeout = 2;
                            abductedComponent.abductionSpeed = 1;
                            abductedComponent.abductionForce += abductionForce;
                        }
                    }
                }
                
                foreach (var target in consumeAbility.abilityTargets)
                {
                    if (target.valid && target.target.entity.Exists())
                    {
                        var e = target.target.entity;
                        if (e.Has<DestroyableComponent>())
                        {
                            e.Get<DestroyableComponent>().destroy = true;
                        }
                    }
                }
                
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
