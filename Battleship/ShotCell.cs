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
        public Point[] CellBox { get; set; }

        public override string ToString()
        {
            return $"({Column},{Row})";
        }
    }

    public class ShotCell : Cell
    {
        public bool HasShot { get; set; } = false;
        public bool IsHit { get; set; } = false;
        //public string Column { get; set; }
        //public string Row { get; set; }
        //public Point[] CellBox { get; set; }

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
