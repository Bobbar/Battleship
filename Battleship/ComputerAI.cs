using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Battleship
{
    public class ComputerAI
    {
        private Random _rnd = new Random();
        private PlayerBoard _board;
        private PlayerBoard _humanBoard;
        private bool _lastShotWasHit = false;
        private bool _lastFollowShotWasHit = false;
        private bool _directionFound { get { return _hitsInARow >= 2; } }
        private bool _returnedToFirst = false;
        private int _hitsInARow = 0;
        private bool _isOnShip = false;
        private ShotCell _lastHit = new ShotCell();
        private ShotCell _firstHit = new ShotCell();

        private int _prevShipsSunk = 0;
        private Direction _followDir = Direction.Up;

        public ComputerAI(PlayerBoard board, PlayerBoard humanBoard)
        {
            _board = board;
            _humanBoard = humanBoard;
        }

        public bool TakeShot()
        {
            bool wasHit = false;
            if (!_isOnShip)
            {
                var shotCell = GetRandomShot();
                wasHit = _board.TakeShot(shotCell, _humanBoard);

                if (wasHit)
                {
                    _firstHit = shotCell;
                    _lastHit = shotCell;
                    _isOnShip = true;
                    _lastShotWasHit = true;
                    _hitsInARow = 1;
                }
                else
                {
                    _isOnShip = false;
                    _lastShotWasHit = false;
                    _hitsInARow = 0;
                }
            }
            else
            {
                if (!_directionFound)
                {
                    var dirs = GetAvailableDirections(_lastHit);

                    if (dirs.Length > 0)
                    {
                        _followDir = dirs[_rnd.Next(0, dirs.Length)];

                    }
                    else
                    {
                        // No direction possible? Reset? Return to first?
                        Debug.WriteLine("No shots possible?");
                        _lastHit = _firstHit;
                        _followDir = Helpers.FlipDirection(_followDir);
                        return TakeShot();
                    }

                    var followShot = GetNextCellInDirection(_lastHit, _followDir);
                    wasHit = _board.TakeShot(followShot, _humanBoard);

                    if (wasHit)
                    {
                        _lastHit = followShot;
                        _hitsInARow++;

                        if (SunkShip())
                        {
                            _isOnShip = false;
                            _hitsInARow = 0;
                        }
                    }
                }
                else
                {
                    if (CanMoveInDirection(_lastHit, _followDir))
                    {
                        var followShot = GetNextCellInDirection(_lastHit, _followDir);
                        wasHit = _board.TakeShot(followShot, _humanBoard);

                        if (wasHit)
                        {
                            _lastHit = followShot;
                            _hitsInARow++;

                            if (SunkShip())
                            {
                                _isOnShip = false;
                                _hitsInARow = 0;
                            }
                        }
                        else
                        {
                            Debug.WriteLine("Missed follow shot. Now what?");
                            _lastHit = _firstHit;
                            _followDir = Helpers.FlipDirection(_followDir);
                        }
                    }
                    else
                    {
                        _hitsInARow = 0;
                    }
                }

            }  

            return wasHit;
        }

      
        private bool SunkShip()
        {
            if (_prevShipsSunk != _humanBoard.ShipsSunk)
            {
                _prevShipsSunk = _humanBoard.ShipsSunk;
                _isOnShip = false;
                _hitsInARow = 0;
                return true;
            }
            else
            {
                return false;
            }
        }

       
        private ShotCell GetRandomShot()
        {
            var shotCell = new ShotCell();
            shotCell.HasShot = true;

            while (shotCell.HasShot)
            {
                int rndIdx = _rnd.Next(0, _board.ShotCells.Length);
                shotCell = _board.ShotCells[rndIdx];
            }

            return shotCell;
        }

        private Direction[] GetAvailableDirections(ShotCell cell)
        {
            var dirs = new List<Direction>();

            for (int i = 0; i < 4; i++)
            {
                var dir = (Direction)i;
                if (CanMoveInDirection(cell, dir))
                    dirs.Add(dir);
            }

            return dirs.ToArray();
        }


        private Direction GetNextDirection(Direction direction)
        {
            int dirIdx = (int)direction;
            if (dirIdx == 3)
                dirIdx = 0;
            else
                dirIdx++;

            return (Direction)dirIdx;


            //var newDir = Helpers.GetRandomDirection();
            //while (newDir == _followDir)
            //    newDir = Helpers.GetRandomDirection();
            //return newDir;
        }

        //private Direction GetRandomDirection()
        //{
        //    return (Direction)_rnd.Next(0, 4);
        //}

        private bool CanMoveInDirection(ShotCell currentCell, Direction direction)
        {
            switch (direction)
            {
                case Direction.Down:
                    if (currentCell.Row + 1 > 9)
                        return false;
                    else
                    {
                        if (GetNextCellInDirection(currentCell, direction).HasShot)
                        {
                            return false;
                        }
                    }

                    return true;

                case Direction.Up:
                    if (currentCell.Row - 1 < 0)
                        return false;
                    else
                    {
                        if (GetNextCellInDirection(currentCell, direction).HasShot)
                        {
                            return false;
                        }
                    }
                    return true;

                case Direction.Left:
                    if (currentCell.Column - 1 < 0)
                        return false;
                    else
                    {
                        if (GetNextCellInDirection(currentCell, direction).HasShot)
                        {
                            return false;
                        }
                    }
                    return true;

                case Direction.Right:
                    if (currentCell.Column + 1 > 9)
                        return false;
                    else
                    {
                        if (GetNextCellInDirection(currentCell, direction).HasShot)
                        {
                            return false;
                        }
                    }
                    return true;
            }

            return false;
        }

        private ShotCell GetNextCellInDirection(ShotCell currentCell, Direction direction)
        {
            ShotCell nextCell = null;

            switch (direction)
            {

                case Direction.Down:
                    nextCell = Helpers.CellFromCoords(_board.ShotCells, currentCell.Row + 1, currentCell.Column) as ShotCell;
                    return nextCell;

                //if (nextCell == null)
                //{
                //    _followDir = GetNextDirection();
                //    return GetNextCellInDirection(currentCell);
                //}
                //else
                //{
                //    return nextCell;
                //}

                case Direction.Up:
                    nextCell = Helpers.CellFromCoords(_board.ShotCells, currentCell.Row - 1, currentCell.Column) as ShotCell;
                    return nextCell;
                //if (nextCell == null)
                //{
                //    _followDir = GetNextDirection();
                //    return GetNextCellInDirection(currentCell);
                //}
                //else
                //{
                //    return nextCell;
                //}

                case Direction.Left:
                    nextCell = Helpers.CellFromCoords(_board.ShotCells, currentCell.Row, currentCell.Column - 1) as ShotCell;
                    return nextCell;
                //if (nextCell == null)
                //{
                //    _followDir = GetNextDirection();
                //    return GetNextCellInDirection(currentCell);
                //}
                //else
                //{
                //    return nextCell;
                //}

                case Direction.Right:
                    nextCell = Helpers.CellFromCoords(_board.ShotCells, currentCell.Row, currentCell.Column + 1) as ShotCell;
                    return nextCell;
                    //if (nextCell == null)
                    //{
                    //    _followDir = GetNextDirection();
                    //    return GetNextCellInDirection(currentCell);
                    //}
                    //else
                    //{
                    //    return nextCell;
                    //}
            }

            return nextCell;
        }
    }
}
