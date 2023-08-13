using UnityEngine;
using UnityEngine.UI;

namespace Abduction101.UI
{
    public class CameraEffectsControls : MonoBehaviour
    {
        public bool defaultScanLines = true;
        public bool defaultCrt = true;
        public bool defaultVignette = true;

        public float scanLinesWidth = 1;
        public float vignetteSize = 1.8f;
        public float crtBend = 3.5f;
        
        public RawImage rawImage;

        private bool scanLinesOn;
        private bool crtOn;
        private bool vignetteOn;

        private Material material;
        
        // Start is called before the first frame update
        private void Start()
        {
            scanLinesOn = defaultScanLines;
            crtOn = defaultCrt;
            vignetteOn = defaultVignette;

            material = new Material(rawImage.material);
            ReloadMaterial();
        }

        private void ReloadMaterial()
        {
            var scanLinesWidth = this.scanLinesWidth;
            var vignetteSize = this.vignetteSize;
            var crtBend = this.crtBend;

            if (!scanLinesOn)
            {
                scanLinesWidth = 0;
            }

            if (!vignetteOn)
            {
                vignetteSize = 0;
            }
            
            if (!crtOn)
            {
                crtBend = 1000;
            }
            
            material.SetFloat("_LineWidth", scanLinesWidth);
            material.SetFloat("_VignetteSize", vignetteSize);
            material.SetFloat("_CRTBend", crtBend);
            rawImage.material = material;
        }

    }
}
