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
        private static Random _rnd = new Random();

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

        public static Direction GetRandomDirection()
        {
            return (Direction)_rnd.Next(0, 4);
        }

        public static Direction FlipDirection(Direction dir)
        {
            switch (dir)
            {
                case Direction.Down:
                    return Direction.Up;
                case Direction.Up:
                    return Direction.Down;
                case Direction.Left:
                    return Direction.Right;
                case Direction.Right:
                    return Direction.Left;
            }

            return dir;
        }

        public static Cell GetRandomCell(Cell[] cells)
        {
            int rndIdx = _rnd.Next(0, cells.Length);
            return cells[rndIdx];

        }

        // Credit: http://csharphelper.com/blog/2014/07/find-the-centroid-of-a-polygon-in-c/
        public static Point CenterOfPolygon(Point[] polygon)
        {
            // Add the first point at the end of the array.
            int num_points = polygon.Length;
            Point[] pts = new Point[num_points + 1];
            polygon.CopyTo(pts, 0);
            pts[num_points] = polygon[0];

            // Find the centroid.
            float X = 0;
            float Y = 0;
            float second_factor;
            for (int i = 0; i < num_points; i++)
            {
                second_factor =
                    pts[i].X * pts[i + 1].Y -
                    pts[i + 1].X * pts[i].Y;

                X += (pts[i].X + pts[i + 1].X) * second_factor;
                Y += (pts[i].Y + pts[i + 1].Y) * second_factor;
            }

            // Get the areas.
            float polygon_area = 0;
            for (int i = 0; i < num_points; i++)
            {
                polygon_area +=
                    (pts[i + 1].X - pts[i].X) *
                    (pts[i + 1].Y + pts[i].Y) / 2;
            }

            // Divide by 6 times the polygon's area.
            X /= (6 * polygon_area);
            Y /= (6 * polygon_area);

            // If the values are negative, the polygon is
            // oriented counterclockwise so reverse the signs.
            if (X < 0 || Y < 0)
            {
                X = -X;
                Y = -Y;
            }

            return new Point((int)X, (int)Y);
        }

    }
}
