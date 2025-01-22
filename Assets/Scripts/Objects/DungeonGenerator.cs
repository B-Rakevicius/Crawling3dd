using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public class Cell
    {
        public bool visited = false;
        public bool[] status = new bool[4];
    }

    [System.Serializable]
    public class Rule
    {
        public GameObject room;
        public Vector2Int minPosition;
        public Vector2Int maxPosition;
        public bool obligatory;
        public int weight;
        public float verticalOffsetChange;

        public int ProbabilityOfSpawning(int x, int y)
        {
            if (x >= minPosition.x && x <= maxPosition.x && y >= minPosition.y && y <= maxPosition.y)
            {
                return obligatory ? 2 : 1;
            }
            return 0;
        }
    }

    public Vector2Int size;
    public int startPos = 0;
    public Rule[] rooms;
    public Vector2 offset;
    public int SeedforSeed = 12345;

    private List<Cell> board;
    private bool isGenerated = false; // Flag to ensure generation happens only once

    void Start()
    {
        if (!isGenerated)
        {
            Random.InitState(SeedforSeed); // Fixed seed for deterministic generation
            MazeGenerator();
            isGenerated = true; // Mark generation as complete
        }
    }

    void GenerateDungeon()
    {
        Queue<Vector2Int> roomQueue = new Queue<Vector2Int>();
        HashSet<Vector2Int> visitedRooms = new HashSet<Vector2Int>();

        float[,] verticalOffsets = new float[size.x, size.y];
        roomQueue.Enqueue(new Vector2Int(startPos % size.x, startPos / size.x));
        visitedRooms.Add(new Vector2Int(startPos % size.x, startPos / size.x));

        while (roomQueue.Count > 0)
        {
            Vector2Int currentRoom = roomQueue.Dequeue();
            int i = currentRoom.x;
            int j = currentRoom.y;
            Cell currentCell = board[i + j * size.x];

            Rule selectedRoom = SelectRoomUsingWeights(i, j);

            var newRoom = Instantiate(
                selectedRoom.room,
                new Vector3(i * offset.x, verticalOffsets[i, j], -j * offset.y),
                Quaternion.identity,
                transform
            ).GetComponent<RoomBehaviour>();

            newRoom.UpdateRoom(currentCell.status);
            newRoom.name += " " + i + "-" + j;

            RoomDecorator decorator = newRoom.GetComponent<RoomDecorator>();
            if (decorator != null)
            {
                decorator.Initialize(SeedforSeed + i + j * size.x); // Unique seed for each room
            }

            for (int dir = 0; dir < 4; dir++)
            {
                if (currentCell.status[dir])
                {
                    Vector2Int neighbor = GetNeighbor(currentRoom, dir);
                    if (!visitedRooms.Contains(neighbor))
                    {
                        visitedRooms.Add(neighbor);
                        roomQueue.Enqueue(neighbor);
                        verticalOffsets[neighbor.x, neighbor.y] = verticalOffsets[i, j] + selectedRoom.verticalOffsetChange;
                    }
                }
            }
        }
    }

    Rule SelectRoomUsingWeights(int x, int y)
    {
        List<int> cumulativeWeights = new List<int>();
        List<int> validRoomIndices = new List<int>();
        int totalWeight = 0;

        for (int k = 0; k < rooms.Length; k++)
        {
            int spawnProbability = rooms[k].ProbabilityOfSpawning(x, y);

            if (spawnProbability > 0)
            {
                validRoomIndices.Add(k);
                totalWeight += rooms[k].weight;
                cumulativeWeights.Add(totalWeight);
            }
        }

        if (validRoomIndices.Count == 0)
        {
            return rooms[0];
        }

        int randomValue = Random.Range(0, totalWeight);
        for (int i = 0; i < cumulativeWeights.Count; i++)
        {
            if (randomValue < cumulativeWeights[i])
            {
                return rooms[validRoomIndices[i]];
            }
        }

        return rooms[validRoomIndices[0]];
    }

    Vector2Int GetNeighbor(Vector2Int current, int direction)
    {
        switch (direction)
        {
            case 0: return new Vector2Int(current.x, current.y - 1); // Up
            case 1: return new Vector2Int(current.x, current.y + 1); // Down
            case 2: return new Vector2Int(current.x + 1, current.y); // Right
            case 3: return new Vector2Int(current.x - 1, current.y); // Left
            default: return current;
        }
    }

    void MazeGenerator()
    {
        board = new List<Cell>();

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                board.Add(new Cell());
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

                if (Random.value < 0.9f && path.Count > 1)
                {
                    currentCell = path.Pop();
                }

                int newCell = neighbors[Random.Range(0, neighbors.Count)];

                if (newCell > currentCell)
                {
                    if (newCell - 1 == currentCell)
                    {
                        board[currentCell].status[2] = true; // Right
                        currentCell = newCell;
                        board[currentCell].status[3] = true; // Left
                    }
                    else
                    {
                        board[currentCell].status[1] = true; // Down
                        currentCell = newCell;
                        board[currentCell].status[0] = true; // Up
                    }
                }
                else
                {
                    if (newCell + 1 == currentCell)
                    {
                        board[currentCell].status[3] = true; // Left
                        currentCell = newCell;
                        board[currentCell].status[2] = true; // Right
                    }
                    else
                    {
                        board[currentCell].status[0] = true; // Up
                        currentCell = newCell;
                        board[currentCell].status[1] = true; // Down
                    }
                }
            }
        }

        GenerateDungeon();
    }

    List<int> CheckNeighbors(int cell)
    {
        List<int> neighbors = new List<int>();

        if (cell - size.x >= 0 && !board[(cell - size.x)].visited)
        {
            neighbors.Add((cell - size.x));
        }

        if (cell + size.x < board.Count && !board[(cell + size.x)].visited)
        {
            neighbors.Add((cell + size.x));
        }

        if ((cell + 1) % size.x != 0 && !board[(cell + 1)].visited)
        {
            neighbors.Add((cell + 1));
        }

        if (cell % size.x != 0 && !board[(cell - 1)].visited)
        {
            neighbors.Add((cell - 1));
        }

        return neighbors;
    }
}