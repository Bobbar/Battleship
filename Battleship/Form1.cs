﻿using System;
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

        private Size _boardSize;
        private Random _rnd = new Random();

        private PlayerBoard _playerBoard;
        private PlayerBoard _computerBoard;

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
                    if (cell.IsHit)
                        gfx.FillPolygon(_hitBrush, cell.CellBox);
                    else
                        gfx.FillPolygon(_missBrush, cell.CellBox);
                }

                gfx.DrawPolygon(_cellPen, cell.CellBox);
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
            _playerBoard.TakeShot(e.Location, _computerBoard);
            Debug.WriteLine($"Comp defeated: {_computerBoard.IsDefeated()}");
            RefreshPlayerBoards();
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
    }
}
