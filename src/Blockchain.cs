using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mzChain
{
    public class Blockchain
    {
        private List<Block> chain;
        private List<Transaction> pendingTransactions;
        private const int difficulty = 5;
        private const decimal miningReward = 100.0m;
        
        public Blockchain()
        {
            this.chain = new List<Block>();
            this.pendingTransactions = new List<Transaction>();
            
            // Create genesis block
            Block genesis = new Block(Encoding.ASCII.GetBytes("Genesis"), Encoding.ASCII.GetBytes(""));
            genesis.Mine(difficulty);
            this.chain.Add(genesis);
        }

        public void AddBlock(byte[] Data)
        {
            Block previousBlock = chain[chain.Count - 1];
            Block newBlock = new Block(Data, previousBlock.Hash);
            newBlock.Mine(difficulty);
            chain.Add(newBlock);
        }
        
        public void AddBlock(List<Transaction> transactions)
        {
            Block previousBlock = chain[chain.Count - 1];
            Block newBlock = new Block(transactions, previousBlock.Hash);
            newBlock.Mine(difficulty);
            chain.Add(newBlock);
        }
        
        public bool AddTransaction(Transaction transaction)
        {
            if (transaction == null)
                return false;
                
            // Verify transaction signature
            if (!transaction.VerifySignature())
                return false;
                
            // Add transaction to pending transactions
            pendingTransactions.Add(transaction);
            return true;
        }
        
        public void ProcessPendingTransactions(string minerAddress)
        {
            // Create a mining reward transaction
            Transaction rewardTransaction = new Transaction("SYSTEM", minerAddress, miningReward);
            
            // Create a list with all pending transactions plus the reward
            List<Transaction> blockTransactions = new List<Transaction>(pendingTransactions);
            blockTransactions.Add(rewardTransaction);
            
            // Create and mine the new block
            Block previousBlock = chain[chain.Count - 1];
            Block newBlock = new Block(blockTransactions, previousBlock.Hash);
            newBlock.Mine(difficulty);
            chain.Add(newBlock);
            
            // Clear pending transactions
            pendingTransactions.Clear();
        }
        
        public decimal GetBalanceForAddress(string address)
        {
            decimal balance = 0;
            
            foreach (Block block in chain)
            {
                List<Transaction> transactions = block.GetTransactions();
                foreach (Transaction transaction in transactions)
                {
                    if (transaction.SenderAddress == address)
                    {
                        balance -= transaction.Amount;
                    }
                    
                    if (transaction.RecipientAddress == address)
                    {
                        balance += transaction.Amount;
                    }
                }
            }
            
            return balance;
        }
        
        public bool IsChainValid()
        {
            // Check the validity of the entire blockchain
            for (int i = 1; i < chain.Count; i++)
            {
                Block currentBlock = chain[i];
                Block previousBlock = chain[i - 1];
                
                // Verify hash integrity
                if (!CryptoUtils.CompareBytes(currentBlock.Hash, currentBlock.CalculateHash()))
                    return false;
                    
                // Verify chain continuity
                if (!CryptoUtils.CompareBytes(currentBlock.PreviousHash, previousBlock.Hash))
                    return false;
            }
            
            return true;
        }
        
        public List<Block> GetBlocks()
        {
            return chain;
        }
        
        public Block GetLatestBlock()
        {
            return chain[chain.Count - 1];
        }
        
        public List<Transaction> GetPendingTransactions()
        {
            return pendingTransactions;
        }

        public void PrintChain()
        {
            foreach (Block b in chain)
            {
                Console.Write(b.ToString());
            }
        }
    }
}
