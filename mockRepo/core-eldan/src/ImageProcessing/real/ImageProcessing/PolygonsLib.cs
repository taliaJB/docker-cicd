using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Eldan.ImageProcessing
{
    public static class PolygonsLib
    {
        private const double EPSILON = 0.000000001;

        public static bool IsInPolygonBySplit(this List<Point> polygon, Point point)
        {
            if (!FixPolygon(ref polygon))
                return false;

            List<PolygonWithAngles> ConvexPolygonsWithAngles = new List<PolygonWithAngles>();
            Stitches Stitches = new Stitches();

            FindConvexPolygons(polygon, ref ConvexPolygonsWithAngles, ref Stitches);

            if (ConvexPolygonsWithAngles.Exists(x => x.Contains(point)))
                return true;

            return Stitches.Contains(point);
        }

        public static bool IsInPolygon(this List<Point> polygon, Point point)
        {
            if (!FixPolygon(ref polygon))
                return false;

            bool result = false;
            int j = polygon.Count() - 1;
            for (int i = 0; i < polygon.Count(); i++)
            {
                if (polygon[i].Y < point.Y && polygon[j].Y >= point.Y || polygon[j].Y < point.Y && polygon[i].Y >= point.Y)
                {
                    if (polygon[i].X + (point.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) * (polygon[j].X - polygon[i].X) < point.X)
                    {
                        result = !result;
                    }
                }
                j = i;
            }
            return result;
        }

        private static bool FixPolygon(ref List<Point> polygon)
        {
            if (polygon.Count < 3)
                return false;

            if (polygon.First().CloseTo(polygon.Last()))
                polygon.RemoveAt(polygon.Count - 1);

            return true;
        }

        public enum EnmDirection
        {
            Clockwise,
            CounterClockwise
        }

        private static void FindConvexPolygons(List<Point> polygon, ref List<PolygonWithAngles> convexPolygons, ref Stitches stitches)
        {
            if (polygon.Count < 3)
                return;

            PolygonWithAngles PolygonWithAngles = GetPolygonWithAngles(polygon, out EnmDirection Direction);

            if (PolygonWithAngles.IsConvexPolygon())
            {
                convexPolygons.Add(PolygonWithAngles);
                return;
            }

            int StartIndex = PolygonWithAngles.GetSplitStartPointIndex(180);
            int EndIndex = PolygonWithAngles.GetSplitEndPointIndex(StartIndex, Direction);
            stitches.Add(new Line(PolygonWithAngles[StartIndex].Point, PolygonWithAngles[EndIndex].Point));

            PolygonWithAngles.Split(StartIndex, EndIndex, out List<Point> PolygonA, out List<Point> PolygonB);

            FindConvexPolygons(PolygonA, ref convexPolygons, ref stitches);
            FindConvexPolygons(PolygonB, ref convexPolygons, ref stitches);
        }

        private class PointAndAngle
        {
            internal Point Point;
            internal double Angle;
        }

        private class PolygonWithAngles : List<PointAndAngle>
        {
            private PolygonWithAngles(List<PointAndAngle> pointAndAngles) : base(pointAndAngles)
            { }

            internal static PolygonWithAngles GetInstance(List<PointAndAngle> pointAndAngles)
            {
                return new PolygonWithAngles(pointAndAngles);
            }

            internal bool IsConvexPolygon()
            {
                if (Count <= 3)
                    return true;

                if (this.All(x => x.Angle <= 180))
                    return true;

                return false;
            }

            internal int GetSplitStartPointIndex(double minAngle)
            {
                return FindIndex(x => x.Angle > minAngle);
            }

            internal int GetSplitEndPointIndex(int startIndex, EnmDirection direction)
            {
                Point FirstPoint = GetPreviousItem(this, startIndex).Point;
                Point CenterPoint = this[startIndex].Point;

                GetNextItem(this, startIndex, out int TempIndex);
                Point SecondPoint = GetNextItem(this, TempIndex, out int SecondIndex).Point;
                while (true)
                {
                    double Angle = GetAngle(FirstPoint, SecondPoint, CenterPoint, direction);
                    
                    if (Angle <= 180)
                    {
                        if (!IsIntersectInPolygon(new Line(CenterPoint, SecondPoint)))
                            break;
                    }
                    SecondPoint = GetNextItem(this, SecondIndex, out SecondIndex).Point;
                }

                return SecondIndex;
            }

            private bool IsIntersectInPolygon(Line line)
            {
                for (int i = 0; i < this.Count(); i++)
                {
                    Point NextPoint = GetNextItem(this, i).Point;
                    if(line.InersectWith(new Line(this[i].Point, NextPoint)))
                        return true;
                }

                return false;
            }

            internal void Split(int startIndex, int endIdex, out List<Point> polygonA, out List<Point> polygonB)
            {
                polygonA = new List<Point> { this[startIndex].Point };
                polygonB = new List<Point> { this[startIndex].Point, this[endIdex].Point };
                bool AComplete = false;

                int CurrentIndex = startIndex;
                while (true)
                {
                    Point CurrentPoint = GetNextItem(this, CurrentIndex, out CurrentIndex).Point;
                    if (CurrentIndex == startIndex)
                        return;

                    if (CurrentIndex != endIdex && !AComplete)
                        polygonA.Add(CurrentPoint);

                    if (AComplete && CurrentIndex != startIndex)
                        polygonB.Add(CurrentPoint);

                    if (CurrentIndex == endIdex)
                    {
                        polygonA.Add(CurrentPoint);
                        AComplete = true;
                    }
                }
            }

            internal bool Contains(Point testPoint)
            {
                var Polygon = (from PointWithAngle in this
                               select PointWithAngle.Point).ToList();

                return Polygon.IsInPolygon(testPoint);
            }
        }

        private static PolygonWithAngles GetPolygonWithAngles(List<Point> polygon, out EnmDirection direction)
        {
            List<PointAndAngle> PolygonWithClockwiseAngles = new List<PointAndAngle>();

            double ClockwiseAnglesSum = 0;
            double CounterClockwiseAnglesSum = 0;
            for (int i = 0; i < polygon.Count; i++)
            {
                double ClockwiseAngle = GetAngle(GetPreviousItem(polygon, i),
                                                 GetNextItem(polygon, i),
                                                 polygon[i], EnmDirection.Clockwise);
                ClockwiseAnglesSum += ClockwiseAngle;
                CounterClockwiseAnglesSum += 360 - ClockwiseAngle;

                PolygonWithClockwiseAngles.Add(new PointAndAngle { Point = polygon[i], Angle = ClockwiseAngle });
            }

            if (ClockwiseAnglesSum < CounterClockwiseAnglesSum)
            {
                direction = EnmDirection.Clockwise;
                return PolygonWithAngles.GetInstance(PolygonWithClockwiseAngles);
            }

            direction = EnmDirection.CounterClockwise;
            var PolygonWithCounterClockwiseAngles = from PointAndAngle in PolygonWithClockwiseAngles
                                                    select PointAndAngle.ChangeAngleDirection();

            return PolygonWithAngles.GetInstance(PolygonWithCounterClockwiseAngles.ToList());
        }

        private static PointAndAngle ChangeAngleDirection(this PointAndAngle pointAndAngle)
        {
            pointAndAngle.Angle = 360 - pointAndAngle.Angle;
            return pointAndAngle;
        }

        private static T GetNextItem<T>(List<T> items, int index)
        {
            return GetNextItem(items, index, out int Dummy);
        }

        private static T GetNextItem<T>(List<T> items, int index, out int nextIndex)
        {
            if (items.Count == index + 1)
            {
                nextIndex = 0;
                return items.First();
            }

            nextIndex = index + 1;
            return items[nextIndex];
        }

        private static T GetPreviousItem<T>(List<T> items, int index)
        {
            return GetPreviousItem(items, index, out int Dummy);
        }

        private static T GetPreviousItem<T>(List<T> items, int index, out int previousIndex)
        {
            if (index == 0)
            {
                previousIndex = items.Count - 1;
                return items.Last();
            }

            previousIndex = index - 1;
            return items[index - 1];
        }

        public static double GetAngle(Point first, Point second, Point center, EnmDirection direction)
        {
            double Angle = GetAngleAndDirection(first, second, center, out EnmDirection ResDirection);
            if (ResDirection == direction)
                return Angle;
            else
                return 360 - Angle;
        }

        private static double ConvertRadiansToDegrees(double radians)
        {
            double degrees = (180 / Math.PI) * radians;
            return (degrees);
        }

        private static double GetAngleAndDirection(Point first, Point second, Point center, out EnmDirection direction)
        {
            Point A = new Point { X = first.X - center.X, Y = first.Y - center.Y };
            Point B = new Point { X = second.X - center.X, Y = second.Y - center.Y };

            double Res = ConvertRadiansToDegrees(GetSignAngle(A, B));
            if (Res < 0)
                direction = EnmDirection.Clockwise;
            else
                direction = EnmDirection.CounterClockwise;

            return Math.Abs(Res);
        }

        private static double GetSignAngle(Point pA, Point pB)
        {
            double dot = pA.X * pB.X + pA.Y * pB.Y;
            double det = pA.X * pB.Y - pA.Y * pB.X;

            return Math.Atan2(det, dot);
        }

        private class Line
        {
            internal Point PointA;
            internal Point PointB;

            internal Line()
            { }

            internal Line(Point pointA, Point pointB)
            {
                PointA = pointA;
                PointB = pointB;
            }
        }

        private class Stitches : List<Line>
        {
            internal bool Contains(Point testPoint)
            {
                return Exists(x => testPoint.IsInLine(x));
            }
        }

        private static bool InersectWith(this Line lineA, Line lineB)
        {
            if (lineA.PointA.CloseTo(lineB.PointA) || lineA.PointA.CloseTo(lineB.PointB) || lineA.PointB.CloseTo(lineB.PointA) || lineA.PointB.CloseTo(lineB.PointB))
                return false;

            bool FuncABExist = GetLineCoefficients(lineA.PointA, lineA.PointB, out double mAB, out double nAB);
            bool FuncCDExist = GetLineCoefficients(lineB.PointA, lineB.PointB, out double mCD, out double nCD);

            if (!FuncABExist && !FuncCDExist)
                return false;

            Point ResultPoint = new Point(0, 0);
            bool FoundExtrimResult = false;
            if (!FuncABExist)
            {
                ResultPoint.X = lineA.PointA.X;
                ResultPoint.Y = mCD * ResultPoint.X + nCD;
                FoundExtrimResult = true;
            }

            if (!FuncCDExist)
            {
                ResultPoint.X = lineB.PointA.X;
                ResultPoint.Y = mAB * ResultPoint.X + nAB;
                FoundExtrimResult = true;
            }

            if (!FoundExtrimResult)
            {
                double delta = -mAB + mCD;

                if (delta.CloseTo(0)) // Lines are parallel
                    return false;

                ResultPoint.X = (nAB - nCD) / delta;
                ResultPoint.Y = ((-mAB) * nCD + mCD * nAB) / delta;
            }

            return IsInRange(ResultPoint, lineA.PointB, lineA.PointB) && IsInRange(ResultPoint, lineB.PointA, lineB.PointB);
        }

        private static bool IsInLine(this Point testPoint, Line line)
        {
            bool FuncExist = GetLineCoefficients(line.PointA, line.PointB, out double m, out double n);

            if (FuncExist)
            {
                if (!testPoint.Y.CloseTo(m * testPoint.X + n))
                    return false;
            }

            return IsInRange(testPoint, line.PointA, line.PointB);
        }

        private static bool CloseTo(this Point pA, Point pB)
        {
            return pA.X.CloseTo(pB.X) && pA.Y.CloseTo(pB.Y);
        }

        private static bool CloseTo(this double a, double b)
        {
            return (a > b - EPSILON) && (a < b + EPSILON);
        }

        private static bool IsInRange(Point testPoint, Point CornerA, Point CornerB)
        {
            if (testPoint.X > Math.Min(CornerA.X, CornerB.X) && testPoint.X < Math.Max(CornerA.X, CornerB.X))
            {
                if (testPoint.Y > Math.Min(CornerA.Y, CornerB.Y) && testPoint.Y < Math.Max(CornerA.Y, CornerB.Y))
                    return true;

                if (testPoint.Y.CloseTo(CornerA.Y) && testPoint.Y.CloseTo(CornerB.Y))
                    return true;
            }

            if (testPoint.Y > Math.Min(CornerA.Y, CornerB.Y) && testPoint.Y < Math.Max(CornerA.Y, CornerB.Y) && testPoint.X.CloseTo(CornerA.X) && testPoint.X.CloseTo(CornerB.X))
                return true;

            return false;
        }

        private static bool GetLineCoefficients(Point pA, Point pB, out double m, out double n)
        {
            m = 0;
            n = 0;

            if (pA.X == pB.X)
                return false;

            m = (pB.Y - pA.Y) / (pB.X - pA.X);
            n = pA.Y - m * pA.X;

            return true;
        }
    }
}
