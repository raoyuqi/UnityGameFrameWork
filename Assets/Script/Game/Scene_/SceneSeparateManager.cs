using FrameWork.Core.Attributes;
using FrameWork.Core.SceneSeparate;
using FrameWork.Core.SceneSeparate.Detector;
using FrameWork.Core.SceneSeparate.SceneObject_;
using FrameWork.Core.SceneSeparate.Utils;
using FrameWork.Core.SingletonManager;
using System.Collections.Generic;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Game.Scene
{
    public class SceneSeparateManager : MonoBehaviour
    {
        [Header("场景物体列表")]
        public List<TestSceneObject> LoadObjects;

        public Bounds Bounds;

        [SerializeField, LabelText("是否异步")]
        private bool m_IsAsyn;

        [SerializeField, LabelText("探测器")]
        private SceneDetectorBase Detector;

        public TreeType m_TreeType = TreeType.LinearQuadTree;

        private SceneObjectManager m_ObjectManager;

        void Start()
        {
            this.m_ObjectManager = this.gameObject.GetComponent<SceneObjectManager>();
            if (this.m_ObjectManager == null)
                this.m_ObjectManager = this.gameObject.AddComponent<SceneObjectManager>();

            this.m_ObjectManager.Init(this.Bounds.center, this.Bounds.size, this.m_IsAsyn, this.m_TreeType);


            for (int i = 0; i < this.LoadObjects.Count; i++)
            {
                this.m_ObjectManager.AddSceneBlockObject(this.LoadObjects[i]);
            }
        }

        //void OnGUI()
        //{
        //    GUI.color = Color.red;
        //    GUILayout.Label("test");
        //}

        void Update()
        {
            this.m_ObjectManager.RefreshDetector(this.Detector);
        }

        void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying)
                this.Bounds.DrawBounds(Color.yellow);
        }
    }

    [System.Serializable]
    public class TestSceneObject : ISceneObject
    {
        [SerializeField, Header("包围盒")]
        private Bounds m_Bounds;

        [SerializeField, LabelText("资源路径")]
        private string m_ResPath;

        [SerializeField, LabelText("物体坐标")]
        private Vector3 m_Position;

        [SerializeField, LabelText("物体旋转")]
        private Vector3 m_Rotation;

        [SerializeField, LabelText("物体尺寸")]
        private Vector3 m_Size;

        private GameObject m_LoadedPrefab;

        public Bounds Bounds
        {
            get { return this.m_Bounds; }
        }

        public void OnHide()
        {
            if (this.m_LoadedPrefab != null)
            {
                this.m_LoadedPrefab.SetActive(false);
                //UnityObject.Destroy(this.m_LoadedPrefab);
                //this.m_LoadedPrefab = null;
                //ResourceManager.UnLoad(m_ResPath);
            }
        }

        public bool OnShow(Transform parent)
        {
            if (this.m_LoadedPrefab == null)
            {
                //var preLoadRes = "prefabs/nature/rock_02.prefab";
                var prefab = ResourceManager.Instance.Load<GameObject>(this.m_ResPath);
                this.m_LoadedPrefab = UnityObject.Instantiate(prefab);
                this.m_LoadedPrefab.transform.SetParent(parent);
                this.m_LoadedPrefab.transform.position = m_Position;
                this.m_LoadedPrefab.transform.eulerAngles = m_Rotation;
                this.m_LoadedPrefab.transform.localScale = m_Size;
                return true;
            }

            this.m_LoadedPrefab.SetActive(true);
            return false;
        }

        public TestSceneObject(Bounds bounds, Vector3 position, Vector3 rotation, Vector3 size, string resPath)
        {
            this.m_Bounds = bounds;
            this.m_Position = position;
            this.m_Rotation = rotation;
            this.m_Size = size;
            this.m_ResPath = resPath;
        }
    }
}