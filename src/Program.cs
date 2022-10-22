using System;
using System.Text;

namespace mzChain
{
    class Program
    {
        static void Main(string[] args)
        {
            Blockchain blockchain = new Blockchain();
            blockchain.AddBlock(Encoding.ASCII.GetBytes("Ciao Mondo"));
            blockchain.PrintChain();
        }
    }
}
