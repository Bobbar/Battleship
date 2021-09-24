using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Battleship
{
    public static class Helpers
    {
        // https://wrf.ecse.rpi.edu/Research/Short_Notes/pnpoly.html
        public static bool PointIsInsidePolygon(PointF[] polygon, PointF testPoint)
        {
            int i, j = 0;
            bool c = false;
            for (i = 0, j = polygon.Length - 1; i < polygon.Length; j = i++)
            {
                if (((polygon[i].Y > testPoint.Y) != (polygon[j].Y > testPoint.Y)) && (testPoint.X < (polygon[j].X - polygon[i].X) * (testPoint.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) + polygon[i].X))
                    c = !c;
            }

            return c;
        }


        public static bool PointIsInsidePolygon(Point[] polygon, Point testPoint)
        {
            int i, j = 0;
            bool c = false;
            for (i = 0, j = polygon.Length - 1; i < polygon.Length; j = i++)
            {
                if (((polygon[i].Y > testPoint.Y) != (polygon[j].Y > testPoint.Y)) && (testPoint.X < (polygon[j].X - polygon[i].X) * (testPoint.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) + polygon[i].X))
                    c = !c;
            }

            return c;
        }

        public static Cell CellFromCoords(Cell[] cells, int row, int column)
        {
            int idx = column * 10 + row;

            if (idx >= cells.Length || idx < 0)
                return null;

            return cells[idx];
        }

        public static Cell CellFromPosition(Cell[] cells, Point pos)
        {
            for (int i = 0; i < cells.Length; i++)
            {
                if (Helpers.PointIsInsidePolygon(cells[i].CellBox, pos))
                    return cells[i];
            }

            return null;
        }

    }
}
