#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;

[Serializable] public enum FileType
{
    fullName,
    simpleName,
    simpleNameWithExten
}

[Serializable]
public class AddRessData
{
    [SerializeField] public string label;
    [SerializeField] public string dirPath;
    [SerializeField] public string filter;
    [SerializeField] public FileType fileType;
    [SerializeField] public bool topDir;
}
[CreateAssetMenu(menuName = "Create AddressConfig", fileName = "AddressConfig", order = 0)]
public class AddressConfig : ScriptableObject
{
    [SerializeField] public List<AddRessData> configs;
}

public partial class EasyEditorAddress
{
    /// <summary>
    /// 自动加入到 Addressable
    /// </summary>
    [MenuItem("Tools/自动导入Address")]
    public static void AutoAddAddress()
    {
        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.GetSettings(false);
        var defaultGroup = settings.FindGroup("Default Local Group");
        if (defaultGroup == null)
        {
            defaultGroup = CreateAddressableGroup("Default Local Group");
        }
        settings.DefaultGroup = defaultGroup;

        var config = AssetDatabase.LoadAssetAtPath<AddressConfig>("Assets/Editor/AddressConfig.asset");
        if (config)
        {
            foreach (var data in config.configs)
            {
                AddGroupToAddress(data.label, data.dirPath, data.filter, data.fileType, data.topDir);
            }
        }
    }

    private static void AddGroupToAddress(string group, string path, string filterName, FileType fileType,
        bool top = false)
    {
        if (Directory.Exists(path) == false) return;
        AddressableAssetGroup aGroup = CreateGroup(group);
        
        var files = Directory.GetFiles(path, filterName,
            top ? SearchOption.TopDirectoryOnly : SearchOption.AllDirectories);
        foreach (var file in files)
        {
            var extension = Path.GetExtension(file);
            if (extension == ".meta") continue;
            var fileName = "";
            if (fileType == FileType.simpleName)
            {
                fileName = Path.GetFileNameWithoutExtension(file);
            }
            if (fileType == FileType.fullName)
            {
                fileName = file.Replace("\\", "/");
            }
            if (fileType == FileType.simpleNameWithExten)
            {
                fileName = Path.GetFileName(file);
            }
            
            AddToAddressable(aGroup, fileName, file);
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

    private static AddressableAssetGroup CreateGroup(string groupName)
    {
        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
        
        AddressableAssetGroup group = settings.FindGroup(groupName);
        if (group == null)
        {
            group = CreateAddressableGroup(groupName);
        }
        if (group != null) group.RemoveAllAssetEntry();
        
        var labels = settings.GetLabels();
        if (labels.Contains(groupName) == false)
        {
            settings.AddLabel(groupName);
        }
        return group;
    }

    /// <summary>
    /// 添加资源到 addressable
    /// </summary>
    /// <param name="groupName"></param>
    /// <param name="fileName"></param>
    /// <param name="path"></param>
    private static void AddToAddressable(AddressableAssetGroup group, string fileName, string path)
    {
        //文件夹直接返回
        if (Directory.Exists(path)) return;

        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
        var guid = AssetDatabase.GUIDFromAssetPath(path).ToString();
        AddressableAssetEntry entry = settings.CreateOrMoveEntry(guid, group, false, false);
        if (entry != null)
        {
            entry.address = fileName;
            entry.SetLabel(group.name, true);
        }
    }
}
#endif