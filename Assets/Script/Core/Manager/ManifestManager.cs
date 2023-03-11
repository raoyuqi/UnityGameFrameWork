using FrameWork.Core.Mixin;
using System.IO;
using UnityEngine;

namespace FrameWork.Core.Manager
{
    public class ManifestManager : SingletonBase<ManifestManager>
    {
        private AssetBundle m_MainAssetBundle;
        private AssetBundleManifest m_AssetBundleManifest;

        public ManifestManager()
        {
            // TODO: 沙盒路径persistentDataPath
            this.m_MainAssetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "StreamingAssets"));
            if (this.m_MainAssetBundle != null)
                this.m_AssetBundleManifest = this.m_MainAssetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }

        public string[] GetAssetBundleDependencies(string bundleName)
        {
            if (this.m_AssetBundleManifest == null)
                return default;

            var dependencies = this.m_AssetBundleManifest.GetAllDependencies(bundleName);
            return dependencies;
        }
    }
}