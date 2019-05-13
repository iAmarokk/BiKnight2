using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace BiKnight2
{
    class Chess
    {
        WhiteBox box;

        public struct Coord
        {
            public byte x; // 0..7
            public byte y; // 0..7

            public int index
            {
                get { return x + y * 8; }
            }

            public string chess
            {
                get { return Convert.ToChar('a' + x).ToString() + (8 - y).ToString(); }
            }

            public static bool operator == (Coord a, Coord b)
            {
                return a.x == b.x && a.y == b.y;
            }

            public static bool operator != (Coord a, Coord b)
            {
                return a.x != b.x && a.y != b.y;
            }

            public bool valid()
            {
                return x >= 0 && x < 8 &&
                       y >= 0 && y < 8;
            }
        }

        public struct Combo
        {
            public Coord whiteKing;
            public Coord whiteBishop;
            public Coord whiteKnight;
            public Coord blackKing;

            public char[,] getBoard ()
            {
                char[,] board = new char[8, 8];
                for (int y = 0; y < 8; y++)
                    for (int x = 0; x < 8; x++)
                        board[x, y] = ' ';
                board[whiteKing.x, whiteKing.y] = 'K';
                board[whiteBishop.x, whiteBishop.y] = 'B';
                board[whiteKnight.x, whiteKnight.y] = 'N';
                board[blackKing.x, blackKing.y] = 'k';
                return board;
            }

            public string getFEN()
            {
                char[,] board = getBoard();
                StringBuilder sb = new StringBuilder();
                for (int y = 0; y < 8; y++)
                {
                    int free = 0;
                    for (int x = 0; x < 8; x++)
                    {
                        if (board[x, y] == ' ')
                            free++;
                        else
                        {
                            if (free > 0)
                                sb.Append(free.ToString());
                            sb.Append(board[x, y]);
                            free = 0;
                        }
                    }
                    if (free > 0)
                        sb.Append(free.ToString());
                    if (y < 7)
                        sb.Append("/");
                }
                return sb.ToString();
            }

            public void print()
            {
                char[,] board = getBoard();
                ConsoleColor fore = Console.ForegroundColor;
                ConsoleColor back = Console.BackgroundColor;
                for (int y = 0; y < 8; y++)
                { 
                    for (int x = 0; x < 8; x++)
                    {
                        Console.BackgroundColor = (x + y) % 2 == 0 ?
                            ConsoleColor.Gray : ConsoleColor.DarkGray;
                        Console.ForegroundColor = board[x, y] == 'k' ?
                            ConsoleColor.Black : ConsoleColor.Yellow;
                        Console.Write(board[x, y]);
                    }
                    Console.WriteLine();
                }
                Console.ForegroundColor = fore;
                Console.BackgroundColor = back;
            }
        }

        public struct WhitesMove
        {
            public Combo combo;
            public byte moves; // полуходы
            public Coord moveFrom;
            public Coord moveTo;
        }

        public struct BlacksMove
        {
            public Combo combo;
            public byte moves; // полуходы
        }

        public Chess()
        {

        }

        public IEnumerable<Combo> AllCheckmates()
        {
            Combo combo;
            foreach (Coord whiteKing in AllCoords())
            {
                combo.whiteKing = whiteKing;
                foreach (Coord whiteBishop in AllCoords())
                {
                    if (whiteKing == whiteBishop)
                        continue;

                    if (isWhiteSquare(whiteBishop))
                        continue;

                    combo.whiteBishop = whiteBishop;
                    foreach (Coord whiteKnight in AllCoords())
                    {
                        if (whiteKnight == whiteKing || whiteKnight == whiteBishop)
                            continue;

                        combo.whiteKnight = whiteKnight;
                        foreach (Coord blackKing in AllCoords())
                        {
                            if (blackKing == whiteKing || blackKing == whiteBishop || blackKing == whiteKnight)
                                continue;

                            if (onKing(whiteKing, blackKing)) 
                                continue;

                            combo.blackKing = blackKing;

                            if (!isCheckmate(combo))
                                continue;

                            yield return combo;

                        }
                    }
                }
            }
        }

        public void FindAllSolutions()
        {
            box = new WhiteBox();
            Queue<BlacksMove> blackQueue = new Queue<BlacksMove>();
            Queue<WhitesMove> whitesQueue = new Queue<WhitesMove>();

            foreach (Combo combo in AllCheckmates())
            {
                BlacksMove checkmate;
                checkmate.combo = combo;
                checkmate.moves = 0;
                blackQueue.Enqueue(checkmate);
            }
            int moves = 0;
            while (blackQueue.Count > 0)
            {
                moves++;
                Console.WriteLine(moves + "\t" + blackQueue.Count + "\t" + whitesQueue.Count + "\t" + box.Count);
                while (blackQueue.Count > 0)
                {
                    BlacksMove black = blackQueue.Dequeue();

                    foreach (WhitesMove white in AllWhiteBackMoves(black))
                    {
                        if (isCheck(white.combo))
                            continue;
                        if (!box.Put(white))
                            continue;

                        whitesQueue.Enqueue(white);

                        //Console.WriteLine((++qty) + " " + white.combo.getFEN() + " " + white.moveFrom.chess + "-" + white.moveTo.chess);
                        //white.combo.print();
                    }
                }
                moves++;
                Console.WriteLine(moves + "\t" + blackQueue.Count + "\t" + whitesQueue.Count + "\t" + box.Count);

                while (whitesQueue.Count > 0)
                {
                    WhitesMove white = whitesQueue.Dequeue();
                    Combo whiteFigures = white.combo;
                    foreach (BlacksMove black in AllBlackBackMoves(white))
                    {
                        bool solved = true;
                        foreach (Coord blackKing in AllKingMoves(black.combo.blackKing))
                        {
                            if (onKing(blackKing, whiteFigures.whiteKing))
                                continue;
                            whiteFigures.blackKing = blackKing;
                            if (isCheck(whiteFigures))
                                continue;
                            if (blackKing == black.combo.whiteBishop)
                            {
                                solved = false;
                                break;
                            }
                            if (blackKing == black.combo.whiteKnight)
                            {
                                solved = false;
                                break;
                            }
                            if (box.Exists(whiteFigures))
                                continue;
                            solved = false;
                            break;
                        }
                        if (!solved)
                            continue;

                        blackQueue.Enqueue(black);
                        //Console.WriteLine((++qty) + " " +
                        //    black.combo.getFEN());
                        //black.combo.print();
                        //Console.ReadLine();
                    }
                }
            }
        }

        private IEnumerable<BlacksMove> AllBlackBackMoves(WhitesMove white)
        {
            BlacksMove black;
            black.combo = white.combo;
            black.moves = (byte)(white.moves + 1);
            foreach(Coord blackKing in AllKingMoves(white.combo.blackKing))
            {
                if (onKing(blackKing, black.combo.whiteKing))
                    continue;
                if (blackKing == white.combo.whiteBishop) continue;
                if (blackKing == white.combo.whiteKnight) continue;
                black.combo.blackKing = blackKing;
                yield return black;
            }
        }

        private IEnumerable<WhitesMove> AllWhiteBackMoves(BlacksMove black)
        {
            foreach (WhitesMove white in AllWhiteKingMoves(black))
                yield return white;
            foreach (WhitesMove white in AllWhiteBishopMoves(black))
                yield return white;
            foreach (WhitesMove white in AllWhiteKnightMoves(black))
                yield return white;
        }

        private IEnumerable<WhitesMove> AllWhiteKingMoves(BlacksMove black)
        {
            WhitesMove white;
            white.combo = black.combo;
            white.moves = (byte)(black.moves + 1);
            foreach(Coord whiteKing in AllKingMoves(black.combo.whiteKing))
            {
                if (onKing(whiteKing, black.combo.blackKing))
                    continue;
                if (whiteKing == black.combo.whiteKnight)
                    continue;
                if (whiteKing == black.combo.whiteBishop)
                    continue;
                white.combo.whiteKing = whiteKing;
                white.moveTo = black.combo.whiteKing;
                white.moveFrom = whiteKing;
                yield return white;
            }
        }

        private IEnumerable<WhitesMove> AllWhiteBishopMoves(BlacksMove black)
        {
            WhitesMove white;
            white.combo = black.combo;
            white.moves = (byte)(black.moves + 1);
            for (int sx = -1; sx <= 1; sx += 2)
                for (int sy = -1; sy <= 1; sy += 2)
                {
                    Coord whiteBishop = black.combo.whiteBishop;
                    while(true)
                    {
                        whiteBishop.x = (byte)(whiteBishop.x + sx);
                        whiteBishop.y = (byte)(whiteBishop.y + sy);
                        if (!whiteBishop.valid())
                            break;
                        if (whiteBishop == black.combo.whiteKing)
                            break;
                        if (whiteBishop == black.combo.whiteKnight)
                            break;
                        if (whiteBishop == black.combo.blackKing)
                            break;
                        white.combo.whiteBishop = whiteBishop;
                        white.moveTo = black.combo.whiteBishop;
                        white.moveFrom = whiteBishop;
                        yield return white;
                    }
                }
        }

        private IEnumerable<WhitesMove> AllWhiteKnightMoves(BlacksMove black)
        {
            int[,] step = {
                           {-1,-2 }, {-1,2 },
                           { 1,-2 }, { 1,2 },
                           {-2,-1 }, {-2,1 },
                           { 2,-1 }, { 2,1 }
                           };
            WhitesMove white;
            white.combo = black.combo;
            white.moves = (byte)(black.moves + 1);
            for (int j = 0; j < 8; j++) 
            {
                Coord whiteKnight;
                whiteKnight.x = (byte)(black.combo.whiteKnight.x + step[j, 0]);
                whiteKnight.y = (byte)(black.combo.whiteKnight.y + step[j, 1]);
                if (!whiteKnight.valid())
                    continue;
                if (whiteKnight == black.combo.whiteKing)
                    continue;
                if (whiteKnight == black.combo.whiteBishop)
                    continue;
                if (whiteKnight == black.combo.blackKing)
                    continue;
                white.combo.whiteKnight = whiteKnight;
                white.moveTo = black.combo.whiteKnight;
                white.moveFrom = whiteKnight;
                yield return white;
            }
        }

        private bool isWhiteSquare(Coord coord)
        {
            return (coord.x + coord.y) % 2 == 1;
        }

        private IEnumerable<Coord> AllCoords()
        {
            Coord coord;
            for (coord.y = 0; coord.y < 8; coord.y++)
                for (coord.x = 0; coord.x < 8; coord.x++)
                    yield return coord;
        }

        public IEnumerable<Coord> AllKingMoves(Coord king)
        {
            Coord coord;
            for (coord.y = (byte)Math.Max(0, king.y - 1); coord.y <= (byte)Math.Min(7, king.y + 1); coord.y++)
                for (coord.x = (byte)Math.Max(0, king.x - 1); coord.x <= (byte)Math.Min(7, king.x + 1); coord.x++)
                    //if (coord != king) // king can move
                        yield return coord;
        }

        private bool isCheckmate(Combo combo)
        {
            if (!isCheck(combo))
                return false;
            foreach (Coord blackKing in AllKingMoves(combo.blackKing))
            {
                combo.blackKing = blackKing;
                if (!isCheck(combo))
                        return false;
            }
            return true;
        }

        private bool isCheck(Combo combo)
        {
            return (onKing(combo) ||
                    onBishop(combo) ||
                    onKnight(combo));
        }

        private bool onKnight(Coord a, Coord b)
        {
            return (Math.Abs(a.x - b.x) == 1 && Math.Abs(a.y - b.y) == 2) ||
                   (Math.Abs(a.x - b.x) == 2 && Math.Abs(a.y - b.y) == 1);
        }

        private bool onKnight(Combo combo)
        {
            return onKnight(combo.whiteKnight, combo.blackKing);
        }

        private bool onBishop(Combo combo)
        {
            if (combo.whiteBishop == combo.blackKing)
                return false;
            if (combo.whiteBishop.x + combo.whiteBishop.y != combo.blackKing.x + combo.blackKing.y &&
                combo.whiteBishop.x - combo.whiteBishop.y != combo.blackKing.x - combo.blackKing.y)
                return false;
            int sx = combo.whiteBishop.x - combo.blackKing.x > 0 ? 1 : -1;// x--sx--> wbx
            int sy = combo.whiteBishop.y - combo.blackKing.y > 0 ? 1 : -1;// y--sy--> wby
            Coord square = combo.blackKing;
            while (true)
            {
                square.x = (byte)(square.x + sx);
                square.y = (byte)(square.y + sy);
                if (square == combo.whiteBishop) return true;
                if (square == combo.whiteKnight) return false;
                if (square == combo.whiteKing) return false;
            }
        }

        private bool onKing(Combo combo)
        {
            return onKing(combo.whiteKing, combo.blackKing);
        }

        private bool onKing(Coord king1, Coord king2)
        {
            return Math.Abs(king1.x - king2.x) <= 1 &&
                   Math.Abs(king1.y - king2.y) <= 1;
        }

        private Combo getCombo(string fen)
        {
            string[] lines = fen.Replace('/', ' ').Split();
            Combo combo = new Combo();
            Coord coord;
            for (byte y = 0; y < lines.Length; y++)
            {
                byte x = 0;
                for (int j = 0; j < lines[y].Length; j++)
                {
                    if (lines[y][j] >= '1' && lines[y][j] <= '8')
                    {
                        x += Convert.ToByte(lines[y].Substring(j, 1));
                        continue;
                    }
                    coord.x = x;
                    coord.y = y;
                    switch(lines[y][j])
                    {
                        case 'K': combo.whiteKing = coord; break;
                        case 'B': combo.whiteBishop = coord; break;
                        case 'N': combo.whiteKnight = coord; break;
                        case 'k': combo.blackKing = coord; break;
                    }
                    x++;
                }
            }
            return combo;
        }

        public string FindMove(string fen)
        {
            Combo combo = getCombo(fen);
            combo.print();
            WhitesMove white = box.Get(combo);
            if (white.moves == 0)
                return "Not found";
            return "Mate in" + white.moves + " " + white.moveFrom.chess + "-" + white.moveTo.chess;

        }

    }
}
