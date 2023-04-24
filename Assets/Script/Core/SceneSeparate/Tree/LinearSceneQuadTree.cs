using FrameWork.Core.SceneSeparate.Detector;
using FrameWork.Core.SceneSeparate.SceneObject_;
using FrameWork.Core.SceneSeparate.Utils;
using UnityEngine;


namespace FrameWork.Core.SceneSeparate.Tree
{
    public class LinearSceneQuadTree<T> : LinearSceneTree<T> where T : ISceneObject, ISOLinkedListNode
    {
        private float m_DeltaWidth;
        private float m_DeltaHeight;

        public LinearSceneQuadTree(Vector3 center, Vector3 size, int maxDepth) : base(center, size, maxDepth)
        {
            this.m_DeltaWidth = base.m_Bounds.size.x / base.m_Cols;
            this.m_DeltaHeight = base.m_Bounds.size.z / base.m_Cols;
        }

        public override void Add(T item)
        {
            if (item == null)
                return;

            if (base.m_Bounds.Intersects(item.Bounds))
            {
                if (m_MaxDepth == 0)
                {
                    if (base.m_Nodes.ContainsKey(0) == false)
                        base.m_Nodes[0] = new LinearSceneTreeLeaf<T>();

                    var node = base.m_Nodes[0].Insert(item);
                    item.SetLinkedListNode<T>(0, node);
                }
                else
                {
                    this.InsertToNode(item, 0, m_Bounds.center.x, m_Bounds.center.z, m_Bounds.size.x, m_Bounds.size.z);
                }
            }
        }

#if UNITY_EDITOR
        public override void DrawTree(Color treeMinDepthColor, Color treeMaxDepthColor, Color objColor, Color hitObjColor, int drawMinDepth, int drawMaxDepth, bool drawObj)
        {
            this.DrawNodeGizmos(treeMinDepthColor, treeMaxDepthColor, objColor, hitObjColor, drawMinDepth, drawMaxDepth, drawObj,
            0, new Vector2(m_Bounds.center.x, m_Bounds.center.z), new Vector2(m_Bounds.size.x, m_Bounds.size.z));
        }

        private bool DrawNodeGizmos(Color treeMinDepthColor, Color treeMaxDepthColor, Color objColor, Color hitObjColor, int drawMinDepth, int drawMaxDepth, bool drawObj, int depth, Vector2 center, Vector2 size)
        {
            if (depth < drawMinDepth || depth > drawMaxDepth)
                return false;

            float d = ((float)depth) / base.m_MaxDepth;
            var color = Color.Lerp(treeMinDepthColor, treeMaxDepthColor, d);
            if (depth == base.m_MaxDepth)
            {
                var m = Morton2FromWorldPos(center.x, center.y);
                if (m_Nodes.ContainsKey(m) && m_Nodes[m] != null)
                {
                    if (m_Nodes[m].DrawNode(objColor, hitObjColor, drawObj))
                    {
                        var b = new Bounds(new Vector3(center.x, m_Bounds.center.y, center.y), new Vector3(size.x, m_Bounds.size.y, size.y));
                        b.DrawBounds(color);
                        return true;
                    }
                }
            }
            else
            {
                var draw = false;
                var sx = size.x * 0.5f;
                var sz = size.y * 0.5f;
                draw = draw | DrawNodeGizmos(treeMinDepthColor, treeMaxDepthColor, objColor, hitObjColor, drawMinDepth, drawMaxDepth, drawObj, depth + 1, new Vector2(center.x - sx * 0.5f, center.y - sz * 0.5f), new Vector2(sx, sz));
                draw = draw | DrawNodeGizmos(treeMinDepthColor, treeMaxDepthColor, objColor, hitObjColor, drawMinDepth, drawMaxDepth, drawObj, depth + 1, new Vector2(center.x + sx * 0.5f, center.y - sz * 0.5f), new Vector2(sx, sz));
                draw = draw | DrawNodeGizmos(treeMinDepthColor, treeMaxDepthColor, objColor, hitObjColor, drawMinDepth, drawMaxDepth, drawObj, depth + 1, new Vector2(center.x - sx * 0.5f, center.y + sz * 0.5f), new Vector2(sx, sz));
                draw = draw | DrawNodeGizmos(treeMinDepthColor, treeMaxDepthColor, objColor, hitObjColor, drawMinDepth, drawMaxDepth, drawObj, depth + 1, new Vector2(center.x + sx * 0.5f, center.y + sz * 0.5f), new Vector2(sx, sz));

                if (draw)
                {
                    var b = new Bounds(new Vector3(center.x, m_Bounds.center.y, center.y), new Vector3(size.x, m_Bounds.size.y, size.y));
                    b.DrawBounds(color);
                }
                return draw;
            }

            return false;
        }
#endif
        public override void Trigger(IDetector detector, TriggerHandle<T> handle)
        {
            if (handle == null)
                return;

            if (detector.UseCameraCulling)
            {
                var code = new TreeCullingCode()
                {
                    leftbottomback = detector.GetDetectedCode(m_Bounds.min.x, m_Bounds.min.y, m_Bounds.min.z, true),
                    leftbottomforward = detector.GetDetectedCode(m_Bounds.min.x, m_Bounds.min.y, m_Bounds.max.z, true),
                    lefttopback = detector.GetDetectedCode(m_Bounds.min.x, m_Bounds.max.y, m_Bounds.min.z, true),
                    lefttopforward = detector.GetDetectedCode(m_Bounds.min.x, m_Bounds.max.y, m_Bounds.max.z, true),
                    rightbottomback = detector.GetDetectedCode(m_Bounds.max.x, m_Bounds.min.y, m_Bounds.min.z, true),
                    rightbottomforward = detector.GetDetectedCode(m_Bounds.max.x, m_Bounds.min.y, m_Bounds.max.z, true),
                    righttopback = detector.GetDetectedCode(m_Bounds.max.x, m_Bounds.max.y, m_Bounds.min.z, true),
                    righttopforward = detector.GetDetectedCode(m_Bounds.max.x, m_Bounds.max.y, m_Bounds.max.z, true),
                };
                this.TriggerToNodeByCamera(detector, handle, 0, code, m_Bounds.center.x, m_Bounds.center.z, m_Bounds.size.x, m_Bounds.size.z);
            }
            else
            {
                if (base.m_MaxDepth == 0)
                {
                    if (base.m_Nodes.ContainsKey(0) && base.m_Nodes[0] != null)
                    {
                        base.m_Nodes[0].Trigger(detector, handle);
                    }
                }
                else
                {
                    TriggerToNode(detector, handle, 0, base.m_Bounds.center.x, base.m_Bounds.center.z, base.m_Bounds.size.x, base.m_Bounds.size.z);
                }
            }
        }

        private uint Morton2FromWorldPos(float x, float z)
        {
            var px = (uint)Mathf.FloorToInt((x - base.m_Bounds.min.x) / this.m_DeltaWidth);
            var pz = (uint)Mathf.FloorToInt((z - base.m_Bounds.min.z) / this.m_DeltaHeight);
            return this.Morton2(px, pz);
        }

        private uint Morton2(uint x, uint y)
        {
            return (this.Part1By1(y) << 1) + this.Part1By1(x);
        }

        private uint Part1By1(uint n)
        {
            n = (n ^ (n << 8)) & 0x00ff00ff;
            n = (n ^ (n << 4)) & 0x0f0f0f0f;
            n = (n ^ (n << 2)) & 0x33333333;
            n = (n ^ (n << 1)) & 0x55555555;
            return n;
        }

        private bool InsertToNode(T obj, int depth, float centerx, float centerz, float sizex, float sizez)
        {
            if (depth == m_MaxDepth)
            {
                uint m = Morton2FromWorldPos(centerx, centerz);
                if (m_Nodes.ContainsKey(m) == false)
                    m_Nodes[m] = new LinearSceneTreeLeaf<T>();

                var node = m_Nodes[m].Insert(obj);
                obj.SetLinkedListNode<T>(m, node);
                return true;
            }
            else
            {
                int colider = 0;
                float minx = obj.Bounds.min.x;
                float minz = obj.Bounds.min.z;
                float maxx = obj.Bounds.max.x;
                float maxz = obj.Bounds.max.z;

                if (minx <= centerx && minz <= centerz)
                    colider |= 1;
                if (minx <= centerx && maxz >= centerz)
                    colider |= 2;
                if (maxx >= centerx && minz <= centerz)
                    colider |= 4;
                if (maxx >= centerx && maxz >= centerz)
                    colider |= 8;
                float sx = sizex * 0.5f, sz = sizez * 0.5f;

                bool insertresult = false;
                if ((colider & 1) != 0)
                    insertresult = insertresult | InsertToNode(obj, depth + 1, centerx - sx * 0.5f, centerz - sz * 0.5f, sx, sz);
                if ((colider & 2) != 0)
                    insertresult = insertresult | InsertToNode(obj, depth + 1, centerx - sx * 0.5f, centerz + sz * 0.5f, sx, sz);
                if ((colider & 4) != 0)
                    insertresult = insertresult | InsertToNode(obj, depth + 1, centerx + sx * 0.5f, centerz - sz * 0.5f, sx, sz);
                if ((colider & 8) != 0)
                    insertresult = insertresult | InsertToNode(obj, depth + 1, centerx + sx * 0.5f, centerz + sz * 0.5f, sx, sz);
                return insertresult;
            }
        }

        private void TriggerToNodeByCamera(IDetector detector, TriggerHandle<T> handle, int depth, TreeCullingCode cullingCode, float centerx, float centerz, float sizex, float sizez)
        {
            if (cullingCode.IsCulled())
                return;
            if (depth == m_MaxDepth)
            {
                uint m = Morton2FromWorldPos(centerx, centerz);
                if (m_Nodes.ContainsKey(m) && m_Nodes[m] != null)
                {
                    m_Nodes[m].Trigger(detector, handle);
                }
            }
            else
            {
                float sx = sizex * 0.5f, sz = sizez * 0.5f;
                int leftbottommiddle = detector.GetDetectedCode(centerx - sx, m_Bounds.min.y, centerz, true);
                int middlebottommiddle = detector.GetDetectedCode(centerx, m_Bounds.min.y, centerz, true);
                int rightbottommiddle = detector.GetDetectedCode(centerx + sx, m_Bounds.min.y, centerz, true);
                int middlebottomback = detector.GetDetectedCode(centerx, m_Bounds.min.y, centerz - sz, true);
                int middlebottomforward = detector.GetDetectedCode(centerx, m_Bounds.min.y, centerz + sz, true);

                int lefttopmiddle = detector.GetDetectedCode(centerx - sx, m_Bounds.max.y, centerz, true);
                int middletopmiddle = detector.GetDetectedCode(centerx, m_Bounds.max.y, centerz, true);
                int righttopmiddle = detector.GetDetectedCode(centerx + sx, m_Bounds.max.y, centerz, true);
                int middletopback = detector.GetDetectedCode(centerx, m_Bounds.max.y, centerz - sz, true);
                int middletopforward = detector.GetDetectedCode(centerx, m_Bounds.max.y, centerz + sz, true);

                TriggerToNodeByCamera(detector, handle, depth + 1, new TreeCullingCode()
                {
                    leftbottomback = cullingCode.leftbottomback,
                    leftbottomforward = leftbottommiddle,
                    lefttopback = cullingCode.lefttopback,
                    lefttopforward = lefttopmiddle,
                    rightbottomback = middlebottomback,
                    rightbottomforward = middlebottommiddle,
                    righttopback = middletopback,
                    righttopforward = middletopmiddle,
                }, centerx - sx * 0.5f, centerz - sz * 0.5f, sx, sz);
                TriggerToNodeByCamera(detector, handle, depth + 1, new TreeCullingCode()
                {
                    leftbottomback = leftbottommiddle,
                    leftbottomforward = cullingCode.leftbottomforward,
                    lefttopback = lefttopmiddle,
                    lefttopforward = cullingCode.lefttopforward,
                    rightbottomback = middlebottommiddle,
                    rightbottomforward = middlebottomforward,
                    righttopback = middletopmiddle,
                    righttopforward = middletopforward,
                }, centerx - sx * 0.5f, centerz + sz * 0.5f, sx, sz);
                TriggerToNodeByCamera(detector, handle, depth + 1, new TreeCullingCode()
                {
                    leftbottomback = middlebottomback,
                    leftbottomforward = middlebottommiddle,
                    lefttopback = middletopback,
                    lefttopforward = middletopmiddle,
                    rightbottomback = cullingCode.rightbottomback,
                    rightbottomforward = rightbottommiddle,
                    righttopback = cullingCode.righttopback,
                    righttopforward = righttopmiddle,
                }, centerx + sx * 0.5f, centerz - sz * 0.5f, sx, sz);
                TriggerToNodeByCamera(detector, handle, depth + 1, new TreeCullingCode()
                {
                    leftbottomback = middlebottommiddle,
                    leftbottomforward = middlebottomforward,
                    lefttopback = middletopmiddle,
                    lefttopforward = middletopforward,
                    rightbottomback = rightbottommiddle,
                    rightbottomforward = cullingCode.rightbottomforward,
                    righttopback = righttopmiddle,
                    righttopforward = cullingCode.righttopforward,
                }, centerx + sx * 0.5f, centerz + sz * 0.5f, sx, sz);
            }
        }

        private void TriggerToNode(IDetector detector, TriggerHandle<T> handle, int depth, float centerx, float centerz, float sizex, float sizez)
        {
            if (depth == m_MaxDepth)
            {
                uint m = Morton2FromWorldPos(centerx, centerz);
                if (base.m_Nodes.ContainsKey(m) && base.m_Nodes[m] != null)
                {
                    base.m_Nodes[m].Trigger(detector, handle);
                }
            }
            else
            {
                int colider = detector.GetDetectedCode(centerx, base.m_Bounds.center.y, centerz, true);
                float sx = sizex * 0.5f, sz = sizez * 0.5f;

                if ((colider & 1) != 0)
                    TriggerToNode(detector, handle, depth + 1, centerx - sx * 0.5f, centerz - sz * 0.5f, sx, sz);
                if ((colider & 2) != 0)
                    TriggerToNode(detector, handle, depth + 1, centerx - sx * 0.5f, centerz + sz * 0.5f, sx, sz);
                if ((colider & 4) != 0)
                    TriggerToNode(detector, handle, depth + 1, centerx + sx * 0.5f, centerz - sz * 0.5f, sx, sz);
                if ((colider & 8) != 0)
                    TriggerToNode(detector, handle, depth + 1, centerx + sx * 0.5f, centerz + sz * 0.5f, sx, sz);
            }
        }
    }
}