using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Diagnostics;

namespace Battleship
{
    public partial class Form1 : Form
    {
        private Bitmap _shotsImage;
        private Bitmap _shipsImage;
        private Graphics _shotsGfx;
        private Graphics _shipsGfx;
        private Pen _cellPen = new Pen(Brushes.Black, 2.0f);
        private SolidBrush _hitBrush = new SolidBrush(Color.Red);
        private SolidBrush _missBrush = new SolidBrush(Color.White);
        private SolidBrush _shipBrush = new SolidBrush(Color.Blue);
        private SolidBrush _invalidShipBrush = new SolidBrush(Color.Yellow);

        private int _sideLen = 40;
        private ShotCell[] _shotCells;
        private ShipCell[] _shipCells;

        private Size _shotBoardSize;

        private Ship[] _ships;
        private int _currentShipIdx = 0;
        private bool _placingShip = true;
        private Ship _currentShip
        {
            get
            {
                if (_currentShipIdx >= 0 && _currentShipIdx < _ships.Length)
                    return _ships[_currentShipIdx];
                else
                    return null;
            }
        }


        //private Ship _currentShip = null;


        private Random _rnd = new Random();



        public Form1()
        {
            InitializeComponent();
            shipsBox.MouseWheel += shipsBox_MouseWheel;

            //_shotsImage = new Bitmap(shotsBox.Width, shotsBox.Height);
            //_shotsImage = new Bitmap(shotsBox.Width, shotsBox.Height);
            InitGfx();
            InitShips();
            InitBoard();

            shotsBox.Invalidate();
        }



        private void InitGfx()
        {
            _shotBoardSize = shotsBox.Size;

            _shotsGfx = shotsBox.CreateGraphics();
            _shipsGfx = shipsBox.CreateGraphics();

        }

        private void InitBoard()
        {
            _shotCells = new ShotCell[100];
            _shipCells = new ShipCell[100];

            //var shotCell = new List<ShotCell>();
            int n = 0;
            int row = 0;
            int column = 0;
            for (int x = 0; x <= _shotBoardSize.Width - _sideLen; x += _sideLen)
            {
                row = 0;
                for (int y = 0; y <= _shotBoardSize.Height - _sideLen; y += _sideLen)
                {
                    var cell = new Point[]
                    {

                        new Point(x,y), // Top-left
                        new Point(x + _sideLen, y), // Top-right
                        new Point(x + _sideLen, y + _sideLen), //Bot-right
                        new Point(x, y + _sideLen) // Bot-left

                        //new Point(x,y), // Top-left
                        //new Point(x, y + _sideLen), // Bot-left
                        //new Point(x + _sideLen, y + _sideLen), //Bot-right
                        //new Point(x + _sideLen, y) // Top-right
                    };

                    //shotCell.Add(new ShotCell(cell));

                    //_shotCells[n] = new ShotCell(cell);
                    _shotCells[n] = new ShotCell(cell, row, column);

                    _shipCells[n] = new ShipCell(cell, row, column);
                    n++;
                    row++;
                }
                column++;
            }
        }

        private void InitShips()
        {
            var ships = new List<Ship>();
            ships.Add(new Ship("Carrier", 5));
            ships.Add(new Ship("Battleship", 4));
            ships.Add(new Ship("Destroyer", 3));
            ships.Add(new Ship("Submarine", 3));
            ships.Add(new Ship("Patrol Boat", 2));

            _ships = ships.ToArray();
            _currentShipIdx = 0;

            //_currentShip = ships.First();
        }


        private void DrawShotsBoard(Graphics gfx)
        {
            //_shotsGfx.Clear(shotsBox.BackColor);
            gfx.Clear(shotsBox.BackColor);

            foreach (var cell in _shotCells)
            {
                //_shotsGfx.DrawPolygon(_cellPen, cell.CellBox);
                if (cell.HasShot)
                {
                    if (cell.IsHit)
                        gfx.FillPolygon(_hitBrush, cell.CellBox);
                    else
                        gfx.FillPolygon(_missBrush, cell.CellBox);
                }


                gfx.DrawPolygon(_cellPen, cell.CellBox);
            }

            //shotsBox.Invalidate();
        }

        private void DrawShipsBoard(Graphics gfx)
        {
            gfx.Clear(shipsBox.BackColor);

            foreach (var cell in _shipCells)
            {
                //_shotsGfx.DrawPolygon(_cellPen, cell.CellBox);


                if (cell.HasShip)
                    gfx.FillPolygon(_shipBrush, cell.CellBox);

                if (cell.PlacingShip && _placingShip)
                    gfx.FillPolygon(_shipBrush, cell.CellBox);

                if (cell.Ship != null && !cell.Ship.ValidPlacement())
                    gfx.FillPolygon(_invalidShipBrush, cell.CellBox);

                if (cell.HasShot)
                {
                    if (cell.IsHit)
                        gfx.FillPolygon(_hitBrush, cell.CellBox);
                    else
                        gfx.FillPolygon(_missBrush, cell.CellBox);
                }

                gfx.DrawPolygon(_cellPen, cell.CellBox);
            }
        }

        private void PositionShip(Ship ship, Point location)
        {
            if (!_placingShip || ship == null)
                return;

            //ClearPlacedShips();
            ClearPlacedShip(ship);

            var overCell = CellFromPosition(_shipCells, location) as ShipCell;
            if (overCell == null)
                return;

            if (overCell.Ship == null || overCell.Ship.Name == ship.Name)
            {
                overCell.Ship = ship;
                overCell.PlacingShip = true;

            }

            int overCol = overCell.Column;
            int overRow = overCell.Row;
            ship.Cells[0] = overCell;
            //Debug.WriteLine($"Loc: {location}  Col: {overCol}  Row: {overRow}");
            //Debug.WriteLine($"Loc: {location} Cell[0]: {overCell}");
            //Debug.WriteLine($"Pos Loc: {location} Cell[0]: {ship.Cells[0]}");

            for (int i = 0; i < ship.Length - 1; i++)
            {
                switch (ship.Direction)
                {
                    case Direction.Down:
                        overRow++;
                        break;
                    case Direction.Up:
                        overRow--;
                        break;
                    case Direction.Left:
                        overCol--;
                        break;
                    case Direction.Right:
                        overCol++;
                        break;
                }

                //Debug.WriteLine($"Loc: {location}  Col: {overCol}  Row: {overRow}");

                var nextCell = CellFromCoords(_shipCells, overRow, overCol) as ShipCell;
                if (nextCell != null)
                {
                    if (nextCell.Ship == null || nextCell.Ship.Name == ship.Name)
                    {
                        nextCell.Ship = ship;
                        nextCell.PlacingShip = true;
                    }
                }

                ship.Cells[i + 1] = nextCell;

                //Debug.WriteLine($"Loc: {location} Cell[{i + 1}]: {nextCell}");
                //Debug.WriteLine($"Pos Loc: {location} Cell[{i + 1}]: {ship.Cells[i + 1]}");

            }




            shipsBox.Invalidate();
            // TODO: How to unset ships cells as a ship is being positioned?
        }

        private void PlaceShip(Ship ship, Point location)
        {
            if (ship == null)
                return;

            Debug.WriteLine($"Place ship: {ship.Name}");

            if (!ship.ValidPlacement())
                return;

            for (int i = 0; i < ship.Cells.Length; i++)
                ship.Cells[i].PlacingShip = false;

            //var overCell = CellFromPosition(_shipCells, location) as ShipCell;
            //if (overCell == null)
            //    return;

            //overCell.Ship = ship;
            //overCell.PlacingShip = false;

            //int overCol = overCell.Column;
            //int overRow = overCell.Row;

            //for (int i = 0; i < ship.Length - 1; i++)
            //{
            //    switch (ship.Direction)
            //    {
            //        case Direction.Down:
            //            overRow++;
            //            break;
            //        case Direction.Up:
            //            overRow--;
            //            break;
            //        case Direction.Left:
            //            overCol--;
            //            break;
            //        case Direction.Right:
            //            overCol++;
            //            break;
            //    }

            //    var nextCell = CellFromCoords(_shipCells, overRow, overCol) as ShipCell;
            //    if (nextCell != null)
            //    {
            //        nextCell.Ship = ship;
            //        nextCell.PlacingShip = false;
            //    }
            //}

            shipsBox.Invalidate();
        }

        private void ClearShips()
        {
            for (int i = 0; i < _shipCells.Length; i++)
            {
                _shipCells[i].Ship = null;
            }
        }

        private void ClearPlacedShip(Ship ship)
        {

            int n = 0;
            for (int i = 0; i < _shipCells.Length; i++)
            {
                if (_shipCells[i].Ship != null && _shipCells[i].Ship.Name == ship.Name)
                {
                    if (_shipCells[i].PlacingShip)
                    {
                        _shipCells[i].Ship = null;
                        _shipCells[i].PlacingShip = false;
                        n++;
                    }
                }

            }

            Debug.WriteLine($"Clear Ship {ship.Name}: {n}");

        }


        private void ClearPlacedShips()
        {
            int n = 0;
            for (int i = 0; i < _shipCells.Length; i++)
            {
                if (_shipCells[i].PlacingShip)
                {
                    _shipCells[i].Ship = null;
                    _shipCells[i].PlacingShip = false;
                    n++;
                }
            }

            Debug.WriteLine($"Clear Ships: {n}");

        }

        private Cell CellFromCoords(Cell[] cells, int row, int column)
        {
            int idx = column * 10 + row;

            if (idx >= cells.Length || idx < 0)
                return null;

            return cells[idx];
        }

        private Cell CellFromPosition(Cell[] cells, Point pos)
        {
            for (int i = 0; i < cells.Length; i++)
            {
                if (Helpers.PointIsInsidePolygon(cells[i].CellBox, pos))
                    return cells[i];
            }

            return null;
        }

        private void CheckForHit(Point location)
        {
            for (int i = 0; i < _shotCells.Length; i++)
            {
                if (Helpers.PointIsInsidePolygon(_shotCells[i].CellBox, location))
                {
                    _shotCells[i].HasShot = true;
                    _shotCells[i].IsHit = IsHit(_shipCells[i], _shipCells);

                    //var test = IsHit(_shipCells[i], _shipCells);
                }
            }

            shotsBox.Invalidate();
            shipsBox.Invalidate();
        }

        private bool IsHit(ShotCell cell, ShipCell[] shipBoard)
        {
            var shipCell = CellFromCoords(shipBoard, cell.Row, cell.Column) as ShipCell;

            if (shipCell != null && shipCell.HasShip)
            {
                shipCell.HasShot = true;
                shipCell.IsHit = true;
                return true;
            }
            else
            {
                shipCell.HasShot = true;
                shipCell.IsHit = false;
            }


            return false;

        }

        private bool RandomHit()
        {
            var num = _rnd.Next(0, 2);

            if (num == 1)
                return true;
            else
                return false;
        }

        private void shotsBox_Paint(object sender, PaintEventArgs e)
        {
            DrawShotsBoard(e.Graphics);
        }

        private void shotsBox_MouseClick(object sender, MouseEventArgs e)
        {
            CheckForHit(e.Location);
        }

        private void shipsBox_Paint(object sender, PaintEventArgs e)
        {
            DrawShipsBoard(e.Graphics);
        }

        private void shipsBox_MouseMove(object sender, MouseEventArgs e)
        {
            //Debug.WriteLine("Move");
            PositionShip(_currentShip, e.Location);
        }

        private void shipsBox_MouseClick(object sender, MouseEventArgs e)
        {
            Debug.WriteLine("Click");

            PlaceShip(_currentShip, e.Location);

            if (_currentShipIdx < _ships.Length - 1)
            {
                _currentShipIdx++;
            }
            else if (_currentShipIdx == _ships.Length - 1)
            {
                _placingShip = false;
                _currentShipIdx = -1;
            }

            PositionShip(_currentShip, e.Location);
        }

        private void shipsBox_MouseWheel(object sender, MouseEventArgs e)
        {
            if (_currentShip == null)
                return;

            int curDir = (int)_currentShip.Direction;

            if (e.Delta > 0) // Wheel UP
            {
                if (curDir == 3)
                    curDir = (int)Direction.Up;
                else
                    curDir++;
            }
            else // Wheel DOWN
            {
                if (curDir == 0)
                    curDir = (int)Direction.Left;
                else
                    curDir--;
            }

            _currentShip.Direction = (Direction)curDir;

            PositionShip(_currentShip, e.Location);
        }
    }
}
