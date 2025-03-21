using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace mzChain
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting mzChain blockchain demonstration...");
            Console.WriteLine("Creating blockchain...");
            Blockchain blockchain = new Blockchain();
            
            // Create wallets
            Console.WriteLine("\nCreating wallets...");
            Wallet aliceWallet = new Wallet();
            Wallet bobWallet = new Wallet();
            Wallet minerWallet = new Wallet();
            
            Console.WriteLine($"Alice's wallet address: {aliceWallet.Address}");
            Console.WriteLine($"Bob's wallet address: {bobWallet.Address}");
            Console.WriteLine($"Miner's wallet address: {minerWallet.Address}");
            
            // Mine some blocks to reward the miner
            Console.WriteLine("\nMining blocks to generate initial coins...");
            Console.WriteLine("Processing block 1...");
            blockchain.ProcessPendingTransactions(minerWallet.Address);
            
            Console.WriteLine("Processing block 2...");
            blockchain.ProcessPendingTransactions(minerWallet.Address);
            
            // Check balances
            Console.WriteLine($"\nMiner's balance: {blockchain.GetBalanceForAddress(minerWallet.Address)} coins");
            Console.WriteLine($"Alice's balance: {blockchain.GetBalanceForAddress(aliceWallet.Address)} coins");
            Console.WriteLine($"Bob's balance: {blockchain.GetBalanceForAddress(bobWallet.Address)} coins");
            
            // Create some transactions
            Console.WriteLine("\nCreating and signing transactions...");
            try
            {
                // Transfer from miner to Alice
                Transaction tx1 = new Transaction(minerWallet.Address, aliceWallet.Address, 50);
                tx1.SignTransaction(minerWallet.PrivateKey);
                blockchain.AddTransaction(tx1);
                Console.WriteLine("Transaction 1: Miner sends 50 coins to Alice");
                
                // Transfer from miner to Bob
                Transaction tx2 = new Transaction(minerWallet.Address, bobWallet.Address, 30);
                tx2.SignTransaction(minerWallet.PrivateKey);
                blockchain.AddTransaction(tx2);
                Console.WriteLine("Transaction 2: Miner sends 30 coins to Bob");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Transaction error: {ex.Message}");
            }
            
            // Mine block to include transactions
            Console.WriteLine("\nMining a new block to process transactions...");
            blockchain.ProcessPendingTransactions(minerWallet.Address);
            
            // Check balances again
            Console.WriteLine($"\nMiner's balance: {blockchain.GetBalanceForAddress(minerWallet.Address)} coins");
            Console.WriteLine($"Alice's balance: {blockchain.GetBalanceForAddress(aliceWallet.Address)} coins");
            Console.WriteLine($"Bob's balance: {blockchain.GetBalanceForAddress(bobWallet.Address)} coins");
            
            // Create transaction from Alice to Bob
            Console.WriteLine("\nCreating transaction from Alice to Bob...");
            try
            {
                Transaction tx3 = new Transaction(aliceWallet.Address, bobWallet.Address, 20);
                tx3.SignTransaction(aliceWallet.PrivateKey);
                blockchain.AddTransaction(tx3);
                Console.WriteLine("Transaction 3: Alice sends 20 coins to Bob");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Transaction error: {ex.Message}");
            }
            
            // Mine another block to include the transaction
            Console.WriteLine("\nMining a new block to process transactions...");
            blockchain.ProcessPendingTransactions(minerWallet.Address);
            
            // Final balances
            Console.WriteLine($"\nFinal balances:");
            Console.WriteLine($"Miner's balance: {blockchain.GetBalanceForAddress(minerWallet.Address)} coins");
            Console.WriteLine($"Alice's balance: {blockchain.GetBalanceForAddress(aliceWallet.Address)} coins");
            Console.WriteLine($"Bob's balance: {blockchain.GetBalanceForAddress(bobWallet.Address)} coins");
            
            // Validate the blockchain
            Console.WriteLine($"\nBlockchain valid: {blockchain.IsChainValid()}");
            
            // Print blockchain
            Console.WriteLine("\nFull blockchain:");
            blockchain.PrintChain();
        }
    }
}
