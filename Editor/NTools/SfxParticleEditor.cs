using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Easy
{
    //https://blog.csdn.net/qq_34035956/article/details/125150682
    [CustomEditor(typeof(SfxParticle))]
    public class SfxParticleEditor : Editor
    {
        private bool _singleCast = true;
        private bool _needTarget = true;

        private float speed = 1;
        private float _rangdomMove = 0;
        private Vector3 castPos = new Vector3(10, 0, 19);
        private Vector3 targetPos = new Vector3(11, 0, 15);

        private string[] enemyList;

        public void OnEnable()
        {
            SceneView.duringSceneGui += SceneGUI;
            _sfxParticle = (SfxParticle)target;

            var cs = PlayerPrefs.GetString("castPos");
            var ts = PlayerPrefs.GetString("targetPos");
            enemyIndex = PlayerPrefs.GetInt("enemyIndex");
            castPos.Parse(cs);
            targetPos.Parse(ts);
            //_sliderStyle = EditorStyleCustom.SliderStyle;

            enemy = EditorPrefs.GetString("enemy", "p_h_001");
            targetPos.Set(EditorPrefs.GetFloat("tpx", 10), 0, EditorPrefs.GetFloat("tpz", 19));
            castPos.Set(EditorPrefs.GetFloat("cpx", 10), 0, EditorPrefs.GetFloat("cpz", 15));

            var files = Directory.GetFiles("Assets/Art/Character", "*.prefab", SearchOption.AllDirectories);
            enemyList = new string[files.Length];
            int i = 0;
            foreach (var file in files)
            {
                var name = Path.GetFileNameWithoutExtension(file);
                enemyList[i] = name;
                i++;
            }
        }

        #region EditorLogic

        private string[] anims;
        private string enemy = "p_bs_yang_01";
        private int aniIndex = 0;
        private int enemyIndex = 1;
        
        private void EnterPlayScene()
        {
            if (Application.isPlaying == false)
            {
                if (_sfx) _sfx.Dispose();

                var scene = EditorSceneManager.GetActiveScene();
                if (scene.name != "s_cd_green_hero_01")
                {
                    EditorSceneManager.OpenScene("Assets\\Art\\scene\\hero\\s_cd_green_hero_01.unity");
                }

                EditorApplication.EnterPlaymode();
            }

            if (Application.isPlaying == false) return;

            if (ECharacter.actorPool.IsInit == false)
            {
                ECharacter.actorPool.Init(5);
                var go = new GameObject();
                go.AddComponent<ESound>();
            }

            if (ECharacter.actorPool.IsInit == false)
            {
                ESfx.sfxPool.Init(5);
            }
            
            
            enemy = enemyList[enemyIndex];
            if (_targetCharacter && _targetCharacter.URL != enemy)
            {
                _targetCharacter.Dispose(0);
                _targetCharacter = null;
            }
            if (_targetCharacter == null )
            {
                _targetCharacter = ECharacter.Create(enemy);
                _targetCharacter.transform.position = targetPos;
            }

            if (_previewCharacter)
            {
                if (_previewCharacter.URL != _sfxParticle.previewName && _sfxParticle.previewName != "none")
                {
                    _previewCharacter.Dispose(0);
                    _previewCharacter = null;
                }
            }

            if (_previewCharacter == null)
            {
                _previewCharacter = ECharacter.Create(_sfxParticle.previewName);
                _previewCharacter.transform.position = castPos;
            }

            var animation = _previewCharacter._eAnimation;
            if (animation && animation.ClipMap != null)
            {
                anims = animation.ClipMap.Keys.ToArray();
            }
        }

        private static ECharacter _targetCharacter;
        private static ECharacter _previewCharacter;
        private static ESfx _sfx;

        private void Play()
        {
            if (Application.isPlaying == false) return;
            if (_sfx)
            {
                GameObject.Destroy(_sfx.gameObject);
                _sfx = null;
            }

            GameObject sfxNode = new GameObject("sfx");
            _sfx = sfxNode.AddComponent<ESfx>();
            _sfx.Owner = _previewCharacter;
            if (_needTarget)
            {
                _sfx.target = _targetCharacter;
            }

            _sfx._LoadCallBack(_sfxParticle);
            _sfx.maxLifeTime = _sfxParticle.lifeTime;
            _sfx.transform.position = _previewCharacter.transform.position;
            _sfx.transform.localPosition = Vector3.zero;
            lastTime = 0;

            if (anims != null && aniIndex < anims.Length && anims.Length != 0)
            {
                _previewCharacter.AnimationName = anims[aniIndex];
            }
        }

        #endregion

        public void OnDisable()
        {
            EditorPrefs.SetString("enemy", enemy);
            EditorPrefs.SetFloat("tpx", targetPos.x);
            EditorPrefs.SetFloat("tpz", targetPos.z);
            EditorPrefs.SetFloat("cpx", castPos.x);
            EditorPrefs.SetFloat("cpz", castPos.z);
            PlayerPrefs.SetInt("enemyIndex", aniIndex);
        }

        private SfxParticle _sfxParticle;
        private GUIStyle _sliderStyle;

        # region UI

        public override bool HasPreviewGUI()
        {
            return true;
        }

        public override GUIContent GetPreviewTitle()
        {
            return new GUIContent("特效标签编辑器");
        }

        void OnDestroy()
        {
            SceneView.duringSceneGui -= SceneGUI;
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            if (anims != null && anims.Length != 0)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("可选动作名:");
                aniIndex = EditorGUILayout.Popup(aniIndex, anims);
                _sfxParticle.previewAniName = anims[aniIndex];
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("敌人角色名:");
            enemyIndex = EditorGUILayout.Popup(enemyIndex, enemyList);
            GUILayout.EndHorizontal();

            _rangdomMove = EditorGUILayout.Slider("随机移动", _rangdomMove, 0, 5);

            GUILayout.BeginHorizontal();
            _singleCast = GUILayout.Toggle(_singleCast, "目标施放");
            _needTarget = GUILayout.Toggle(_needTarget, "朝目标释放");
            GUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            targetPos = EditorGUILayout.Vector3Field("敌人位置", targetPos);
            castPos = EditorGUILayout.Vector3Field("我方位置", castPos);
            if (EditorGUI.EndChangeCheck())
            {
                if (_targetCharacter) _targetCharacter.transform.position = targetPos;
                if (_previewCharacter) _previewCharacter.transform.position = castPos;
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("播放",GUILayout.Width(60), GUILayout.Height(40)))
            {
                EnterPlayScene();
                Play();
            }
            speed = EditorGUILayout.Slider(speed, 0, 3);
            if (_previewCharacter) _previewCharacter.PlaySpeed = speed;
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("销毁", "preButton"))
            {
                if (_sfx)
                {
                    GameObject.Destroy(_sfx.gameObject);
                }

                if (_previewCharacter) _previewCharacter.Dispose();
                _previewCharacter = null;
                anims = null;
            }
        }

        private float lastTime = 0;

        void SceneGUI(SceneView view)
        {
            //c参考：https://zhuanlan.zhihu.com/p/124269658
            Handles.BeginGUI();
            SceneView sceneView = SceneView.currentDrawingSceneView;
            var width = sceneView.position.width;
            var height = sceneView.position.height;

            float currentTime = 0;
            if (_sfx) currentTime = _sfx.updatedTime;

            GUILayout.BeginArea(new Rect(width - 150, height - 50, 145, 20)); // 规定显示区域为屏幕大小
            Color originalColor = EditorStyles.label.normal.textColor;
            // 设置标签颜色为红色
            EditorStyles.label.normal.textColor = Color.red;
            GUILayout.Label($"已播放时间：{currentTime}s", EditorStyles.label);
            // 将标签颜色恢复为原始颜色
            EditorStyles.label.normal.textColor = originalColor;
            GUILayout.EndArea();
            Handles.EndGUI();

            //========================================================
            if (_rangdomMove != 0)
            {
                if (_previewCharacter)
                {
                    _previewCharacter.transform.Translate(Vector3.forward * 0.01f * _rangdomMove);
                    var pos = _previewCharacter.transform.position;
                    if (pos.x < 10 || pos.x > 18 || pos.z < 10 || pos.z > 22)
                    {
                        var angle = _previewCharacter.transform.eulerAngles;
                        _previewCharacter.transform.eulerAngles =
                            new Vector3(0, angle.y + 180 + Random.Range(-30, 30), 0);
                    }
                }

                if (_targetCharacter && _previewCharacter)
                {
                    _targetCharacter.transform.Translate(Vector3.forward * 0.001f * _rangdomMove);
                    var pos = _targetCharacter.transform.position;
                    if (pos.x < 10 || pos.x > 18 || pos.z < 10 || pos.z > 22)
                    {
                        var angle = _previewCharacter.transform.eulerAngles;
                        _targetCharacter.transform.eulerAngles =
                            new Vector3(0, angle.y + 180 + Random.Range(-30, 30), 0);
                    }
                }
            }

            if (_sfx && _sfxParticle.moveSpeed != 0)
            {
                UpdateMove();
            }
        }

        #endregion

        private void UpdateMove()
        {
          if (_sfx == null)return;

            float currentTime = 0;
            if (_sfx) currentTime = _sfx.updatedTime;
            var speed = (float)(_sfxParticle.moveSpeed / 2) * (currentTime - lastTime);
            _sfx.transform.Translate(Vector3.forward * speed);
            if (_needTarget)
            {
                var dir = (_targetCharacter.transform.position - _sfx.transform.position).normalized;
                Quaternion rotation = Quaternion.LookRotation(dir);
                _sfx.transform.rotation = rotation;

                var dis = _sfx.transform.position - _targetCharacter.transform.position;
                if (dis.magnitude < 0.1f)
                {
                    GameObject.Destroy(_sfx.gameObject);
                    _sfx = null;
                }
            }

            lastTime = currentTime;

            if (_sfx && _sfx.updatedTime > 3)
            {
                GameObject.Destroy(_sfx.gameObject);
                _sfx = null;
            }
        }
    }
}