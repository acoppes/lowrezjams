using System.Collections.Generic;
using Game.Components;
using Game.Components.Abilities;
using Game.Controllers;
using Gemserk.Leopotam.Ecs;
using Gemserk.Leopotam.Ecs.Components;
using Gemserk.Leopotam.Ecs.Controllers;
using Gemserk.Leopotam.Ecs.Events;
using Gemserk.Utilities;
using MyBox;
using UnityEngine;

namespace Abduction101.Controllers
{
    public class SpawnerStateController : ControllerBase, IUpdate, IActiveController
    {
        public MinMaxInt spawnCount;
        public List<Object> definitions;
        
        public void OnUpdate(World world, Entity entity, float dt)
        {
            ref var abilities = ref entity.Get<AbilitiesComponent>();
            var spawnAbility = abilities.GetAbility("Spawn");
            ref var states = ref entity.Get<StatesComponent>();
            ref var activeController = ref entity.Get<ActiveControllerComponent>();
            
            ref var spawner = ref entity.Get<SpawnerComponent>();

            if (states.HasState("Spawning"))
            {
                if (spawner.pending.Count == 0)
                {
                    ExitState(world, entity);
                }

                return;
            }
            
            if (spawnAbility.isReady && activeController.CanInterrupt(entity, this))
            {
                EnterState(world, entity);
            }
        }

        private void EnterState(World world, Entity entity)
        {
            ref var abilities = ref entity.Get<AbilitiesComponent>();
            ref var states = ref entity.Get<StatesComponent>();
            ref var activeController = ref entity.Get<ActiveControllerComponent>();
            
            var spawnAbility = abilities.GetAbility("Spawn");
            
            // TODO: play spawn animation if it has any (could open door if building for example)

            var count = spawnCount.RandomInRange();
            
            if (count > 0)
            {
                activeController.TakeControl(entity, this);
                spawnAbility.Start();
                states.EnterState("Spawning");

                var spawnPackData = new SpawnPackData()
                {
                    name = string.Empty,
                    definitions = new List<IEntityDefinition>()
                };

                for (int i = 0; i < count; i++)
                {
                    spawnPackData.definitions.Add(definitions.GetRandom().GetInterface<IEntityDefinition>());
                }
                
                ref var spawner = ref entity.Get<SpawnerComponent>();
                spawner.pending.Add(spawnPackData);
            }
            else
            {
                spawnAbility.Stop(Ability.StopType.Cancelled);
            }
            

        }
        
        private void ExitState(World world, Entity entity)
        {
            ref var abilities = ref entity.Get<AbilitiesComponent>();
            ref var states = ref entity.Get<StatesComponent>();
            ref var activeController = ref entity.Get<ActiveControllerComponent>();
            
            // TODO: resume idle animation?

            var spawnAbility = abilities.GetAbility("Spawn");
            activeController.ReleaseControl(this);
            
            spawnAbility.Stop(Ability.StopType.Completed);
            
            states.ExitState("Spawning");
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