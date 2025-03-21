using System;
using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Crypto.Digests;

namespace mzChain
{
    public static class CryptoUtils
    {
        public static byte[] ComputeSha256Hash(byte[] data)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(data);
            }
        }
        
        public static byte[] ComputeRipeMd160Hash(byte[] data)
        {
            // Using BouncyCastle for RIPEMD160 implementation
            RipeMD160Digest digest = new RipeMD160Digest();
            byte[] output = new byte[20]; // RIPEMD160 produces a 20-byte hash
            
            digest.BlockUpdate(data, 0, data.Length);
            digest.DoFinal(output, 0);
            
            return output;
        }
        
        public static string BytesToHex(byte[] bytes, bool withDashes = false)
        {
            string result = BitConverter.ToString(bytes);
            return withDashes ? result : result.Replace("-", "").ToLower();
        }
        
        public static bool CompareBytes(byte[] a, byte[] b)
        {
            if (a == null || b == null)
                return false;
                
            if (a.Length != b.Length)
                return false;
                
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i])
                    return false;
            }
            
            return true;
        }
    }
}