using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeDataGenerator
{
    // generator params
    public float placementThreshold;    // chance of empty space

    public MazeDataGenerator()
    {
        placementThreshold = .1f;
    }

    public int[,] FromDimensions(int sizeRows, int sizeCols)
    {
        int[,] maze = new int[sizeRows, sizeCols];

        int rMax = maze.GetUpperBound(0);
        int cMax = maze.GetUpperBound(1);

        for (int i = 1; i < rMax; i++)
        {
            for (int j = 1; j < cMax; j++)
            {
                // outside wall
                if (i == 0 || j == 0 || i == rMax || j == cMax)
                {
                    maze[i, j] = 0;
                }

                // every other inside space
                else if (i % 2 == 0 && j % 2 == 0)
                {
                    if (Random.value > placementThreshold)
                    {
                        maze[i, j] = 1;

                        // in addition to this spot, randomly place adjacent
                        int a = Random.value < .5 ? 0 : (Random.value < .5 ? -1 : 1);
                        int b = a != 0 ? 0 : (Random.value < .5 ? -1 : 1);
                        maze[i + a, j + b] = 1;
                    }
                }
                Debug.Log(maze[i, j]);
            }
        }

        return maze;
    }

    public (List<Vector2Int>, bool) GetPath(int[,] maze, Vector2Int curr, Vector2Int skip, Vector2Int finish, List<Vector2Int> currPath)
    {
        Debug.Log($"finish: {finish}, curr: {curr}");

        if (NearFinish(curr, finish))
        {
            return (new List<Vector2Int> { finish, curr }, true);
        }

        if (curr + Vector2Int.right != skip && curr.x < 5 && maze[curr.x + 1, curr.y] == 0 && currPath.Contains(curr) == false)
        {
            var lastPath = new List<Vector2Int>(currPath);
            lastPath.Add(curr);
            var path = GetPath(maze, curr + Vector2Int.right, curr, finish, lastPath);
            if (path.Item2)
            {
                path.Item1.Add(curr);
                return path;
            }
        }
        if (curr + Vector2Int.up != skip && curr.y < 5 && maze[curr.x, curr.y + 1] == 0 && currPath.Contains(curr) == false)
        {
            var lastPath = new List<Vector2Int>(currPath);
            lastPath.Add(curr);
            var path = GetPath(maze, curr + Vector2Int.up, curr, finish, lastPath);
            if (path.Item2)
            {
                path.Item1.Add(curr);
                return path;
            }
        }
        if (curr - Vector2Int.up != skip && curr.y > 1 && maze[curr.x, curr.y - 1] == 0 && currPath.Contains(curr) == false)
        {
            var lastPath = new List<Vector2Int>(currPath);
            lastPath.Add(curr);
            var path = GetPath(maze, curr - Vector2Int.up, curr, finish, lastPath);
            if (path.Item2)
            {
                path.Item1.Add(curr);
                return path;
            }
        }
        
        else if (curr - Vector2Int.right != skip && curr.x > 1 && maze[curr.x - 1, curr.y] == 0 && currPath.Contains(curr) == false)
        {
            var lastPath = new List<Vector2Int>(currPath);
            lastPath.Add(curr);
            var path = GetPath(maze, curr - Vector2Int.right, curr, finish, lastPath);
            if (path.Item2)
            {
                path.Item1.Add(curr);
                return path;
            }
        }
        return (new List<Vector2Int> { }, false);
    }

    private bool NearFinish(Vector2Int curr, Vector2Int finish)
    {
        if (curr + Vector2Int.right == finish)
            return true;
        if (curr - Vector2Int.right == finish)
            return true;
        if (curr + Vector2Int.up == finish)
            return true;
        if (curr - Vector2Int.up == finish)
            return true;
        return false;
    }

}