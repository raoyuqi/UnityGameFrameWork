﻿using UnityEngine;

namespace FrameWork.Core.SceneSeparate.Detector
{
    public abstract class SceneDetectorBase : MonoBehaviour, IDetector
    {
        public Vector3 Position
        {
            get { return this.transform.position; }
        }

        public abstract bool UseCameraCulling { get; }

        public abstract int GetDetectedCode(float x, float y, float z, bool ignoreY);

        // 两个包围盒是否相交
        public abstract bool IsDetected(Bounds bounds);
    }
}