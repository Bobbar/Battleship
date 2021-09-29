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
        private bool _directionFound { get { return _hitsInARow >= 2; } }
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

            // Try to find hits from unsunk ships, and set parameters to try to focus on them.
            LookForUnfinishedBusiness();

            // Not on ship, take random shots.
            if (!_isOnShip)
            {
                var shotCell = GetRandomShot();
                wasHit = _board.TakeShot(shotCell, _humanBoard);

                if (wasHit)
                {
                    _firstHit = shotCell;
                    _lastHit = shotCell;
                    _isOnShip = true;
                    _hitsInARow = 1;
                }
                else
                {
                    _isOnShip = false;
                    _hitsInARow = 0;
                }
            }
            else // We've found a ship.
            {
                // If we don't think we have found the direction the ship is pointed.
                if (!_directionFound)
                {
                    // Get available directions and pick one at random.
                    var dirs = GetAvailableDirections(_lastHit);

                    if (dirs.Length > 0)
                    {
                        _followDir = dirs[_rnd.Next(0, dirs.Length)];
                    }
                    else
                    {
                        // No directions possible.
                        // Reset and try again.
                        _hitsInARow = 0;
                        _isOnShip = false;

                        return TakeShot();
                    }

                    // Take a shot in the random direction.
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
                else // We may have found the direction of the ship.
                {
                    // Make sure we can keep moving in the current direction.
                    if (CanMoveInDirection(_lastHit, _followDir))
                    {
                        // Take the next shot.
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
                            // Follow shot missed.
                            // Go back to first hit cell and flip direction.
                            _lastHit = _firstHit;
                            _followDir = Helpers.FlipDirection(_followDir);
                        }
                    }
                    else // Can't take shot in the current direction.
                    {
                        // See if we can move back to the first hit and move in the opposite direction.
                        if (CanMoveInDirection(_firstHit, Helpers.FlipDirection(_followDir)))
                        {
                            // Move to first hit, and flip directions.
                            _lastHit = _firstHit;
                            _followDir = Helpers.FlipDirection(_followDir);
                            return TakeShot();
                        }
                        else
                        {
                            // Reset start from scratch.
                            _hitsInARow = 0;
                            return TakeShot();
                        }
                    }
                }
            }

            return wasHit;
        }

        private void LookForUnfinishedBusiness()
        {
            if (_isOnShip)
                return;

            var sunkShips = _humanBoard.SunkShips;

            for (int i = 0; i < _board.ShotCells.Length; i++)
            {
                var cell = _board.ShotCells[i];

                var dirs = GetAvailableDirections(cell);

                if (cell.IsHit && dirs.Length > 0)
                {
                    bool isOnShip = false;
                    foreach (var ship in sunkShips)
                    {
                        if (IsOnSunkShip(cell, ship.Cells))
                        {
                            isOnShip = true;
                        }
                    }

                    if (!isOnShip)
                    {
                        // We found a viable cell.
                        // Set the parameters accordingly to force shots around the cell.
                        _lastHit = cell;
                        _firstHit = cell;
                        _isOnShip = true;
                        _hitsInARow = 1;
                        return;
                    }
                }
            }
        }

        private bool IsOnSunkShip(ShotCell shotCell, ShipCell[] sunkShipCells)
        {
            bool isOn = false;
            foreach (var shipCell in sunkShipCells)
            {
                if (shipCell.Row == shotCell.Row && shipCell.Column == shotCell.Column)
                    isOn = true;

            }

            return isOn;
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

            bool viableShot = false;
            while (viableShot == false)
            {
                int rndIdx = _rnd.Next(0, _board.ShotCells.Length);
                shotCell = _board.ShotCells[rndIdx];

                // Check available directions and only choose cells which have atleast one open neighbor cell. 
                // No sense wasting shots on cells that couldn't possibly contain a ship.
                var dirs = GetAvailableDirections(shotCell);
                if (!shotCell.HasShot && dirs.Length > 0)
                    viableShot = true;
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
        }

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

                case Direction.Up:
                    nextCell = Helpers.CellFromCoords(_board.ShotCells, currentCell.Row - 1, currentCell.Column) as ShotCell;
                    return nextCell;

                case Direction.Left:
                    nextCell = Helpers.CellFromCoords(_board.ShotCells, currentCell.Row, currentCell.Column - 1) as ShotCell;
                    return nextCell;

                case Direction.Right:
                    nextCell = Helpers.CellFromCoords(_board.ShotCells, currentCell.Row, currentCell.Column + 1) as ShotCell;
                    return nextCell;
            }

            return nextCell;
        }
    }
}
