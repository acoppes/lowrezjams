using Game.Components;
using Game.Utilities;
using Gemserk.Leopotam.Ecs;
using Gemserk.Utilities;
using Leopotam.EcsLite;
using UnityEngine;

namespace Game.Systems
{
    public class PhysicsCreationSystem : BaseSystem, IEntityCreatedHandler, IEntityDestroyedHandler, IEcsInitSystem
    {
        private const string GeneratedColliderName = "GeneratedCollider";
        
        public PhysicMaterial defaultMaterial;
        
#if GEMSERK_PARENTS_DEBUG && UNITY_EDITOR
        private GameObject instancesParent;
#endif
        
        public void Init(EcsSystems systems)
        {
#if GEMSERK_PARENTS_DEBUG && UNITY_EDITOR
            instancesParent = GameObject.Find("~PhysicsObjects");
            if (instancesParent == null)
            {
                instancesParent = new GameObject("~PhysicsObjects");
            }
#endif
        }

        private Collider CreateCollider(int layer, PhysicsComponent physicsComponent)
        {
            if (physicsComponent.shapeType == PhysicsComponent.ShapeType.Circle)
            {
                var colliderObject = new GameObject(GeneratedColliderName);
                colliderObject.layer = layer;
                
                // colliderObject.transform.parent = physicsComponent.transform;

                var collider = colliderObject.AddComponent<SphereCollider>();
                collider.isTrigger = physicsComponent.isTrigger;
                collider.radius = physicsComponent.size.x;
                
                if (physicsComponent.centerType == PhysicsComponent.CenterType.CenterOnBase)
                {
                    collider.center = new Vector3(0, collider.radius, 0);
                }
                else
                {
                    collider.center = physicsComponent.center;
                }
                
                collider.sharedMaterial = defaultMaterial;
                
                // colliderObject.transform.localPosition = new Vector3(0, physicsComponent.size.y, 0);
                // colliderObject.AddComponent<PhysicsCollisionsDelegate>();

                return collider;
            }

            if (physicsComponent.shapeType == PhysicsComponent.ShapeType.Box)
            {
                var colliderObject = new GameObject(GeneratedColliderName);
                colliderObject.layer = layer;
                // colliderObject.transform.parent = physicsGameObject.transform;
                    
                var collider = colliderObject.AddComponent<BoxCollider>();
                collider.isTrigger = physicsComponent.isTrigger;
                collider.size = physicsComponent.size;
                
                if (physicsComponent.centerType == PhysicsComponent.CenterType.CenterOnBase)
                {
                    collider.center = new Vector3(0, collider.size.y * 0.5f, 0);
                }
                else
                {
                    collider.center = physicsComponent.center;
                }

                collider.sharedMaterial = defaultMaterial;
                
                // colliderObject.transform.localPosition = new Vector3(0, physicsComponent.size.y / 2, 0);
                // colliderObject.AddComponent<PhysicsCollisionsDelegate>();
                
                return collider;
            }
            
            if (physicsComponent.shapeType == PhysicsComponent.ShapeType.Capsule)
            {
                var colliderObject = new GameObject(GeneratedColliderName);
                colliderObject.layer = layer;
                // colliderObject.transform.parent = physicsGameObject.transform;
                    
                var collider = colliderObject.AddComponent<CapsuleCollider>();
                collider.isTrigger = physicsComponent.isTrigger;
                collider.radius = physicsComponent.size.x;
                collider.height = physicsComponent.size.y;
                
                if (physicsComponent.centerType == PhysicsComponent.CenterType.CenterOnBase)
                {
                    collider.center = new Vector3(0, physicsComponent.size.y / 2, 0);
                }
                else
                {
                    collider.center = physicsComponent.center;
                }
                
                collider.sharedMaterial = defaultMaterial;

                // colliderObject.transform.localPosition = new Vector3(0, physicsComponent.size.y / 2, 0);
                // colliderObject.AddComponent<PhysicsCollisionsDelegate>();
                
                return collider;
            }

            return null;
        }
       
        public void OnEntityCreated(World world, Entity entity)
        {
            if (world.HasComponent<PhysicsComponent>(entity))
            {
                ref var physicsComponent = ref world.GetComponent<PhysicsComponent>(entity);

                if (physicsComponent.prefab != null)
                {
                    physicsComponent.gameObject = GameObject.Instantiate(physicsComponent.prefab);
                    physicsComponent.gameObject.SetActive(true);
                    
                    physicsComponent.body = physicsComponent.gameObject.GetComponent<Rigidbody>();
                    
                    var entityReference = physicsComponent.gameObject.AddComponent<EntityReference>();
                    entityReference.entity = entity;
                }
                else
                {
                    physicsComponent.gameObject = new GameObject("~PhysicsObject");
                    
    #if GEMSERK_PARENTS_DEBUG && UNITY_EDITOR
                    physicsComponent.gameObject.transform.parent = instancesParent.transform;
    #endif

                    var entityReference = physicsComponent.gameObject.AddComponent<EntityReference>();
                    entityReference.entity = entity;

                    var layer = physicsComponent.isStatic ? LayerMask.NameToLayer("StaticObstacle") : 
                        LayerMask.NameToLayer("DynamicObstacle");
                    
                    if (physicsComponent.isStatic)
                    {
                        physicsComponent.body = null;
                        physicsComponent.gameObject.isStatic = true;
                     
                        // obstacle.body.constraints = RigidbodyConstraints.FreezeAll;
                    }
                    else
                    {
                        physicsComponent.body = physicsComponent.gameObject.AddComponent<Rigidbody>();

                        // physicsComponent.body.drag = 0;
                        physicsComponent.body.angularDrag = 10;
                        physicsComponent.body.useGravity = false;
                        physicsComponent.body.mass = physicsComponent.mass;

                        physicsComponent.body.constraints = RigidbodyConstraints.FreezeRotation;
                        physicsComponent.body.collisionDetectionMode = CollisionDetectionMode.Continuous;

                        if (world.HasComponent<PositionComponent>(entity))
                        {
                            var position = world.GetComponent<PositionComponent>(entity);
                            physicsComponent.body.position = position.value;
                        }
                    }

                    if (physicsComponent.colliderType.HasColliderType(PhysicsComponent.ColliderType.CollideWithDynamicObstacles))
                    {
                        physicsComponent.obstacleCollider = CreateCollider(layer, physicsComponent);
                        physicsComponent.obstacleCollider.transform.SetParent(physicsComponent.gameObject.transform, false);
                    }

                    if (!physicsComponent.isStatic)
                    {
                        if (physicsComponent.colliderType.HasColliderType(PhysicsComponent.ColliderType.CollideWithStaticObstacles))
                        {
                            physicsComponent.collideWithStaticCollider =
                                CreateCollider(LayerMask.NameToLayer("CollideWithStaticObstacles"), physicsComponent);
                            physicsComponent.collideWithStaticCollider.transform.SetParent(physicsComponent.gameObject.transform, false);
                        }
                    }
                }

                if (physicsComponent.body != null)
                {
                    physicsComponent.gameObject.AddComponent<PhysicsCollisionsDelegate>();
                    physicsComponent.collisionsEventsDelegate =
                        physicsComponent.gameObject.AddComponent<EntityCollisionDelegate>();
                    physicsComponent.collisionsEventsDelegate.world = world;
                    physicsComponent.collisionsEventsDelegate.entity = world.GetEntity(entity);
                }
            }
        }
        
        public void OnEntityDestroyed(World world, Entity entity)
        {
            if (world.HasComponent<PhysicsComponent>(entity))
            {
                ref var physicsComponent = ref world.GetComponent<PhysicsComponent>(entity);
                
                if (physicsComponent.collisionsEventsDelegate != null)
                {
                    physicsComponent.collisionsEventsDelegate.onCollisionEnter = null;
                }

                if (physicsComponent.gameObject != null)
                {
                    GameObject.Destroy(physicsComponent.gameObject);
                }

                physicsComponent.gameObject = null;
                physicsComponent.body = null;
                physicsComponent.obstacleCollider = null;
                physicsComponent.collideWithStaticCollider = null;
            }
        }
    }
}