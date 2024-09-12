using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using File = System.IO.File;

public class Ramp2TextureEditor
{
    private static readonly int RampMap = Shader.PropertyToID("_RampMap");
    private List<Gradient> _gradients = new();
    private Texture2D _rampTexture;
    private RampTextureData _rampTextureData;
    private RampTextureData _lastRampTextureData;
    private Vector2 _scrollPosition;
    private bool _showGradient;
    private bool _showTools;

    private Material oldMaterial;

    public int DrawArea(Material material)
    {
        OnGUI(material);

        // if (initLoad)
        // {
        //     string materialPath = AssetDatabase.GetAssetPath(material);
        //     string texPath = materialPath.Replace(".mat", "_ramp.png");
        //     Texture2D texture2d = AssetDatabase.LoadAssetAtPath<Texture2D>(texPath);
        //     SetTexture(material, texture2d);
        //
        //     _showTools = true;
        // }

        return _gradients.Count;
    }

    private bool autoLoad;

    private void OnGUI(Material material)
    {
        autoLoad = EditorGUILayout.Toggle("     创建/加载数据", autoLoad);
        if (autoLoad && _rampTextureData == null)
        {
            string materialPath = AssetDatabase.GetAssetPath(material);
            //materialPath = materialPath.Replace("Assets", "");
            var path = materialPath.Replace(".mat", ".asset");
            _rampTextureData = AssetDatabase.LoadAssetAtPath<RampTextureData>(path);

            if (_rampTextureData == null)
            {
                _rampTextureData = ScriptableObject.CreateInstance<RampTextureData>();
                AssetDatabase.CreateAsset(_rampTextureData, path);
            }
        }

        EditorGUILayout.BeginHorizontal();
        _rampTextureData =
            EditorGUILayout.ObjectField("     数据", _rampTextureData, typeof(RampTextureData), false) as
                RampTextureData;
        if (_rampTextureData != null)
        {
            InitScriptable(material, 1);
        }

        EditorGUILayout.EndHorizontal();

        if (_rampTextureData == null) return;

        EditorGUI.BeginChangeCheck();
        _gradients[0] = EditorGUILayout.GradientField("     正光 ", _gradients[0]);
        _gradients[1] = EditorGUILayout.GradientField("     补光 ", _gradients[1]);
        if (EditorGUI.EndChangeCheck())
        {
            _rampTextureData.gradients = _gradients;
            UpdateData();
            SetTexture(material, _rampTexture);
            needSave = true;
        }

        if (needSave)
        {
            EditorGUILayout.LabelField("目前处于编辑器模式，贴图并为保存.");
            DrawSave(material);
        }
    }

    private bool needSave = false;

    public void DrawSave(Material material)
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("保存"))
        {
            UpdateData();
            Save(material);
            needSave = false;
        }

        if (GUILayout.Button("取消Ramp贴图"))
        {
            SetTexture(material, null);
            needSave = false;
        }

        EditorGUILayout.EndHorizontal();
    }

    private void SetTexture(Material material, Texture2D texture2D)
    {
        if (texture2D == null)
        {
            material.SetTexture(RampMap, null);
        }
        else
        {
            material.SetTexture(RampMap, texture2D);
        }
    }

    public void DrawCreate(Material material)
    {
        if (GUILayout.Button("创建"))
        {
            InitScriptable(material, 0);
        }
    }

    public void InitScriptable(Material material, float val)
    {
        if (_lastRampTextureData == _rampTextureData)
        {
            return;
        }

        _lastRampTextureData = _rampTextureData;


        string materialPath = AssetDatabase.GetAssetPath(material);
        materialPath = materialPath.Replace("Assets", "");
        _rampTextureData.savePath = materialPath.Replace(".mat", "_ramp.png");

        _gradients = _rampTextureData.gradients;
        for (int i = _gradients.Count; i < 2; i++)
        {
            _gradients.Add(new Gradient());
        }

        string texPath = "Assets" + _rampTextureData.savePath;
        CreateTexture2D();
        UpdateData();
        SetTexture(material, _rampTexture);
    }

    private void UpdateData()
    {
        if (_rampTexture == null)
        {
            return;
        }

        int width = _rampTextureData.width;
        int height = _gradients.Count * 4;

        float inv = 1f / (width - 1);

        int eachHeight = height / 1;
        if (_gradients.Count != 0)
        {
            eachHeight = height / _gradients.Count;
        }

        int howMany = 0;
        while (howMany != _gradients.Count)
        {
            int start = height - (eachHeight * howMany) - 1;
            int end = start - eachHeight;
            for (int y = start; y > end; y--)
            {
                for (int x = 0; x < width; x++)
                {
                    float t = x * inv;
                    Color col = _gradients[howMany].Evaluate(t);
                    _rampTexture.SetPixel(x, y, col);
                }
            }

            howMany++;
        }

        _rampTexture.Apply();
        _rampTexture.wrapMode = TextureWrapMode.Clamp;
    }

    private void CreateTexture2D()
    {
        int count = _gradients.Count;
        if (count == 0)
        {
            count = 1;
        }

        Object.DestroyImmediate(_rampTexture);
        _rampTexture = new Texture2D(_rampTextureData.width, 4 * count, TextureFormat.ARGB32, false)
        {
            filterMode = FilterMode.Bilinear,
            wrapMode = TextureWrapMode.Clamp
        };
    }

    private void Save(Material material)
    {
        if (_rampTexture == null)
        {
            return;
        }

        string path = Application.dataPath + _rampTextureData.savePath;

        byte[] bytes = _rampTexture.EncodeToPNG();
        File.WriteAllBytes(path, bytes);

        EditorUtility.SetDirty(_rampTextureData);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        string texPath = "Assets" + _rampTextureData.savePath;
        Texture2D texture2d = AssetDatabase.LoadAssetAtPath<Texture2D>(texPath);
        if (texture2d)
        {
            texture2d.wrapMode = TextureWrapMode.Clamp;
            SetTexture(material, texture2d);

            TextureImporter importer = AssetImporter.GetAtPath(texPath) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Default;
                importer.wrapMode = TextureWrapMode.Clamp; // 修改为你想要的Wrap Mode
                importer.filterMode = FilterMode.Point;
                importer.isReadable = false;
                importer.mipmapEnabled = false;
                AssetDatabase.ImportAsset(texPath);
                AssetDatabase.Refresh();
            }
        }
    }
}