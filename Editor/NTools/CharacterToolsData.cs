using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Easy
{
    [CreateAssetMenu(fileName = "CharacterTools", menuName = "CharacterTools", order = 0)]
    public class CharacterToolsData : ScriptableObject
    {
        [Tooltip("公共的节点动画")] public List<AnimationClip> defaultNodeClip;

        [Tooltip("需要连线的动画名")] public List<string> needLineAniName;

        [Tooltip("循环播放的动画名")] public List<string> loopAniName;
        [Tooltip("默认动画名字")] public string defaultAniName;
        [Tooltip("影子预制件")] public GameObject shadowPrefab;

        [Tooltip("默认需要添加的插槽名字")] public List<string> defaultLocatorName;

        [Tooltip("默认碰撞盒中心点")] public Vector3 boxCenter;
        [Tooltip("默认碰撞盒大小")] public Vector3 boxSize;

        [Tooltip("默认Shader名字")] public Shader defaultShaderName;

        [Tooltip("默认层级名字")] public string defaultLayer;
    }
}