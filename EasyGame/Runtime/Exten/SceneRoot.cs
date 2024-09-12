using System.Collections.Generic;
using UnityEngine;

namespace Easy
{
    public class SceneRoot : MonoBehaviour
    {
        public Transform wujian;

        [Header("主界面场景表现切换")] [SerializeField] private MeshRenderer bgRender;

        [SerializeField] private MeshRenderer landRender;
        [SerializeField] private GameObject effectNode;
        private readonly List<ECharacter> characters = new();
        private GameObject effectInstance;

        internal void Unload()
        {
            foreach (var ec in characters) ec.Dispose();
            characters.Clear();
            Destroy(effectInstance);
        }

        public void CreateSceneElement(string url, Vector3 pos)
        {
            var ec = ECharacter.Create(url);
            if (ec)
            {
                ec.transform.SetParent(wujian);
                ec.transform.position = pos;
                characters.Add(ec);
            }
        }

        public async void ChangeBattleRes(string bgUrl, string islandUrl, string effectUrl)
        {
            var tex2D = await ELoader.LoadAsset<Texture2D>(bgUrl);
            if (bgRender) bgRender.sharedMaterial.mainTexture = tex2D;

            var tex2D1 = await ELoader.LoadAsset<Texture2D>(islandUrl);
            if (landRender) landRender.sharedMaterial.mainTexture = tex2D1;

            var effect = await ELoader.LoadAsset<GameObject>(effectUrl);
            if (effectNode && effect)
            {
                effectNode.transform.RemoveAllChild();
                effectInstance = Instantiate(effect, effectNode.transform, false);
                effectInstance.SetActive(true);
            }
        }
    }
}