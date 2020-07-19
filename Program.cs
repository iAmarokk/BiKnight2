using System;

namespace BiKnight2
{
    class Program
    {
        static void Main(string[] args)
        {
            Chess chess = new Chess();
            //int qty = 0;
            ////foreach (Chess.Combo combo in chess.AllCheckmates())
            ////   Console.WriteLine((++qty) + " " + combo.getFEN());
            //chess.FindAllSolutions();
            //while (true)
            //{
            //    Console.Write("FEN: ");
            //    string fen = Console.ReadLine();
            //    Console.WriteLine(chess.FindMove(fen));
            //}
            //DateTime date = new DateTime();
            //date = DateTime.Now;
            //chess.Save();
            chess.Load();
            chess.PlayFEN("8/8/5k2/8/2NK4/3B4/8/8 w - -");
            //Console.WriteLine(date.ToString()+" "+ DateTime.Now);
            //Console.WriteLine("Done");
            Console.ReadLine();
        }
    }
}
