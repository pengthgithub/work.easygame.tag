using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;

namespace Easy
{
    [CustomEditor(typeof(SfxParticle))]
    public class SfxParticleEditor : UnityEditor.Editor
    { 
        private int tabID = 0;
        private string[] tableNames;

        private SfxParticle sfxParticle;

        private SerializedProperty lifeTimePro;
        private SerializedProperty speedPro;
        private SerializedProperty deathSfxPro;

        private SerializedProperty prefabPro;
        private SerializedProperty ownerPro;
        private SerializedProperty cameraPro;
        private SerializedProperty soundPro;
        private bool hasEnable = false;
        
        void OnEnable()
        {
            hasEnable = true;
            sfxParticle = target as SfxParticle;

            if (sfxParticle && (string.IsNullOrEmpty(sfxParticle.url) || sfxParticle.url != sfxParticle.name ))
            {
                sfxParticle.url = sfxParticle.name;
                EditorUtility.SetDirty(sfxParticle);
                AssetDatabase.SaveAssets();
            }
            
            if (sfxParticle && sfxParticle.deathTimeTotal == 0)
            {
                foreach (var sp in sfxParticle.sfxPrefab)
                {
                    sfxParticle.deathTimeTotal += sp.deleteNow;
                }

                if (sfxParticle.deathTimeTotal != 0)
                {
                    EditorUtility.SetDirty(sfxParticle);
                    AssetDatabase.SaveAssets();
                }
            }

            CalAudioLifeTime();

            ReadProperty();
            Read();

            tableNames = new string[] { "粒子", "拥有者", "镜头", "声音" };
            tableNames[0] = "粒子 " + prefabPro.arraySize;
            tableNames[1] = "拥有者 " + ownerPro.arraySize;
            tableNames[2] = "镜头 " + cameraPro.arraySize;
            tableNames[3] = "声音 " + soundPro.arraySize;
            
            oldVerision = UnityEngine.PlayerPrefs.GetInt("version");
        }

        void OnDisable()
        {
            if (hasEnable) Save();
            hasEnable = false;
            
            UnityEngine.PlayerPrefs.SetInt("version", oldVerision == 1?1:0);
        }

        void ReadProperty()
        {
            lifeTimePro = serializedObject.FindProperty("lifeTime");
            speedPro = serializedObject.FindProperty("speed");
            deathSfxPro = serializedObject.FindProperty("deathSfx");
            prefabPro = serializedObject.FindProperty("sfxPrefab");
            ownerPro = serializedObject.FindProperty("sfxOwner");
            cameraPro = serializedObject.FindProperty("sfxShark");
            soundPro = serializedObject.FindProperty("sfxSound");
        }
        

        //=====================================================================
        // 存取读取数据
        //=====================================================================

        #region 存取读取数据
        private string previewAni = "run";
        private string[] arrayNames;
        private float speed = 1;
        private static string lastPreviewText;
        private bool recyleModle = false;
        private void Save()
        {
        }

        private void Read()
        {
            if (string.IsNullOrEmpty(sfxParticle.enemy)) sfxParticle.enemy = "p_h_001";
            if (string.IsNullOrEmpty(sfxParticle.preview)) sfxParticle.preview = "p_h_001";
            
            if (string.IsNullOrEmpty(sfxParticle.defaultScene)) sfxParticle.defaultScene = "xingqiu2";
            
            if (sfxParticle.pos.x == 0 && sfxParticle.pos.z == 0)
            {
                sfxParticle.pos = new Vector3(14, 0, 14);
            }
        }
        #endregion

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("");
            DrawEditor();

            serializedObject.Update();
            GUILayout.BeginVertical("box");
            EditorGUILayout.LabelField(sfxParticle.url);
            EditorGUILayout.Space(5);
            EditorGUILayout.PropertyField(lifeTimePro);
            EditorGUILayout.Space(5);
            EditorGUILayout.PropertyField(speedPro);
            EditorGUILayout.Space(5);
            EditorGUILayout.PropertyField(deathSfxPro);
            GUILayout.EndVertical();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
    
            tabID = GUILayout.Toolbar(tabID, tableNames);
            switch (tabID)
            {
                case 0:
                {
                    DrawLizi();
                }
                    break;
                case 1:
                {
                    EditorGUILayout.PropertyField(ownerPro, true);
                }
                    break;
                case 2:
                {
                    EditorGUILayout.PropertyField(cameraPro, true);
                }
                    break;
                case 3:
                {
                    EditorGUILayout.PropertyField(soundPro, true);
                    CalAudioLifeTime();
                }
                    break;
            }
            serializedObject.ApplyModifiedProperties();

        }

        private int oldVerision = 0;
        private void DrawLizi()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+", GUILayout.Width(40), GUILayout.Height(40)))
            {
                sfxParticle.sfxPrefab.Add(new SfxPrefab());
            }
            EditorGUILayout.LabelField("", GUILayout.Width(100));
            if (GUILayout.Button(oldVerision == 1 ? "老版本" : "新版本", GUILayout.Width(80), GUILayout.Height(40)))
            {
                oldVerision = oldVerision == 0 ? 1 : 0;
            }
            EditorGUILayout.EndHorizontal();
            
            if (oldVerision == 0)
            {
                EditorGUILayout.PropertyField(prefabPro, true);
                return;
            }
            
            var width = GUILayoutUtility.GetLastRect().width;
            int c = sfxParticle.sfxPrefab.Count;
            for (int i = 0; i < c; i++)
            {
                EditorGUILayout.BeginVertical("box");
                SfxPrefab sfxPrefab = sfxParticle.sfxPrefab[i];
                EditorGUILayout.BeginHorizontal();
               
                EditorGUILayout.LabelField("Element " + i, GUILayout.Width(100));
                EditorGUILayout.LabelField("调试",GUILayout.Width(30));
                sfxPrefab.debug = EditorGUILayout.Toggle(sfxPrefab.debug,GUILayout.Width(30));
                EditorGUILayout.Space(5,true);
                if (GUILayout.Button("-", GUILayout.Width(30)))
                {
                    sfxParticle.sfxPrefab.RemoveAt(i);
                    return;
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space(5);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("",GUILayout.Width(15));
                EditorGUILayout.LabelField( "绑定时间",GUILayout.Width(55));
                sfxPrefab.bindTime = EditorGUILayout.Slider(sfxPrefab.bindTime, 0.0f, 10.0f);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space(5);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("",GUILayout.Width(15));
                EditorGUILayout.LabelField( "插槽", GUILayout.Width(30));
                sfxPrefab.locatorType = (LocatorType)EditorGUILayout.EnumPopup(sfxPrefab.locatorType, GUILayout.Width(120));
                EditorGUILayout.Space(5,true);
                EditorGUILayout.LabelField( "目标", GUILayout.Width(30));
                sfxPrefab.targetlocatorType = (LocatorType)EditorGUILayout.EnumPopup(sfxPrefab.targetlocatorType, GUILayout.Width(120));
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space(5);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("",GUILayout.Width(15));
                EditorGUILayout.LabelField("永远不更新旋转",GUILayout.Width(100));
                sfxPrefab.noRotation = EditorGUILayout.Toggle(sfxPrefab.noRotation,GUILayout.Width(30));
                EditorGUILayout.Space(5,true);
                EditorGUILayout.LabelField("永远不更新缩放",GUILayout.Width(100));
                sfxPrefab.noScale = EditorGUILayout.Toggle(sfxPrefab.noScale,GUILayout.Width(30));
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space(5);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("",GUILayout.Width(15));
                EditorGUILayout.LabelField("只在创建时刷新一次",GUILayout.Width(120));
                EditorGUILayout.LabelField("位置",GUILayout.Width(30));
                sfxPrefab.useSfxPosition = EditorGUILayout.Toggle(sfxPrefab.useSfxPosition,GUILayout.Width(30));
                if (sfxPrefab.noRotation == false)
                {
                    EditorGUILayout.LabelField("旋转",GUILayout.Width(30));
                    sfxPrefab.useSfxRotation = EditorGUILayout.Toggle(sfxPrefab.useSfxRotation,GUILayout.Width(30));
                }

                if (sfxPrefab.noScale == false)
                {
                    EditorGUILayout.LabelField("缩放",GUILayout.Width(30));
                    sfxPrefab.useSfxScale = EditorGUILayout.Toggle(sfxPrefab.useSfxScale,GUILayout.Width(30)); 
                }
    
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space(5);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("",GUILayout.Width(15));
                EditorGUILayout.LabelField("表现",GUILayout.Width(40), GUILayout.Height(25));
                sfxPrefab.prefab = (GameObject)EditorGUILayout.ObjectField(sfxPrefab.prefab, typeof(GameObject), true, GUILayout.Height(25));
                EditorGUILayout.EndHorizontal();
                float maxLifeTime = 0;
                float maxDeleteTime = 10;
                if (sfxPrefab.prefab)
                {
                    maxLifeTime = sfxPrefab.prefab.transform.GetMaxParticleLifetime(out float delayTime);
                    maxDeleteTime = delayTime * 0.8f;
                }

                if (maxLifeTime != 0)
                {
                    EditorGUILayout.Space(5);
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("",GUILayout.Width(30));
                    EditorGUILayout.LabelField("生命周期",GUILayout.Width(60));
                    sfxPrefab.lifeTime = EditorGUILayout.Slider(sfxPrefab.lifeTime, 0,maxLifeTime);
                    EditorGUILayout.EndHorizontal(); 
                }

                if (sfxPrefab.prefab)
                {
                    SfxControl sfx = sfxPrefab.prefab.GetComponent<SfxControl>();
                    if (sfx && sfx.animLists != null && sfx.animLists.Count > 0)
                    {
                        foreach (var clipdata in sfx.animLists)
                        {
                            if (clipdata.enabled && maxDeleteTime <= clipdata.length)
                                maxDeleteTime = clipdata.length;
                        }
                    }
                    else
                    {
                        maxDeleteTime = 2;
                    }
                }
                
                if (maxDeleteTime != 0)
                {
                    EditorGUILayout.Space(5);
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("",GUILayout.Width(30));
                    EditorGUILayout.LabelField("死亡消失时间",GUILayout.Width(80));
                    sfxPrefab.deleteNow = EditorGUILayout.Slider(sfxPrefab.deleteNow, 0,maxDeleteTime);
                    EditorGUILayout.EndHorizontal();
                }
                
                EditorGUILayout.Space();
                EditorGUILayout.EndVertical();
            }
        }

        private void DrawEditor()
        {
            // 绘制一个黑色的背景框
            GUILayout.BeginVertical("box");
            if (Application.isPlaying == false)
            {
                sfxParticle.enemy = EditorGUILayout.TextField("敌人", sfxParticle.enemy);
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("预览:", GUILayout.Width(60));
            sfxParticle.preview = EditorGUILayout.TextField(sfxParticle.preview);
            if (caster && caster.__0Control)
            {
                var ani = caster.__0Control.allAni;
                if (arrayNames == null || ani.Count > arrayNames.Length)
                {
                    arrayNames = new string[ani.Count];
                }

                for (int i = 0; i < ani.Count; i++)
                {
                    arrayNames[i] = ani[i].name;
                }

                sfxParticle.aniIndex = EditorGUILayout.Popup(sfxParticle.aniIndex, arrayNames);
                if (sfxParticle.aniIndex < arrayNames.Length)
                {
                    previewAni = arrayNames[sfxParticle.aniIndex];
                }
            }

            EditorGUILayout.EndHorizontal();

            sfxParticle.moveSpeed = EditorGUILayout.IntSlider("移动速度", sfxParticle.moveSpeed, 0, 128);
            if (Application.isPlaying == false)
            {
                sfxParticle.defaultScene = EditorGUILayout.TextField("默认场景:", sfxParticle.defaultScene);
            }
            else
            {
                sfxParticle.pos = EditorGUILayout.Vector3Field("位置:", sfxParticle.pos);
            }

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical();
            if (GUILayout.Button("计算时间"))
            {
                CalLifeTime();
            }

            recyleModle = GUILayout.Toggle(recyleModle, "回收");
            EditorGUILayout.EndVertical();
            if (GUILayout.Button("预览", GUILayout.Width(60), GUILayout.Height(40)))
            {
                EnterPlaymode();
                if (Application.isPlaying)
                {
                    Represent.PoolInit(10);
                    CreateCastAndEnemy();
                    InitAudioSound();
                }
            }

            if (GUILayout.Button("停止", GUILayout.Width(60), GUILayout.Height(40)))
            {
                if (sfx)
                {
                    Represent.RemoveWithEditorModel(sfx);
                    //sfx.Dispose();
                    GameObject.DestroyImmediate(sfx.gameObject);
                    sfx = null;
                }
            }

            if (GUILayout.Button("保存", GUILayout.Width(60), GUILayout.Height(40)))
            {
                EditorUtility.SetDirty(sfxParticle);
                AssetDatabase.SaveAssets();
            }

            EditorGUILayout.EndHorizontal();

            EditorGUI.BeginChangeCheck();
            speed = EditorGUILayout.Slider("速度", speed, 0, 3);
            if (EditorGUI.EndChangeCheck())
            {
                if (caster) caster.Speed = speed;
            }

            GUILayout.EndVertical();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
        }

        private void InitAudioSound()
        {
            if(SoundMgr.Instance != null) return;
            GameObject audio = new GameObject("Audio");
            var soundMgr = audio.AddComponent<SoundMgr>();
            
            GameObject sound = new GameObject("Sound");
            sound.transform.parent = audio.transform;
            var bgAudio = sound.AddComponent<AudioSource>();
            bgAudio.playOnAwake = false;
            soundMgr.bgmSource = bgAudio;
            soundMgr.audioSources = new List<AudioSource>();
            for (int i = 0; i < 10; i++)
            {
                GameObject audioSound = new GameObject("Sound");
                audioSound.transform.parent = audio.transform;
                
                var audioSource = audioSound.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                
                soundMgr.audioSources.Add(audioSource);
            }

            soundMgr.groupCount = new List<int> { 0, 4, 3, 3 };
            soundMgr.Init();
        }

        private void CalAudioLifeTime()
        {
            if (sfxParticle.sfxSound.Count > 0)
            {
                bool changed = false;
                foreach (var sfx in sfxParticle.sfxSound)
                {
                    if (sfx.randomClips != null && sfx.randomClips.Count != 0)
                    {
                        float lifeTime = 0;
                        foreach (var _clip in sfx.randomClips)
                        {
                            if (_clip && lifeTime < _clip.length)
                            {
                                lifeTime = _clip.length;
                            }
                        }

                        if (sfx.lifeTime > lifeTime)
                        {
                            sfx.lifeTime = lifeTime;
                            changed = true;
                        }
                    }
                }

                if (changed)
                {
                    EditorUtility.SetDirty(sfxParticle);
                    AssetDatabase.SaveAssets();
                }
            }
        }
        
        private void CalLifeTime()
        {
            foreach (var owner in sfxParticle.sfxOwner)
            {
                if (owner.animationClip != null)
                {
                    owner.clipName = owner.animationClip.name;
                }
            }

            float maxLifeTime = 0;
            //特效的生命周期计算为：Duration + StartDelay + MaxLifeTime
            foreach (SfxPrefab sfx in sfxParticle.sfxPrefab)
            {
                if (sfx.prefab)
                {
                    if (maxLifeTime < sfx.lifeTime + sfx.bindTime + sfx.deleteNow)
                    {
                        maxLifeTime = sfx.lifeTime + sfx.bindTime + sfx.deleteNow;
                    }
                }
            }

            foreach (var owner in sfxParticle.sfxOwner)
            {
                if (owner.animationClip != null)
                {
                    owner.lifeTime = owner.animationClip.length;
                    owner.clipName = owner.animationClip.name;
                }

                if (maxLifeTime < owner.lifeTime + owner.bindTime)
                {
                    maxLifeTime = owner.lifeTime + owner.bindTime;
                }
            }

            foreach (var shark in sfxParticle.sfxShark)
            {
                if (maxLifeTime < shark.lifeTime + shark.bindTime)
                {
                    maxLifeTime = shark.lifeTime + shark.bindTime;
                }
            }

            CalAudioLifeTime();

            if (sfxParticle.lifeTime > maxLifeTime)
            {
                sfxParticle.lifeTime = maxLifeTime;
            }
           
            if (sfxParticle.lifeTime >= 20)
            {
                sfxParticle.lifeTime = 20;
            }
        }

        #region Editor

        private void EnterPlaymode()
        {
            if (Application.isPlaying == false)
            {
                var name = EditorSceneManager.GetActiveScene().name;
                if (name != sfxParticle.defaultScene)
                {
                    var files = Directory.GetFiles("Assets/Art/Scene/", $"{sfxParticle.defaultScene}.unity", SearchOption.AllDirectories);
                    if (files.Length > 0)
                    {
                        EditorSceneManager.OpenScene(files[0]);
                    }
                    else
                    {
                        Debug.LogError("不存在地图:" + sfxParticle.defaultScene);
                        return;
                    }
                }

                EditorApplication.EnterPlaymode();
                return;
            }
        }

        private static Represent caster;
        private static Represent enemy;
        internal static Represent sfx;

        private void CreateCastAndEnemy()
        {
            if (lastPreviewText != sfxParticle.preview && caster)
            {
                caster.Dispose();
                caster = null;
            }

            if (caster == null && sfxParticle.preview != "")
            {
                lastPreviewText = sfxParticle.preview;
                caster = Represent.Create(sfxParticle.preview);
                caster.Position = sfxParticle.pos;
            }

            if (enemy == null)
            {
                enemy = Represent.Create(sfxParticle.preview);
                var pos = sfxParticle.pos + new Vector3(0, 0, 2);
                enemy.Position = pos;
            }

            if (sfx)
            {
                if (recyleModle)
                {
                    sfx.Dispose();
                    sfx = null;
                }
                else
                {
                    Represent.RemoveWithEditorModel(sfx);
                    GameObject.DestroyImmediate(sfx.gameObject);
                }
            }

            if (sfx == null)
            {
                sfx = Represent.Create(sfxParticle.name, caster);
                sfx.Target = enemy;
                if (caster)
                {
                    caster.AnimationName = previewAni;
                }
                else
                {
                    sfx.Position = sfxParticle.pos;
                }

                if (sfxParticle.moveSpeed != 0)
                {
                    sfx.MoveBullet(sfxParticle.moveSpeed, enemy.transform);
                }
            }
        }

        #endregion
    }
}