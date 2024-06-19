using System;
using System.Linq;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace _18247Zadatak1.Algorithms
{
    public class Blake
    {
        private const int blockSize = 64;

        private ulong[] hash;
        private ulong[] messageBlock;
        private ulong[] vv;

        //constants
        private readonly ulong[][] Sigma = new ulong[][]
        {
             new ulong[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 },
        };


        private readonly ulong[] U = new ulong[]
         {
            0x01234567, 0x89abcdef, 0x01234567, 0x89abcdef,
            0x01234567, 0x89abcdef, 0x01234567, 0x89abcdef,
            0x01234567, 0x89abcdef, 0x01234567, 0x89abcdef,
            0x01234567, 0x89abcdef, 0x01234567, 0x89abcdef
         };



        public Blake(IConfiguration configuration)
        {
            hash = new ulong[8];
            messageBlock = new ulong[16];
            vv = new ulong[16];

            string ivString = configuration["IV"];

            // Ako je IV vrednost u heksadekadnom formatu
            if (!string.IsNullOrEmpty(ivString) && ivString.Length == 32)
            {
                for (int i = 0; i < 8; i++)
                {
                    hash[i] = ulong.Parse(ivString.Substring(i * 4, 4), System.Globalization.NumberStyles.HexNumber);
                    vv[i] = hash[i];
                }
            }
        }


        public byte[] ComputeHash(byte[] input, ulong[] salt, ulong[] counter)
        {
            byte[] paddedInput = AddPadding(input);
            int paddedLength = paddedInput.Length;

            vv[8] = salt[0] ^ U[0];
            vv[9] = salt[1] ^ U[1];
            vv[10] = salt[2] ^ U[2];
            vv[11] = salt[3] ^ U[3];
            vv[12] = counter[0] ^ U[4];
            vv[13] = counter[0] ^ U[5];
            vv[14] = counter[1] ^ U[6];
            vv[15] = counter[1] ^ U[7];

            int blockCount = paddedLength / blockSize;

            for (int blockIndex = 0; blockIndex < blockCount; blockIndex++)
            {
                Buffer.BlockCopy(paddedInput, blockIndex * blockSize, messageBlock, 0, blockSize);

                for (int i = 0; i < 14; ++i)
                {
                    G(vv, messageBlock, Sigma[0], U);
                }

                for (int i = 0; i < 8; ++i)
                {
                    hash[i] ^= vv[i] ^ vv[i + 8] ^ salt[i % 4];
                }
            }

            byte[] hashBytes = new byte[64];
            for (int i = 0; i < 8; i++)
            {
                Array.Copy(BitConverter.GetBytes(hash[i]), 0, hashBytes, i * 8, 8);
            }

            return hashBytes;
        }

        private static byte[] AddPadding(byte[] input)
        {
            int paddingLength = blockSize - (input.Length % blockSize);
            byte paddingByte = (byte)paddingLength;

            byte[] paddedInput = new byte[input.Length + paddingLength];
            Buffer.BlockCopy(input, 0, paddedInput, 0, input.Length);

            for (int i = input.Length; i < paddedInput.Length; i++)
            {
                paddedInput[i] = paddingByte;
            }

            return paddedInput;
        }

        private void G(ulong[] v, ulong[] m, ulong[] s, ulong[] u)
        {

            for (int i = 0; i < 13; i += 2)
            {
                v[s[i]] += (m[s[i + 1]] ^ u[(s[i + 1] + 1) % 16]) + v[s[i + 1]];
                v[s[i + 3]] = RotateRight(v[s[i + 3]] ^ v[s[i]], 16);
                v[s[i + 2]] += v[s[i + 3]];
                v[s[i + 1]] = RotateRight(v[s[i + 1]] ^ v[s[i + 2]], 12);
                v[s[i]] += (m[(s[i + 1] + 1) % 16] ^ u[s[i + 1] % 16]) + v[s[i + 1]];
                v[s[i + 3]] = RotateRight(v[s[i + 3]] ^ v[s[i]], 8);
                v[s[i + 2]] += v[s[i + 3]];
                v[s[i + 1]] = RotateRight(v[s[i + 1]] ^ v[s[i + 2]], 7);
            }
        }
        private ulong RotateRight(ulong value, int bits)
        {
            return (value >> bits) | (value << (64 - bits));
        }
    }
}
