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
        private Pen _cellPen = new Pen(Brushes.Black, 2.0f);
        private SolidBrush _hitBrush = new SolidBrush(Color.Red);
        private SolidBrush _missBrush = new SolidBrush(Color.White);
        private SolidBrush _shipBrush = new SolidBrush(Color.Blue);
        private SolidBrush _invalidShipBrush = new SolidBrush(Color.Yellow);
        private SolidBrush _sunkShipBrush = new SolidBrush(Color.MediumSeaGreen);
        private Font _cellCoordFont = new Font("Tahoma", 5, FontStyle.Regular);
        private Size _boardSize;
        private Random _rnd = new Random();

        private PlayerBoard _playerBoard;
        private PlayerBoard _computerBoard;

        private ComputerAI _compAI;

        public Form1()
        {
            InitializeComponent();

            _boardSize = shotsBox.Size;
            shipsBox.MouseWheel += shipsBox_MouseWheel;
            shipsBox2.MouseWheel += ShipsBox2_MouseWheel;

            InitBoard();
            RefreshPlayerBoards();
        }

        private void InitBoard()
        {
            _playerBoard = new PlayerBoard(_boardSize, GetShips(), 1);
            _computerBoard = new PlayerBoard(_boardSize, GetShips(), 2);

            _compAI = new ComputerAI(_computerBoard, _playerBoard);
            WireEvents();

        }

        private void WireEvents()
        {
            _playerBoard.ShipWasSunk -= _playerBoard_ShipWasSunk;
            _playerBoard.ShipWasSunk += _playerBoard_ShipWasSunk;

            _computerBoard.ShipWasSunk -= _computerBoard_ShipWasSunk;
            _computerBoard.ShipWasSunk += _computerBoard_ShipWasSunk;

        }


        private Ship[] GetShips()
        {
            var ships = new List<Ship>();
            ships.Add(new Ship("Carrier", 5));
            ships.Add(new Ship("Battleship", 4));
            ships.Add(new Ship("Destroyer", 3));
            ships.Add(new Ship("Submarine", 3));
            ships.Add(new Ship("Patrol Boat", 2));
            return ships.ToArray();
        }

        private void DrawShotsBoard(Graphics gfx, PlayerBoard board)
        {
            gfx.Clear(shotsBox.BackColor);

            foreach (var cell in board.ShotCells)
            {
                if (cell.HasShot)
                {
                    if (cell.IsHit)
                        gfx.FillPolygon(_hitBrush, cell.CellBox);
                    else
                        gfx.FillPolygon(_missBrush, cell.CellBox);
                }

                gfx.DrawPolygon(_cellPen, cell.CellBox);

                var center = Helpers.CenterOfPolygon(cell.CellBox);
                string label = $"({cell.Column}/{cell.Row})";
                var lblSize = gfx.MeasureString(label, _cellCoordFont);
                center.X -= (int)(lblSize.Width / 2f);
                center.Y -= (int)(lblSize.Height / 2f);

                gfx.DrawString(label, _cellCoordFont, Brushes.Black, center);
            }
        }

        private void DrawShipsBoard(Graphics gfx, PlayerBoard board)
        {
            gfx.Clear(shipsBox.BackColor);

            foreach (var cell in board.ShipCells)
            {
                if (cell.HasShip)
                    gfx.FillPolygon(_shipBrush, cell.CellBox);


                if (cell.PlacingShip && board.PlacingShip)
                    gfx.FillPolygon(_shipBrush, cell.CellBox);

                if (cell.Ship != null && !cell.Ship.ValidPlacement())
                    gfx.FillPolygon(_invalidShipBrush, cell.CellBox);

                if (cell.HasShot)
                {
                    if (cell.HasShip && cell.Ship.IsSunk)
                    {
                        gfx.FillPolygon(_sunkShipBrush, cell.CellBox);
                    }
                    else
                    {
                        if (cell.IsHit)
                            gfx.FillPolygon(_hitBrush, cell.CellBox);
                        else
                            gfx.FillPolygon(_missBrush, cell.CellBox);
                    }
                }

                gfx.DrawPolygon(_cellPen, cell.CellBox);

                var center = Helpers.CenterOfPolygon(cell.CellBox);
                string label = $"({cell.Column}/{cell.Row})";
                var lblSize = gfx.MeasureString(label, _cellCoordFont);
                center.X -= (int)(lblSize.Width / 2f);
                center.Y -= (int)(lblSize.Height / 2f);

                gfx.DrawString(label, _cellCoordFont, Brushes.Black, center);
            }
        }

        private bool RandomHit()
        {
            var num = _rnd.Next(0, 2);

            if (num == 1)
                return true;
            else
                return false;
        }

        private void RefreshPlayerBoards()
        {
            shotsBox.Invalidate();
            shipsBox.Invalidate();
            shotsBox2.Invalidate();
            shipsBox2.Invalidate();

            shotsBox.Refresh();
            shipsBox.Refresh();
            shotsBox2.Refresh();
            shipsBox2.Refresh();

            playerShipsLabel.Text = $"Ships ({5 - _playerBoard.ShipsSunk}/5)";
            computerShipsLabel.Text = $"Ships ({5 - _computerBoard.ShipsSunk}/5)";
            playerShotsLabel.Text = $"Shots Taken: {_playerBoard.ShotsTaken}";
            computerShotsLabel.Text = $"Shots Taken: {_computerBoard.ShotsTaken}";
        }

        private void shotsBox_Paint(object sender, PaintEventArgs e)
        {
            DrawShotsBoard(e.Graphics, _playerBoard);
        }

        private void shipsBox_Paint(object sender, PaintEventArgs e)
        {
            DrawShipsBoard(e.Graphics, _playerBoard);
        }

        private void shotsBox2_Paint(object sender, PaintEventArgs e)
        {
            DrawShotsBoard(e.Graphics, _computerBoard);

        }

        private void shipsBox2_Paint(object sender, PaintEventArgs e)
        {
            DrawShipsBoard(e.Graphics, _computerBoard);
        }

        private void shotsBox_MouseClick(object sender, MouseEventArgs e)
        {
            shipSunkLabel.Visible = false;

            var playerHit = _playerBoard.TakeShot(e.Location, _computerBoard);

            //Debug.WriteLine($"Player hit: {playerHit}");
            Debug.WriteLine($"Player defeated: {_playerBoard.IsDefeated()}");
            Debug.WriteLine($"Comp defeated: {_computerBoard.IsDefeated()}");
            RefreshPlayerBoards();

            Task.Delay(500).Wait();
            var compHit = _compAI.TakeShot();
            //Debug.WriteLine($"Comp hit: {compHit}");

            RefreshPlayerBoards();

            if (_playerBoard.IsDefeated())
            {
                winnerLabel.Text = "Computer player wins!!!";
                winnerLabel.Visible = true;
            }
            else if (_computerBoard.IsDefeated())
            {
                winnerLabel.Text = "Player 1 wins!!!";
                winnerLabel.Visible = true;
            }
            else if (!_playerBoard.IsDefeated() && !_computerBoard.IsDefeated())
            {
                winnerLabel.Visible = false;
            }
        }

        private void shotsBox2_MouseClick(object sender, MouseEventArgs e)
        {
            _computerBoard.TakeShot(e.Location, _playerBoard);
            Debug.WriteLine($"Player defeated: {_playerBoard.IsDefeated()}");
            RefreshPlayerBoards();
        }

        private void shipsBox_MouseMove(object sender, MouseEventArgs e)
        {
            _playerBoard.PositionShip(e.Location);
            RefreshPlayerBoards();
        }

        private void shipsBox2_MouseMove(object sender, MouseEventArgs e)
        {
            _computerBoard.PositionShip(e.Location);
            RefreshPlayerBoards();
        }

        private void shipsBox_MouseClick(object sender, MouseEventArgs e)
        {
            _playerBoard.PlaceShip(e.Location);
            _playerBoard.PositionShip(e.Location);
            RefreshPlayerBoards();
        }

        private void shipsBox2_MouseClick(object sender, MouseEventArgs e)
        {
            _computerBoard.PlaceShip(e.Location);
            _computerBoard.PositionShip(e.Location);
            RefreshPlayerBoards();
        }

        private void shipsBox_MouseWheel(object sender, MouseEventArgs e)
        {
            _playerBoard.RotateShip(e.Delta, e.Location);
            RefreshPlayerBoards();
        }

        private void ShipsBox2_MouseWheel(object sender, MouseEventArgs e)
        {
            _computerBoard.RotateShip(e.Delta, e.Location);
            RefreshPlayerBoards();
        }

        private void randomizeButton_Click(object sender, EventArgs e)
        {
            _playerBoard = new PlayerBoard(_boardSize, GetShips(), 1);
            _playerBoard.RandomizeBoard();

            _computerBoard = new PlayerBoard(_boardSize, GetShips(), 2);
            _computerBoard.RandomizeBoard();

            _compAI = new ComputerAI(_computerBoard, _playerBoard);

            WireEvents();

            winnerLabel.Visible = false;

            RefreshPlayerBoards();
        }

        private void hideShowButton_Click(object sender, EventArgs e)
        {
            shipsBox2.Visible = !shipsBox2.Visible;
        }

        private void clearBoardsButton_Click(object sender, EventArgs e)
        {
            _playerBoard = new PlayerBoard(_boardSize, GetShips(), 1);
            _computerBoard = new PlayerBoard(_boardSize, GetShips(), 2);
            _compAI = new ComputerAI(_computerBoard, _playerBoard);
            
            WireEvents();
           
            winnerLabel.Visible = false;
            shipSunkLabel.Visible = false;

            RefreshPlayerBoards();
        }

        private void randomizeComputerButton_Click(object sender, EventArgs e)
        {
            _computerBoard = new PlayerBoard(_boardSize, GetShips(), 2);
            _computerBoard.RandomizeBoard();
            _compAI = new ComputerAI(_computerBoard, _playerBoard);
          
            WireEvents();

            winnerLabel.Visible = false;

            RefreshPlayerBoards();
        }



        private void _computerBoard_ShipWasSunk(object sender, string e)
        {
            shipSunkLabel.Text = $"Player {e} was sunk!";
            shipSunkLabel.Visible = true;
        }

        private void _playerBoard_ShipWasSunk(object sender, string e)
        {
            shipSunkLabel.Text = $"Computer {e} was sunk!";
            shipSunkLabel.Visible = true;

        }
    }
}
