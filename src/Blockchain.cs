using System;
using System.Collections.Generic;
using System.Text;

namespace mzChain
{
    class Blockchain
    {
        List<Block> chain;
        const int difficulty = 5;
        public Blockchain()
        {
            this.chain = new List<Block>();
            Block genesis = new Block(Encoding.ASCII.GetBytes("Genesis"),Encoding.ASCII.GetBytes(""));
            genesis.Mine(difficulty);
            this.chain.Add(genesis);
        }

        public void AddBlock(byte[] Data)
        {
            Block PreviousBlock = chain[chain.Count - 1];
            Block NewBlock = new Block(Data, PreviousBlock.Hash);
            NewBlock.Mine(difficulty);
            chain.Add(NewBlock);
        }

        public void PrintChain()
        {
            foreach(Block b in chain)
            {
                Console.Write(b.ToString());
            }
        }

    }
}
