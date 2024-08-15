using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static UnityEngine.ParticleSystem;

public class ParticleSystemChangeColor : MonoBehaviour
{
    public Gradient[] gradients;
    private ParticleSystem particleSystem;
    private ParticleSystem.MainModule mainModule;

    private int currentGradientIndex = 0;

    void Start()
    {
        if (particleSystem == null)
        {
            particleSystem = GetComponent<ParticleSystem>();
            mainModule = particleSystem.main;
        }

    }

    private void OnEnable()
    {
        if(particleSystem == null)
        {
            particleSystem = GetComponent<ParticleSystem>();
            mainModule = particleSystem.main;
        }

        CustomDataModule cdm = particleSystem.customData;


    }

    void Update()
    {
        // 检查是否有渐变可供切换
        if (gradients != null && gradients.Length > 0)
        {
            // 在播放时切换渐变
            if (particleSystem.isPlaying)
            {
                ColorOverLifetimeModule coltm = particleSystem.colorOverLifetime;
                //coltm.color。
  
                coltm.color = new ParticleSystem.MinMaxGradient(gradients[currentGradientIndex]);
            }
        }
    }
}