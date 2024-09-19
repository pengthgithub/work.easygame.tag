using System;
using System.Collections.Generic;
using UnityEngine;

namespace Easy
{
    [Serializable] public enum FileType
    {
        fullName,
        simpleName,
        simpleNameWithExten
    }

    [Serializable]
    public class AddRessData
    {
        [SerializeField] public string label;
        [SerializeField] public string dirPath;
        [SerializeField] public string filter;
        [SerializeField] public FileType fileType;
        [SerializeField] public bool topDir;
    }

    [CreateAssetMenu(menuName = "Create AddressConfig", fileName = "AddressConfig", order = 0)]
    public class AddressConfig : ScriptableObject
    {
        [SerializeField] public List<AddRessData> configs;
    } 
}