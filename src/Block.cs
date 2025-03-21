using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Linq;

namespace mzChain
{
    public class Block
    {
        public byte[] Hash { get; set; }
        public byte[] Data { get; }
        public byte[] PreviousHash { get; }
        public ulong Nonce { get; set; }
        public DateTime Timestamp { get; }
        public List<Transaction> Transactions { get; }

        public Block(byte[] Data, byte[] PreviousHash)
        {
            this.Data = Data;
            this.PreviousHash = PreviousHash;
            this.Hash = null;
            this.Nonce = 0;
            this.Timestamp = DateTime.UtcNow;
            this.Transactions = new List<Transaction>();
        }

        public Block(List<Transaction> transactions, byte[] PreviousHash)
        {
            this.Transactions = transactions;
            this.PreviousHash = PreviousHash;
            this.Data = SerializeTransactions();
            this.Hash = null;
            this.Nonce = 0;
            this.Timestamp = DateTime.UtcNow;
        }

        private byte[] SerializeTransactions()
        {
            if (Transactions == null || Transactions.Count == 0)
                return new byte[0];

            // Create a simple serialized version of all transactions
            var transactionData = String.Join("|", Transactions.Select(t => t.TransactionId));
            return Encoding.UTF8.GetBytes(transactionData);
        }
        
        public List<Transaction> GetTransactions()
        {
            return Transactions;
        }

        public void AddTransaction(Transaction transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            if (!transaction.VerifySignature())
                throw new InvalidOperationException($"Cannot add transaction {transaction.TransactionId} with invalid signature");

            Transactions.Add(transaction);
        }

        public void Mine(int difficulty)
        {
            string targetZeros = new string('0', difficulty);
            while (this.Hash == null || BitConverter.ToString(Hash).Replace("-","").Substring(0, difficulty) != targetZeros)
            {
                this.Nonce++;
                this.Hash = CalculateHash();
            }
        }

        public byte[] CalculateHash()
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] nonceBytes = BitConverter.GetBytes(Nonce);
                byte[] timestampBytes = BitConverter.GetBytes(Timestamp.ToBinary());
                byte[] transactionBytes = SerializeTransactions();
                
                // Calculate data size and create buffer
                int totalLength = PreviousHash.Length + (Data?.Length ?? 0) + nonceBytes.Length + timestampBytes.Length + transactionBytes.Length;
                byte[] concatDatas = new byte[totalLength];
                
                int offset = 0;
                
                // Copy all data into buffer
                Buffer.BlockCopy(PreviousHash, 0, concatDatas, offset, PreviousHash.Length);
                offset += PreviousHash.Length;
                
                if (Data != null && Data.Length > 0)
                {
                    Buffer.BlockCopy(Data, 0, concatDatas, offset, Data.Length);
                    offset += Data.Length;
                }
                
                Buffer.BlockCopy(nonceBytes, 0, concatDatas, offset, nonceBytes.Length);
                offset += nonceBytes.Length;
                
                Buffer.BlockCopy(timestampBytes, 0, concatDatas, offset, timestampBytes.Length);
                offset += timestampBytes.Length;
                
                if (transactionBytes.Length > 0)
                {
                    Buffer.BlockCopy(transactionBytes, 0, concatDatas, offset, transactionBytes.Length);
                }
                
                return sha256.ComputeHash(concatDatas);
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("------------BLOCK--------------");
            sb.AppendLine("Hash: " + BitConverter.ToString(Hash).Replace("-","").ToLower());
            
            if (Data.Length > 0)
                sb.AppendLine("Data: " + Encoding.UTF8.GetString(Data));
            
            sb.AppendLine("Previous Hash: " + BitConverter.ToString(PreviousHash).Replace("-","").ToLower());
            sb.AppendLine("Timestamp: " + Timestamp.ToString("yyyy-MM-dd HH:mm:ss"));
            sb.AppendLine("Nonce: " + Nonce);
            
            if (Transactions.Count > 0)
            {
                sb.AppendLine($"Transactions: {Transactions.Count}");
                foreach (var tx in Transactions)
                {
                    sb.AppendLine($"  - {tx.TransactionId.Substring(0, 8)}... | {tx.SenderAddress.Substring(0, 6)}... -> {tx.RecipientAddress.Substring(0, 6)}... | {tx.Amount}");
                }
            }
            
            return sb.ToString();
        }
    }
}
