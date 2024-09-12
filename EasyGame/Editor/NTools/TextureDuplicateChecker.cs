using System;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Object = UnityEngine.Object;

public class TextureDuplicateChecker : EditorWindow
{
    private string checkPath = "Assets/Art";
    private string[] directories;
    [MenuItem("Tools/Check Duplicate Textures")]
    public static void ShowWindow()
    {
        GetWindow<TextureDuplicateChecker>("Duplicate Texture Checker");
    }
    SerializedObject serizlized;
    SerializedProperty repeatedTexturesPro;
    [SerializeField] private List<Texture2D> repeatedTextures;
    private void OnEnable()
    {
        repeatedTextures = new List<Texture2D>();
        serizlized = new SerializedObject(this);
        repeatedTexturesPro = serizlized.FindProperty("repeatedTextures");
        
         directories = Directory.GetDirectories("Assets/Art", "*", SearchOption.TopDirectoryOnly);
    }

    private Vector2 scrollPosition; // 用于滚动视图的位置
    private int selectIndex = 0;
    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Checker Duplicate Texture Dir", EditorStyles.boldLabel);
        selectIndex = EditorGUILayout.Popup(selectIndex, directories, GUILayout.Width(200));
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Check Duplicate Textures"))
        {
            checkPath = directories[selectIndex];
            CheckForDuplicateTextures();
            repeatedTexturesPro.arraySize = repeatedTextures.Count;
            for (int i = 0; i < repeatedTextures.Count; i++)
            {
                repeatedTexturesPro.GetArrayElementAtIndex(i).objectReferenceValue = repeatedTextures[i];
            }
            serizlized.ApplyModifiedProperties();
        }
        
        if (GUILayout.Button("Modify Duplicate Textures"))
        {
            ModifyDuplicateTextures();
        }
        EditorGUILayout.EndHorizontal();
        
        // 显示重复纹理信息
        GUILayout.Label($"Duplicate Textures: need Modify Count {repeatedTexturesPro.arraySize}", EditorStyles.boldLabel);
       if(repeatedTexturesPro != null) EditorGUILayout.PropertyField(repeatedTexturesPro);
    }
    Dictionary<string, List<string>> duplicateTextures = new Dictionary<string, List<string>>();
    Dictionary<string, Texture2D> textureMap = new Dictionary<string, Texture2D>();
    Dictionary<string, string> hashMap = new Dictionary<string, string>();
    
    
    private void CheckForDuplicateTextures()
    {
        // 获取所有纹理资源
        string[] textureGUIDs = AssetDatabase.FindAssets("t:texture2D", new[] { checkPath });
        textureMap.Clear();
        hashMap.Clear();
        if (textureMap.Count == 0)
        {
            foreach (string guid in textureGUIDs)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                // 设置为可读取并保存设置
                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if(importer == null) continue;
                importer.isReadable = true;
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path); // 重新加载
                if (texture != null)
                {
                    string textureHash = GetTextureHash(texture);
                    hashMap[path] = textureHash;
                }
                textureMap[path] = texture;
                importer.isReadable = false;
            }
        }
        
        duplicateTextures.Clear();
        foreach (var hash in hashMap)
        {
            var textureHash = hash.Value;
            if (!duplicateTextures.ContainsKey(textureHash))
            {
                duplicateTextures[textureHash] = new List<string>();
            }            
            duplicateTextures[textureHash].Add(hash.Key);  
        }
        repeatedTextures.Clear();
        foreach (var textureInfo in duplicateTextures)
        {
            if (textureInfo.Value.Count > 1)
            {
                foreach (var tex in textureInfo.Value)
                {
                    GUILayout.Label(tex);
                             
                    textureMap.TryGetValue(tex, out Texture2D texture);
                    repeatedTextures.Add(texture);
                }
            }
        }
        AssetDatabase.Refresh();
    }

    private void ModifyDuplicateTextures()
    {
        // 处理重复纹理
        foreach (var entry in duplicateTextures)
        {
            if (entry.Value.Count > 1)
            {
                string referencePath = entry.Value[0];
                Texture2D referenceTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(referencePath);
                // 修改引用
                foreach (string duplicatePath in entry.Value)
                {
                    if (duplicatePath != referencePath)
                    {
                        Texture2D duplicateTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(duplicatePath);
                        ReplaceTextureReferences(duplicateTexture, referenceTexture);
                        AssetDatabase.DeleteAsset(duplicatePath);
                    }
                }
            }
        }
    }

    private string GetTextureHash(Texture2D texture)
    {
        // 生成纹理的哈希值
        Color[] pixels = texture.GetPixels();
        int hash = 17;
        foreach (Color pixel in pixels)
        {
            hash = hash * 31 + pixel.GetHashCode();
        }
        return hash.ToString();
    }

    private void ReplaceTextureReferences(Texture2D oldTexture, Texture2D newTexture)
    {
        // 查找所有引用旧纹理的对象并替换
        string[] allAssetPaths = AssetDatabase.GetAllAssetPaths();
        foreach (string assetPath in allAssetPaths)
        {
            Object asset = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
            if (asset != null)
            {
                SerializedObject serializedObject = new SerializedObject(asset);
                SerializedProperty property = serializedObject.GetIterator();
                while (property.Next(true))
                {
                    if (property.propertyType == SerializedPropertyType.ObjectReference)
                    {
                        if (property.objectReferenceValue == oldTexture)
                        {
                            property.objectReferenceValue = newTexture;
                            serializedObject.ApplyModifiedProperties();
                        }
                    }
                }
            }
        }
    }
}
