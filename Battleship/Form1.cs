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
        private Pen _cellPen = new Pen(Brushes.Black, 1.0f);
        private SolidBrush _hitBrush = new SolidBrush(Color.Red);
        private SolidBrush _missBrush = new SolidBrush(Color.White);
        private SolidBrush _shipBrush = new SolidBrush(Color.LightGray);
        private SolidBrush _invalidShipBrush = new SolidBrush(Color.Yellow);
        private SolidBrush _sunkShipBrush = new SolidBrush(Color.Blue);
        private SolidBrush _emptyPegBrush = new SolidBrush(Color.FromArgb(150, Color.Gray));
        private const int _pegSize = 30;
        //private Font _cellCoordFont = new Font("Tahoma", (_pegSize / 3), FontStyle.Regular);
        private Font _cellCoordFont = new Font("Tahoma", 10, FontStyle.Regular);

        private Size _boardSize;
        private Random _rnd = new Random();

        private PlayerBoard _playerBoard;
        private PlayerBoard _computerBoard;

        private ComputerAI _compAI;
        private ComputerAI _compAI2;

        private bool _drawCoords = false;
        private bool _drawHeatMap = false;
        private bool _compVsComp = false;

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
            _compAI2 = new ComputerAI(_computerBoard, _playerBoard);
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


        private void DrawShotsBoard(Graphics gfx, PlayerBoard board, PlayerBoard otherBoard)
        {
            gfx.SmoothingMode = SmoothingMode.HighQuality;
            gfx.Clear(shotsBox.BackColor);

            foreach (var ship in otherBoard.Ships)
                DrawShip(gfx, ship, true);

            foreach (var cell in board.ShotCells)
            {
                gfx.DrawPolygon(_cellPen, cell.CellBox);

                DrawPeg(gfx, cell);

                if (_drawCoords)
                {
                    // Draw coords.
                    var center = Helpers.CenterOfPolygon(cell.CellBox);
                    string label = $"({cell.Column}/{cell.Row})";
                    var lblSize = gfx.MeasureString(label, _cellCoordFont);
                    center.X -= (int)(lblSize.Width / 2f);
                    center.Y -= (int)(lblSize.Height / 2f);
                    gfx.DrawString(label, _cellCoordFont, Brushes.Black, center);
                }

            }
        }

        private void DrawHeatMap(Graphics gfx, Cell[] cells)
        {
            int max = int.MinValue;

            foreach (var c in cells)
                max = Math.Max(max, c.Rank);

            foreach (var cell in cells)
            {
                var center = Helpers.CenterOfPolygon(cell.CellBox);
                var probColor = Helpers.GetVariableColor(Color.Blue, Color.Red, Color.Yellow, max, cell.Rank, 255, true);
                gfx.FillEllipse(new SolidBrush(probColor), center.X - _pegSize / 2, center.Y - _pegSize / 2, _pegSize, _pegSize);

                if (_drawCoords)
                {
                    string label = $"{cell.Rank}";
                    var lblSize = gfx.MeasureString(label, _cellCoordFont);
                    gfx.DrawString(label, _cellCoordFont, Brushes.Black, center.X - (int)(lblSize.Width / 2f), center.Y - (int)(lblSize.Height));

                    label = $"({cell.Column}/{cell.Row})";
                    lblSize = gfx.MeasureString(label, _cellCoordFont);
                    gfx.DrawString(label, _cellCoordFont, Brushes.Black, center.X - (int)(lblSize.Width / 2f), center.Y);
                }
                else
                {
                    string label = $"{cell.Rank}";
                    var lblSize = gfx.MeasureString(label, _cellCoordFont);
                    center.X -= (int)(lblSize.Width / 2f);
                    center.Y -= (int)(lblSize.Height / 2f);
                    gfx.DrawString(label, _cellCoordFont, Brushes.Black, center);
                }
            }
        }

        private void DrawShipsBoard(Graphics gfx, PlayerBoard board)
        {
            gfx.SmoothingMode = SmoothingMode.HighQuality;
            gfx.Clear(shipsBox.BackColor);

            foreach (var ship in board.Ships)
                DrawShip(gfx, ship);

            foreach (var cell in board.ShipCells)
            {
                DrawShipCell(gfx, cell, board);

                gfx.DrawPolygon(_cellPen, cell.CellBox);

                DrawPeg(gfx, cell);

                if (_drawCoords)
                {
                    // Draw coords.
                    var center = Helpers.CenterOfPolygon(cell.CellBox);
                    string label = $"({cell.Column}/{cell.Row})";
                    var lblSize = gfx.MeasureString(label, _cellCoordFont);
                    center.X -= (int)(lblSize.Width / 2f);
                    center.Y -= (int)(lblSize.Height / 2f);
                    gfx.DrawString(label, _cellCoordFont, Brushes.Black, center);
                }
            }
        }

        private void DrawPeg(Graphics gfx, ShotCell cell)
        {
            var center = Helpers.CenterOfPolygon(cell.CellBox);
            center.X -= _pegSize / 2;
            center.Y -= _pegSize / 2;

            if (cell.IsHit)
            {
                gfx.DrawEllipse(Pens.Black, center.X, center.Y, _pegSize, _pegSize);
                gfx.FillEllipse(_hitBrush, center.X, center.Y, _pegSize, _pegSize);
                return;
            }

            if (cell.HasShot)
            {
                gfx.DrawEllipse(Pens.Black, center.X, center.Y, _pegSize, _pegSize);
                gfx.FillEllipse(_missBrush, center.X, center.Y, _pegSize, _pegSize);
            }
            else
            {
                gfx.DrawEllipse(Pens.Black, center.X, center.Y, _pegSize, _pegSize);
                gfx.FillEllipse(_emptyPegBrush, center.X, center.Y, _pegSize, _pegSize);
            }
        }

        private void DrawShip(Graphics gfx, Ship ship, bool shotBoard = false)
        {
            if (!ship.ValidPlacement())
                return;

            if (shotBoard && !ship.IsSunk)
                return;

            var rect = ship.GetRectangle();
            var rectPath = Helpers.RoundedRect(rect, 15);
            var brush = ship.IsSunk ? _sunkShipBrush : _shipBrush;

            if (shotBoard)
                brush = _hitBrush;

            gfx.FillPath(brush, rectPath);
        }

        private void DrawShipCell(Graphics gfx, ShipCell cell, PlayerBoard board)
        {
            if (cell.PlacingShip && board.PlacingShip)
            {
                gfx.FillPolygon(_shipBrush, cell.CellBox);
            }

            if (cell.Ship != null && !cell.Ship.ValidPlacement())
                gfx.FillPolygon(_invalidShipBrush, cell.CellBox);

        }

        private bool RandomHit()
        {
            var num = _rnd.Next(0, 2);

            if (num == 1)
                return true;
            else
                return false;
        }


        private void StressTest(int its)
        {
            //_playerBoard = new PlayerBoard(_boardSize, GetShips(), 1);
            //_playerBoard.RandomizeBoard();

            //_computerBoard = new PlayerBoard(_boardSize, GetShips(), 2);
            //_computerBoard.RandomizeBoard();

            //_compAI = new ComputerAI(_computerBoard, _playerBoard);

            var shotsTaken = new List<int>();
            int best = int.MaxValue;
            int worst = int.MinValue;

            var timer = new System.Diagnostics.Stopwatch();
            timer.Restart();

            for (int i = 0; i < its; i++)
            {
                _playerBoard = new PlayerBoard(_boardSize, GetShips(), 1);
                _playerBoard.RandomizeBoard();

                _computerBoard = new PlayerBoard(_boardSize, GetShips(), 2);
                _computerBoard.RandomizeBoard();

                _compAI = new ComputerAI(_computerBoard, _playerBoard);


                bool gameOver = false;

                while (gameOver == false)
                {
                    try
                    {
                        _compAI.TakeShot();
                    }
                    catch (Exception)
                    {

                        Debug.WriteLine("[AI] Clicked the same cell twice?");
                        return;
                    }


                    if (_playerBoard.IsDefeated())
                    {
                        gameOver = true;
                    }
                    else if (_computerBoard.IsDefeated())
                    {
                        gameOver = true;

                    }

                    //RefreshPlayerBoards();
                    //Application.DoEvents();
                    //Task.Delay(50).Wait();
                }

                //Debug.WriteLine($"[Game Over] Shots Taken: {_computerBoard.ShotsTaken}");
                shotsTaken.Add(_computerBoard.ShotsTaken);
                best = Math.Min(best, _computerBoard.ShotsTaken);
                worst = Math.Max(worst, _computerBoard.ShotsTaken);
                //Task.Delay(2000).Wait();
            }

            timer.Stop();
            System.Diagnostics.Debug.WriteLine(string.Format("Timer: {0} ms  {1} ticks", timer.Elapsed.TotalMilliseconds, timer.Elapsed.Ticks));


            float totShots = 0;
            foreach (var shots in shotsTaken)
                totShots += shots;

            float avgShots = totShots / shotsTaken.Count;
            Debug.WriteLine($"Games: {its} TotShots: {totShots}  AvgShots: {avgShots}  Best: {best}  Worst: {worst}");
            RefreshPlayerBoards();
        }

        private void shotsBox_Paint(object sender, PaintEventArgs e)
        {
            DrawShotsBoard(e.Graphics, _playerBoard, _computerBoard);

            if (_drawHeatMap && _compVsComp)
                DrawHeatMap(e.Graphics, _compAI2.ShipProbabilityHeatMap());
        }

        private void shipsBox_Paint(object sender, PaintEventArgs e)
        {
            DrawShipsBoard(e.Graphics, _playerBoard);
        }

        private void shotsBox2_Paint(object sender, PaintEventArgs e)
        {
            DrawShotsBoard(e.Graphics, _computerBoard, _playerBoard);

            if (_drawHeatMap)
                DrawHeatMap(e.Graphics, _compAI.ShipProbabilityHeatMap());

        }

        private void shipsBox2_Paint(object sender, PaintEventArgs e)
        {
            DrawShipsBoard(e.Graphics, _computerBoard);
        }

        private void shotsBox_MouseClick(object sender, MouseEventArgs e)
        {
            shipSunkLabel.Visible = false;

            try
            {
                _playerBoard.TakeShot(e.Location, _computerBoard);
                RefreshPlayerBoards();
            }
            catch (Exception)
            {
                Debug.WriteLine("[Human] Clicked the same cell twice?");
                return;
            }

            Task.Delay(500).Wait();

            try
            {
                _compAI.TakeShot();
                RefreshPlayerBoards();
            }
            catch (Exception)
            {

                Debug.WriteLine("[AI] Clicked the same cell twice?");
                return;
            }

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
            try
            {
                _computerBoard.TakeShot(e.Location, _playerBoard);
            }
            catch
            {

            }
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
            if (_compVsComp)
                _compAI2 = new ComputerAI(_playerBoard, _computerBoard);

            WireEvents();

            winnerLabel.Visible = false;
            shipSunkLabel.Visible = false;


            RefreshPlayerBoards();
        }

        private void hideShowButton_Click(object sender, EventArgs e)
        {
            shipsBox2.Visible = !shipsBox2.Visible;
            shotsBox2.Visible = !shotsBox2.Visible;

        }

        private void clearBoardsButton_Click(object sender, EventArgs e)
        {
            _playerBoard = new PlayerBoard(_boardSize, GetShips(), 1);
            _computerBoard = new PlayerBoard(_boardSize, GetShips(), 2);

            _compAI = new ComputerAI(_computerBoard, _playerBoard);
            if (_compVsComp)
                _compAI2 = new ComputerAI(_playerBoard, _computerBoard);

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
            if (_compVsComp)
                _compAI2 = new ComputerAI(_playerBoard, _computerBoard);

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

        private void button1_Click(object sender, EventArgs e)
        {
            StressTest(1000);
        }

        private void drawCoordsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            _drawCoords = drawCoordsCheckBox.Checked;
            RefreshPlayerBoards();
        }

        private void DrawHeatMapCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            _drawHeatMap = DrawHeatMapCheckBox.Checked;
            RefreshPlayerBoards();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _compAI.Test();
            RefreshPlayerBoards();
        }

        private void CompTakeShotButton_Click(object sender, EventArgs e)
        {
            try
            {
                _compAI.TakeShot();
                RefreshPlayerBoards();
            }
            catch (Exception)
            {

                Debug.WriteLine("[AI] Clicked the same cell twice?");
                return;
            }

            if (_compVsComp)
            {
                Task.Delay(500).Wait();

                try
                {
                    _compAI2.TakeShot();
                    RefreshPlayerBoards();
                }
                catch (Exception)
                {

                    Debug.WriteLine("[AI] Clicked the same cell twice?");
                    return;
                }
            }


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
    }
}
