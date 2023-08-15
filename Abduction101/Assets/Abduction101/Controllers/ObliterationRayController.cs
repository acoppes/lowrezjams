using Game.Components;
using Game.Components.Abilities;
using Game.Controllers;
using Gemserk.Leopotam.Ecs;
using Gemserk.Leopotam.Ecs.Components;
using Gemserk.Leopotam.Ecs.Controllers;
using Gemserk.Leopotam.Ecs.Events;

namespace Abduction101.Controllers
{
    public class ObliterationRayController : ControllerBase, IInit, IUpdate, IActiveController
    {
        public void OnInit(World world, Entity entity)
        {
            ref var animations = ref entity.Get<AnimationComponent>();
            animations.Play("Start", 1);
            
            entity.Get<ActiveControllerComponent>().TakeControl(entity, this);
        }

        public void OnUpdate(World world, Entity entity, float dt)
        {
            ref var animations = ref entity.Get<AnimationComponent>();
            ref var abilities = ref entity.Get<AbilitiesComponent>();
            var obliterateAbility = abilities.GetAbility("Obliterate");
            
            if (animations.IsPlaying("End") && animations.isCompleted)
            {
                entity.Get<DestroyableComponent>().destroy = true;
                return;
            }
            
            if (animations.IsPlaying("Start") && animations.isCompleted)
            {
                animations.Play("Loop");
                obliterateAbility.Start();
                return;
            }
            
            if (animations.IsPlaying("Loop"))
            {
                // perform damage, search targets, etc..
                foreach (var abilityTarget in obliterateAbility.abilityTargets)
                {
                    if (!abilityTarget.valid)
                        continue;
                    
                    if (abilityTarget.target.entity == entity) 
                        continue;

                    if (abilityTarget.target.entity.Exists() && abilityTarget.target.entity.Has<HealthComponent>())
                    {
                        ref var health = ref abilityTarget.target.entity.Get<HealthComponent>();
                        health.damages.Add(new DamageData()
                        {
                            value = 10000,
                            source = entity
                        });
                    }
                }

                if (!obliterateAbility.isExecuting)
                {
                    animations.Play("End", 1);
                }

            }
        }

        public bool CanBeInterrupted(Entity entity, IActiveController activeController)
        {
            return false;
        }

        public void OnInterrupt(Entity entity, IActiveController activeController)
        {
            throw new System.NotImplementedException();
        }
    }
}