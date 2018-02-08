using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
//using Petronode.CommonControls;

namespace Petronode.CommonData
{
    public class DigitizerProcessor
    {
        /// <summary>
        /// Finds the closest point based on colour tolerance
        /// </summary>
        /// <param name="Image">Image to process</param>
        /// <param name="initial">x,y coordinate of start point</param>
        /// <param name="final">x,y coordinate final point</param>
        /// <param name="c0">Desired colour under the new location</param>
        /// <param name="tolerance">Desired tolerance under the new location</param>
        /// <returns>(-1,-1) if not found, or the new point</returns>
        public static Point FindEdgePoint(Bitmap Image, Point initial, Point final, Color c0, int tolerance)
        {
            Point dummy = new Point(-1, -1);
            Point loc0 = new Point(initial.X, initial.Y);
            Point direction = new Point(final.X - initial.X, final.Y - initial.Y);
            int nStepX = (direction.X >= 0) ? direction.X : -direction.X;
            int nStepY = (direction.Y >= 0) ? direction.Y : -direction.Y;
            int nStep = (nStepX > nStepY) ? nStepX : nStepY;
            if (direction.X < 0) direction.X = -1;
            if (direction.Y < 0) direction.Y = -1;
            if (direction.X > 0) direction.X = 1;
            if (direction.Y > 0) direction.Y = 1;
            Point loc1 = new Point(initial.X + direction.X, initial.Y + direction.Y);
            for (int i = 0; i < nStep; i++)
            {
                if (loc0.X < 0 || loc0.Y < 0) return dummy;
                if (loc0.X >= Image.Width || loc0.Y >= Image.Height) return dummy;
                if (loc1.X < 0 || loc1.Y < 0) return dummy;
                if (loc1.X >= Image.Width || loc1.Y >= Image.Height) return dummy;
                Color cc0 = Image.GetPixel(loc0.X, loc0.Y);
                Color cc1 = Image.GetPixel(loc1.X, loc1.Y);
                if (ColorParser.IsEdge(cc0, cc1, c0, tolerance))
                    return loc0;
                loc0.X = loc1.X;
                loc0.Y = loc1.Y;
                loc1.X += direction.X;
                loc1.Y += direction.Y;
            }
            return loc0;
        }

        /// <summary>
        /// Finds the best fit for line, moving the point in a vertical window
        /// </summary>
        /// <param name="Image">Image to process</param>
        /// <param name="initial">x,y coordinate of start point</param>
        /// <param name="c0">Desired colour under the new location</param>
        /// <param name="window">Search window up or down</param>
        /// <returns>new point</returns>
        public static Point FindLinePointVertical(Bitmap Image, Point initial, Color c0, int window)
        {
            int locX = initial.X;
            if (locX < 0 || locX >= Image.Width) return initial;
            int locY = initial.Y;
            if (locY < 0 || locY >= Image.Height) return initial;
            Color cc0 = Image.GetPixel(locX, locY);
            long fit0 = ColorParser.GetColorDiff2(cc0, c0);
            int locY0 = initial.Y - window;
            if (locY0 < 0) locY0 = 0;
            int locY1 = initial.Y + window;
            if (locY1 >= Image.Height) locY1 = Image.Height - 1;

            long fitUp = fit0;
            int locUp = locY;
            for (int y = locY - 1; y >= locY0; y--)
            {
                cc0 = Image.GetPixel(locX, y);
                long fit = ColorParser.GetColorDiff2(cc0, c0);
                if (fit > fitUp) break;
                fitUp = fit;
                locUp = y;
            }

            long fitDown = fit0;
            int locDown = locY;
            for (int y = locY + 1; y <= locY1; y++)
            {
                cc0 = Image.GetPixel(locX, y);
                long fit = ColorParser.GetColorDiff2(cc0, c0);
                if (fit > fitDown) break;
                fitDown = fit;
                locDown = y;
            }

            return new Point(initial.X, (locUp + locDown) / 2);
        }

        /// <summary>
        /// Finds the best fit for line, moving the point in a horizontal window
        /// </summary>
        /// <param name="Image">Image to process</param>
        /// <param name="initial">x,y coordinate of start point</param>
        /// <param name="c0">Desired colour under the new location</param>
        /// <param name="window">Search window left or right</param>
        /// <returns>new point</returns>
        public static Point FindLinePointHorizontal(Bitmap Image, Point initial, Color c0, int window)
        {
            int locX = initial.X;
            if (locX < 0 || locX >= Image.Width) return initial;
            int locY = initial.Y;
            if (locY < 0 || locY >= Image.Height) return initial;
            Color cc0 = Image.GetPixel(locX, locY);
            long fit0 = ColorParser.GetColorDiff2(cc0, c0);
            int locX0 = initial.X - window;
            if (locX0 < 0) locX0 = 0;
            int locX1 = initial.X + window;
            if (locX1 >= Image.Width) locX1 = Image.Width - 1;

            long fitLeft = fit0;
            int locLeft = locX;
            for (int x = locX - 1; x >= locX0; x--)
            {
                cc0 = Image.GetPixel(x, locY);
                long fit = ColorParser.GetColorDiff2(cc0, c0);
                if (fit > fitLeft) break;
                fitLeft = fit;
                locLeft = x;
            }

            long fitRight = fit0;
            int locRight = locX;
            for (int x = locX + 1; x <= locX1; x++)
            {
                cc0 = Image.GetPixel(x, locY);
                long fit = ColorParser.GetColorDiff2(cc0, c0);
                if (fit > fitRight) break;
                fitRight = fit;
                locRight = x;
            }

            return new Point((locLeft + locRight) / 2, initial.Y);
        }

        /// <summary>
        /// Finds the best fit in a box 2window x 2window pixels
        /// </summary>
        /// <param name="Image">Image to process</param>
        /// <param name="initial">x,y coordinate of start point</param>
        /// <param name="c0">Desired colour under the new location</param>
        /// <param name="window">Search window size</param>
        /// <returns>new point</returns>
        public static Point FindBoxPoint(Bitmap Image, Point initial, Color c0, int window)
        {
            int locX = initial.X;
            if (locX < 0 || locX >= Image.Width) return initial;
            int locY = initial.Y;
            if (locY < 0 || locY >= Image.Height) return initial;

            int locX0 = initial.X - window;
            if (locX0 < 0) locX0 = 0;
            int locX1 = initial.X + window;
            if (locX1 >= Image.Width) locX1 = Image.Width - 1;
            int locY0 = initial.Y - window;
            if (locY0 < 0) locY0 = 0;
            int locY1 = initial.Y + window;
            if (locY1 >= Image.Height) locY1 = Image.Height - 1;

            List<Point> minimumPoints = new List<Point>();
            minimumPoints.Add(new Point(locX, locY));
            //long minimum = GetFunction3x3(Image, locX, locY, c0);
            long minimum = ColorParser.GetColorDiff2(Image.GetPixel(locX, locY), c0);
            for (int x = locX0; x <= locX1; x++)
            {
                for (int y = locY0; y <= locY1; y++)
                {
                    if (x == locX && y == locY) continue;
                    //long w = GetFunction3x3(Image, x, y, c0);
                    long w = ColorParser.GetColorDiff2(Image.GetPixel(x, y), c0);
                    if (w > minimum) continue;
                    if (w < minimum)
                    {
                        minimumPoints.Clear();
                        minimum = w;
                    }
                    minimumPoints.Add(new Point(x, y));
                }
            }
            float shiftX = 0f;
            float shiftY = 0f;
            foreach (Point p in minimumPoints)
            {
                shiftX += Convert.ToSingle(p.X);
                shiftY += Convert.ToSingle(p.Y);
            }
            shiftX /= minimumPoints.Count;
            shiftY /= minimumPoints.Count;
            return new Point((int)shiftX, (int)shiftY);
        }

        /// <summary>
        /// Weight function
        /// 121
        /// 242
        /// 121
        /// </summary>
        private static long GetFunction3x3(Bitmap b, int x, int y, Color c0)
        {
            long f = ColorParser.GetColorDiff2(b.GetPixel(x, y), c0) << 2;
            f += ColorParser.GetColorDiff2(b.GetPixel(x-1, y-1), c0);
            f += ColorParser.GetColorDiff2(b.GetPixel(x, y-1), c0) << 1;
            f += ColorParser.GetColorDiff2(b.GetPixel(x+1, y-1), c0);
            f += ColorParser.GetColorDiff2(b.GetPixel(x-1, y), c0) << 1;
            f += ColorParser.GetColorDiff2(b.GetPixel(x+1, y), c0) << 1;
            f += ColorParser.GetColorDiff2(b.GetPixel(x-1, y+1), c0);
            f += ColorParser.GetColorDiff2(b.GetPixel(x, y+1), c0) << 1;
            f += ColorParser.GetColorDiff2(b.GetPixel(x+1, y+1), c0);
            return f;
        }
    }
}
