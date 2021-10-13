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
        private bool _directionFound = false;
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

            if (HasUnSunkHits() && !_directionFound)
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
                    _directionFound = false;
                }
                else
                {
                    _isOnShip = false;
                    _directionFound = false;
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
                        _directionFound = false;
                        _isOnShip = false;
                        return TakeShot();
                    }

                    // Take a shot in the random direction.
                    var followShot = GetNextCellInDirection(_lastHit, _followDir);
                    wasHit = _board.TakeShot(followShot, _humanBoard);

                    if (wasHit)
                    {
                        _lastHit = followShot;
                        _directionFound = true;

                        if (SunkShip())
                        {
                            _isOnShip = false;
                            _directionFound = false;
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
                                _directionFound = false;
                            }
                            else
                            {
                                _lastHit = followShot;
                                _directionFound = true;
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
                            _directionFound = false;
                            return TakeShot();
                        }
                    }
                }
            }

            SunkShip();
            return wasHit;
        }

        private bool HasUnSunkHits()
        {
            foreach (var shot in _board.ShotCells)
            {
                if (shot.IsHit && !shot.IsOnSunkShip)
                    return true;
            }

            return false;
        }

        private ShotCell[] ShipProbabilityHeatMap()
        {
            var cells = new Dictionary<int, ShotCell>();

            foreach (var shotCell in _board.ShotCells)
            {
                if (!cells.ContainsKey(shotCell.Index)) 
                {
                    shotCell.Rank = 0;
                    cells.Add(shotCell.Index, shotCell);
                }
            }

            foreach (var shotCell in _board.ShotCells)
            {
                foreach (var ship in _humanBoard.UnSunkShips)
                {
                    for (int d = 0; d < 4; d++)
                    {
                        var dir = (Direction)d;

                        if (ShipCanFit(shotCell, ship, dir))
                        {
                            var shipCells = GetCellsInDirection(shotCell, dir);

                            foreach (var shipCell in shipCells)
                            {
                                cells[shipCell.Index].Rank++;
                            }
                        }
                        else
                        {
                            cells[shotCell.Index].Rank--;
                        }
                    }
                }
            }

            var cellRanks = cells.Values.Where(c => c.HasShot == false).ToArray();
            cellRanks = cellRanks.OrderBy(i => i.Rank).ToArray();
            return cellRanks;
        }

        private bool ShipCanFit(ShotCell cell, Ship ship, Direction direction)
        {
            if (cell.HasShot)
                return false;

            var next = cell;
            for (int i = 0; i < ship.Length - 1; i++)
            {
                next = GetNextCellInDirection(next, direction);
                if (next == null)
                    return false;

                if (next.HasShot)
                    return false;
            }

            return true;
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
                        _directionFound = true;
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

                            //return dir;
                            return Helpers.FlipDirection(dir);
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
                _directionFound = false;
                return true;
            }
            else
            {
                return false;
            }
        }

        private ShotCell GetRandomShot()
        {
            //if (_board.ShotsTaken <= 30)
            //    return GetRandomShot2();
            //else
            return GetRandomShot1();
        }

        private ShotCell GetRandomShot1()
        {
            var cells = ShipProbabilityHeatMap();

            int rndIdx = _rnd.Next(cells.Length - 3, cells.Length);

            return cells[rndIdx];
            //return cells.Last().Item2;
        }

        //private ShotCell GetRandomShot2()
        //{
        //    var possibleCells = GetPossibleShipCells();
        //    int rndIdx = _rnd.Next(0, possibleCells.Length);
        //    var possibleCell = possibleCells[rndIdx];
        //    var shotCell = possibleCell.Cell;

        //    // Does this really have an effect?
        //    //_followDir = possibleCell.Direction;

        //    return shotCell;
        //}

        private Direction[] GetAvailableDirections(ShotCell cell)
        {
            var dirs = new List<Direction>();

            var heatMap = ShipProbabilityHeatMap();

            int bestRank = int.MinValue;
            Direction bestDirection = Direction.Down;

            for (int i = 0; i < 4; i++)
            {
                var dir = (Direction)i;
                if (CanMoveInDirection(cell, dir))
                {
                    var next = GetNextCellInDirection(cell, dir);

                    foreach (var c in heatMap)
                    {
                        if (c.Index == next.Index)
                        {
                            if (c.Rank > bestRank)
                            {
                                bestRank = c.Rank;
                                bestDirection = dir;
                            }
                        }
                    }
                }
            }

            if (bestRank > int.MinValue)
                dirs.Add(bestDirection);

            return dirs.ToArray();
        }

        ///// <summary>
        ///// Find a collection of cells and directions where an unsunken ship could possibly be located.
        ///// </summary>
        ///// <returns></returns>
        //private PossibleShipCell[] GetPossibleShipCells()
        //{
        //    //var possibleCells = new List<PossibleShipCell>();
        //    var possibleCells = new Dictionary<int, PossibleShipCell>();

        //    var unSunkShips = _humanBoard.UnSunkShips;
        //    var emptyCells = _board.ShotCells.Where(cell => cell.HasShot == false).ToArray();
        //    foreach (var ship in unSunkShips)
        //    {
        //        for (int i = 0; i < 4; i++)
        //        {
        //            var dir = (Direction)i;
        //            foreach (var cell in emptyCells)
        //            {
        //                var cells = GetCellsInDirection(cell, dir);
        //                if (cells.Length >= ship.Length)
        //                {
        //                    // If the ship is longer than 3 cells, try to pick a cell in the middle of the group of possible cells.
        //                    // That's what I would do... ;)
        //                    if (cells.Length >= 3)
        //                    {
        //                        var pcell = cells[cells.Length / 2];
        //                        if (possibleCells.ContainsKey(pcell.Index))
        //                            possibleCells[pcell.Index].Directions.Add(dir);
        //                        else
        //                            possibleCells.Add(pcell.Index, new PossibleShipCell(pcell, dir));
        //                    }
        //                    else
        //                    {
        //                        if (possibleCells.ContainsKey(cell.Index))
        //                            possibleCells[cell.Index].Directions.Add(dir);
        //                        else
        //                            possibleCells.Add(cell.Index, new PossibleShipCell(cell, dir));
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return possibleCells.Values.ToArray();
        //}

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

            //while (next != null && !next.HasShot)
            while (next != null)
            {
                cells.Add(next);
                if (CanMoveInDirection(next, direction))
                    next = GetNextCellInDirection(next, direction);
                else
                    break;
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

        public void Test()
        {
            for (int i = 0; i < _board.ShotCells.Length; i++)
            {
                var shotCell = _board.ShotCells[i];

                //if (shotCell.Index % 2 == 0)
                if (_rnd.Next(0, 2) > 0)
                {

                    if (!_humanBoard.ShipCells[shotCell.Index].HasShip && !shotCell.HasShot)
                        _board.TakeShot(shotCell, _humanBoard);
                }

            }
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

    public class PossibleShipCell
    {
        public ShotCell Cell { get; set; }
        public HashSet<Direction> Directions { get; set; } = new HashSet<Direction>();

        public PossibleShipCell(ShotCell cell, Direction direction)
        {
            Cell = cell;
            Directions.Add(direction);
        }
    }
}
