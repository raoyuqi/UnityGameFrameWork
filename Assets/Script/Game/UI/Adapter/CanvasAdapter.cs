using FrameWork.Core.Utils;
using Game.Config;
using UnityEngine;

namespace Game.UI
{
    [AddComponentMenu("CanvasAdapter")]
    public sealed class CanvasAdapter : MonoBehaviour
    {
        private RectTransform m_SafeAreaTrans;
        private RectTransform SafeAreaTrans
        {
            get
            {
                if (this.m_SafeAreaTrans == null)
                {
                    var safeAreaRoot = this.transform.Find("SafeArea");
                    if (safeAreaRoot != null)
                        this.m_SafeAreaTrans = safeAreaRoot.GetComponent<RectTransform>();
                }

                return this.m_SafeAreaTrans;
            }
        }

        private void Awake()
        {
            ApplicationManager.s_OnScreenResolutionChangedEvent += OnScreenResolutionChangedEvent;
        }

        private void Start()
        {
            this.ApplyRectRootAdaption();
        }

        private void OnDestroy()
        {
            ApplicationManager.s_OnScreenResolutionChangedEvent -= OnScreenResolutionChangedEvent;
        }

        private void OnScreenResolutionChangedEvent(int width, int height)
        {
            this.ApplyRectRootAdaption();
        }

        private void ApplyRectRootAdaption()
        {
            if (this.SafeAreaTrans == null)
                return;

            var padding = 0f;
            GameUtils.GetScreenDimensions(out var deviceWidth, out var deviceHeight);

            var ratio = Mathf.Min(AppConst.MaxAspectRatio, 1f * deviceWidth / deviceHeight);
            // 设置安全区域大小
            if (ratio > AppConst.ReferenceAspectRatio)
                padding = AppConst.ReferenceResolution.x * (ratio - AppConst.ReferenceAspectRatio) / 2;

            this.SafeAreaTrans.offsetMin = new Vector2(padding, 0);
            this.SafeAreaTrans.offsetMax = new Vector2(0 - padding, 0);
        }
    }

    static class CanvasAspectRatioAdjustorLogic
    {
        //public static void ApplyScalerAdaption(this CanvasScaler canvasScaler)
        //{
        //    GameUtils.GetScreenPixelDimensions(out var deviceWidth, out var deviceHeight);

        //    var standardWidth = canvasScaler.referenceResolution.x;
        //    var standardHeight = canvasScaler.referenceResolution.y;

        //    var screenWidthScaler = deviceWidth / standardWidth;
        //    var screenHeightScaler = deviceHeight / standardHeight;
        //    float match = screenWidthScaler >= screenHeightScaler ? 1 : 0;
        //    canvasScaler.matchWidthOrHeight = match;
        //    canvasScaler.Handle();
        //}
    }
}