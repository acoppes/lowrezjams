using Abduction101.Components;
using Abduction101.Systems;
using Abduction101.UI;
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
    public class UfoController : ControllerBase, IUpdate, IInit, IDamagedEvent
    {
        public float abductionSpeedMultiplier = 0.25f;
        public float abductionForce = 100;
        public float abductionCenterForce = 1;

        public GameObject particlesPrefab;
        
        [EntityDefinition]
        [SerializeField]
        private Object abductEffectDefinition;
        
        [EntityDefinition]
        [SerializeField]
        private Object printEffectDefinition;
        
        [EntityDefinition]
        [SerializeField]
        private Object obliterationRayDefinition;
        
        [EntityDefinition]
        [SerializeField]
        private Object alienDefinition;

        private ParticleSystem particles;
        private Entity abductEffect = Entity.NullEntity;
        
        private Entity obliterationRayEntity = Entity.NullEntity;
        
        private Entity printEffect = Entity.NullEntity;
        private Entity printedAlien = Entity.NullEntity;

        private GameHud gameHud;
        
        public Object abductCompletedSfxDefinition;
        
        public void OnInit(World world, Entity entity)
        {
            particles = GameObject.Instantiate(particlesPrefab).GetComponent<ParticleSystem>();
            particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            gameHud = FindObjectOfType<GameHud>();
        }
        
        public void OnDamaged(World world, Entity entity)
        {
            if (entity.Get<HealthComponent>().aliveType == HealthComponent.AliveType.Death)
            {
                if (abductEffect.Exists())
                {
                    abductEffect.Get<DestroyableComponent>().destroy = true;
                    abductEffect = Entity.NullEntity;
                    
                    particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                }

                if (printEffect.Exists())
                {
                    printEffect.Get<DestroyableComponent>().destroy = true;
                    printEffect = Entity.NullEntity;
                }
                
                if (obliterationRayEntity.Exists())
                {
                    obliterationRayEntity.Get<DestroyableComponent>().destroy = true;
                    obliterationRayEntity = Entity.NullEntity;
                }
            }
        }
        
        public void OnUpdate(World world, Entity entity, float dt)
        {
            ref var movement = ref entity.Get<MovementComponent>();
            movement.speed = movement.baseSpeed;
            
            ref var input = ref entity.Get<InputComponent>();
            ref var bufferedInput = ref entity.Get<BufferedInputComponent>();
            ref var states = ref entity.Get<StatesComponent>();
            ref var biomassContainer = ref entity.Get<BiomassContainerComponent>();
           
            // ref var hasShadow = ref entity.Get<HasShadowComponent>();
            
            var position = entity.Get<PositionComponent>();

            var abilities = entity.Get<AbilitiesComponent>();
            var abductAbility = abilities.GetAbility("Abduct");
            var consumeAbility = abilities.GetAbility("Consume");
            var printAbility = abilities.GetAbility("Print");
            var obliterationAbility = abilities.GetAbility("Obliteration");
            
            if (gameHud != null)
            {
                gameHud.totalBiomass = biomassContainer.value.current;
            }

            if (states.HasState("ObliterationRay"))
            {
                obliterationRayEntity.Get<PositionComponent>().value = new Vector3(position.value.x, 0, position.value.z);
                return;
            }

            if (states.HasState("PrintingAlien"))
            {
                // do stuff..
                movement.speed = 0;

                var spawnCompleted = false;

                var alienBiomass = printedAlien.Get<BiomassSourceComponent>();

                if (gameHud != null)
                {
                    gameHud.totalBiomass = biomassContainer.value.current;
                }
                
                if (printedAlien.Exists())
                {
                    var spawnAbility = printedAlien.Get<AbilitiesComponent>().GetAbility("Spawn");
                    spawnCompleted = !spawnAbility.isExecuting && !spawnAbility.pendingExecution;
                }

                if (!input.button2().isPressed || spawnCompleted || biomassContainer.value.IsEmpty)
                {
                    printEffect.Get<DestroyableComponent>().destroy = true;
                    printEffect = Entity.NullEntity;
                    
                    printAbility.Stop(Ability.StopType.Interrupted);
                    states.ExitState("PrintingAlien");

                    if (printedAlien.Exists())
                    {
                        if (!spawnCompleted)
                        {
                            // perform damage
                            printedAlien.Get<HealthComponent>().damages.Add(new DamageData()
                            {
                                value = 1000
                            });
                        }
                    }
                    
                    printedAlien= Entity.NullEntity;
                }
                
                biomassContainer.value.Decrease(alienBiomass.spawnConsumeSpeed * dt);
                
                return;
            }
            
            foreach (var target in consumeAbility.abilityTargets)
            {
                if (target.valid && target.target.entity.Exists() && target.target.entity != entity)
                {
                    var e = target.target.entity;
                    
                    if (e.Has<HealthComponent>())
                    {
                        ref var targetHealth = ref e.Get<HealthComponent>();

                        if (targetHealth.aliveType != HealthComponent.AliveType.Alive)
                        {
                            continue;
                        }
                        
                        if (e.Has<BiomassSourceComponent>())
                        {
                            ref var biomassSource = ref e.Get<BiomassSourceComponent>();
                            biomassContainer.value.Increase(biomassSource.consumeValue);
                            biomassSource.consumeValue = 0;
                            
                            if (gameHud != null)
                            {
                                gameHud.totalBiomass = biomassContainer.value.current;
                            }

                            // in this case, I want to consume the entity without destroying it.
                            e.Get<DestroyableComponent>().destroy = true;
                            
                                                
                            if (abductCompletedSfxDefinition != null)
                            {
                                var sfxEntity = world.CreateEntity(abductCompletedSfxDefinition);
                                sfxEntity.Get<PositionComponent>().value = entity.Get<PositionComponent>().value;
                            }
                        }
                        else
                        {
                            targetHealth.damages.Add(new DamageData()
                            {
                                position = target.position,
                                value = 1000,
                                source = entity
                            });
                        }
                    }
                }
            }

            if (!abductAbility.isExecuting)
            {
                if (input.button1().isPressed)
                {
                    abductAbility.Start();
                    states.EnterState("Abducting");

                    abductEffect = world.CreateEntity(abductEffectDefinition);
                    particles.Play();

                    abductEffect.Get<PlayerComponent>().player = entity.Get<PlayerComponent>().player;
                }
            }
            
            if (abductAbility.isExecuting)
            {
                // input.horizontal().vector2 = Vector2.zero;
                movement.speed = movement.baseSpeed * abductionSpeedMultiplier;
                
                // update abduct effect position to my ground position...
                abductEffect.Get<PositionComponent>().value = new Vector3(position.value.x, 0, position.value.z);
                particles.transform.position =
                    GamePerspective.ConvertFromWorld(new Vector3(position.value.x, 0, position.value.z));

                foreach (var target in abductAbility.abilityTargets)
                {
                    if (target.valid && target.target.entity.Exists())
                    {
                        if (target.target.entity.Has<AbductionComponent>())
                        {
                            ref var abductedComponent = ref target.target.entity.Get<AbductionComponent>();
                            abductedComponent.abductedTimeout = 2;
                            abductedComponent.abductionSpeed = 1;
                            abductedComponent.abductionForce += abductionForce;
                            abductedComponent.abductionCenterForce += abductionCenterForce;
                            abductedComponent.source = entity;
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
            
            if (printAbility.isReady && bufferedInput.HasBufferedAction(input.button2()) && input.button2().isPressed && !biomassContainer.value.IsEmpty)
            {
                // check if enough biomass 
                
                bufferedInput.ConsumeBuffer();
                
                // start printing new alien...
                printAbility.Start();
                states.EnterState("PrintingAlien");

                printEffect = world.CreateEntity(printEffectDefinition);
                printEffect.Get<PlayerComponent>().player = entity.Get<PlayerComponent>().player;
                printEffect.Get<PositionComponent>().value = new Vector3(position.value.x, 0, position.value.z);

                printedAlien = world.CreateEntity(alienDefinition);
                printedAlien.Get<PlayerComponent>().player = entity.Get<PlayerComponent>().player;
                printedAlien.Get<PositionComponent>().value = new Vector3(position.value.x, 0, position.value.z);

                printedAlien.Get<AbilitiesComponent>().GetAbility("Spawn").pendingExecution = true;
                
                
                movement.speed = 0;

                // create new alien in spawn state...
                // locate effect
            }
            
            if (obliterationAbility.isReady && bufferedInput.HasBufferedAction(input.button3()) && input.button3().isPressed)
            {
                bufferedInput.ConsumeBuffer();
                obliterationAbility.Start();
                
                states.EnterState("ObliterationRay");

                obliterationRayEntity = world.CreateEntity(obliterationRayDefinition);
                obliterationRayEntity.Get<PlayerComponent>().player = entity.Get<PlayerComponent>().player;
                obliterationRayEntity.Get<PositionComponent>().value = new Vector3(position.value.x, 0, position.value.z);
            }
        }



    }
}
