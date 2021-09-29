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
        public ShipCell[] Cells { get; set; }
        public Direction Direction { get; set; } = Direction.Down;
        public int Length { get; set; }
        public string Name { get; set; } = string.Empty;

        public bool IsSunk
        {
            get
            {
                int hits = 0;
                foreach (var cell in Cells)
                    if (cell != null && cell.IsHit)
                        hits++;

                return hits == Length;
            }
        }

        public Ship(string name, int length)
        {
            Name = name;
            Length = length;
            Cells = new ShipCell[Length];
        }

        public Ship(string name, int length, Direction direction)
        {
            Name = name;
            Length = length;
            Direction = direction;
            Cells = new ShipCell[Length];
        }


        public bool ValidPlacement()
        {
            if (Cells.Length <= 0)
                return false;

            var firstCell = Cells[0];
            for (int i = 1; i < Cells.Length; i++)
            {
                if (Cells[i] == null)
                    return false;


                if (Direction == Direction.Up || Direction == Direction.Down)
                {
                    if (Cells[i].Column != firstCell.Column)
                        return false;
                }
               
            }

            return true;

        }

        public RectangleF GetRectangle()
        {
            var first = Cells.First();
            var last = Cells.Last();

            if (first == null || last == null)
            {
                return new RectangleF(0, 0, 0, 0);
            }

            if (Direction == Direction.Left || Direction == Direction.Up)
            {
                var width = first.CellBox[2].X - last.CellBox[0].X;
                var height = first.CellBox[2].Y - last.CellBox[0].Y;

                return new RectangleF(last.CellBox[0].X, last.CellBox[0].Y, width, height);
            }
            else
            {
                var width = last.CellBox[2].X - first.CellBox[0].X;
                var height = last.CellBox[2].Y - first.CellBox[0].Y;

                return new RectangleF(first.CellBox[0].X, first.CellBox[0].Y, width, height);
            }
        }

    }

    public enum Direction
    {
        Up,
        Right,
        Down,
        Left
    }
}
