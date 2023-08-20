using System;
using Game.Components;
using Game.Components.Abilities;
using Game.DataAssets;
using Game.Utilities;
using Gemserk.Leopotam.Ecs;
using Leopotam.EcsLite;
using UnityEngine;
using UnityEngine.UI;

namespace Abduction101.UI
{
    public class GameHud : MonoBehaviour
    {
        public ElementCountIndicatorUI humanIndicatorUI;
        public ElementCountIndicatorUI alienIndicatorUI;
        public HealthUI healthUI;
        public ObliterationRayUI obliterationRayUI;

        private World world;

        private EcsFilter unitsFilter;
        private EcsFilter mainPlayerFilter;

        public float alienCost = 8;
        
        [NonSerialized]
        public float totalBiomass;

        public Image alienIcon;

        public UnitTypeAsset humansType;
        public UnitTypeAsset invadersType;
        
        private void Start()
        {
            world = World.Instance;
            unitsFilter = world.EcsWorld.Filter<UnitTypeComponent>()
                .Inc<HealthComponent>()
                .Exc<DisabledComponent>().End();
            mainPlayerFilter = world.EcsWorld.Filter<MainPlayerComponent>()
                .Inc<AbilitiesComponent>()
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

                if (unitTypeComponent.unitType == humansType.type)
                {
                    humanCount++;
                }else if (unitTypeComponent.unitType == invadersType.type)
                {
                    alienCount++;
                }
            }
            
            healthUI.factor = 0;
            obliterationRayUI.factor = 0;
            
            // foreach (var e in mainPlayerFilter)
            // {
            //     var entity = world.GetEntity(e);
            //     var healthComponent = entity.Get<HealthComponent>();
            //     
            //     if (healthComponent.aliveType != HealthComponent.AliveType.Alive)
            //         continue;
            //
            //     healthUI.factor = healthComponent.factor;
            // }
            
            foreach (var e in mainPlayerFilter)
            {
                var entity = world.GetEntity(e);
                var obliterationRayAbility = entity.Get<AbilitiesComponent>().GetAbility("Obliteration");
                obliterationRayUI.factor = obliterationRayAbility.cooldown.Progress;
            }
            
            humanIndicatorUI.count = humanCount;
            alienIndicatorUI.count = alienCount;
        }
    }
}