﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;

namespace Battleship
{
    public class PlayerBoard
    {
        private const int _sideLen = 40;
        private ShotCell[] _shotCells;
        private ShipCell[] _shipCells;
        private Ship[] _ships;
        private int _currentShipIdx = 0;
        private bool _placingShip = true;
        private Size _boardSize;
        private int _playerNumber = -1;
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

        public ShotCell[] ShotCells { get { return _shotCells; } }
        public ShipCell[] ShipCells { get { return _shipCells; } }

        public bool PlacingShip { get { return _placingShip; } }
        public PlayerBoard(Size boardSize, Ship[] ships, int playerNumber)
        {
            _boardSize = boardSize;
            _ships = new Ship[ships.Length];
            Array.Copy(ships, _ships, ships.Length);
            _playerNumber = playerNumber;

            InitBoard();

        }

        private void InitBoard()
        {
            _shotCells = new ShotCell[100];
            _shipCells = new ShipCell[100];

            int n = 0;
            int row = 0;
            int column = 0;
            for (int x = 0; x <= _boardSize.Width - _sideLen; x += _sideLen)
            {
                row = 0;
                for (int y = 0; y <= _boardSize.Height - _sideLen; y += _sideLen)
                {
                    var cell = new Point[]
                    {
                        new Point(x,y), // Top-left
                        new Point(x + _sideLen, y), // Top-right
                        new Point(x + _sideLen, y + _sideLen), //Bot-right
                        new Point(x, y + _sideLen) // Bot-left
                    };

                    _shotCells[n] = new ShotCell(cell, row, column);
                    _shipCells[n] = new ShipCell(cell, row, column);

                    n++;
                    row++;
                }
                column++;
            }
        }

        public void PositionShip(Point location)
        {
            if (!_placingShip || _currentShip == null)
                return;

            ClearPlacedShip(_currentShip);

            var overCell = Helpers.CellFromPosition(_shipCells, location) as ShipCell;
            if (overCell == null)
                return;

            if (overCell.Ship == null || overCell.Ship.Name == _currentShip.Name)
            {
                overCell.Ship = _currentShip;
                overCell.PlacingShip = true;

            }

            int overCol = overCell.Column;
            int overRow = overCell.Row;
            _currentShip.Cells[0] = overCell;

            for (int i = 0; i < _currentShip.Length - 1; i++)
            {
                switch (_currentShip.Direction)
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

                var nextCell = Helpers.CellFromCoords(_shipCells, overRow, overCol) as ShipCell;
                if (nextCell != null)
                {
                    if (nextCell.Ship == null || nextCell.Ship.Name == _currentShip.Name)
                    {
                        nextCell.Ship = _currentShip;
                        nextCell.PlacingShip = true;
                    }
                }

                _currentShip.Cells[i + 1] = nextCell;
            }
        }


        public void PlaceShip(Point location)
        {
            if (_currentShip == null)
                return;

            if (!_currentShip.ValidPlacement())
                return;

            for (int i = 0; i < _currentShip.Cells.Length; i++)
                _currentShip.Cells[i].PlacingShip = false;

            NextShip();
        }

        public void RotateShip(int mDelta, Point location)
        {
            if (_currentShip == null)
                return;

            int curDir = (int)_currentShip.Direction;

            if (mDelta > 0) // Wheel UP
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

            PositionShip(location);
        }

        private void NextShip()
        {
            if (_currentShipIdx < _ships.Length - 1)
            {
                _currentShipIdx++;
            }
            else if (_currentShipIdx == _ships.Length - 1)
            {
                _placingShip = false;
                _currentShipIdx = -1;
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
        }

        public void TakeShot(Point location, PlayerBoard otherBoard)
        {
            var shotCell = Helpers.CellFromPosition(ShotCells, location) as ShotCell;
            if (shotCell == null)
                return;

            var shipCell = Helpers.CellFromCoords(otherBoard.ShipCells, shotCell.Row, shotCell.Column) as ShipCell;

            if (shipCell.HasShip)
            {
                shipCell.SetHit();
                shotCell.SetHit();
            }
            else
            {
                shipCell.SetMiss();
                shotCell.SetMiss();
            }

        }

        public bool IsDefeated()
        {
            foreach(var ship in _ships)
            {
                if (ship.IsSunk == false)
                    return false;
            }

            return true;
        }

    }
}
