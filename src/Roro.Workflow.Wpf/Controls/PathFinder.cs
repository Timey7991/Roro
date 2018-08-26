using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace Roro.Workflow.Wpf
{
    public class PathFinder
    {
        private class Tile
        {
            public Point Point { get; set; }

            public Vector Vector { get; set; }

            public Tile Parent { get; set; }

            public bool IsWall { get; set; }

            public bool IsOpen { get; set; }

            public Guid Session { get; set; }

            public double Priority { get; set; }

            public double Effort { get; set; }

            public Tile(double x, double y)
            {
                this.Point = new Point(x, y);
            }
        }

        private class Map
        {
            private readonly Tile[,] _tiles;

            public int Width { get; }

            public int Height { get; }

            public Tile this[double x, double y]
            {
                get => this._tiles[(int)y, (int)x];
                set => this._tiles[(int)y, (int)x] = value;
            }

            public Map(int width, int height)
            {
                this.Width = width;
                this.Height = height;
                this._tiles = new Tile[height, width];
            }
        }

        private Map _map;

        private Guid _session;

        public PathFinder(IEnumerable<NodeRect> rects)
        {
            this.CreateMap(rects);
        }

        private void CreateMap(IEnumerable<NodeRect> rects)
        {
            var mapWidth = 0;
            var mapHeight = 0;
            foreach (var rect in rects)
            {
                mapWidth = Math.Max(mapWidth, 2 + (rect.X + rect.Width) / Page.GRID_SIZE);
                mapHeight = Math.Max(mapHeight, 2 + (rect.Y + rect.Height) / Page.GRID_SIZE);
            }
            this._map = new Map(mapWidth, mapHeight);
            foreach (var rect in rects)
            {
                var x = rect.X / Page.GRID_SIZE;
                var right = x + rect.Width / Page.GRID_SIZE;
                for (; x <= right; x++)
                {
                    var y = rect.Y / Page.GRID_SIZE;
                    var bottom = y + rect.Height / Page.GRID_SIZE;
                    for (; y <= bottom; y++)
                    {
                        if (x < 0 || y < 0 || x > mapWidth || y > mapHeight)
                        {
                            continue;
                        }
                        this._map[x, y] = new Tile(x, y)
                        {
                            IsWall = true
                        };
                    }
                }
            }
        }

        public string GetPath(Point startPoint, Point endPoint)
        {
            var startTile = this._map[(int)startPoint.X / Page.GRID_SIZE, (int)startPoint.Y / Page.GRID_SIZE];
            var endTile = this._map[(int)endPoint.X / Page.GRID_SIZE, (int)endPoint.Y / Page.GRID_SIZE];

            var path = this.GetPath(startTile, endTile);
            return path;
        }

        private string GetPath(Tile startTile, Tile endTile)
        {
            this._session = Guid.NewGuid();

            startTile.IsOpen = true;
            startTile.Parent = null;
            startTile.Effort = 0;

            endTile.IsOpen = true;
            endTile.Parent = startTile;

            var stopWatch = Stopwatch.StartNew();
            var tileQueue = new PriorityQueue<Tile>();
            tileQueue.Enqueue(startTile, startTile.Priority);
            while (stopWatch.ElapsedMilliseconds < 50 && tileQueue.Count > 0)
            {
                var currentTile = tileQueue.Dequeue();
                if (currentTile == endTile)
                {
                    break;
                }
                if (currentTile.IsOpen)
                {
                    currentTile.IsOpen = false;
                    foreach (var nextTile in this.GetNextTiles(currentTile, endTile))
                    {
                        tileQueue.Enqueue(nextTile, nextTile.Priority);
                    }
                }
            }

            return TracePath(startTile, endTile);
        }

        private IEnumerable<Tile> GetNextTiles(Tile currentTile, Tile endTile)
        {
            var nextTiles = new List<Tile>();
            var nextTileVectors = new List<Vector>
            {
               new Vector(0, +1),
               new Vector(+1, 0),
               new Vector(0, -1),
               new Vector(-1, 0)
            };
            foreach (var nextTileVector in nextTileVectors)
            {
                var nextTilePoint = currentTile.Point + nextTileVector;
                var nextTile = new Tile(nextTilePoint.X, nextTilePoint.Y);
                if (nextTile.Point.X < 0) continue;
                if (nextTile.Point.Y < 0) continue;
                if (nextTile.Point.X >= this._map.Width) continue;
                if (nextTile.Point.Y >= this._map.Height) continue;
                if (this._map[nextTile.Point.X, nextTile.Point.Y] == null)
                {
                    this._map[nextTile.Point.X, nextTile.Point.Y] = nextTile;
                }
                nextTile = this._map[nextTile.Point.X, nextTile.Point.Y];

                if (nextTile.Session != this._session)
                {
                    nextTile.Parent = null;
                    nextTile.Effort = double.MaxValue;
                    nextTile.IsOpen = nextTile == endTile || !nextTile.IsWall;
                    nextTile.Session = this._session;
                }

                if (nextTile.IsOpen)
                {
                    var nextTileNewEffort = currentTile.Effort + (nextTile.Point - currentTile.Point).Length;
                    if (nextTileNewEffort < nextTile.Effort)
                    {
                        nextTile.Parent = currentTile;
                        nextTile.Vector = nextTileVector;
                        nextTile.Effort = nextTileNewEffort;
                        nextTile.Priority = nextTile.Effort + Math.Abs(endTile.Point.X - nextTile.Point.X)
                                                            + Math.Abs(endTile.Point.Y - nextTile.Point.Y);
                        nextTiles.Add(nextTile);
                    }
                }
            }

            return nextTiles.OrderByDescending(x => x.Vector == currentTile.Vector);
        }

        private string TracePath(Tile startTile, Tile endTile)
        {
            var a = endTile;
            var arrow = CreateArrow(endTile.Point, endTile.Vector);
            
            var path = arrow + string.Format("M {0} {1} ", endTile.Point.X * Page.GRID_SIZE, endTile.Point.Y * Page.GRID_SIZE);
            while (a != startTile && a.Parent is Tile b)
            {
                var aVector = a.Vector * -1;
                var bVector = b.Vector * -1;
                var aPoint = a.Point;
                var bPoint = b.Point;
                var cPoint = b.Point + bVector;
                if (aVector != bVector)
                {
                    var aPointEx = aPoint + aVector / 2;
                    var cPointEx = cPoint - bVector / 2;
                    path += string.Format("L {0} {1} ", aPointEx.X * Page.GRID_SIZE, aPointEx.Y * Page.GRID_SIZE);
                    path += string.Format("Q {0} {1} {2} {3} ",
                        bPoint.X * Page.GRID_SIZE,
                        bPoint.Y * Page.GRID_SIZE,
                        cPointEx.X * Page.GRID_SIZE,
                        cPointEx.Y * Page.GRID_SIZE);
                }
                a = b;
            }
            path += string.Format("L {0} {1} ", startTile.Point.X * Page.GRID_SIZE, startTile.Point.Y * Page.GRID_SIZE);

            return path;
        }

        private string CreateArrow(Point endPoint, Vector vector)
        {
            var sin = Math.Sin(45 * Math.PI / 180);
            var cos = Math.Cos(45 * Math.PI / 180);

            var rotatingPoint = new Point() - vector / 2;

            var clockwisePoint = new Point()
            {
                X = endPoint.X + rotatingPoint.X * cos + rotatingPoint.Y * sin,
                Y = endPoint.Y - rotatingPoint.X * sin + rotatingPoint.Y * cos,
            };

            var counterCwPoint = new Point()
            {
                X = endPoint.X + rotatingPoint.X * cos - rotatingPoint.Y * sin,
                Y = endPoint.Y + rotatingPoint.X * sin + rotatingPoint.Y * cos,
            };

            var arrow = string.Format("M {0} {1} L {2} {3} L {4} {5} M {2} {3} ",
                Page.GRID_SIZE * clockwisePoint.X,
                Page.GRID_SIZE * clockwisePoint.Y,
                Page.GRID_SIZE * endPoint.X,
                Page.GRID_SIZE * endPoint.Y,
                Page.GRID_SIZE * counterCwPoint.X,
                Page.GRID_SIZE * counterCwPoint.Y);

            return arrow;

        }
    }
}