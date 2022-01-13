using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Battleship
{
    public class BoardAnalyzer
    {
        private PlayerBoard _ourBoard;
        private PlayerBoard _opponentBoard;
        private int _prevShotsTaken = 0;
        private ShotCell[] _prevHeatMap;
        public BoardAnalyzer(PlayerBoard ourBoard, PlayerBoard opponentBoard)
        {
            _ourBoard = ourBoard;
            _opponentBoard = opponentBoard;
        }

        /// <summary>
        /// Computes the probabilities for each cell containing any remaining ships.
        /// </summary>
        /// <param name="focusCell">Optional. If specified, the probabilities will only be computed for ships potentially located within the specified cell.</param>
        /// <returns></returns>
        public ShotCell[] ComputeProbabilityHeatMap(ShotCell focusCell = null)
        {
            // Return cached heat map if applicable.
            if (_prevShotsTaken > 0 && _prevShotsTaken == _ourBoard.ShotsTaken)
                return _prevHeatMap;

            // Reset ranks.
            foreach (var shotCell in _ourBoard.ShotCells)
                shotCell.Rank = 0;

            if (focusCell != null)
            {
                foreach (var ship in _opponentBoard.UnSunkShips)
                {
                    for (int d = 0; d < 4; d++)
                    {
                        var dir = (Direction)d;

                        if (TryGetShipCells(focusCell, ship, dir, out ShotCell[] shipCells))
                        {
                            if (shipCells.Length != ship.Length)
                                continue;

                            foreach (var shipCell in shipCells)
                                shipCell.Rank++;
                        }
                    }
                }
            }
            else
            {
                foreach (var shotCell in _ourBoard.ShotCells)
                {
                    foreach (var ship in _opponentBoard.UnSunkShips)
                    {
                        for (int d = 0; d < 4; d++)
                        {
                            var dir = (Direction)d;

                            if (TryGetShipCells(shotCell, ship, dir, out ShotCell[] shipCells))
                            {
                                if (shipCells.Length != ship.Length)
                                    continue;

                                foreach (var shipCell in shipCells)
                                    shipCell.Rank++;
                            }
                        }
                    }
                }
            }

            var heatMap = _ourBoard.ShotCells;
            _prevHeatMap = heatMap;
            _prevShotsTaken = _ourBoard.ShotsTaken;

            return heatMap;
        }

        public bool TryGetNextCellInDirection(ShotCell currentCell, Direction? direction, bool ignoreShots, out ShotCell nextCell)
        {
            if (CanMoveInDirection(currentCell, direction, ignoreShots))
            {
                nextCell = GetNextCellInDirection(currentCell, direction);
                return true;
            }

            nextCell = null;
            return false;
        }

        public ShotCell GetNextCellInDirection(ShotCell currentCell, Direction? direction)
        {
            ShotCell nextCell = null;

            switch (direction)
            {
                case Direction.Down:
                    nextCell = Helpers.CellFromCoords(_ourBoard.ShotCells, currentCell.Row + 1, currentCell.Column) as ShotCell;
                    return nextCell;

                case Direction.Up:
                    nextCell = Helpers.CellFromCoords(_ourBoard.ShotCells, currentCell.Row - 1, currentCell.Column) as ShotCell;
                    return nextCell;

                case Direction.Left:
                    nextCell = Helpers.CellFromCoords(_ourBoard.ShotCells, currentCell.Row, currentCell.Column - 1) as ShotCell;
                    return nextCell;

                case Direction.Right:
                    nextCell = Helpers.CellFromCoords(_ourBoard.ShotCells, currentCell.Row, currentCell.Column + 1) as ShotCell;
                    return nextCell;
            }

            return nextCell;
        }

        public bool IsSurrounded(ShotCell cell)
        {
            bool surrounded = true;
            for (int d = 0; d < 4; d++)
            {
                var dir = (Direction)d;
                if (CanMoveInDirection(cell, dir, ignoreShots: false))
                    surrounded = false;
            }

            return surrounded;
        }

        /// <summary>
        /// Is the next cell in the specified direction possible?
        /// </summary>
        /// <param name="currentCell"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public bool CanMoveInDirection(ShotCell currentCell, Direction? direction, bool ignoreShots = false)
        {
            switch (direction)
            {
                case Direction.Down:
                    if (currentCell.Row + 1 > 9)
                        return false;
                    else
                    {
                        if (!ignoreShots && GetNextCellInDirection(currentCell, direction).HasShot)
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
                        if (!ignoreShots && GetNextCellInDirection(currentCell, direction).HasShot)
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
                        if (!ignoreShots && GetNextCellInDirection(currentCell, direction).HasShot)
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
                        if (!ignoreShots && GetNextCellInDirection(currentCell, direction).HasShot)
                        {
                            return false;
                        }
                    }
                    return true;
            }

            return false;
        }

        public bool ShipCanFit(ShotCell cell, Ship ship, Direction direction)
        {
            if (cell.HasShot && cell.IsOnSunkShip)
                return false;

            if (cell.HasShot && !cell.IsHit)
                return false;

            var next = cell;
            for (int i = 0; i < ship.Length - 1; i++)
            {
                if (!TryGetNextCellInDirection(next, direction, ignoreShots: true, out next))
                    return false;

                if (next.HasShot && next.IsOnSunkShip)
                    return false;

                if (next.HasShot && !next.IsHit)
                    return false;
            }

            return true;
        }

        public bool TryGetShipCells(ShotCell cell, Ship ship, Direction direction, out ShotCell[] shipCells)
        {
            shipCells = null;

            if (cell.HasShot && cell.IsOnSunkShip)
                return false;

            if (cell.HasShot && !cell.IsHit)
                return false;

            var cells = GetCellsInDirectionConstrained(cell, direction, ship.Length);

            if (cells.Any(c => c.HasShot && c.IsOnSunkShip || c.HasShot && !c.IsHit))
                return false;

            //if (cells.Any(c => c.HasShot && !c.IsHit))
            //    return false;
            shipCells = cells;
            return true;
        }

        private ShotCell[] GetCellsInDirectionConstrained(ShotCell cell, Direction direction, int num)
        {
            var cells = new List<ShotCell>();
            int count = 0;
            var next = cell;

            while (next != null && count < num)
            {
                cells.Add(next);

                if (!TryGetNextCellInDirection(next, direction, ignoreShots: true, out next))
                    break;

                count++;
            }

            return cells.ToArray();
        }

        public ShotCell[] GetCellsInDirection(ShotCell cell, Direction direction)
        {
            var cells = new List<ShotCell>();
            int count = 0;
            var next = cell;

            while (next != null)
            {
                cells.Add(next);

                if (!TryGetNextCellInDirection(next, direction, ignoreShots: false, out next))
                    break;

                count++;
            }

            return cells.ToArray();
        }
    }
}
