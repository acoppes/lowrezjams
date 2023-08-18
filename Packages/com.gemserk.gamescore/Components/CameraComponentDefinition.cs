﻿using Gemserk.Leopotam.Ecs;
using Gemserk.Leopotam.Ecs.Controllers;
using UnityEngine;

namespace Game.Components
{
    public struct CameraComponent : IEntityComponent
    {
        
    }
    
    public class CameraComponentDefinition : ComponentDefinitionBase
    {
        public override string GetComponentName()
        {
            return nameof(CameraComponent);
        }

        public override void Apply(World world, Entity entity)
        {
            world.AddComponent(entity, new CameraComponent());
        }
    }
}