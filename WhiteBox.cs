using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace BiKnight2
{
    class WhiteBox
    {

        struct WhitesMoveLite
        {
            public byte moves; // 0 .. 66
            public byte moveBits;
        }

        //WhitesMoveLite[,,,] box;

        public int Count { get; private set; }

        public WhiteBox()
        {
            Count = 0;
            //box = new WhitesMoveLite[64, 32, 64, 64];
        }

        private WhitesMoveLite WhitesMoveToLite(Chess.WhitesMove white)
        {
            WhitesMoveLite lite;
            lite.moves = white.moves;
            lite.moveBits = (byte)white.moveTo.index;
            if (white.moveFrom == white.combo.whiteKing)
                lite.moveBits |= 64;
            if (white.moveFrom == white.combo.whiteBishop)
                lite.moveBits |= 128;
            if (white.moveFrom == white.combo.whiteKnight)
                lite.moveBits |= 128 + 64;
            return lite;
        }

        private Chess.WhitesMove WhitesMoveFromLite (WhitesMoveLite lite, Chess.Combo combo)
        {
            Chess.WhitesMove white;
            white.combo = combo;
            white.moves = lite.moves;
            white.moveTo = new Chess.Coord { index = lite.moveBits & 63 };
            white.moveFrom = new Chess.Coord { index = 0 };
            if ((lite.moveBits >> 6) == 1) //king
                white.moveFrom = combo.whiteKing;
            if ((lite.moveBits >> 6) == 2) //bishop
                white.moveFrom = combo.whiteBishop;
            if ((lite.moveBits >> 6) == 3) //knight
                white.moveFrom = combo.whiteKnight;
            return white;
        }

        public bool Put(Chess.WhitesMove white)
        {
            if (Exists(white.combo))
                return false;
            /*box[white.combo.whiteKing.index,
                white.combo.whiteBishop.index / 2,
                white.combo.whiteKnight.index,
                white.combo.blackKing.index] = WhitesMoveToLite(white);*/
            Count++;
            return true;
        }

        public bool Exists(Chess.Combo combo)
        {
            return true; /*(box[combo.whiteKing.index,
                        combo.whiteBishop.index / 2,
                        combo.whiteKnight.index,
                        combo.blackKing.index].moves > 0);*/ 
        }

        public Chess.WhitesMove Get(Chess.Combo combo)
        {
            WhitesMoveLite lite;
            using (FileStream file = new FileStream("BiKnight.box", FileMode.Open))
            {
                file.Seek(
                    combo.whiteKing.index * 32 * 64 * 64 * 2 +
                    combo.whiteBishop.index/2    * 64 * 64 * 2 +
                    combo.whiteKnight.index         * 64 * 2 +
                    combo.blackKing.index                * 2, 
                    SeekOrigin.Begin);
                lite.moves = (byte)file.ReadByte();
                lite.moveBits = (byte)file.ReadByte();

            }
            //return WhitesMoveFromLite(
            //    box[combo.whiteKing.index,
            //        combo.whiteBishop.index / 2,
            //        combo.whiteKnight.index,
            //        combo.blackKing.index], combo);
            return WhitesMoveFromLite(lite,combo);
        }

        public void Save()
        {
            //using (FileStream file = new FileStream("BiKnight.box", FileMode.Create))
            //{
            //    for (int a = 0; a < 64; a++)
            //    for (int b = 0; b < 32; b++)
            //    for (int c = 0; c < 64; c++)
            //    for (int d = 0; d < 64; d++)
            //    {
            //     file.WriteByte(box[a, b, c, d].moves);
            //     file.WriteByte(box[a, b, c, d].moveBits);
            //    }

            //}
        }

        public void Load()
        {
            //using (FileStream file = new FileStream("BiKnight.box", FileMode.Open))
            //{
            //    for (int a = 0; a < 64; a++)
            //        for (int b = 0; b < 32; b++)
            //            for (int c = 0; c < 64; c++)
            //                for (int d = 0; d < 64; d++)
            //                {
            //                    box[a, b, c, d].moves = (byte)file.ReadByte();
            //                    box[a, b, c, d].moveBits = (byte)file.ReadByte();
            //                }
            //}
        }
    }
}
