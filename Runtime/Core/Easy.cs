using UnityEngine;

namespace Easy
{
    public class Easy : MonoBehaviour
    {
        [SerializeField] public GameObject PlayerRoot;
        [SerializeField] public GameObject SfxRoot;
        public static Easy Instance { get; set; }

        public void Awake()
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
    }
}