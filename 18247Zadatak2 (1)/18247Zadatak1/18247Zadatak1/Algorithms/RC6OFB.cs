
namespace _18247Zadatak1.Algorithms
{
    public class RC6OFB
    {
        private RivestCipher6 rc6;
        private byte[] iv; // initialization vector

        public RC6OFB(byte[] key, byte[] iv)
        {
            rc6 = new RivestCipher6(key);
            this.iv = iv;
        }

        //dakle isti su
        public byte[] Encrypt(byte[] plaintext)
        {
            byte[] encrypted = new byte[plaintext.Length];
            byte[] feedback = iv;

            for(int i=0;i<plaintext.Length; i++)
            {
                byte[] encryptedBlock = rc6.Encrypt(feedback);
                encrypted[i] = (byte)(plaintext[i] ^ encryptedBlock[i % 16]);
                feedback = encryptedBlock;
            }

            return encrypted;
        }

        public byte[] Decrypt(byte[] ciphertext)
        {
            byte[] decrypted = new byte[ciphertext.Length];
            byte[] feedback = iv;

            for(int i = 0; i < ciphertext.Length; i++)
            {
                byte[] decryptedBlock = rc6.Encrypt(feedback);
                decrypted[i] = (byte)(ciphertext[i] ^ decryptedBlock[i % 16]);
                feedback = decryptedBlock; 
            }

            return decrypted;
        }
    }
}
