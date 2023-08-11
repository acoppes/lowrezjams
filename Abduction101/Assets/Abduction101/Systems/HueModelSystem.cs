using Abduction101.Components;
using Game.Components;
using Gemserk.Leopotam.Ecs;
using Gemserk.Utilities;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Abduction101.Systems
{
    public class HueModelSystem : BaseSystem, IEcsRunSystem, IEntityDestroyedHandler
    {
        readonly EcsFilterInject<Inc<ModelComponent, HueModelComponent>, Exc<DisabledComponent>> filter = default;
        
        public void OnEntityDestroyed(World world, Entity entity)
        {
            if (entity.Has<HueModelComponent>())
            {
                ref var hue = ref entity.Get<HueModelComponent>();
                hue.materialPropertyBlock = null;
            }
        }
        
        public void Run(EcsSystems systems)
        {
            foreach (var e in filter.Value)
            {
                ref var model = ref filter.Pools.Inc1.Get(e);
                ref var hueModel = ref filter.Pools.Inc2.Get(e);

                if (hueModel.materialPropertyBlock == null && model.instance != null && model.instance.spriteRenderer != null)
                {
                    hueModel.materialPropertyBlock = new MaterialPropertyBlock();
                    model.instance.spriteRenderer.GetPropertyBlock(hueModel.materialPropertyBlock, 0);
                    if (model.instance.spriteRenderer.sprite != null)
                    {
                        hueModel.materialPropertyBlock.SetTexture("_MainTex",
                            model.instance.spriteRenderer.sprite.texture);
                    }
                    hueModel.materialPropertyBlock.SetFloat("_Hue", UnityEngine.Random.Range(-0.5f, 0.5f));
                    model.instance.spriteRenderer.SetPropertyBlock(hueModel.materialPropertyBlock);
                }
            }
        }
    }
}