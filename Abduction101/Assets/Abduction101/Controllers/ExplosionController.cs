using System.Collections.Generic;
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

        public float damage = 2.0f;

        private List<Target> targets = new List<Target>();
        
        public void OnUpdate(World world, Entity entity, float dt)
        {
            ref var states = ref entity.Get<StatesComponent>();
            var position = entity.Get<PositionComponent>();

            if (!states.HasState("Explosion"))
            {
                targets.Clear();
                
                var count = world.GetTargets(new RuntimeTargetingParameters()
                {
                    filter = targeting.targeting,
                    direction = Vector3.right,
                    position = position.value,
                    alliedPlayersBitmask = entity.Get<PlayerComponent>().GetAlliedPlayers()
                }, targets);

                foreach (var target in targets)
                {
                    if (target.entity.Exists())
                    {
                        target.entity.Get<HealthComponent>().damages.Add(new DamageData()
                        {
                            value = damage,
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