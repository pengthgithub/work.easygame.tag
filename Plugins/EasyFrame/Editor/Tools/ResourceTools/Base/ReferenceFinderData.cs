// Decompiled with JetBrains decompiler
// Type: WeChatWASM.Analysis.ReferenceFinderData
// Assembly: wx-editor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 491FB96F-C7AE-469B-A755-45BC3F066C9B
// Assembly location: C:\Users\pengt\Documents\SVN\Luck\Client\Lucky\Packages\com.qq.weixin.minigame@981890fdaa\Editor\wx-editor.dll
// XML documentation location: C:\Users\pengt\Documents\SVN\Luck\Client\Lucky\Packages\com.qq.weixin.minigame@981890fdaa\Editor\wx-editor.xml

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;

#nullable disable
namespace Analysis
{
    public class ReferenceFinderData
    {
        private const string at = "Library/ReferenceFinderCache";

        public Dictionary<string, ReferenceFinderData.AssetDescription> assetDict =
            new Dictionary<string, ReferenceFinderData.AssetDescription>();

        public void CollectDependenciesInfo()
        {
            try
            {
                string[] allAssetPaths = AssetDatabase.GetAllAssetPaths();
                int length = allAssetPaths.Length;
                for (int index = 0; index < allAssetPaths.Length; ++index)
                {
                    if (index % 100 == 0 && EditorUtility.DisplayCancelableProgressBar(StringUtils.HX,
                            string.Format(StringUtils.Hx, (object)index), (float)index / (float)length))
                    {
                        EditorUtility.ClearProgressBar();
                        return;
                    }

                    if (File.Exists(allAssetPaths[index]))
                        this.A(allAssetPaths[index]);
                    if (index % 2000 == 0)
                        GC.Collect();
                }

                this.UpdateReferenceInfo();
                EditorUtility.ClearProgressBar();
            }
            catch (Exception ex)
            {
                Debug.LogError((object)ex);
                EditorUtility.ClearProgressBar();
            }
        }

        public void UpdateReferenceInfo()
        {
            foreach (KeyValuePair<string, ReferenceFinderData.AssetDescription> keyValuePair in this.assetDict)
            {
                foreach (string dependency in keyValuePair.Value.dependencies)
                    this.assetDict[dependency].references.Add(keyValuePair.Key);
            }
        }

        private void A( string obj0_1)
        {
            string guid = AssetDatabase.AssetPathToGUID(obj0_1);
            Hash128 assetDependencyHash = AssetDatabase.GetAssetDependencyHash(obj0_1);
            if (this.assetDict.ContainsKey(guid) && !(this.assetDict[guid].assetDependencyHash != assetDependencyHash))
                return;
            List<string> list = ((IEnumerable<string>)AssetDatabase.GetDependencies(obj0_1, false))
                .Select<string, string>((Func<string, string>)(( obj0_2) => AssetDatabase.AssetPathToGUID(obj0_2)))
                .ToList<string>();
            ReferenceFinderData.AssetDescription assetDescription = new ReferenceFinderData.AssetDescription();
            assetDescription.name = Path.GetFileNameWithoutExtension(obj0_1);
            assetDescription.path = obj0_1;
            assetDescription.assetDependencyHash = assetDependencyHash;
            assetDescription.dependencies = list;
            if (this.assetDict.ContainsKey(guid))
                this.assetDict[guid] = assetDescription;
            else
                this.assetDict.Add(guid, assetDescription);
        }

        public bool ReadFromCache()
        {
            this.assetDict.Clear();
            if (!File.Exists(StringUtils.HY))
                return false;
            // ISSUE: object of a compiler-generated type is created
            // ISSUE: variable of a compiler-generated type
            ReferenceFinderData.B b = new ReferenceFinderData.B();
            // ISSUE: reference to a compiler-generated field
            b.cA = new List<string>();
            List<Hash128> hash128List = new List<Hash128>();
            List<int[]> numArrayList = new List<int[]>();
            using (FileStream serializationStream = File.OpenRead(StringUtils.HY))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                // ISSUE: reference to a compiler-generated field
                b.cA = (List<string>)binaryFormatter.Deserialize((Stream)serializationStream);
                hash128List = (List<Hash128>)binaryFormatter.Deserialize((Stream)serializationStream);
                numArrayList = (List<int[]>)binaryFormatter.Deserialize((Stream)serializationStream);
            }

            // ISSUE: reference to a compiler-generated field
            for (int index = 0; index < b.cA.Count; ++index)
            {
                // ISSUE: reference to a compiler-generated field
                string assetPath = AssetDatabase.GUIDToAssetPath(b.cA[index]);
                if (!string.IsNullOrEmpty(assetPath))
                {
                    // ISSUE: reference to a compiler-generated field
                    this.assetDict.Add(b.cA[index], new ReferenceFinderData.AssetDescription()
                    {
                        name = Path.GetFileNameWithoutExtension(assetPath),
                        path = assetPath,
                        assetDependencyHash = hash128List[index]
                    });
                }
            }

            // ISSUE: reference to a compiler-generated field
            for (int index = 0; index < b.cA.Count; ++index)
            {
                // ISSUE: reference to a compiler-generated field
                string key = b.cA[index];
                if (this.assetDict.ContainsKey(key))
                {
                    // ISSUE: reference to a compiler-generated field
                    // ISSUE: reference to a compiler-generated field
                    // ISSUE: reference to a compiler-generated method
                    List<string> list = ((IEnumerable<int>)numArrayList[index])
                        .Select<int, string>(b.ca ?? (b.ca = new Func<int, string>(b.A)))
                        .Where<string>((Func<string, bool>)(( obj0) => this.assetDict.ContainsKey(obj0)))
                        .ToList<string>();
                    this.assetDict[key].dependencies = list;
                }
            }

            this.UpdateReferenceInfo();
            return true;
        }

        private void A()
        {
            // ISSUE: object of a compiler-generated type is created
            // ISSUE: variable of a compiler-generated type
            ReferenceFinderData.C c = new ReferenceFinderData.C();
            if (File.Exists(StringUtils.HY))
                File.Delete(StringUtils.HY);
            List<string> graph1 = new List<string>();
            List<Hash128> graph2 = new List<Hash128>();
            List<int[]> graph3 = new List<int[]>();
            // ISSUE: reference to a compiler-generated field
            c.cB = new Dictionary<string, int>();
            using (FileStream serializationStream = File.OpenWrite(StringUtils.HY))
            {
                foreach (KeyValuePair<string, ReferenceFinderData.AssetDescription> keyValuePair in this.assetDict)
                {
                    // ISSUE: reference to a compiler-generated field
                    // ISSUE: reference to a compiler-generated field
                    c.cB.Add(keyValuePair.Key, c.cB.Count);
                    graph1.Add(keyValuePair.Key);
                    graph2.Add(keyValuePair.Value.assetDependencyHash);
                }

                foreach (string key in graph1)
                {
                    // ISSUE: reference to a compiler-generated field
                    // ISSUE: reference to a compiler-generated field
                    // ISSUE: reference to a compiler-generated method
                    int[] array = this.assetDict[key].dependencies
                        .Select<string, int>(c.cb ?? (c.cb = new Func<string, int>(c.A))).ToArray<int>();
                    graph3.Add(array);
                }

                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize((Stream)serializationStream, (object)graph1);
                binaryFormatter.Serialize((Stream)serializationStream, (object)graph2);
                binaryFormatter.Serialize((Stream)serializationStream, (object)graph3);
            }
        }

        public void UpdateAssetState(string guid)
        {
            ReferenceFinderData.AssetDescription assetDescription;
            if (this.assetDict.TryGetValue(guid, out assetDescription) &&
                assetDescription.state != ReferenceFinderData.AssetState.NODATA)
            {
                if (File.Exists(assetDescription.path))
                {
                    if (assetDescription.assetDependencyHash !=
                        AssetDatabase.GetAssetDependencyHash(assetDescription.path))
                        assetDescription.state = ReferenceFinderData.AssetState.CHANGED;
                    else
                        assetDescription.state = ReferenceFinderData.AssetState.NORMAL;
                }
                else
                    assetDescription.state = ReferenceFinderData.AssetState.MISSING;
            }
            else
            {
                if (this.assetDict.TryGetValue(guid, out assetDescription))
                    return;
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                this.assetDict.Add(guid, new ReferenceFinderData.AssetDescription()
                {
                    name = Path.GetFileNameWithoutExtension(assetPath),
                    path = assetPath,
                    state = ReferenceFinderData.AssetState.NODATA
                });
            }
        }

        public static string GetInfoByState(ReferenceFinderData.AssetState state)
        {
            switch (state)
            {
                case ReferenceFinderData.AssetState.CHANGED:
                    return StringUtils.Hy;
                case ReferenceFinderData.AssetState.MISSING:
                    return StringUtils.HZ;
                case ReferenceFinderData.AssetState.NODATA:
                    return StringUtils.Hz;
                default:
                    return StringUtils.hA;
            }
        }

        public class AssetDescription
        {
            public string name = string.Empty;
            public string path = string.Empty;
            public Hash128 assetDependencyHash;
            public List<string> dependencies = new List<string>();
            public List<string> references = new List<string>();
            public ReferenceFinderData.AssetState state = ReferenceFinderData.AssetState.NORMAL;
        }


        public class B
        {
            public List<string> cA;
            public Func<int, string> ca;

            public string A(int val)
            {
                return "";
            }
        }
        
        public class C
        {
            public Dictionary<string, int> cB;
            public Func<string, int> cb;
            public int A(string val)
            {
                return 0;
            }
        }
        public enum AssetState
        {
            NORMAL,
            CHANGED,
            MISSING,
            NODATA,
        }
    }
}