using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class CutoutImage : Image
    {
        public override Material materialForRendering
        {
            get
            {
                var mat = new Material(base.materialForRendering);
                mat.SetInt("_StencilComp", (int)CompareFunction.NotEqual);
                return mat;
            }
        }
    }
}