using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    //collection of hash function
    public static class HashHelper
    {
        public static int HASH_TIME_33(string str)
        {
            int hash = 0;
            char[] arr = str.ToCharArray();

            for (int i = 0; i<arr.Length; i++)
            {
                int v = arr[i];
                hash = hash* 33 + v;
            }
            return hash;
        }

        public static int HASH_CRC32(string str)
        {
            int hash = 0;
            char[] arr= str.ToCharArray();

            for (int i = 0; i < arr.Length; i++)
            {
                byte data = (byte)arr[i];
                hash ^= (data << 24);
                
                for(int j=0;j<8;j++)
                {
                    if ((hash & 0x80000000) > 0)
                    {
                        hash <<= 1;
                        hash ^= 0x04C11DB7;
                    }
                    else
                        hash <<= 1;
                }
            }

            return hash;
        }
    }
