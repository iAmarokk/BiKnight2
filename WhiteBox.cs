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
        Chess.WhitesMove[,,,] box;

        public int Count { get; private set; }

        public WhiteBox()
        {
            Count = 0;
            box = new Chess.WhitesMove[64, 64, 64, 64];
        }

        public bool Put(Chess.WhitesMove white)
        {
            if (Exists(white.combo))
                return false;
            box[white.combo.whiteKing.index,
                white.combo.whiteBishop.index,
                white.combo.whiteKnight.index,
                white.combo.blackKing.index] = white;
            Count++;
            return true;
        }

        public bool Exists(Chess.Combo combo)
        {
            return (box[combo.whiteKing.index,
                        combo.whiteBishop.index,
                        combo.whiteKnight.index,
                        combo.blackKing.index].moves > 0); 
        }

        public Chess.WhitesMove Get(Chess.Combo combo)
        {
            return box[combo.whiteKing.index,
                combo.whiteBishop.index,
                combo.whiteKnight.index,
                combo.blackKing.index];
        }

        public void Save()
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream("BiKnight.bin", FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream,box);
            stream.Close();
        }

        public void Load()
        {

        }
    }
}
