using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;


namespace Battleship
{
    public class Ship
    {
        public Point Position { get; set; }
        public Point[] Cells { get; set; }
        public Direction Direction { get; set; } = Direction.Down;
        public int Length { get; set; }
        public string Name { get; set; }

        public Ship(string name, int length)
        {
            Name = name;
            Length = length;
        }

        public Ship(string name, int length, Direction direction)
        {
            Name = name;
            Length = length;
            Direction = direction;
        }

        //private void InitCells()
        //{
        //    Cells = new Point[Length];

        //    int offset = 
        //    for(int i = 0; i < Length; i++)
        //    {

        //    }
        //}
    }

    public enum Direction
    {
        Up,
        Right,
        Down,
        Left

        //Up,
        //Down,
        //Left,
        //Right
    }
}
