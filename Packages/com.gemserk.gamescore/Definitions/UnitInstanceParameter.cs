using Game.Components;
using Game.Models;
using Game.Systems;
using Gemserk.Leopotam.Ecs;
using Gemserk.Utilities;
using MyBox;
using UnityEngine;

namespace Game.Definitions
{
    public class UnitInstanceParameter : MonoBehaviour, IEntityInstanceParameter
    {
        public enum PositionType
        {
            None = 0,
            CopyFromTransform = 1,
            CopyFromTransformDontConvert = 2,
        }
        
        public enum NamingType
        {
            None = 0,
            String = 1,
            CopyFromGameObject = 2
        }

        [Separator("Identify")] 
        public NamingType namingType = NamingType.None;
        [ConditionalField(nameof(namingType), false, NamingType.String)]
        public string entityName;
        [ConditionalField(nameof(namingType), true, NamingType.None)]
        public bool singleton;

        [Separator("Location")] 
        public PositionType positionType = PositionType.None;

        [Separator("Player Input")] 
        public bool controllable = false;
        [ConditionalField(nameof(controllable))]
        public int playerInput;

        [Separator("Player Team")] 
        public bool overridePlayer;
        [ConditionalField(nameof(overridePlayer))]
        public int team;
        
        [Separator("Looking Direction")] 
        public bool overrideLookingDirection = false;

        [ConditionalField(nameof(overrideLookingDirection))]
        public Vector3 startLookingDirection;
        
        [Separator("Model")]
        public bool overrideModel;
        [ConditionalField(nameof(overrideModel))]
        public GameObject modelInstance;
        [ConditionalField(nameof(overrideModel))]
        public int startingTextureVariant;
        [ConditionalField(nameof(overrideModel))]
        public bool overridePositionFromModel;

        [Separator("Animation")] 
        public StartingAnimationComponent.StartingAnimationType startingAnimationType = StartingAnimationComponent.StartingAnimationType.None;
        [ConditionalField(nameof(startingAnimationType), false, StartingAnimationComponent.StartingAnimationType.Name)]
        public string startingAnimation;
        [ConditionalField(nameof(startingAnimationType), true, StartingAnimationComponent.StartingAnimationType.None)]
        public bool randomizeStartFrame;
        [ConditionalField(nameof(startingAnimationType), true, StartingAnimationComponent.StartingAnimationType.None)]
        public bool startLooping = true;
        
        [Separator("HitPoints")]
        public bool overrideHitPoints;
        [ConditionalField(nameof(overrideHitPoints))]
        public int hitPoints;

        [Separator("Spawner")]
        public bool isSpawner;
        [ConditionalField(nameof(isSpawner))]
        public Object spawnArea;

        //
        public void Apply(World world, Entity entity)
        {
            if (positionType != PositionType.None)
            {
                if (!world.HasComponent<PositionComponent>(entity))
                {
                    world.AddComponent(entity, new PositionComponent());
                } 
            }
            
            if (positionType == PositionType.CopyFromTransform)
            {
                ref var position = ref world.GetComponent<PositionComponent>(entity);
                position.value =
                    GamePerspective.ConvertToWorld(transform.position);
            } else if (positionType == PositionType.CopyFromTransformDontConvert)
            {
                ref var position = ref world.GetComponent<PositionComponent>(entity);
                position.value = transform.position;
            }
            
            if (controllable)
            {
                if (!entity.Has<PlayerInputComponent>())
                {
                    world.AddComponent(entity, new PlayerInputComponent());    
                }

                ref var playerInputComponent = ref entity.Get<PlayerInputComponent>();
                playerInputComponent.playerInput = playerInput;
            }

            if (overrideLookingDirection && world.HasComponent<LookingDirection>(entity))
            {
                ref var lookingDirection = ref world.GetComponent<LookingDirection>(entity);
                lookingDirection.value = startLookingDirection.normalized;
            }
            
            if (overrideModel && world.HasComponent<ModelComponent>(entity))
            {
                if (world.HasComponent<ModelRemapTexturePerPlayerComponent>(entity))
                {
                    ref var remapComponent = ref world.GetComponent<ModelRemapTexturePerPlayerComponent>(entity);
                    remapComponent.textureVariant = startingTextureVariant;
                }
                
                ref var modelComponent = ref world.GetComponent<ModelComponent>(entity);

                if (modelInstance != null && modelComponent.prefab == null)
                {
                    modelComponent.instance = modelInstance.GetComponent<Model>();

                    if (overridePositionFromModel)
                    {
                        ref var position = ref world.GetComponent<PositionComponent>(entity);
                        position.value =
                            GamePerspective.ConvertToWorld(modelInstance.transform.position);
                    }
                }
            }
            
            if (startingAnimationType != StartingAnimationComponent.StartingAnimationType.None && world.HasComponent<AnimationComponent>(entity))
            {
                if (!entity.Has<StartingAnimationComponent>())
                {
                    world.AddComponent(entity, new StartingAnimationComponent());
                }

                ref var startingAnimationComponent = ref entity.Get<StartingAnimationComponent>();

                startingAnimationComponent.startingAnimationType = startingAnimationType;
                startingAnimationComponent.randomizeStartFrame = randomizeStartFrame;
                startingAnimationComponent.name = startingAnimation;
                startingAnimationComponent.loop = startLooping;
            }

            if (overrideHitPoints && world.HasComponent<HealthComponent>(entity))
            {
                ref var hitPointsComponent = ref world.GetComponent<HealthComponent>(entity);
                hitPointsComponent.total = hitPoints;
                hitPointsComponent.current = hitPoints;
            }

            if (overridePlayer)
            {
                if (!world.HasComponent<PlayerComponent>(entity))
                {
                    world.AddComponent(entity, new PlayerComponent());
                } 
                
                ref var playerComponent = ref world.GetComponent<PlayerComponent>(entity);
                playerComponent.player = team;
            }
            
            if (namingType == NamingType.String)
            {
                if (!entity.Has<NameComponent>())
                {
                    entity.Add(new NameComponent());
                }

                ref var nameComponent = ref entity.Get<NameComponent>();
                nameComponent.name = entityName;
                nameComponent.singleton = singleton;
                
            } else if (namingType == NamingType.CopyFromGameObject)
            {
                if (!entity.Has<NameComponent>())
                {
                    entity.Add(new NameComponent());
                }

                ref var nameComponent = ref entity.Get<NameComponent>();
                nameComponent.name = gameObject.name;
                nameComponent.singleton = singleton;
            }

            if (isSpawner)
            {
                ref var spawnerComponent = ref world.GetComponent<SpawnerComponent>(entity);
                spawnerComponent.area = spawnArea;
            }
        }

        private void OnValidate()
        {
            if (!gameObject.IsSafeToModifyName())
                return;

            if (namingType == NamingType.String)
            {
                if (!string.IsNullOrEmpty(entityName))
                {
                    gameObject.name = $"Spawn({entityName})";
                }
                else
                {
                    gameObject.name = $"Spawn()"; 
                }
            }
        }
    }
}