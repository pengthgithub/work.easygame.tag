using System;
using System.Collections.Generic;
using Easy;
using UnityEngine;

[Serializable]
struct UIShowData
{
    [SerializeField] [Range(0,10)] [Rename("绑定时间")] public float activeTime;
    [SerializeField] [Range(0,20)] [Rename("生命周期")] public float lifeTime;
    [SerializeField] [Rename("控制节点")] public GameObject sfxNode;
    [SerializeField] [Rename("插槽点")] public GameObject locatorNode;
    [SerializeField] private bool bBind;
    [SerializeField] private bool bLifeEnd;
    [SerializeField] private float _currentTime;

    public void Init()
    {
        bBind = false;
        bLifeEnd = false;
        _currentTime = 0;
        sfxNode.SetActive(false);
    }
    public bool Update()
    {
        if (bLifeEnd) return true;
        float realTime = activeTime + lifeTime;
        _currentTime += Time.deltaTime;

        if (_currentTime > activeTime && bBind == false)
        {
            bBind = true;
            if(sfxNode)sfxNode.SetActive(true);
            if (locatorNode)
            {
                sfxNode.transform.SetParent(locatorNode.transform);
                sfxNode.transform.Reset();
            }
        }

        if (lifeTime != 0 && _currentTime > realTime && bLifeEnd == false)
        {
            bLifeEnd = true;
            if(sfxNode)sfxNode.SetActive(false);
        }

        return false;
    }
}

public class UIShow : MonoBehaviour
{
    [SerializeField] private EAnimation _eAnimation;
    [SerializeField] private List<UIShowData> _sfxList;
    
    [SerializeField] [Rename("影子高度")] private float _shadowHeight = 3;

    [SerializeField] [Rename("影子角度")]
    private float _shadowAngleRadians = -13;
    
    [SerializeField] private SkinnedMeshRenderer[] renders;
    
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_eAnimation == null) _eAnimation = gameObject.GetComponentInChildren<EAnimation>();

        // if (Application.isPlaying)
        // {
        //     ChangeShadowHeight();
        // }
        // else
        // {
        //     if (renders == null) renders = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();  
        // }
    }
#endif
    private void Awake()
    {
        if (_sfxList != null)
        {
            for (int i = _sfxList.Count - 1; i >= 0; i--)
            {
                var sfx = _sfxList[i];
                sfx.Init();
                _sfxList[i] = sfx;
            }
        }
        playEnd = true;

        //ChangeShadowHeight();
    }

    private void Start()
    {
        gameObject.SetActive(true);
    }

    private void ChangeShadowHeight()
    {
        if (renders == null || renders.Length == 0) renders = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
        if (renders != null)
        {
            foreach (var render in renders)
            {
                if (render.materials != null)
                {
                    foreach (var mat in render.materials)
                    {
                        mat.SetFloat("_ShadowHeightOffset", _shadowHeight);
                        mat.SetFloat("_shadowAngleRadians", _shadowAngleRadians * 0.01745f);
                        
                    }
                }
            }  
        }
    }

    private bool playEnd = false;
    private void Update()
    {
        if(playEnd) return;

        playEnd = true;

        for (int i = _sfxList.Count - 1; i >= 0; i--)
        {
            var sfx = _sfxList[i];
            var _playEnd = sfx.Update();
            if (_playEnd == false)
            {
                playEnd = false;
            }
            _sfxList[i] = sfx;
        }
    }
    
    internal void Play()
    {
        if (_eAnimation)
        {
            _eAnimation.oncePlayEnd2 = PlayEnd;
            _eAnimation.PlayAnimation("show");
        }
        playEnd = false;
        for (int i = _sfxList.Count - 1; i >= 0; i--)
        {
            var sfx = _sfxList[i];
            sfx.Init();
            _sfxList[i] = sfx;
        }
    }

    private void PlayEnd()
    {
        if (_eAnimation) _eAnimation.PlayAnimation("battle_idle");
    }
}

#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(UIShow))]
public class UIShowEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Play"))
        {
            var ani = target as UIShow;
            ani.Play();
        }
    }
}
#endif