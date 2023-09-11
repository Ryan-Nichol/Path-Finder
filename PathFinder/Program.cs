using System;
using System.Collections.Generic;
using System.Configuration.Assemblies;
using System.IO;
using System.Linq;

namespace PathFinder
{
    internal class Program
    {
        public static char[][] Grid;
        
        public static bool[][] TraversedGrid;

        public static char Wall = 'w', Empty = 'e';

        public static Vector Player;

        public static Stack<Vector> MovesTaken;
        
        public static Stack<int> MoveCount;

        public static Stack<Vector> PlayerHistory;
        
        public static void Main(string[] args)
        {
            var lines = File.ReadLines(@"C:\Users\Ryan.Nichol\RiderProjects\PathFinder\PathFinder\Map.txt").Reverse().ToList();
            Grid = new char[lines.Count()][];
            TraversedGrid = new bool[lines.Count()][];
            MovesTaken = new Stack<Vector>();
            PlayerHistory = new Stack<Vector>();
            MoveCount = new Stack<int>();
            for (int y = 0; y < Grid.Length; y++)
            {
                Grid[y] = lines[y].ToCharArray();
                TraversedGrid[y] = new bool[lines[y].Length];
                
                for (int w = 0; w < lines[y].Length; w++)
                {
                    if (Grid[y][w] == Wall)
                    {
                        TraversedGrid[y][w] = true;
                    }
                }
            }

            Player = new Vector(1, 1);
            Search();
        }

        public static Vector NegationStartPoint;
        
        public static void Search(int negation = 0)
        {
            bool won = Won();
            // Print(won ? "Won: " : "Searching");
            if (Won())
            {
                Print("Won");
                return;
            }

            if (negation > 0 && NegationStartPoint.Equals(Player))
            {
                // WE ARE IN A LOOP
                Print("Loop");
                return;
            }
            
            var directions = new List<Vector>();
            if (IsEmpty(Player.Plus(Vector.Right))) directions.Add(Vector.Right);
            if (IsEmpty(Player.Plus(Vector.Left))) directions.Add(Vector.Left);
            if (IsEmpty(Player.Plus(Vector.Up))) directions.Add(Vector.Up);
            if (IsEmpty(Player.Plus(Vector.Down))) directions.Add(Vector.Down);

            var untraversed = GetUnTraversedDirections(directions);

            // if all paths are traversed then we must negate and search again
            // until we find another path we can traverse
            if (!untraversed.Any())
            {
                if (negation == 0)
                {
                    NegationStartPoint = Player.Copy();
                }
                
                var list = MovesTaken.ToList();
                
                var lastDirection = list[negation];
                Move(lastDirection.Negate());

                negation += 2;
                Search(negation);
                
                Undo();
            }
            
            foreach (var direction in untraversed)
            {
                Move(direction);
                Search();
                Undo();
            }
        }

        public static void Print(string message)
        {
            var list = MoveCount.ToList();
            Console.Write($"{message} {list.Sum()}: ");
            foreach (var move in MovesTaken.Reverse())
            {
                if (move.Equals(Vector.Right)) Console.Write("Right ");
                if (move.Equals(Vector.Left)) Console.Write("Left ");
                if (move.Equals(Vector.Up)) Console.Write("Up ");
                if (move.Equals(Vector.Down)) Console.Write("Down ");
            }
            
            Console.WriteLine();
        }
        
        public static bool Won()
        {
            for (int y = 0; y < Grid.Length; y++)
            {
                for (int w = 0; w < Grid[0].Length; w++)
                {
                    if (Player.y == y && Player.w == w)
                    {
                        continue;
                    }
                    
                    if (TraversedGrid[y][w] == false)
                    {
                        return false;
                    } 
                }
            }

            return true;
        }
        
        public static void Move(Vector direction)
        {
            MovesTaken.Push(direction.Copy());
            PlayerHistory.Push(Player.Copy());
            int moves = 0;
            while (IsEmpty(Player.Plus(direction)))
            {
                TraversedGrid[Player.y][Player.w] = true;
                Player.Move(direction);
                moves++;
            }
            
            MoveCount.Push(moves);
        }

        public static void Undo()
        {
            var playersLastPoint = PlayerHistory.Peek();
            var playersLastMove = MovesTaken.Peek();
            
            PlayerHistory.Pop();
            MovesTaken.Pop();
            MoveCount.Pop();
            
            while (!Player.Equals(playersLastPoint))
            {
                TraversedGrid[Player.y][Player.w] = false;
                Player.Move(playersLastMove.Negate());
            }
        }
        
        public static List<Vector> GetUnTraversedDirections(List<Vector> directions)
        {
            var untraversed = new List<Vector>();
            foreach (var direction in directions)
            {
                var player = Player.Copy();
                while (IsEmpty(player.Plus(direction)))
                {
                    player.Move(direction);
                    if (TraversedGrid[player.y][player.w] == false)
                    {
                        untraversed.Add(direction);
                        break;
                    }
                }
            }
            return untraversed;
        }
        
        public static char Get(Vector point) => Grid[point.y][point.w];

        public static bool IsEmpty(Vector point) => InBounds(point) && !IsWall(point);
        
        public static bool IsWall(Vector point) => !InBounds(point) || Get(point).Equals(Wall);

        public static bool InBounds(Vector point)
        {
            return point.y < Grid.Length && point.y >= 0 && point.w < Grid[0].Length && point.w >= 0;
        }

        public static bool OutOfBounds(Vector point) => !InBounds(point);
    }
}