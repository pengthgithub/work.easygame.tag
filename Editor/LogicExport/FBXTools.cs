using System.IO;

using UnityEditor;
using UnityEditor.Animations;

using UnityEngine;

namespace Easy
{
    public class FBXTools
    {
        public static FBXTools Instance = new FBXTools();

        /// <summary>
        /// �Զ�����������
        /// </summary>
        [MenuItem("FBXTools/Uper Controller")]
        public static void UperController()
        {
            /// ���п�����
            string[] characterFiles = Directory.GetFiles(Application.dataPath + "/assets/character/", "*.controller",
                SearchOption.AllDirectories);
            /// ���¿�����
            for (int i = 0, n = characterFiles.Length; i < n; i++)
            {
                string _path = characterFiles[i];
                if (_path.IndexOf(".meta") != -1) continue;
                _path = "Assets" + _path.Replace(Application.dataPath, "");

                FBXTools.Instance._UperController(_path);
            }

            /// ���п�����
            string[] doodadFiles = Directory.GetFiles(Application.dataPath + "/assets/doodad/", "*.controller",
                SearchOption.AllDirectories);
            /// ���¿�����
            for (int i = 0, n = doodadFiles.Length; i < n; i++)
            {
                string _path = doodadFiles[i];
                if (_path.IndexOf(".meta") != -1) continue;
                _path = "Assets" + _path.Replace(Application.dataPath, "");
                FBXTools.Instance._UperController(_path);
            }
        }

        [MenuItem("FBXTools/UperCharacter")]
        public static void UperCharacter()
        {

        }

        //===============================================================================================
        //===============================================================================================

        #region ������

        /// <summary>
        /// ���¿�����
        /// </summary>
        /// <param name="path"></param>
        public void _UperController(string path)
        {
            /// ��ӿ��Ʋ���
            AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(path);
            if (controller == null)
            {
                return;
            }

            //��������
            _UperClip(controller, path);
            //����״̬
            _UperState(controller);
        }

        /// <summary>
        /// �ڿ��������״̬
        /// </summary>
        /// <param name="controller"></param>
        public void _UperState(AnimatorController controller)
        {
            /// ������
            var rootStateMachine = controller.layers[0].stateMachine;
            if (rootStateMachine == null || rootStateMachine.defaultState == null)
            {
                Debug.LogError(controller.name + " û����Ӷ����ڵ�.");
                return;
            }

            string rootName = rootStateMachine.defaultState.name;

            UnityEditor.Animations.AnimatorControllerLayer layer = controller.layers[0];
            ChildAnimatorState[] states = layer.stateMachine.states;
            for (int j = 0; j < states.Length; j++)
            {
                ChildAnimatorState _aniState = states[j];
                AnimatorStateTransition[] ast = _aniState.state.transitions;
                if (ast.Length != 0)
                {
                    for (int i = 0; i < ast.Length; i++)
                    {
                        _aniState.state.RemoveTransition(ast[i]);
                    }
                }
            }

            for (int j = 0; j < states.Length; j++)
            {
                ChildAnimatorState _aniState = states[j];
                string statName = _aniState.state.name;
                if (statName == rootName)
                {
                    continue;
                }

                //idle�������̶�����
                AnimatorStateTransition statTrans = rootStateMachine.defaultState.AddTransition(_aniState.state);
                statTrans.AddCondition(AnimatorConditionMode.If, 1, statName);
                statTrans.hasFixedDuration = false;
                statTrans.hasExitTime = false;
                statTrans.duration = 0.2f;
                statTrans.offset = 0;

                if (statName != "dead")
                {
                    // �̶�������idle����
                    statTrans = _aniState.state.AddTransition(rootStateMachine.defaultState);
                    statTrans.AddCondition(AnimatorConditionMode.If, 0, rootName);
                    statTrans.hasFixedDuration = false;
                    statTrans.hasExitTime = false;
                    statTrans.duration = 0.2f;
                    statTrans.offset = 0;
                }
            }
        }

        /// <summary>
        /// �ڿ���������Ӷ���
        /// </summary>
        /// <param name="controller"></param>
        public void _UperClip(AnimatorController controller, string path)
        {
            /// ��Ӷ����ڵ�
            AnimationClip[] clips = controller.animationClips;
            if (clips.Length == 0)
            {

            }

            for (int j = 0, k = clips.Length; j < k; j++)
            {
                AnimationClip clip = clips[j];
                string _name = clip.name;

                bool _enabled = false;
                AnimatorControllerParameter[] _params = controller.parameters;
                for (int l = 0; l < _params.Length; l++)
                {
                    AnimatorControllerParameter _param = _params[l];
                    if (_param.name == _name)
                    {
                        _enabled = true;
                    }
                }

                if (!_enabled)
                {
                    controller.AddParameter(clip.name, AnimatorControllerParameterType.Bool);
                }
            }
        }

        #endregion ������



    }
}