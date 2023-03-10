using FrameWork.Core.Utils;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Sprites;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;

public class BuildAssetBundles
{
    private static string _ASSETS_DIRECTORY_ROOT = PathCombine(Application.dataPath, "AssetsPackage");
    private const string _PREFABS_ROOT_DIR = "Prefabs/";
    private const string _SPRITE_ATLAS_ROOT_DIR = "SpriteAltas/";
    private const string _RELATIVE_ROOT_DIR = "Assets/AssetsPackage";

    private static string PathCombine(params string[] paths) => string.Join(
        "/",
        paths.Where(r => !string.IsNullOrEmpty(r))
    ).Replace('\\', '/');

    [MenuItem("自定义工具/Build Asset Bundles")]
    public static void Build()
    {
        // Create the array of bundle build details.
        //AssetBundleBuild[] buildMap = new AssetBundleBuild[2];
        //List<AssetBundleBuild> buildList = new List<AssetBundleBuild>();

        //buildMap[0].assetBundleName = "atlastestbundle";

        //string[] assets = new string[] { };
        //enemyAssets[0] = "Assets/Textures/char_enemy_alienShip.jpg";
        //enemyAssets[1] = "Assets/Textures/char_enemy_alienShip-damaged.jpg";

        //buildMap[0].assetNames = enemyAssets;
        //buildMap[1].assetBundleName = "herobundle";

        //string[] heroAssets = new string[1];
        //heroAssets[0] = "char_hero_beanMan";
        //buildMap[1].assetNames = heroAssets;

        //var root = "Assets/Resources/Atlas/Test/";
        //buildMap[0].assetBundleName = "atlas/test/bundle";
        //List<string> assets = new List<string>();
        //DirectoryInfo dir = new DirectoryInfo(Application.dataPath + "/Resources/Atlas/Test");
        //FileInfo[] files = dir.GetFiles("*", SearchOption.AllDirectories);
        //foreach (var fileInfo in files)
        //{
        //    if (fileInfo.Name.EndsWith(".meta"))
        //        continue;

        //    assets.Add(root + fileInfo.Name);
        //    Debug.Log(root + fileInfo.Name);
        //}

        //buildMap[0].assetNames = assets.ToArray();

        //buildMap[1].assetBundleName = "prefab/test/bundle";
        //string[] heroAssets = new string[1];
        //heroAssets[0] = "Assets/Resources/Prefab/Test/Test.prefab";
        //buildMap[1].assetNames = heroAssets;

        //BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath, buildMap, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);

        // 加载逻辑
        //var path = Path.Combine(Application.streamingAssetsPath, "prefab/test/bundle");
        //var ab1 = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "atlas/test/bundle"));
        //var ab = AssetBundle.LoadFromFile(path);
        //var prefab = ab.LoadAsset<GameObject>("Test.prefab");
        //Debug.Log(prefab);
        //Debug.Log(ab);
        //GameObject.Instantiate(prefab, GameObject.Find("Canvas").transform);

        //var asset1 = ab1.LoadAsset<Sprite>("img_toggle_selected.png");
        //var asset2 = ab1.LoadAllAssets<Sprite>();

        //foreach (var item in asset2)
        //{
        //    Debug.Log(item.name);
        //}

        //Debug.Log(asset2);
        //Debug.Log("***************** ");

        var buildList = new List<AssetBundleBuild>();
        BuildAtlasAssetBundle(buildList);
        BuildImageAssetBundle(buildList);
        BuildPrefabAssetBundle(buildList);

        AssetBundleBuild[] buildMap = buildList.ToArray();
        BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath, buildMap, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
        AssetDatabase.Refresh();
    }

    // 打包图集
    private static void BuildAtlasAssetBundle(List<AssetBundleBuild> buildList)
    {
        var path = PathCombine(_ASSETS_DIRECTORY_ROOT, _SPRITE_ATLAS_ROOT_DIR);
        if (!Directory.Exists(path))
        {
            Debug.LogError($"文件夹不存在: path = {path}");
            return;
        }

        BuildSpriteAtlas(path);

        var dir = new DirectoryInfo(path);
        var files = dir.GetFiles("*", SearchOption.AllDirectories);
        foreach (var fileInfo in files)
        {
            if (fileInfo.Name.EndsWith(".meta"))
                continue;

            var name = PathTool.GetDirectoryRelativelyPath($"{_ASSETS_DIRECTORY_ROOT}/", fileInfo.FullName);
            buildList.Add(new AssetBundleBuild()
            {
                assetBundleName = name,
                assetNames = new string[] { PathCombine(_RELATIVE_ROOT_DIR, name) }
            });
        }
    }

    // 打包静态图片
    private static void BuildImageAssetBundle(List<AssetBundleBuild> buildList)
    {
        var path = PathCombine(_ASSETS_DIRECTORY_ROOT, "Temp");
        if (!Directory.Exists(path))
        {
            Debug.LogError($"文件夹不存在: path = {path}");
            return;
        }

        var dir = new DirectoryInfo(path);
        var files = dir.GetFiles("*", SearchOption.AllDirectories);

        var assetNames = new List<string>();
        foreach (var fileInfo in files)
        {
            if (fileInfo.Name.EndsWith(".meta"))
                continue;

            var name = PathTool.GetDirectoryRelativelyPath($"{_ASSETS_DIRECTORY_ROOT}/", fileInfo.FullName);
            //Debug.Log(PathCombine(_RELATIVE_ROOT_DIR, name));
            assetNames.Add(PathCombine(_RELATIVE_ROOT_DIR, name));
        }

        buildList.Add(new AssetBundleBuild()
        {
            assetBundleName = "Temp/Image/Bundle.static",
            assetNames = assetNames.ToArray()
        });
    }

    // 打包预制体
    private static void BuildPrefabAssetBundle(List<AssetBundleBuild> buildList)
    {
        var path = PathCombine(_ASSETS_DIRECTORY_ROOT, _PREFABS_ROOT_DIR);
        if (!Directory.Exists(path))
        {
            Debug.LogError($"文件夹不存在: path = {path}");
            return;
        }

        var dir = new DirectoryInfo(path);
        var files = dir.GetFiles("*", SearchOption.AllDirectories);
        foreach (var fileInfo in files)
        {
            if (fileInfo.Name.EndsWith(".meta"))
                continue;

            var name = PathTool.GetDirectoryRelativelyPath($"{_ASSETS_DIRECTORY_ROOT}/", fileInfo.FullName);
            buildList.Add(new AssetBundleBuild()
            {
                assetBundleName = name,
                assetNames = new string[] { PathCombine(_RELATIVE_ROOT_DIR, name) }
            });
        }
    }

    // 创建图集
    private static void BuildSpriteAtlas(string dirPath)
    {
        if (!Directory.Exists(dirPath))
        {
            Debug.LogError($"文件夹不存在: path = {dirPath}");
            return;
        }

        var targetRoot = "Assets/AssetsPackage/SpriteAltas/";
        var sourceRoot = "Assets/AssetsPackage/Arts/Atlas";
        var dir = new DirectoryInfo(dirPath);
        var dirs = dir.GetDirectories();
        if (dirs.Length == 0)
            return;

        foreach (var dirInfo in dirs)
        {
            var relativelyPath = PathTool.GetDirectoryRelativelyPath(dirPath, dirInfo.FullName);
            var spriteAtlasPath = $"{PathTool.PathCombine(targetRoot, relativelyPath)}.spriteatlas";

            var spriteAtlas = new SpriteAtlas();
            var packingSetting = new SpriteAtlasPackingSettings()
            {
                enableRotation = false,
                enableTightPacking = false,
                padding = 2,
            };
            spriteAtlas.SetPackingSettings(packingSetting);
            spriteAtlas.SetIncludeInBuild(true);

            var spriteList = new List<Sprite>();
            var files = dirInfo.GetFiles("*", SearchOption.TopDirectoryOnly);
            foreach (var fileInfo in files)
            {
                if (fileInfo.Name.EndsWith(".meta"))
                    continue;

                var spritePath = PathTool.PathCombine(sourceRoot, relativelyPath, fileInfo.Name);
                var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
                if (sprite != null)
                    spriteList.Add(sprite);
            }

            if (spriteList.Count > 0)
            {
                spriteAtlas.Add(spriteList.ToArray());
                AssetDatabase.CreateAsset(spriteAtlas, spriteAtlasPath);
                SpriteAtlasUtility.PackAtlases(new[] { spriteAtlas }, EditorUserBuildSettings.activeBuildTarget);
            }

            BuildSpriteAtlas(dirInfo.FullName);
        }

        AssetDatabase.Refresh();
    }
}
