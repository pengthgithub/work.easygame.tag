using System;
using System.Collections.Generic;
using Easy;
using UnityEngine;

[Serializable]
public struct PersonData
{
    [SerializeField] public int index;
    [SerializeField] public Animation animation;
}

/// <summary>
/// 人口石块 出生和显示
/// </summary>
[ExecuteAlways]
public class PersonBirth : MonoBehaviour
{
    [SerializeField] [Rename("调试")] [Range(-1, 18)]
    public int debugIndex;

    [SerializeField] [Rename("自动覆盖")] public bool autoAdd;
    [SerializeField] public List<PersonData> persons;
    [SerializeField] [Rename("石块预制件")] public GameObject shikuai;
    [Header("陈列柜位置")] [SerializeField] public List<Transform> chengLieGuiList;
    private void Start()
    {
        if (persons == null || persons.Count == 0) return;
        foreach (var per in persons)
        {
            if (per.animation)
            {
                per.animation.gameObject.SetActive(false);
            }
        }
    }

#if UNITY_EDITOR && !UNITY_WEBGL_WX
    private void OnValidate()
    {
        if (autoAdd)
        {
            if (shikuai)
            {
                for (int i = 0; i < persons.Count; i++)
                {
                    var data = persons[i];
                    if (data.animation == null || data.animation.gameObject.name != shikuai.name)
                    {
                        var clone = GameObject.Instantiate(shikuai);
                        clone.name = shikuai.name;
                        data.animation = clone.GetComponentInChildren<Animation>();
                    }
                }
            }

            var aniArray = gameObject.GetComponentsInChildren<Animation>(true);
            for (int i = 0; i < aniArray.Length; i++)
            {
                var data = persons[i];
                data.animation = aniArray[i];

                persons[i] = data;
            }
        }

        if (debugIndex < 0)
        {
            foreach (var per in persons)
            {
                if (per.animation)
                {
                    per.animation.gameObject.SetActive(false);
                }
            }

            return;
        }

        ShowPersonPos(debugIndex);
    }
#endif

    /// <summary>
    /// 显示人口位置
    /// </summary>
    /// <param name="index"></param>
    public void ShowPersonPos(int index)
    {
        if (persons == null || persons.Count == 0) return;

        foreach (var per in persons)
        {
            if (per.index == index && per.animation && per.animation.gameObject.activeInHierarchy == false)
            {
                per.animation.gameObject.SetActive(true);
                per.animation.Play();
                return;
            }
        }
    }
}