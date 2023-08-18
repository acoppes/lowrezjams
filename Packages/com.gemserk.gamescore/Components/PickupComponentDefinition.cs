﻿using System.Collections.Generic;
using Gemserk.Leopotam.Ecs;

namespace Game.Components
{
    public struct PickupData
    {
        public string name;
        public string type;
        public float value;
    }
    
    public struct PickupComponent : IEntityComponent
    {
        public string name;
        public string type;
        public float value;

        public bool picked;
        public bool wasPicked;

        public bool wasPickedThisFrame => !wasPicked && picked;

        // entities in range to pick up this pickup
        public List<Entity> pickedByEntities;

        public bool autoDestroyOnPickup;
        
        public PickupData ToData()
        {
            return new PickupData
            {
                name = name,
                type = type,
                value = value
            };
        }
    }
    
    public class PickupComponentDefinition : ComponentDefinitionBase
    {
        public new string name;
        
        public string type;
        public float value;

        public bool autoDestroyOnPickup = false;
        
        public override string GetComponentName()
        {
            return nameof(PickupComponent);
        }

        public override void Apply(World world, Entity entity)
        {
            world.AddComponent(entity, new PickupComponent() {
                name = name,
                type = type, 
                value = value,
                autoDestroyOnPickup = autoDestroyOnPickup,
                pickedByEntities = new List<Entity>()
            });
        }
    }
}