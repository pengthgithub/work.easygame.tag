using System;
using UnityEngine;

namespace Easy
{
    public class Easy : MonoBehaviour
    {
        public static Easy Instance { get; set; }
        [SerializeField] public GameObject PlayerRoot;
        [SerializeField] public GameObject SfxRoot;



        public void Awake()
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;

        }
    }
}