using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Battleship
{
    public class Cell
    {
        public int Column { get; set; } = -1;
        public int Row { get; set; } = -1;
        public int Index 
        { 
            get
            {
                int idx = Column * 10 + Row;
                return idx;
            } 
        }
        public Point[] CellBox { get; set; }
        public Point Location { get { return Helpers.CenterOfPolygon(CellBox); } }
        public int Rank { get; set; }

        public bool IsParity
        {
            get
            {
                return (Column % 2 + Row % 2 == 1);
            }
        }

        public override string ToString()
        {
            return $"({Column},{Row})";
        }

        public bool IsNextToCell(Cell cell)
        {
            if (Math.Abs(Column - cell.Column) == 1 || Math.Abs(Row - cell.Row) == 1)
                return true;

            return false;
        }

        public Direction DirectionTo(Cell cell)
        {
            if (Row < cell.Row)
                return Direction.Up;
            else if (Row > cell.Row)
                return Direction.Down;
            else if (Column < cell.Column)
                return Direction.Right;
            else if (Column > cell.Column)
                return Direction.Left;

            return Direction.Down;
        }

    }

    public class ShotCell : Cell
    {
        public bool HasShot { get; set; } = false;
        public bool IsHit { get; set; } = false;
        public bool IsOnSunkShip { get; set; } = false;

        public ShotCell()
        {

        }

        public ShotCell(Point[] cellBox)
        {
            CellBox = cellBox;
        }

        public ShotCell(Point[] cellBox, int row, int col)
        {
            CellBox = cellBox;
            Row = row;
            Column = col;
        }

        public void SetHit()
        {
            HasShot = true;
            IsHit = true;
        }

        public void SetMiss()
        {
            HasShot = true;
            IsHit = false;
        }
    }

    public class ShipCell : ShotCell
    {
        public Ship Ship { get; set; } = null;
        public bool PlacingShip { get; set; } = false;
        public bool HasShip
        {
            get { return Ship != null; }
        }

        public ShipCell(Point[] cellBox) : base(cellBox)
        {

        }

        public ShipCell(Point[] cellBox, int row, int col) : base(cellBox, row, col)
        {

        }

        public ShipCell(Ship ship, Cell cell) : base(cell.CellBox)
        {
            Ship = ship;
            
        }
    }
}
