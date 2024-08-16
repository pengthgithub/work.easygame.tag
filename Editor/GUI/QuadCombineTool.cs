using System.Collections.Generic;

using UnityEditor;

using UnityEngine;

public class QuadCombineTool : EditorWindow
{
    [MenuItem("Tools/�ϲ�ģ��")]
    public static void OpenWindow()
    {
        EditorWindow.GetWindow<QuadCombineTool>().Show();
    }
    private GameObject gameObject;
    private string savePath = "Assets";

    private void OnGUI()
    {
        gameObject = EditorGUILayout.ObjectField("�ϲ�������", gameObject, typeof(GameObject), true) as GameObject;
        savePath = EditorGUILayout.TextField("�ļ�����·����", savePath);

        if (GUILayout.Button("�ϲ�Quad"))
        {
            CombineQuads();
        }
    }
    private void CombineQuads()
    {
        var meshfilters = gameObject.GetComponentsInChildren<MeshFilter>();
        if (meshfilters != null && meshfilters.Length > 0)
        {
            var centerOffset = new List<Vector4>(); //��¼ƫ��������list

            var combineInstances = new CombineInstance[meshfilters.Length];
            for (int i = 0; i < meshfilters.Length; i++)
            {
                var mesh = meshfilters[i].sharedMesh;
                combineInstances[i] = new CombineInstance()
                {
                    mesh = mesh,
                    transform = meshfilters[i].transform.localToWorldMatrix
                };
                for (int j = 0; j < mesh.vertexCount; j++)
                {
                    //Ĭ�Ϻϲ��ṹ�ǣ�quad��һ���������£���ôlocalPosition���Ǿ��븸�������ģ��ֲ��ռ�ԭ�㣩��ƫ��������
                    centerOffset.Add(meshfilters[i].transform.position);
                }
            }

            var newMesh = new Mesh();
            newMesh.CombineMeshes(combineInstances, true);

            //��ƫ������д������������
            newMesh.tangents = centerOffset.ToArray();

            var fullPath = $"{savePath}/NewMesh.asset";
            AssetDatabase.CreateAsset(newMesh, fullPath);
            Debug.Log("�����ļ�����" + fullPath);
        }

    }
}