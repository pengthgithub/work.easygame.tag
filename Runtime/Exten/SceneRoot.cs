using System.Collections.Generic;
using Easy;
using UnityEngine;

public class SceneRoot : MonoBehaviour
{
    public Transform wujian;
    
    internal void Unload()
    {
        foreach (var ec in characters)
        {
            ec.Dispose();
        }
        characters.Clear();  
        GameObject.Destroy(effectInstance);
    }
    private List<ECharacter> characters = new List<ECharacter>();
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
    [Header("主界面场景表现切换")]
    [SerializeField] private MeshRenderer bgRender;
    [SerializeField] private MeshRenderer landRender;
    [SerializeField] private GameObject effectNode;
    private GameObject effectInstance;
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
            effectInstance = GameObject.Instantiate(effect, effectNode.transform, false);
            effectInstance.SetActive(true);
        }
    }
}
