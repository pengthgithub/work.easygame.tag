//using Behaviour;

//using Effect;

//using UnityEditor;

//using UnityEngine;

//[ExecuteInEditMode]
//public class ChangeToEffect
//{
//    public static ChangeToEffect Instance = new();

//    public static void UpdateEffect(GameObject go)
//    {
//        if (go == null)
//        {
//            return;
//        }
//        BehaviourTag tag = go.GetComponent<BehaviourTag>();

//        if (tag == null)
//        {
//            return;
//        }

//        tag.Awake();
//        tag.ParseByJson();

//        string _name = "new_" + go.name;
//        GameObject _go = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Art/Effect/" + _name + ".prefab");
//        if (_go)
//        {
//            _name = "r_" + _name;
//        }
//        Instance.count = 0;
//        Instance.gameObject = new GameObject(_name);
//        Instance.maxLifeTime = 0;
//        Instance.CreateEffectTrack(tag);
//        Instance.CreateCameraTrack(tag);
//        Instance.CreateCasterTrack(tag);
//        Instance.CreateAudioTrack(tag);
//        Instance.CreateHardEffectTrack(tag);
//        if (Instance.count == 0)
//        {
//            go.SetActive(true);
//            GameObject.DestroyImmediate(Instance.gameObject);
//            Instance.gameObject = null;
//            return;
//        }

//        EffectPlay effectPlay = Instance.gameObject.AddComponent<EffectPlay>();
//        effectPlay.mLifeTime = Instance.maxLifeTime;
//        //GameObject.DestroyImmediate(tag);
//        Instance.savePrefab();
//    }
//    public string[] LocatorOptions = new string[] { "origin", "top", "body", "bip_r_hand", "bip_l_hand" };
//    private float maxLifeTime;

//    private GameObject gameObject;

//    private int count = 0;

//    private void CreateEffectTrack(BehaviourTag tag)
//    {
//        // 添加声音的轨道
//        System.Collections.Generic.List<BehaviourEffect> bsArray = tag.behaviourSystem.effectElement;
//        foreach (BehaviourEffect bs in bsArray)
//        {
//            count++;
//            EffectPrefab prefab = gameObject.AddComponent<EffectPrefab>();
//            prefab.mLifeTime = bs.mLifeTime;         //绑定时间
//            if (bs.mLifeTime == 0)
//            {
//                prefab.mLifeTime = tag.behaviourSystem.mLifeTime;
//            }
//            prefab.mBindTime = bs.mBindTime;         //绑定时间
//            prefab.mDelayTime = bs.mDelayTime;

//            prefab.mBindPointString = prefab.mBindPointString != "origin" ? bs.mBindPointString : LocatorOptions[(int)bs.mBindPoint];

//            prefab.mLockRotation = bs.mLockRotation;
//            prefab.mLocalPosition = bs.mLocalPosition;
//            prefab.mLocalRotation = bs.mLocalRotation;
//            prefab.mLocalScale = bs.mLocalScale;
//            prefab.mChangeSpeed = bs.mChangeSpeed;
//            prefab.tiles = bs.tiles;
//            prefab.tilingName = bs.tilingName;
//            prefab.mDelayRemove = bs.mDelayRemove;
//            prefab.mDelayRemoveWithLifeTimeEnd = bs.mDelayRemoveWithLifeTimeEnd;

//            prefab.mPrefab = bs.particleSystem;
//            //PrefabUtility.GetCorrespondingObjectFromSource(tag.gameObject);

//            maxLifeTime = Mathf.Max(maxLifeTime, prefab.mLifeTime);
//        }
//    }
//    private void CreateCameraTrack(BehaviourTag tag)
//    {
//        // 添加声音的轨道
//        System.Collections.Generic.List<BehaviourCamera> bsArray = tag.behaviourSystem.cameraElement;
//        foreach (BehaviourCamera bs in bsArray)
//        {
//            count++;
//            EffectCamera prefab = gameObject.AddComponent<EffectCamera>();
//            prefab.mLifeTime = bs.mLifeTime;
//            if (bs.mLifeTime == 0)
//            {
//                prefab.mLifeTime = tag.behaviourSystem.mLifeTime;
//            }
//            prefab.mBindTime = bs.mBindTime;
//            prefab.mDelayTime = bs.mDelayTime;
//            prefab.mShakeType = (Effect.ShakeCameraType)bs.mShakeType;
//            prefab.shakeCurve = bs.shakeCurve;
//            prefab.mShakeScale = bs.mShakeScale;

//            maxLifeTime = Mathf.Max(maxLifeTime, prefab.mLifeTime);
//        }
//    }
//    private void CreateCasterTrack(BehaviourTag tag)
//    {
//        // 添加声音的轨道
//        System.Collections.Generic.List<BehaviourCaster> bsArray = tag.behaviourSystem.casterElement;
//        foreach (BehaviourCaster bs in bsArray)
//        {
//            count++;
//            EffectCaster prefab = gameObject.AddComponent<EffectCaster>();
//            prefab.mLifeTime = bs.mLifeTime;         //绑定时间
//            if (bs.mLifeTime == 0)
//            {
//                prefab.mLifeTime = tag.behaviourSystem.mLifeTime;
//            }
//            prefab.mBindTime = bs.mBindTime;         //绑定时间
//            prefab.mDelayTime = bs.mDelayTime;
//            prefab.mAnimationIndex = (Effect.EnumAnimation)bs.mAnimationIndex;   //动作序列
//            prefab.animSpeedEnable = bs.animSpeedEnable;
//            prefab.animSpeed = bs.animSpeed;

//            prefab.fadeEnable = bs.fadeEnable;
//            prefab.fade = bs.fade;

//            prefab.frozenEnable = bs.frozenEnable;
//            prefab.frozen = bs.frozen;
//            prefab.frozenTexture = bs.frozenTexture;

//            prefab.stoneEnable = bs.petrifactionEnable;
//            prefab.stoneCure = bs.petrifaction;
//            prefab.stoneTexture = bs.petrifactionTexture;

//            prefab.dissolveEnable = bs.dissolveEnable;
//            prefab.dissolve = bs.dissolve;
//            prefab.dissolveTexture = bs.dissolveTexture;
//            prefab.outDissolveColor = bs.outDissolveColor;
//            prefab.inDissolveColor = bs.inDissolveColor;

//            prefab.edgeColorEnable = bs.edgeColorEnable;
//            prefab.edgeFade = bs.edgeFade;
//            prefab.outEdgeColor = bs.outEdgeColor;
//            prefab.inEdgeColor = bs.inEdgeColor;
//            prefab.edgeColorPower = bs.edgeColorPower;

//            prefab.transformClip = bs.transformClip;
//            prefab.bFlowTerrainHeight = bs.bFlowTerrainHeight;

//            maxLifeTime = Mathf.Max(maxLifeTime, prefab.mLifeTime);
//        }
//    }
//    private void CreateAudioTrack(BehaviourTag tag)
//    {
//        // 添加声音的轨道
//        System.Collections.Generic.List<BehaviourSound> bsArray = tag.behaviourSystem.soundElement;
//        foreach (BehaviourSound bs in bsArray)
//        {
//            count++;
//            EffectSound prefab = gameObject.AddComponent<EffectSound>();
//            prefab.mLifeTime = bs.mLifeTime;         //绑定时间
//            if (bs.mLifeTime == 0)
//            {
//                prefab.mLifeTime = tag.behaviourSystem.mLifeTime;
//            }
//            prefab.mBindTime = bs.mBindTime;         //绑定时间
//            prefab.mDelayTime = bs.mDelayTime;
//            prefab.mVolume = bs.mVolume;
//            prefab.mLoop = bs.mLoop;
//            prefab.audioClips = bs.audioClips;

//            maxLifeTime = Mathf.Max(maxLifeTime, prefab.mLifeTime);
//        }
//    }
//    private void CreateHardEffectTrack(BehaviourTag tag)
//    {
//        // 添加声音的轨道
//        System.Collections.Generic.List<BehaviourHardEffect> bsArray = tag.behaviourSystem.hardEffectElement;
//        foreach (BehaviourHardEffect bs in bsArray)
//        {
//            count++;
//            EffectHard prefab = gameObject.AddComponent<EffectHard>();
//            prefab.mLifeTime = bs.mLifeTime;         //绑定时间
//            if (bs.mLifeTime == 0)
//            {
//                prefab.mLifeTime = tag.behaviourSystem.mLifeTime;
//            }
//            prefab.mBindTime = bs.mBindTime;         //绑定时间
//            prefab.mDelayTime = bs.mDelayTime;
//            prefab.minVibrate = bs.minVibrate;
//            prefab.maxVibrate = bs.maxVibrate;

//            maxLifeTime = Mathf.Max(maxLifeTime, prefab.mLifeTime);
//        }
//    }

//    private void savePrefab()
//    {
//        _ = PrefabUtility.SaveAsPrefabAsset(gameObject, "Assets/Art/Effect/" + gameObject.name + ".prefab");

//        GameObject.DestroyImmediate(gameObject);
//        gameObject = null;
//    }



//}
