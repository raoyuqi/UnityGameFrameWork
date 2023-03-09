using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class BuildAssetBundles
{
    private static string _ASSETS_DIRECTORY_ROOT = PathCombine(Application.dataPath, "AssetsPackage");
    private const string _PREFABS_ROOT_DIR = "Prefabs/";
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
        BuildPrefabAssetBundle(buildList);

        AssetBundleBuild[] buildMap = buildList.ToArray();
        BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath, buildMap, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
    }

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

            var name = PathCombine(fileInfo.FullName).Replace(_ASSETS_DIRECTORY_ROOT + "/", "");
            buildList.Add(new AssetBundleBuild()
            {
                assetBundleName = name,
                assetNames = new string[] { PathCombine(_RELATIVE_ROOT_DIR, name) }
            });
        }
    }
}
