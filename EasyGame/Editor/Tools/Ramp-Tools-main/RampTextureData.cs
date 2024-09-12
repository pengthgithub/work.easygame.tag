using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Create RampTextureData", fileName = "RampTexture", order = 0)]
public class RampTextureData : ScriptableObject
{
    public string gradientName = "RampTexture";
    public int width = 256;//每一条渐变的宽度
    public List<Gradient> gradients = new List<Gradient>();
    public string savePath = "";
}