using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
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
            ReadProperty();
            Read();

            tableNames = new string[] { "粒子", "拥有者", "镜头", "声音" };
            tableNames[0] = "粒子 " + prefabPro.arraySize;
            tableNames[1] = "拥有者 " + ownerPro.arraySize;
            tableNames[2] = "镜头 " + cameraPro.arraySize;
            tableNames[3] = "声音 " + soundPro.arraySize;
        }

        void OnDisable()
        {
            if (hasEnable) Save();
            hasEnable = false;
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
            EditorGUILayout.PropertyField(lifeTimePro);
            EditorGUILayout.PropertyField(speedPro);
            EditorGUILayout.PropertyField(deathSfxPro);
            GUILayout.EndVertical();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            GUILayout.BeginVertical("box");
            tabID = GUILayout.Toolbar(tabID, tableNames);
            switch (tabID)
            {
                case 0:
                {
                    DrawLizi();
                    EditorGUILayout.PropertyField(prefabPro, true);
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
                }
                    break;
            }

            serializedObject.ApplyModifiedProperties();
            GUILayout.EndVertical();
        }

        private void DrawLizi()
        {
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
            if (caster && caster._0Control)
            {
                var ani = caster._0Control.allAni;
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
                }
            }

            if (GUILayout.Button("停止", GUILayout.Width(60), GUILayout.Height(40)))
            {
                if (sfx)
                {
                    Represent.Remove(sfx);
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
                    sfx.lifeTime = sfx.prefab.transform.GetMaxParticleLifetime(out float delayTime);
                    sfx.deleteNow = delayTime;

                    if (maxLifeTime < sfx.lifeTime + sfx.bindTime + sfx.deleteNow)
                    {
                        maxLifeTime = sfx.lifeTime + sfx.bindTime + sfx.deleteNow;
                    }
                }
            }

            foreach (SfxSound sfx in sfxParticle.sfxSound)
            {
                if (sfx.randomClips != null && sfx.randomClips.Count != 0)
                {
                    foreach (var _clip in sfx.randomClips)
                    {
                        if (sfx.lifeTime < _clip.length)
                        {
                            sfx.lifeTime = _clip.length;
                        }
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

            foreach (var sound in sfxParticle.sfxSound)
            {
                if (maxLifeTime < sound.lifeTime + sound.bindTime)
                {
                    maxLifeTime = sound.lifeTime + sound.bindTime;
                }
            }

            sfxParticle.lifeTime = maxLifeTime;
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
                    Represent.Remove(sfx);
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