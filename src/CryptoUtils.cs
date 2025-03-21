using System;
using System.Security.Cryptography;
using System.Text;

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
            using (RIPEMD160 ripemd160 = RIPEMD160.Create())
            {
                return ripemd160.ComputeHash(data);
            }
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