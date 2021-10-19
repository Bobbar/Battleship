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
        private Direction? _followDir = Direction.Up;
        private BoardAnalyzer _boardAnalyzer;
      
        public ComputerAI(PlayerBoard board, PlayerBoard humanBoard)
        {
            _board = board;
            _humanBoard = humanBoard;

            _boardAnalyzer = new BoardAnalyzer(board, humanBoard);
        }

        public bool TakeShot()
        {
            if (_humanBoard.UnSunkShips.Length == 0)
                return false;

            bool wasHit = false;

            if (HasUnSunkHits() && !_directionFound)
                LookForUnSunkHits();

            // Not on ship, take shots in areas with the highest probability of containing a ship.
            if (!_isOnShip)
            {
                var shotCell = GetNextShot();
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
                    // Try to get best available directions based on the heat map.
                    var bestDir = GetBestDirectionFromHeatMap(_lastHit);

                    if (bestDir != null)
                    {
                        _followDir = bestDir;
                    }
                    else
                    {
                        // No direction possible.
                        // Reset and try again.
                        _directionFound = false;
                        _isOnShip = false;
                        return TakeShot();
                    }

                    // Take a shot in the found direction.
                    var followShot = _boardAnalyzer.GetNextCellInDirection(_lastHit, _followDir);
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
                    if (_boardAnalyzer.TryGetNextCellInDirection(_lastHit, _followDir, false, out ShotCell followShot))
                    {
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
                        if (_boardAnalyzer.CanMoveInDirection(_firstHit, Helpers.FlipDirection(_followDir)))
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

            return wasHit;
        }

        public ShotCell[] ShipProbabilityHeatMap(bool focusShip = true)
        {
            if (focusShip)
                return _boardAnalyzer.ComputeProbabilityHeatMap(_isOnShip ? _lastHit : null);
            else
                return _boardAnalyzer.ComputeProbabilityHeatMap(null);

        }

        private bool HasUnSunkHits()
        {
            var unsunkHits = _board.ShotCells.Where(c => c.IsHit && !c.IsOnSunkShip);
            if (unsunkHits.Count() > 0)
                return true;

            return false;
        }

        private void LookForUnSunkHits()
        {
            if (_isOnShip)
                return;

            var unsunkHits = _board.ShotCells.Where(c => c.IsHit && !c.IsOnSunkShip).ToArray();

            foreach (var cell in unsunkHits)
            {
                var bestDir = GetBestDirectionFromHeatMap(cell);

                if (bestDir != null)
                {
                    _followDir = bestDir;
                    _lastHit = cell;
                    _firstHit = cell;
                    _isOnShip = true;
                    _directionFound = true;
                    return;
                }
            }
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

        private ShotCell GetNextShot()
        {
            var allCells = ShipProbabilityHeatMap();
            var cells = allCells.AsEnumerable().Where(c => c.HasShot == false).OrderBy(c => c.Rank).ToArray();

            // Find the max rank, and include only cells matching the max rank.
            var maxRank = cells.Max(c => c.Rank);
            cells = cells.Where(c => c.Rank == maxRank).ToArray();

            // Pick a random cell.
            int rndIdx = _rnd.Next(cells.Length);

            return cells[rndIdx];
        }

        private Direction? GetBestDirectionFromHeatMap(ShotCell cell)
        {
            var heatMap = ShipProbabilityHeatMap(focusShip: true); // Get probabilities for ships at the current ship cell.
            var heatMapAll = ShipProbabilityHeatMap(focusShip: false); // All other probabilities.

            // Filter out sunk ship cells.
            heatMap = heatMap.AsEnumerable().Where(c => !c.IsOnSunkShip).ToArray();
            heatMapAll = heatMapAll.AsEnumerable().Where(c => !c.IsOnSunkShip).ToArray();

            // Overlay ranks from all cells so that the decision is influenced by the current board state.
            for (int i = 0; i < heatMap.Length; i++)
            {
                heatMap[i].Rank += heatMapAll[i].Rank;
            }

            int bestRank = int.MinValue;
            Direction bestDirection = Direction.Down;

            for (int i = 0; i < 4; i++)
            {
                var dir = (Direction)i;
                if (_boardAnalyzer.TryGetNextCellInDirection(cell, dir, false, out ShotCell next))
                {
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
                return bestDirection;
            else
                return null;
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
    }
}
