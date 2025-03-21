using System;
using System.Security.Cryptography;
using System.Text;

namespace mzChain
{
    public class Wallet
    {
        public byte[] PrivateKey { get; }
        public byte[] PublicKey { get; }
        public string Address { get; }
        
        public Wallet()
        {
            GenerateKeyPair();
            GenerateAddress();
        }
        
        private void GenerateKeyPair()
        {
            using (ECDsa ecdsa = ECDsa.Create())
            {
                // Save the private and public keys
                PrivateKey = ecdsa.ExportPkcs8PrivateKey();
                PublicKey = ecdsa.ExportSubjectPublicKeyInfo();
            }
        }
        
        private void GenerateAddress()
        {
            // Create address from public key hash
            using (SHA256 sha256 = SHA256.Create())
            {
                // In .NET Core, RIPEMD160 may not be available in some environments
                // So we use our utility method that handles the required provider
                byte[] publicKeyHash = sha256.ComputeHash(PublicKey);
                byte[] ripedHash = CryptoUtils.ComputeRipeMd160Hash(publicKeyHash);
                
                // Convert to hex string with prefix
                Address = "mz" + BitConverter.ToString(ripedHash).Replace("-", "").ToLower();
            }
        }
        
        public Transaction CreateTransaction(string recipientAddress, decimal amount, Blockchain blockchain)
        {
            if (GetBalance(blockchain) < amount)
            {
                throw new Exception("Not enough funds in wallet to create transaction");
            }
            
            Transaction transaction = new Transaction(Address, recipientAddress, amount);
            transaction.SignTransaction(PrivateKey);
            return transaction;
        }
        
        public decimal GetBalance(Blockchain blockchain)
        {
            // In a real implementation, this would scan the blockchain for unspent transaction outputs
            // This is a simplified version
            decimal balance = 0;
            
            foreach (Block block in blockchain.GetBlocks())
            {
                // Parse transactions from block data
                List<Transaction> transactions = block.GetTransactions();
                
                foreach (Transaction transaction in transactions)
                {
                    if (transaction.SenderAddress == Address)
                    {
                        balance -= transaction.Amount;
                    }
                    
                    if (transaction.RecipientAddress == Address)
                    {
                        balance += transaction.Amount;
                    }
                }
            }
            
            return balance;
        }
    }
}
