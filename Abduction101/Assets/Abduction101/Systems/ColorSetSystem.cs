using Abduction101.Components;
using Game.Components;
using Gemserk.Leopotam.Ecs;
using Gemserk.Utilities;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Abduction101.Systems
{
    public class ColorSetSystem : BaseSystem, IEcsRunSystem, IEntityDestroyedHandler
    {
        readonly EcsFilterInject<Inc<ModelComponent, ColorSetComponent>, Exc<DisabledComponent>> filter = default;
        
        // public void OnEntityCreated(World world, Entity entity)
        // {
        //     if (entity.Has<ColorSetComponent>() && entity.Has<ModelComponent>())
        //     {
        //         // return model to original setting
        //         ref var model = ref entity.Get<ModelComponent>()
        //         ref var colorSet = ref filter.Pools.Inc2.Get(e);
        //
        //         if (colorSet.lutTexture == null)
        //         {
        //             
        //         }
        //         
        //     }
        // }
        
        public void OnEntityDestroyed(World world, Entity entity)
        {
            if (entity.Has<ColorSetComponent>() && entity.Has<ModelComponent>())
            {
                // return model to original setting
                ref var model = ref entity.Get<ModelComponent>();
                ref var colorSet = ref entity.Get<ColorSetComponent>();

                if (colorSet.lutTexture != null)
                {
                    Destroy(colorSet.lutTexture);
                    colorSet.lutTexture = null;
                }

                // if (model.instance != null)
                // {
                //     model.instance.spriteRenderer.sharedMaterial = 
                // }
            }
        }
        
        public void Run(EcsSystems systems)
        {
            foreach (var e in filter.Value)
            {
                ref var model = ref filter.Pools.Inc1.Get(e);
                ref var colorSet = ref filter.Pools.Inc2.Get(e);

                if (colorSet.lutTexture == null && model.instance != null && model.instance.spriteRenderer != null)
                {
                    var lutColors = new Color[]
                    {
                        colorSet.body.colors.Random(),
                        colorSet.skin.colors.Random(),
                        colorSet.hair.colors.Random()
                    };
                    
                    colorSet.lutTexture = new Texture2D(lutColors.Length, 1, TextureFormat.ARGB32, false);
                    colorSet.lutTexture.filterMode = FilterMode.Point;
                    colorSet.lutTexture.wrapMode = TextureWrapMode.Clamp;
        
                    colorSet.lutTexture.SetPixels(lutColors);
                    colorSet.lutTexture.Apply();
                    
                    colorSet.materialPropertyBlock = new MaterialPropertyBlock();
                    model.instance.spriteRenderer.GetPropertyBlock(colorSet.materialPropertyBlock, 0);
                    // colorSet.materialPropertyBlock.SetTexture("_MainTex", model.instance.spriteRenderer.sprite.texture);
                    colorSet.materialPropertyBlock.SetTexture("_LutTex", colorSet.lutTexture);
                    model.instance.spriteRenderer.SetPropertyBlock(colorSet.materialPropertyBlock);
                }
            }
        }



    }
}