using Gemserk.Leopotam.Ecs;
using Gemserk.Utilities;

namespace Abduction101.Components
{
    public struct BiomassContainerComponent : IEntityComponent
    {
        public Cooldown value;
    }
    
    public class BiomassContainerComponentDefinition : ComponentDefinitionBase
    {
        public float total;
        public float startingValue;
        
        public override string GetComponentName()
        {
            return nameof(BiomassContainerComponent);
        }

        public override void Apply(World world, Entity entity)
        {
            world.AddComponent(entity, new BiomassContainerComponent()
            {
                value = new Cooldown(total)
                {
                    current = startingValue
                }
            });
        }
    }
}