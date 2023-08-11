using Game.Components;
using Game.Utilities;
using Gemserk.Leopotam.Ecs;
using Gemserk.Leopotam.Ecs.Components;
using Gemserk.Leopotam.Ecs.Controllers;
using Gemserk.Leopotam.Ecs.Events;
using UnityEngine;

namespace Abduction101.Controllers
{
    public class ExplosionController : ControllerBase, IUpdate
    {
        public Targeting targeting;
        
        public void OnUpdate(World world, Entity entity, float dt)
        {
            ref var states = ref entity.Get<StatesComponent>();
            var position = entity.Get<PositionComponent>();

            if (!states.HasState("Explosion"))
            {
                var targets = world.GetTargets(new RuntimeTargetingParameters()
                {
                    filter = targeting.targeting,
                    direction = Vector3.right,
                    position = position.value,
                    player = entity.Get<PlayerComponent>().player
                });

                foreach (var target in targets)
                {
                    if (target.entity.Exists())
                    {
                        target.entity.Get<HealthComponent>().damages.Add(new DamageData()
                        {
                            value = 2f,
                            position = position.value,
                            knockback = true,
                            source = entity,
                            vfxDefinition = null
                        });
                    }
                }
                
                states.EnterState("Explosion");
            }
        }
    }
}