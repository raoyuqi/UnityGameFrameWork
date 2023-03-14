using FrameWork.Core.Utils;
using System.IO;
using UnityEngine;

namespace FrameWork.Core.AssetsLoader
{
    public sealed class ManifestManager
    {
        private AssetBundle m_MainAssetBundle;
        private AssetBundleManifest m_AssetBundleManifest;

        public ManifestManager()
        {
            // TODO: 沙盒路径persistentDataPath
            this.m_MainAssetBundle = AssetBundle.LoadFromFile(
                Path.Combine(PathTool.GetAssetsBundleStreamingPath(), "AssetBundle")
            );
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