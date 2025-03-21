using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace mzChain
{
    public class Transaction
    {
        public string SenderAddress { get; }
        public string RecipientAddress { get; }
        public decimal Amount { get; }
        public byte[] Signature { get; private set; }
        public string TransactionId { get; private set; }
        
        public Transaction(string senderAddress, string recipientAddress, decimal amount)
        {
            SenderAddress = senderAddress;
            RecipientAddress = recipientAddress;
            Amount = amount;
            CalculateTransactionId();
        }
        
        private void CalculateTransactionId()
        {
            string dataToHash = SenderAddress + RecipientAddress + Amount.ToString();
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(dataToHash));
                TransactionId = BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }
        
        public void SignTransaction(byte[] privateKey)
        {
            if (String.IsNullOrEmpty(SenderAddress))
                throw new Exception("Cannot sign a transaction without a sender address");
            
            string dataToSign = SenderAddress + RecipientAddress + Amount.ToString();
            using (ECDsa ecdsa = ECDsa.Create())
            {
                ecdsa.ImportPkcs8PrivateKey(privateKey, out _);
                Signature = ecdsa.SignData(Encoding.UTF8.GetBytes(dataToSign), HashAlgorithmName.SHA256);
            }
        }
        
        public bool VerifySignature()
        {
            if (SenderAddress == "SYSTEM" || Signature == null || Signature.Length == 0)
                return true; // Allow system rewards or coinbase transactions
                
            try
            {
                string dataToVerify = SenderAddress + RecipientAddress + Amount.ToString();
                using (ECDsa ecdsa = ECDsa.Create())
                {
                    // Convert address (public key hash) back to public key format
                    // Note: In a real implementation, you'd need to derive the actual public key
                    // This is a simplified version assuming address can be used to verify
                    byte[] publicKeyBytes = Encoding.UTF8.GetBytes(SenderAddress);
                    
                    // Import the public key and verify
                    ecdsa.ImportSubjectPublicKeyInfo(publicKeyBytes, out _);
                    return ecdsa.VerifyData(
                        Encoding.UTF8.GetBytes(dataToVerify),
                        Signature,
                        HashAlgorithmName.SHA256);
                }
            }
            catch
            {
                return false;
            }
        }
        
        public override string ToString()
        {
            return $"Transaction: {TransactionId}\nFrom: {SenderAddress}\nTo: {RecipientAddress}\nAmount: {Amount}";
        }
    }
}
