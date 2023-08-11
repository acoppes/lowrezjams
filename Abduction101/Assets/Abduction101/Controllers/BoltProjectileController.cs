using Abduction101.Systems;
using Game.Components;
using Gemserk.Leopotam.Ecs;
using Gemserk.Leopotam.Ecs.Controllers;
using UnityEngine;

namespace Abduction101.Controllers
{
    public class BoltProjectileController : ControllerBase, IProjectileImpactEvent
    {
        // TODO: auto kill on impact || auto destroy on impact, etc
        // in system.

        public float damage;
        
        public void OnProjectileImpact(World world, Entity entity)
        {
            ref var health = ref entity.Get<HealthComponent>();
            health.damages.Add(new DamageData()
            {
                // kill projectile
                value = health.total
            });
            
            // apply effect to targets nearby or to impact target 
            var projectile = entity.Get<ProjectileComponent>();
            // Debug.Log($"Collision with: {projectile.impactEntity}");

            if (projectile.impactEntity.Exists())
            {
                projectile.impactEntity.Get<HealthComponent>().damages.Add(new DamageData()
                {
                    value = damage
                });
            }
        }
    }
}