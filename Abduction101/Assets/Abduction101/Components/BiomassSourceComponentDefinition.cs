using Gemserk.Leopotam.Ecs;

namespace Abduction101.Components
{
    public struct BiomassSourceComponent : IEntityComponent
    {
        public float consumeValue;
        public float spawnCostValue;
    }
    
    public class BiomassSourceComponentDefinition : ComponentDefinitionBase
    {
        public float biomass;
        public float spawnCost;
        
        public override string GetComponentName()
        {
            return nameof(BiomassSourceComponent);
        }

        public override void Apply(World world, Entity entity)
        {
            world.AddComponent(entity, new BiomassSourceComponent()
            {
                consumeValue = biomass,
                spawnCostValue = spawnCost
            });
        }
    }
}