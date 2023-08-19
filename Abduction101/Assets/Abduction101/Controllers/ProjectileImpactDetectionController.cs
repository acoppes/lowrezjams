using Game.Components;
using Game.Utilities;
using Gemserk.Leopotam.Ecs;
using Gemserk.Leopotam.Ecs.Controllers;
using Gemserk.Leopotam.Ecs.Events;

namespace Abduction101.Controllers
{
    public class ProjectileImpactDetectionController : ControllerBase, IInit, IDestroyed
    {
        public void OnInit(World world, Entity entity)
        {
            var physics = entity.Get<PhysicsComponent>();
            physics.collisionsEventsDelegate.onCollisionEnter += OnEntityCollision;
        }
        
        public void OnDestroyed(World world, Entity entity)
        {
            var physics = entity.Get<PhysicsComponent>();
            physics.collisionsEventsDelegate.onCollisionEnter -= OnEntityCollision;
        }

        private void OnEntityCollision(World world, Entity entity, IEntityCollisionDelegate.EntityCollision entityCollision)
        {
            // Debug.Log("COLLISION");

            if (entity.Get<ProjectileComponent>().impacted)
            {
                return;
            }

            if (entityCollision.entity.Exists())
            {
                // could search for targets and perform some effect...
                // entityCollision.entity.Get<HealthComponent>().damages.Add(new DamageData()
                // {
                //     value = 1
                // });
                
                // entity.Get<HealthComponent>().damages.Add(new DamageData()
                // {
                //     value = 1000
                // });

                entity.Get<ProjectileComponent>().impacted = true;
                entity.Get<ProjectileComponent>().impactEntity = entityCollision.entity;
            }
        }
    }
}