using UnityEngine;
using UnityEngine.UI;

namespace Abduction101.UI
{
    public class ObliterationRayUI : MonoBehaviour
    {
        public Image bar;

        public float factor;
        
        private void LateUpdate()
        {
            bar.fillAmount = factor;
        }
    }
}