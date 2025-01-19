// DungeonGen.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGen : MonoBehaviour
{
    public class Cell
    {
        public bool visited = false;
        public bool[] status = new bool[6]; // 0-North, 1-South, 2-East, 3-West, 4-Up, 5-Down
    }

    [System.Serializable]
    public class Rule
    {
        public GameObject room;
        public Vector3Int minPosition;
        public Vector3Int maxPosition;

        public bool obligatory;

        public int ProbabilityOfSpawning(int x, int y, int z)
        {
            // 0 - cannot spawn 1 - can spawn 2 - HAS to spawn
            if (x >= minPosition.x && x <= maxPosition.x &&
                y >= minPosition.y && y <= maxPosition.y &&
                z >= minPosition.z && z <= maxPosition.z)
            {
                return obligatory ? 2 : 1;
            }

            return 0;
        }
    }

    public Vector3Int size;
    public int startPos = 0;
    public Rule[] rooms;
    public Vector3 offset;

    List<Cell> board;

    void Start()
    {
        MazeGenerator();
    }

    void GenerateDungeon()
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                for (int z = 0; z < size.z; z++)
                {
                    Cell currentCell = board[x + y * size.x + z * size.x * size.y];
                    if (currentCell.visited)
                    {
                        int randomRoom = -1;
                        List<int> availableRooms = new List<int>();

                        for (int k = 0; k < rooms.Length; k++)
                        {
                            int p = rooms[k].ProbabilityOfSpawning(x, y, z);

                            if (p == 2)
                            {
                                randomRoom = k;
                                break;
                            }
                            else if (p == 1)
                            {
                                availableRooms.Add(k);
                            }
                        }

                        if (randomRoom == -1)
                        {
                            if (availableRooms.Count > 0)
                            {
                                randomRoom = availableRooms[Random.Range(0, availableRooms.Count)];
                            }
                            else
                            {
                                randomRoom = 0;
                            }
                        }

                        var newRoom = Instantiate(
                            rooms[randomRoom].room,
                            new Vector3(x * offset.x, y * offset.y, -z * offset.z),
                            Quaternion.identity,
                            transform
                        ).GetComponent<RoomBehaviour>();
                        newRoom.UpdateRoom(currentCell.status);
                        newRoom.name += $" {x}-{y}-{z}";
                    }
                }
            }
        }
    }

    void MazeGenerator()
    {
        board = new List<Cell>();

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                for (int z = 0; z < size.z; z++)
                {
                    board.Add(new Cell());
                }
            }
        }

        int currentCell = startPos;

        Stack<int> path = new Stack<int>();
        int k = 0;

        while (k < 1000)
        {
            k++;

            board[currentCell].visited = true;

            List<int> neighbors = CheckNeighbors(currentCell);

            if (neighbors.Count == 0)
            {
                if (path.Count == 0)
                {
                    break;
                }
                else
                {
                    currentCell = path.Pop();
                }
            }
            else
            {
                path.Push(currentCell);

                int newCell = neighbors[Random.Range(0, neighbors.Count)];

                int diff = newCell - currentCell;
                if (diff == -size.x) // North
                {
                    board[currentCell].status[5] = true;
                    board[newCell].status[4] = true;
                }
                else if (diff == size.x) // South
                {
                    board[currentCell].status[4] = true;
                    board[newCell].status[5] = true;
                }
                else if (diff == 1) // East
                {
                    board[currentCell].status[2] = true;
                    board[newCell].status[3] = true;
                }
                else if (diff == -1) // West
                {
                    board[currentCell].status[3] = true;
                    board[newCell].status[2] = true;
                }
                else if (diff == -size.x * size.y) // Up
                {
                    board[currentCell].status[0] = true;
                    board[newCell].status[1] = true;
                }
                else if (diff == size.x * size.y) // Down
                {
                    board[currentCell].status[1] = true;
                    board[newCell].status[0] = true;
                }

                currentCell = newCell;
            }
        }

        GenerateDungeon();
    }

    List<int> CheckNeighbors(int cell)
    {
        List<int> neighbors = new List<int>();

        int x = cell % size.x;
        int y = (cell / size.x) % size.y;
        int z = cell / (size.x * size.y);

        // Check neighbors in all 6 directions
        if (y - 1 >= 0 && !board[cell - size.x].visited) neighbors.Add(cell - size.x); // North
        if (y + 1 < size.y && !board[cell + size.x].visited) neighbors.Add(cell + size.x); // South
        if (x + 1 < size.x && !board[cell + 1].visited) neighbors.Add(cell + 1); // East
        if (x - 1 >= 0 && !board[cell - 1].visited) neighbors.Add(cell - 1); // West
        if (z - 1 >= 0 && !board[cell - size.x * size.y].visited) neighbors.Add(cell - size.x * size.y); // Up
        if (z + 1 < size.z && !board[cell + size.x * size.y].visited) neighbors.Add(cell + size.x * size.y); // Down

        return neighbors;
    }
}
