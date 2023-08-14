using System;
using Abduction101.Components;
using Game.Components;
using Gemserk.Leopotam.Ecs;
using Leopotam.EcsLite;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Abduction101.UI
{
    public class GameHud : MonoBehaviour
    {
        public ElementCountIndicatorUI humanIndicatorUI;
        public ElementCountIndicatorUI alienIndicatorUI;

        private World world;

        private EcsFilter unitsFilter;

        public float alienCost = 8;
        
        [NonSerialized]
        public float totalBiomass;

        public Image alienIcon;
        
        private void Start()
        {
            world = World.Instance;
            unitsFilter = world.EcsWorld.Filter<UnitTypeComponent>()
                .Inc<HealthComponent>()
                .Exc<DisabledComponent>().End();
        }

        private void LateUpdate()
        {
            alienIcon.fillAmount = Mathf.Clamp01(totalBiomass / alienCost);
            // alienIcon.color = totalBiomass < alienCost ? new Color(1, 1, 1, 0.25f) : Color.white;
            
            if (world == null)
            {
                humanIndicatorUI.count = 0;
                alienIndicatorUI.count = 0;
                return;
            }

            var humanCount = 0;
            var alienCount = 0;

            foreach (var e in unitsFilter)
            {
                var entity = world.GetEntity(e);
                var unitTypeComponent = entity.Get<UnitTypeComponent>();
                var healthComponent = entity.Get<HealthComponent>();
                
                if (healthComponent.aliveType != HealthComponent.AliveType.Alive)
                    continue;

                if (unitTypeComponent.type == 0)
                {
                    humanCount++;
                }else if (unitTypeComponent.type == 1)
                {
                    alienCount++;
                }
            }
            
            humanIndicatorUI.count = humanCount;
            alienIndicatorUI.count = alienCount;
        }
    }
}