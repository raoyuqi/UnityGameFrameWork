using FrameWork.Core.Modules.Signal;
using FrameWork.Core.SingletonManager;
using Game.Config;
using Game.Scene;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

namespace FrameWork.Core.Service
{
    public sealed class SceneService : IGameService
    {
        public event Action<float> OnLoadSceneProgress;
        public event Action<string> OnLoadSceneComplete;

        private IGameScene m_CurrentScene;

        public void Initialize()
        {
            GlobalSignalSystem.Instance.RegisterSignal(GlobalSignal.TransScene, (args) => {
                if (args.Length > 0)
                {
                    if (this.m_CurrentScene != null)
                        this.m_CurrentScene.Exite();

                    var fileSuffix = AppConst.IsAssetBundle ? "unity3d" : "unity";
                    MonoBehaviourRuntime.Instance.StartCoroutine(ResourceManager.Instance.LoadSceneAsync($"scenes/loading.{ fileSuffix }", (progress) =>
                    {
                        if (progress == 1)
                        {
                            this.m_CurrentScene = args[0] as IGameScene;
                            var path = $"scenes/{ this.m_CurrentScene.Name.ToLower() }.{ fileSuffix }";
                            MonoBehaviourRuntime.Instance.StartCoroutine(this.LoadSceneAsync(path));
                        }
                    }, LoadSceneMode.Additive));
                }
            });
        }

        private IEnumerator LoadSceneAsync(string path)
        {
            yield return ResourceManager.Instance.LoadSceneAsync(path, (progress) =>
            {
                this.OnLoadSceneProgress?.Invoke(progress);
                if (progress == 1)
                {
                    //this.OnLoadSceneComplete?.Invoke(this.m_LoadSceneName);
                    this.m_CurrentScene.Enter();
                }
            });
        }

        public void Dispose()
        {
            this.OnLoadSceneProgress = null;
            this.OnLoadSceneComplete = null;
        }
    }
}
