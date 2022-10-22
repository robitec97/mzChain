using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace mzChain
{
    class Block
    {
        public byte[] Hash { get; set; }
        public byte[] Data { get; }
        public byte[] PreviousHash {get;}
        public ulong Nonce { get; set; }

        public Block(byte[] Data, byte[] PreviousHash)
        {
            this.Data = Data;
            this.PreviousHash = PreviousHash;
            this.Hash = null;
            this.Nonce = 0;
        }

        public void Mine(int difficulty)
        {
            string targetZeros = new string('0', difficulty);
            while (this.Hash == null || Convert.ToBase64String(Hash).Substring(0, difficulty) != targetZeros)
            {
                this.Nonce++;
                this.Hash = CalculateHash();
            }
        }

        private byte[] CalculateHash()
        {
            SHA256 sha256 = SHA256.Create();
            byte[] nonceBytes = BitConverter.GetBytes(Nonce);
            byte[] concatDatas = new byte[this.Data.Length + this.PreviousHash.Length + nonceBytes.Length];
            Buffer.BlockCopy(this.PreviousHash, 0, concatDatas, 0, this.PreviousHash.Length);
            Buffer.BlockCopy(this.Data, 0, concatDatas, 0, this.Data.Length);
            Buffer.BlockCopy(nonceBytes, 0, concatDatas, 0, nonceBytes.Length);
            return sha256.ComputeHash(concatDatas);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Hash: " + Convert.ToBase64String(Hash));
            sb.AppendLine("Data: " + Encoding.ASCII.GetString(Data));
            sb.AppendLine("Previous Hash: " + Convert.ToBase64String(PreviousHash));
            sb.AppendLine("Nonce: " + Nonce);
            return sb.ToString();
        }
    }
}
