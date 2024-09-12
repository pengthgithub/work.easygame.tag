using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    /*
     * https://zhuanlan.zhihu.com/p/43566129
     * LCG不能用于随机数要求高的场合，例如不能用于Monte Carlo模拟，不能用于加密应用.
     * 如果需要高质量的伪随机数，内存充足(约2kb)，Mersenne twister算法是个不错的选择。
     * Mersenne twister产生随机数的质量几乎超过任何LCG。不过一般Mersenne twister的实现使用LCG产生种子。
     */
    public class LCG
    {
        private static ulong ms_modulus = 2147483647;
        private static ulong a = 48271;
        private static ulong c = 1;

        private uint mSeed;

        public LCG(uint seed)
        {
            mSeed = seed;
        }

        public int Next()
        {
            mSeed = (uint)((a * mSeed + c) % ms_modulus);
            return (int)mSeed;
        }

        public int Next(int min, int max)
        {
            int x = Next();
            x = x % (max - min);
            return x + min;
        }

        public double NextDouble()
        {
            int x = Next();
            return x / 2147483647.0;
        }
    }

