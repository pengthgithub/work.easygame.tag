using System;
using System.IO;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;

public class AddressTools
{
    /// <summary>
    /// 自动加入到 Addressable
    /// </summary>
    [MenuItem("Tools/自动导入Address")]
    public static void AutoAddAddress()
    {
        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.GetSettings(true);

        var defaultGroup = settings.FindGroup("Default Local Group");
        if (defaultGroup == null)
        {
            defaultGroup = CreateAddressableGroup("Default Local Group");
        }

        settings.DefaultGroup = defaultGroup;
        
        AddGroupToAddress("np", "Assets/Art/Character", "*.prefab", PathType.simpleName);
        AddGroupToAddress("config", "Assets/Art/Config", "*.bytes", PathType.simpleNameWithExten);
        AddGroupToAddress("scene", "Assets/Art/scene", "*.unity", PathType.simpleName);
        AddGroupToAddress("wujian", "Assets/Art/scene", "s_*.prefab", PathType.simpleName);
        AddGroupToAddress("doodad", "Assets/Art/doodad", "*.prefab", PathType.simpleName);
        AddGroupToAddress("shader", "Assets/Art/Shader", "*.shader", PathType.simpleName);
        AddGroupToAddress("sfx", "Assets/Art/sfxTag", "*.asset", PathType.simpleName);
        AddGroupToAddress("uisfx", "Assets/Art/sfxshow", "*.prefab", PathType.simpleName);
        AddGroupToAddress("sound", "Assets/Art/sound", "*.*", PathType.simpleName);
        AddGroupToAddress("spine", "Assets/Art/Spine", "*.prefab", PathType.simpleName);
        AddGroupToAddress("uibg", "Assets/Art/UI/bg", "*.*", PathType.simpleNameWithExten);
        AddGroupToAddress("font", "Assets/Art/UI/font", "*.*", PathType.simpleNameWithExten);
        AddGroupToAddress("ui", "Assets/Art/UI", "*.*", PathType.simpleName, true);
        
        
        
        
        
        
    }

    enum PathType
    {
        fullName,
        simpleName,
        simpleNameWithExten
    }

    private static void AddGroupToAddress(string group, string path, string filterName, PathType pathType,
        bool top = false)
    {
        if (Directory.Exists(path) == false) return;
        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
        AddressableAssetGroup addressableAssetGroup = settings.FindGroup(group);
        if (addressableAssetGroup != null) addressableAssetGroup.RemoveAllAssetEntry();

        var files = Directory.GetFiles(path, filterName,
            top ? SearchOption.TopDirectoryOnly : SearchOption.AllDirectories);
        foreach (var file in files)
        {
            var extension = Path.GetExtension(file);
            if (extension == ".meta") continue;

            if (pathType == PathType.simpleName)
            {
                var fileName = Path.GetFileNameWithoutExtension(file);
                AddToAddressable(group, fileName, file);
            }

            if (pathType == PathType.fullName)
            {
                AddToAddressable(group, file.Replace("\\", "/"), file);
            }

            if (pathType == PathType.simpleNameWithExten)
            {
                var fileName = Path.GetFileName(file);
                AddToAddressable(group, fileName.Replace("\\", "/"), file);
            }
        }
    }

    private static AddressableAssetGroup CreateAddressableGroup(string groupName)
    {
        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
        Type[] _types = new Type[3]
        {
            typeof(AddressableAssetGroup),
            typeof(ContentUpdateGroupSchema),
            typeof(BundledAssetGroupSchema)
        };
        return settings.CreateGroup(groupName, false, false, false, null, _types);
    }

    /// <summary>
    /// 添加资源到 addressable
    /// </summary>
    /// <param name="groupName"></param>
    /// <param name="fileName"></param>
    /// <param name="path"></param>
    private static AddressableAssetGroup AddToAddressable(string groupName, string fileName, string path)
    {
        //文件夹直接返回
        if (Directory.Exists(path)) return null;

        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
        var labels = settings.GetLabels();
        if (labels.Contains(groupName) == false)
        {
            settings.AddLabel(groupName);
        }

        AddressableAssetGroup group = settings.FindGroup(groupName);
        if (group == null)
        {
            group = CreateAddressableGroup(groupName);
        }

        var guid = AssetDatabase.GUIDFromAssetPath(path).ToString();
        AddressableAssetEntry entry = settings.CreateOrMoveEntry(guid, group, false, false);
        if (entry != null)
        {
            entry.address = fileName;
            entry.SetLabel(groupName, true);
        }

        return group;
    }
}