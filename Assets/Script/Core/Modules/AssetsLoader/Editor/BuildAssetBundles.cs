﻿using FrameWork.Core.Utils;
using Game.Config;
using MiniJSON;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.Sprites;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;

public class BuildAssetBundles
{
    private static string _STREAMING_ASSETS_PATH_ROOT = $"{Application.streamingAssetsPath}/";
    private static string _ASSETS_DIRECTORY_ROOT = PathCombine(Application.dataPath, "AssetsPackage");
    private const string _PREFABS_ROOT_DIR = "Prefabs/";
    private const string _SPRITE_ATLAS_ROOT_DIR = "SpriteAltas/";
    private const string _RELATIVE_ROOT_DIR = "Assets/AssetsPackage";

    private static List<SpriteAtlas> _CREATE_ATLASES = new List<SpriteAtlas>();

    private static string PathCombine(params string[] paths) => string.Join(
        "/",
        paths.Where(r => !string.IsNullOrEmpty(r))
    ).Replace('\\', '/');

    [MenuItem("自定义工具/Build Asset Bundles")]
    public static void Build()
    {
        var buildList = new List<AssetBundleBuild>();
        BuildAtlasAssetBundle(buildList);

        var path = PathCombine(_ASSETS_DIRECTORY_ROOT, "Arts/Images");
        BuildImageAssetBundle(path, buildList);

        BuildScenesAssetBundle(buildList);
        BuildPrefabAssetBundle(buildList);

        SpriteAtlasUtility.PackAtlases(_CREATE_ATLASES.ToArray(), EditorUserBuildSettings.activeBuildTarget);

        AssetBundleBuild[] buildMap = buildList.ToArray();
        BuildPipeline.BuildAssetBundles(
            Application.streamingAssetsPath + "/AssetBundle",
            buildMap,
            BuildAssetBundleOptions.None,
            BuildTarget.StandaloneWindows
        );

        GenerateVersionFile();

        GenerateResourceListFile();

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

        _CREATE_ATLASES.Clear();
        BuildSpriteAtlas(PathCombine(_ASSETS_DIRECTORY_ROOT, "Arts/Atlas"), _CREATE_ATLASES);

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
    private static void BuildImageAssetBundle(string dirPath, List<AssetBundleBuild> buildList)
    {
        
        if (!Directory.Exists(dirPath))
        {
            Debug.LogError($"文件夹不存在: path = {dirPath}");
            return;
        }

        var dir = new DirectoryInfo(dirPath);
        var dirs = dir.GetDirectories();
        if (dirs.Length == 0)
            return;

        foreach (var dirInfo in dirs)
        {
            var files = dirInfo.GetFiles("*", SearchOption.TopDirectoryOnly);
            var relativelyPath = PathTool.GetDirectoryRelativelyPath($"{_ASSETS_DIRECTORY_ROOT}/", dirInfo.FullName);
            var assetBundleName = $"{relativelyPath}/Images.staticimages";

            var nameList = new List<string>();
            foreach (var fileInfo in files)
            {
                if (fileInfo.Name.EndsWith(".meta"))
                    continue;

                var name = PathTool.GetDirectoryRelativelyPath($"{_ASSETS_DIRECTORY_ROOT}/", fileInfo.FullName);
                nameList.Add(PathTool.PathCombine(_RELATIVE_ROOT_DIR, name));
            }

            if (nameList.Count > 0)
            {
                buildList.Add(new AssetBundleBuild()
                {
                    assetBundleName = assetBundleName,
                    assetNames = nameList.ToArray()
                });
            }

            BuildImageAssetBundle(dirInfo.FullName, buildList);
        }
    }

    // 打包场景
    private static void BuildScenesAssetBundle(List<AssetBundleBuild> buildList)
    {
        var path = PathCombine(_ASSETS_DIRECTORY_ROOT, "Scenes/");
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
                assetBundleName = name.Replace("unity", "unity3d"),
                assetNames = new string[] { PathCombine(_RELATIVE_ROOT_DIR, name) }
            });
        }
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
    private static void BuildSpriteAtlas(string dirPath, List<SpriteAtlas> atlases)
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
            AssetDatabase.DeleteAsset(spriteAtlasPath);

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
                atlases.Add(spriteAtlas);
            }

            BuildSpriteAtlas(dirInfo.FullName, atlases);
        }

        AssetDatabase.Refresh();
    }

    // 生成资源清单
    private static void GenerateResourceListFile()
    {
        var directoryInfo = new DirectoryInfo(Application.streamingAssetsPath);
        if (directoryInfo == null)
            return;

        //var list = new List<Dictionary<string, string>>();
        var dic = new Dictionary<string, Dictionary<string, string>>();
        var files = directoryInfo.GetFiles("*", SearchOption.AllDirectories);
        foreach (var file in files)
        {
            if (file.Name.EndsWith(".meta"))
                continue;

            var fileAttributes = File.GetAttributes(file.FullName);
            if (fileAttributes == FileAttributes.Directory)
                continue;

            var filePath = PathTool.GetDirectoryRelativelyPath(_STREAMING_ASSETS_PATH_ROOT, file.FullName);
            var dirPath = PathTool.GetDirectoryRelativelyPath(_STREAMING_ASSETS_PATH_ROOT, file.DirectoryName);
            dic.Add(filePath, new Dictionary<string, string>() {
                { "dir", dirPath },
                { "file", filePath },
                { "md5", MD5Util.GetFileInfoMD5(file) },
                { "size", file.Length.ToString() }
            });
        }

        var json = Json.Serialize(dic);
        var resFilePath = $"{ PathTool.GetAssetsBundleStreamingPath() }res_list.json";
        using (FileStream fs = File.OpenWrite(resFilePath))
        {
            var info = new UTF8Encoding(true).GetBytes(json);
            fs.Write(info, 0, info.Length);
        }        
    }

    // 生成版本文件
    private static void GenerateVersionFile()
    {
        var appVersion = PlayerSettings.bundleVersion;
        var filePath = $"{ PathTool.GetAssetsBundleStreamingPath() }app_version.json";
        if (!File.Exists(filePath))
        {
            var versionJson = Json.Serialize(new Dictionary<string, string>() {
                { "app_version", appVersion }
            });
            using (var fs = File.OpenWrite(filePath))
            {
                var info = new UTF8Encoding(true).GetBytes(versionJson);
                fs.Write(info, 0, info.Length);
            }
        }
        else
        {
            var jsonContent = File.ReadAllText(filePath);
            if(JsonUtil.TryDeserializeToDictionary(jsonContent, out Dictionary<string, string> dic))
            {
                var version = dic["app_version"];
                var versionArr = version.Split('.');
                var nextVersion = int.Parse(versionArr[2]) + 1;
                var newVersion = $"{versionArr[0]}.{versionArr[1]}.{nextVersion}";
                dic["app_version"] = newVersion;

                // 全部覆盖写入
                using (var fs = File.OpenWrite(filePath))
                {
                    var versionJson = Json.Serialize(dic);
                    var info = new UTF8Encoding(true).GetBytes(versionJson);
                    fs.Write(info, 0, info.Length);
                }
            }
        }
    }

    // 将热更资源上传到远端
    private static void SubmitToRemote()
    {
        // TODO: 
    }
}
