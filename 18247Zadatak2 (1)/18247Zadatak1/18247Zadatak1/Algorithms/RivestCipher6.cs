namespace _18247Zadatak1.Algorithms
{
    public class RivestCipher6
    {
        private const int wordSize = 32;
        private const int rounds = 20;
        private const int keySize = 256;

        private uint[] S; //round keys
        public RivestCipher6(byte[] key)
        {
            InitializeKey(key);
        }

        private void InitializeKey(byte[] key)
        {
            int c = keySize / 8; // key length in bytes
            int u = wordSize / 8; //word size in bytes
            int t = 2 * rounds + 4;
            int i;

            uint[] L = new uint[c / u]; //key buffer
            for (i = 0; i < key.Length; i++)
            {
                L[i / u] = (L[i / u] << 8) + key[i];
               // Console.WriteLine("L[" + (i / u) + "] = " + L[i / u]);
            }

            S = new uint[t];
            S[0] = 0xB7E15163;
            for (i = 1; i < t; i++)
                S[i] = S[i - 1] + 0x9E3779B9;

            int A = 0, B = 0, j = 0;
            i = 0;
            int v = 3 * Math.Max(c, t);

            for(int s = 0;s<v; s++)
            {
                A = (int)(S[i] = RotateLeft((uint)(S[i] + A + B), 3));
                B = (int)(L[j] = RotateLeft((uint)(L[j] + A + B), (A + B)));
                i = (i + j) % t;
                j = (j + 1) % (c/u);
            }
        }

        public byte[] Encrypt(byte[] plaintext)
        {
            uint A =0 , B=0, C = 0, D = 0;
            uint[] list = { A, B, C, D };
            
            for (int i=0;i< list.Length;i++)
                list[i] = BitConverter.ToUInt32(plaintext, i*4);
            

            B += S[0];
            D += S[1];

            for(int i = 1; i <= rounds; i++)
            {
                uint t = RotateLeft((B * (2 * B + 1)), 5);
                uint u = RotateLeft((D * (2 * D + 1)), 5);

                A = RotateLeft((A ^ t), (int)u) + S[2 * i];
                C = RotateLeft((C ^ u), (int)t) + S[2 * i + 1];

                (A, B, C, D) = (B, C, D, A);


            }

            A += S[2 * rounds + 2];
            C += S[2 * rounds + 3];

            byte[] ciphertext = new byte[16];
            for (int i = 0; i < list.Length; i++)
                Array.Copy(BitConverter.GetBytes(list[i]), 0, ciphertext, i * 4, 4);

            return ciphertext;

        }


        public byte[] Decrypt(byte[] ciphertext)
        {
            uint A = 0, B = 0, C = 0, D = 0;
            uint[] list = { A, B, C, D };

            for (int i = 0; i < list.Length; i++)
                list[i] = BitConverter.ToUInt32(ciphertext, i * 4);



            C -= S[2 * rounds + 3];
            A -= S[2 * rounds + 2];

            for (int i = rounds; i >= 1; i--)
            {
                (A, B, C, D) = (D, A, B, C);

                uint u = RotateLeft((D * (2 * D + 1)), 5);
                uint t = RotateLeft((B * (2 * B + 1)), 5);

                C = RotateRight((C - S[2 * i + 1]), (int)t) ^ u;
                A = RotateRight((A - S[2 * i]), (int)u) ^ t;



            }

            D -= S[1];
            B -= S[0];

            byte[] plaintext = new byte[16];
            for (int i = 0; i < list.Length; i++)
                Array.Copy(BitConverter.GetBytes(list[i]), 0, plaintext, i * 4, 4);

            return plaintext;

        }

        private uint RotateRight(uint value, int count)
        {
            return (value >> count) | (value << (wordSize - count));
        }

        private uint RotateLeft(uint value, int count)
        {
            return (value << count) | (value >> (wordSize - count));
        }

      
    }
}
