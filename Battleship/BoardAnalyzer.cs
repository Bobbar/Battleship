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
        /// 
        public ShotCell[] ComputeProbabilityHeatMap(Cell focusCell = null)
        {
            var cells = new Dictionary<int, ShotCell>();

            foreach (var shotCell in _ourBoard.ShotCells)
            {
                if (!cells.ContainsKey(shotCell.Index))
                {
                    shotCell.Rank = 0;
                    cells.Add(shotCell.Index, shotCell);
                }
            }

            foreach (var shotCell in _ourBoard.ShotCells)
            {
                foreach (var ship in _opponentBoard.UnSunkShips)
                {
                    for (int d = 0; d < 4; d++)
                    //for (int d = 1; d <= 2; d++) // Does it really make a difference to only compute for vertical & horizontal directions?
                    {
                        var dir = (Direction)d;

                        if (ShipCanFit(shotCell, ship, dir))
                        {
                            var shipCells = GetCellsInDirectionConstrained(shotCell, dir, ship.Length);

                            if (focusCell != null && shipCells.CellsContainIndex(focusCell.Index))
                            {
                                foreach (var shipCell in shipCells)
                                {
                                    cells[shipCell.Index].Rank++;
                                }
                            }
                            else if (focusCell == null)
                            {
                                foreach (var shipCell in shipCells)
                                {
                                    cells[shipCell.Index].Rank++;
                                }
                            }
                        }
                    }
                }
            }

            return cells.Values.ToArray();
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
                if (CanMoveInDirection(cell, dir, ignoreShots: false)) ;
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
