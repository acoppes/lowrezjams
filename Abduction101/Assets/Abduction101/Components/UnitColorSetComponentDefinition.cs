using Abduction101.Data;
using Gemserk.Leopotam.Ecs;
using UnityEngine;

namespace Abduction101.Components
{
    public struct ColorSetComponent : IEntityComponent
    {
        public ColorSet hair;
        public ColorSet skin;
        public ColorSet body;

        public Texture2D lutTexture;
        public MaterialPropertyBlock materialPropertyBlock;
    }
    
    public class UnitColorSetComponentDefinition : ComponentDefinitionBase
    {
        public ColorSet hair;
        public ColorSet skin;
        public ColorSet body;
        
        public override string GetComponentName()
        {
            return nameof(ColorSetComponent);
        }

        public override void Apply(World world, Entity entity)
        {
            world.AddComponent(entity, new ColorSetComponent()
            {
                hair = hair,
                skin = skin,
                body = body
            });
        }
    }
}