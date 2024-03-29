﻿using FrameWork.Core.Modules.Signal;
using FrameWork.Core.SingletonManager;
using Game.Config;
using Game.Scene;
using System;
using System.Collections;
using UnityEngine;

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
                    MonoBehaviourRuntime.Instance.StartCoroutine(this.LoadSceneAsync(args[0] as IGameScene));
            });
        }

        private IEnumerator LoadSceneAsync(IGameScene scene)
        {
            if (this.m_CurrentScene != null)
                this.m_CurrentScene.Exite();

            this.m_CurrentScene = scene;
            yield return new WaitForEndOfFrame();

            UIManager.Instance.OpenPanel<LoadingPanel>();
            yield return null;

            this.m_CurrentScene.PreLoadResourceCallBack += PreLoadResourceProgress;
            yield return this.m_CurrentScene.PreLoad();

            var fileSuffix = AppConst.IsAssetBundle ? "unity3d" : "unity";
            var path = $"scenes/{ scene.Name.ToLower() }.{ fileSuffix }";
            yield return ResourceManager.Instance.LoadSceneAsync(path, (progress) =>
            {
                this.LoadSceneProgress(progress);
                if (progress == 1)
                {
                    //this.OnLoadSceneComplete?.Invoke(this.m_LoadSceneName);
                    GlobalSignalSystem.Instance.RaiseSignal(GlobalSignal.TransFinished);
                    this.m_CurrentScene.Enter();
                }
            });
        }

        private void PreLoadResourceProgress(float progress)
        {
            this.OnLoadSceneProgress?.Invoke(progress * 0.9f);
        }

        private void LoadSceneProgress(float progress)
        {
            this.OnLoadSceneProgress?.Invoke(0.9f + progress * 0.1f);
        }

        public void Dispose()
        {
            this.OnLoadSceneProgress = null;
            this.OnLoadSceneComplete = null;
        }
    }
}
