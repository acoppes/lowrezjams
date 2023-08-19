using MyBox;
using UnityEngine;

public class GenerateLutTextureTest : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;

    public Color[] testLutColors;
    
    public Texture2D lutTexture;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnDestroy()
    {
        if (lutTexture != null)
        {
            DestroyImmediate(lutTexture);
        }
    }

    [ButtonMethod]
    void RegenerateLut()
    {
        if (lutTexture != null)
        {
            DestroyImmediate(lutTexture);
            lutTexture = null;
        }

        if (spriteRenderer == null)
        {
            return;
        }

        lutTexture = new Texture2D(testLutColors.Length, 1, TextureFormat.ARGB32, false);
        lutTexture.filterMode = FilterMode.Point;
        lutTexture.wrapMode = TextureWrapMode.Clamp;
        
        lutTexture.SetPixels(testLutColors);
        lutTexture.Apply();
        
        // copy of the material
        // material = spriteRenderer.material;
        var propertyBlock = new MaterialPropertyBlock();
        spriteRenderer.GetPropertyBlock(propertyBlock, 0);
        propertyBlock.SetTexture("_MainTex", spriteRenderer.sprite.texture);
        propertyBlock.SetTexture("_LutTex", lutTexture);
        spriteRenderer.SetPropertyBlock(propertyBlock);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
