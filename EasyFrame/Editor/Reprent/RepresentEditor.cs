#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Easy
{
    [CustomEditor(typeof(Represent))]
    public class RepresentEditor:Editor
    {
        private Represent rep;
        private void OnEnable()
        {
            rep = target as Represent;
        }

        private void OnDisable()
        {
            
        }

        private string url = "sfx_p_bz_xz_dy_s_01_buff";// "p_boss_zddj_01";
        private Represent _represent;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            url = EditorGUILayout.TextField("路径", url);
            if (GUILayout.Button("创建"))
            {
                Represent.PoolInit(10);
                _represent = Represent.Create(url);
                _represent.Position = rep.transform.position;
            }
        }
    }
}
#endif