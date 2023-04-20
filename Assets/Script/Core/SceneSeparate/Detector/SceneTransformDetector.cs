using FrameWork.Core.Attributes;
using FrameWork.Core.SceneSeparate.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.Core.SceneSeparate.Detector
{
    // Transform的包围盒触发器
    public sealed class SceneTransformDetector : SceneDetectorBase
    {
        public override bool UseCameraCulling
        {
            get { return false; }
        }

        [SerializeField, LabelText("包围盒尺寸")]
        private Vector3 DetectorSize;

        private Bounds m_Bounds;

        public void RefreshBounds()
        {
            this.m_Bounds.center = this.Position;
            this.m_Bounds.size = this.DetectorSize;
        }

        public override int GetDetectedCode(float x, float y, float z, bool ignoreY)
        {
            this.RefreshBounds();

            var code = 0;
            if (ignoreY)
            {
                var minX = this.m_Bounds.min.x;
                var minZ = this.m_Bounds.min.z;
                var maxX = this.m_Bounds.max.x;
                var maxZ = this.m_Bounds.max.z;

                // 点在包围盒右边
                if (x >= minX && z >= minZ)
                    code |= 1;

                // 点在包围盒下边
                if (x >= minX && z <= maxZ)
                    code |= 2;

                // 点在包围盒上边
                if (x <= maxX && z >= minZ)
                    code |= 4;

                // 点在包围盒左边
                if (x <= maxX && z <= maxZ)
                    code |= 8;
            }
            else
            {
                var minX = m_Bounds.min.x;
                var minY = m_Bounds.min.y;
                var minZ = m_Bounds.min.z;
                var maxX = m_Bounds.max.x;
                var maxY = m_Bounds.max.y;
                var maxZ = m_Bounds.max.z;
                if (x >= minX && y >= minY && z >= minZ)
                    code |= 1;

                if (x >= minX && y >= minY && z <= maxZ)
                    code |= 2;

                if (x >= minX && y <= maxY && z >= minZ)
                    code |= 4;

                if (x >= minX && y <= maxY && z <= maxZ)
                    code |= 8;

                if (x <= maxX && y >= minY && z >= minZ)
                    code |= 16;

                if (x <= maxX && y >= minY && z <= maxZ)
                    code |= 32;

                if (x <= maxX && y <= maxY && z >= minZ)
                    code |= 64;

                if (x <= maxX && y <= maxY && z <= maxZ)
                    code |= 128;
            }

            return code;
        }

        public override bool IsDetected(Bounds bounds)
        {
            this.RefreshBounds();
            return this.m_Bounds.Intersects(bounds);
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            var bound = new Bounds(this.transform.position, this.DetectorSize);
            bound.DrawBounds(Color.green);
        }
#endif
    }
}