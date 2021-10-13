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
            if (_humanBoard.UnSunkShips.Length == 0)
                return false;

            bool wasHit = false;

            // Try to find hits from unsunk ships, and set parameters to try to focus on them.
            if (!_directionFound)
                LookForUnfinishedBusiness();

            // Not on ship, take random shots in areas that could fit any of the remaining ships.
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
                            if (SunkShip())
                            {
                                _isOnShip = false;
                                _hitsInARow = 0;
                            }
                            else
                            {
                                _lastHit = followShot;
                                _hitsInARow++;
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

        /// <summary>
        /// Look for cells with hits on ships that haven't been sunk yet and set parameters to focus on them.
        /// </summary>
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
                        _followDir = FindHitDirection(cell);
                        _lastHit = cell;
                        _firstHit = cell;
                        _isOnShip = true;
                        _hitsInARow = 2;
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Try to find hits on unsunk ships and determine the most likely direction to try next.
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        private Direction FindHitDirection(ShotCell cell)
        {
            for (int i = 0; i < 4; i++)
            {
                var dir = (Direction)i;

                if (CanMoveInDirection(cell, dir))
                {
                    var next = GetNextCellInDirection(cell, dir);
                    foreach (var sunkShip in _humanBoard.SunkShips)
                    {
                        if (next != null && next.IsHit && !IsOnSunkShip(next, sunkShip.Cells))
                        {
                            // Should we flip or not???
                            // Doesn't seem to make much of a difference.

                            return dir;
                            //return Helpers.FlipDirection(dir);
                        }
                    }
                }
            }

            var dirs = GetAvailableDirections(cell);
            return dirs[_rnd.Next(0, dirs.Length)];

        }

        /// <summary>
        /// Is the specified cell on a sunken ship?
        /// </summary>
        /// <param name="shotCell"></param>
        /// <param name="sunkShipCells"></param>
        /// <returns></returns>
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
            var possibleCells = GetPossibleShipCells();
            int rndIdx = _rnd.Next(0, possibleCells.Length);
            var possibleCell = possibleCells[rndIdx];
            var shotCell = possibleCell.Cell;

            // Does this really have an effect?
            //_followDir = possibleCell.Direction;

            return shotCell;
        }

        /// <summary>
        /// Find all possible directions from the specified cell.
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Find a collection of cells and directions where an unsunken ship could possibly be located.
        /// </summary>
        /// <returns></returns>
        private PossibleShipCell[] GetPossibleShipCells()
        {
            var possibleCells = new List<PossibleShipCell>();
            var unSunkShips = _humanBoard.UnSunkShips;
            var emptyCells = _board.ShotCells.Where(cell => cell.HasShot == false).ToArray();
            foreach (var ship in unSunkShips)
            {
                for (int i = 0; i < 4; i++)
                {
                    var dir = (Direction)i;
                    foreach (var cell in emptyCells)
                    {
                        // TODO: This seems to return more cells than expected.....
                        // It's taking shots in areas too small to contain the last ship.
                        var cells = GetCellsInDirection(cell, dir);
                        if (cells.Length >= ship.Length)
                        {
                            // If the ship is longer than 3 cells, try to pick a cell in the middle of the group of possible cells.
                            // That's what I would do... ;)
                            if (cells.Length >= 3)
                                possibleCells.Add(new PossibleShipCell(cells[cells.Length / 2], dir));
                            else
                                possibleCells.Add(new PossibleShipCell(cell, dir));
                        }
                    }
                }
            }

            return possibleCells.ToArray();
        }

        /// <summary>
        /// Find all cells between the specified cell and the next hit/wall in the specified direction.
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        private ShotCell[] GetCellsInDirection(ShotCell cell, Direction direction)
        {
            var cells = new List<ShotCell>();
            var next = cell;

            while (next != null && !next.HasShot)
            {
                cells.Add(next);
                next = GetNextCellInDirection(next, direction);
            }

            return cells.ToArray();
        }

        /// <summary>
        /// Is the next cell in the specified direction possible?
        /// </summary>
        /// <param name="currentCell"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
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

        //private ShotCell GetNextCellInDirection(ShotCell currentCell, Direction direction)
        //{
        //    ShotCell nextCell = null;

        //    var emptyCells = _board.ShotCells.Where(cell => cell.HasShot == false).ToArray();

        //    switch (direction)
        //    {

        //        case Direction.Down:
        //            nextCell = Helpers.CellFromCoords(emptyCells, currentCell.Row + 1, currentCell.Column) as ShotCell;
        //            return nextCell;

        //        case Direction.Up:
        //            nextCell = Helpers.CellFromCoords(emptyCells, currentCell.Row - 1, currentCell.Column) as ShotCell;
        //            return nextCell;

        //        case Direction.Left:
        //            nextCell = Helpers.CellFromCoords(emptyCells, currentCell.Row, currentCell.Column - 1) as ShotCell;
        //            return nextCell;

        //        case Direction.Right:
        //            nextCell = Helpers.CellFromCoords(emptyCells, currentCell.Row, currentCell.Column + 1) as ShotCell;
        //            return nextCell;
        //    }

        //    return nextCell;
        //}

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

    public class PossibleShipCell
    {
        public ShotCell Cell { get; set; }
        public Direction Direction { get; set; }

        public PossibleShipCell(ShotCell cell, Direction direction)
        {
            Cell = cell;
            Direction = direction;
        }
    }
}
