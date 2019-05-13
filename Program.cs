using System;

namespace BiKnight2
{
    class Program
    {
        static void Main(string[] args)
        {
            Chess chess = new Chess();
            int qty = 0;
            //foreach (Chess.Combo combo in chess.AllCheckmates())
            //   Console.WriteLine((++qty) + " " + combo.getFEN());
            chess.FindAllSolutions();
            while(true)
            {
                Console.Write("FEN: ");
                string fen = Console.ReadLine();
                Console.WriteLine(chess.FindMove(fen));
            }
            Console.ReadLine();
        }
    }
}
