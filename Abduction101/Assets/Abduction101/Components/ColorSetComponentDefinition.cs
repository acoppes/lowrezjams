using Abduction101.Data;
using Gemserk.Leopotam.Ecs;
using UnityEngine;

namespace Abduction101.Components
{
    public struct ColorSetComponent : IEntityComponent
    {
        public ColorSet[] colorSets;
        public Texture2D lutTexture;
        public MaterialPropertyBlock materialPropertyBlock;
    }
    
    public class ColorSetComponentDefinition : ComponentDefinitionBase
    {
        public ColorSet[] colorSets;
        
        public override string GetComponentName()
        {
            return nameof(ColorSetComponent);
        }

        public override void Apply(World world, Entity entity)
        {
            world.AddComponent(entity, new ColorSetComponent()
            {
                colorSets = colorSets
            });
        }
    }
}