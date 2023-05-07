using FrameWork.Core.Attributes;
using FrameWork.Core.SceneSeparate.Utils;
using UnityEngine;

public class SceneObjectTag : MonoBehaviour
{
    [SerializeField, LabelText("包围盒中心")]
    private Vector3 Center;

    [SerializeField, LabelText("包围盒尺寸")]
    private Vector3 Size;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        var bound = new Bounds(this.Center, this.Size);
        bound.DrawBounds(Color.yellow);
    }
#endif
}
