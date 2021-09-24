using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship
{
    public class ComputerAI
    {
        private Random _rnd = new Random();
        private PlayerBoard _board;
        private PlayerBoard _humanBoard;

        public ComputerAI(PlayerBoard board, PlayerBoard humanBoard)
        {
            _board = board;
            _humanBoard = humanBoard;
        }

        public void TakeShot()
        {
            var shotCell = GetRandomShot();
            _board.TakeShot(shotCell, _humanBoard);
        }

        private ShotCell GetRandomShot()
        {
            var shotCell = new ShotCell();
            shotCell.HasShot = true;

            while (shotCell.HasShot)
            {
                int rndIdx = _rnd.Next(0, _board.ShotCells.Length + 1);
                shotCell = _board.ShotCells[rndIdx];
            }

            return shotCell;
        }
    }
}
