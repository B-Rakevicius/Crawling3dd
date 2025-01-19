using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public class Cell
    {
        public bool visited = false;
        public bool isPartOfRoom = false;
        public bool[] status = new bool[4];
        public bool isBlocked = false;
        public Vector2Int parentCell = Vector2Int.zero;
    }

    [System.Serializable]
    public class Rule
    {
        public GameObject room;
        public Vector2Int minPosition;
        public Vector2Int maxPosition;
        public Vector3Int size = Vector3Int.one;
        public bool isLShaped = false;
        public bool obligatory;

        public int ProbabilityOfSpawning(int x, int y)
        {
            // 0 - cannot spawn 1 - can spawn 2 - HAS to spawn

            if (x>= minPosition.x && x<=maxPosition.x && y >= minPosition.y && y <= maxPosition.y)
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

    List<Cell> board;

    // Start is called before the first frame update
    void Start()
    {
        InitializeBoard();
        //BlockCells();
        EnsureConnectivity();
        GenerateDungeon();
        //MazeGenerator();
    }
    void InitializeBoard()
    {
        board = new List<Cell>();

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                board.Add(new Cell());
            }
        }
    }
    void BlockCells()
    {
        int totalCells = size.x * size.y;
        int maxBlockedCells = (int)(totalCells * 0.3f); // No more than 20% of cells blocked
        int blockedCount = 0;

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                Cell cell = board[i + j * size.x];


                // Block corners by default
                if ((i == 0 && j == 0) || (i == 0 && j == size.y - 1) || (i == size.x - 1 && j == 0) || (i == size.x - 1 && j == size.y - 1))
                {
                    cell.isBlocked = true;
                    blockedCount++;
                    continue;
                }

                // Block edges (70% chance)
                if (i == 0 || j == 0 || i == size.x - 1 || j == size.y - 1)
                {
                    if (Random.value < 0.2f && blockedCount < maxBlockedCells)
                    {
                        cell.isBlocked = true;
                        blockedCount++;
                    }
                    continue;
                }

                // Randomly block other cells (10% chance)
                if (Random.value < 0.2f && blockedCount < maxBlockedCells)
                {
                    cell.isBlocked = true;
                    blockedCount++;
                }
                
            }
        }
    }

    void EnsureConnectivity()
    {
        // Perform a flood-fill to find all connected cells
        HashSet<int> connectedCells = new HashSet<int>();
        Queue<int> toVisit = new Queue<int>();

        // Start flood-fill from the first unblocked cell
        for (int i = 0; i < board.Count; i++)
        {
            if (!board[i].isBlocked)
            {
                toVisit.Enqueue(i);
                break;
            }
        }

        while (toVisit.Count > 0)
        {
            int current = toVisit.Dequeue();
            if (connectedCells.Contains(current) || board[current].isBlocked)
                continue;

            connectedCells.Add(current);

            // Add neighbors to the queue
            foreach (int neighbor in GetNeighbors(current))
            {
                if (!connectedCells.Contains(neighbor) && !board[neighbor].isBlocked)
                {
                    toVisit.Enqueue(neighbor);
                }
            }
        }
        /*
        // Block any cells not in the connected set
        for (int i = 0; i < board.Count; i++)
        {
            if (!connectedCells.Contains(i))
            {
                board[i].isBlocked = true;
            }
        }
        */
    }

    List<int> GetNeighbors(int cellIndex)
    {
        List<int> neighbors = new List<int>();

        int x = cellIndex % size.x;
        int y = cellIndex / size.x;

        // Check up, down, left, right
        if (y > 0) neighbors.Add(cellIndex - size.x);      // Up
        if (y < size.y - 1) neighbors.Add(cellIndex + size.x); // Down
        if (x > 0) neighbors.Add(cellIndex - 1);          // Left
        if (x < size.x - 1) neighbors.Add(cellIndex + 1);     // Right

        return neighbors;
    }

    void GenerateDungeon()
    {
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                Cell currentCell = board[i + j * size.x];
                if (currentCell.isBlocked || currentCell.isPartOfRoom)
                    continue;

                int randomRoom = -1;
                List<int> availableRooms = new List<int>();

                for (int k = 0; k < rooms.Length; k++)
                {
                    int p = rooms[k].ProbabilityOfSpawning(i, j);

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

                Rule selectedRoom = rooms[randomRoom];

                // Check if the room can fit here
                if (CanPlaceRoom(i, j, selectedRoom))
                {
                    PlaceRoom(i, j, selectedRoom);
                }
            }
        }
    }

    bool CanPlaceRoom(int x, int y, Rule room)
    {
        for (int dx = 0; dx < room.size.x; dx++)
        {
            for (int dy = 0; dy < room.size.y; dy++)
            {
                int targetX = x + dx;
                int targetY = y + dy;

                if (targetX >= size.x || targetY >= size.y)
                    return false;

                if (board[targetX + targetY * size.x].visited || board[targetX + targetY * size.x].isBlocked || board[targetX + targetY * size.x].isPartOfRoom)
                    return false;
            }
        }
        return true;
    }

    void PlaceRoom(int x, int y, Rule room)
    {
        for (int dx = 0; dx < room.size.x; dx++)
        {
            for (int dy = 0; dy < room.size.y; dy++)
            {
                int targetX = x + dx;
                int targetY = y + dy;
                Cell cell = board[targetX + targetY * size.x];

                cell.visited = true;
                cell.isPartOfRoom = true;
                cell.parentCell = new Vector2Int(x, y);
            }
        }

        Vector3 roomPosition = new Vector3(x * offset.x, 0, -y * offset.y);
        GameObject newRoom = Instantiate(room.room, roomPosition, Quaternion.identity, transform);
        //newRoom.transform.localScale = new Vector3(1, 1, 1);
        newRoom.name += $" {x}-{y}";
    }
    void GenerateDungeonOld()
    {

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                Cell currentCell = board[(i + j * size.x)];
                if (currentCell.visited)
                {
                    int randomRoom = -1;
                    List<int> availableRooms = new List<int>();

                    for (int k = 0; k < rooms.Length; k++)
                    {
                        int p = rooms[k].ProbabilityOfSpawning(i, j);

                        if(p == 2)
                        {
                            randomRoom = k;
                            break;
                        } else if (p == 1)
                        {
                            availableRooms.Add(k);
                        }
                    }

                    if(randomRoom == -1)
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


                    var newRoom = Instantiate(rooms[randomRoom].room, new Vector3(i * offset.x, 0, -j * offset.y), Quaternion.identity, transform).GetComponent<RoomBehaviour>();
                    //var newRoom = Instantiate(rooms[randomRoom].room, new Vector3(i * offset.x, 0, -j * offset.y), Quaternion.identity, transform).GetComponent<RoomBehaviour>();
                    //newRoom.UpdateRoom(currentCell.status);
                    newRoom.name += " " + i + "-" + j;

                }
            }
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

        while (k<1000)
        {
            k++;

            board[currentCell].visited = true;

            if(currentCell == board.Count - 1)
            {
                break;
            }

            //Check the cell's neighbors
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

                if (newCell > currentCell)
                {
                    //down or right
                    if (newCell - 1 == currentCell)
                    {
                        board[currentCell].status[2] = true;
                        currentCell = newCell;
                        board[currentCell].status[3] = true;
                    }
                    else
                    {
                        board[currentCell].status[1] = true;
                        currentCell = newCell;
                        board[currentCell].status[0] = true;
                    }
                }
                else
                {
                    //up or left
                    if (newCell + 1 == currentCell)
                    {
                        board[currentCell].status[3] = true;
                        currentCell = newCell;
                        board[currentCell].status[2] = true;
                    }
                    else
                    {
                        board[currentCell].status[0] = true;
                        currentCell = newCell;
                        board[currentCell].status[1] = true;
                    }
                }

            }

        }
        GenerateDungeonOld();
    }

    List<int> CheckNeighbors(int cell)
    {
        List<int> neighbors = new List<int>();

        //check up neighbor
        if (cell - size.x >= 0 && !board[(cell-size.x)].visited)
        {
            neighbors.Add((cell - size.x));
        }

        //check down neighbor
        if (cell + size.x < board.Count && !board[(cell + size.x)].visited)
        {
            neighbors.Add((cell + size.x));
        }

        //check right neighbor
        if ((cell+1) % size.x != 0 && !board[(cell +1)].visited)
        {
            neighbors.Add((cell +1));
        }

        //check left neighbor
        if (cell % size.x != 0 && !board[(cell - 1)].visited)
        {
            neighbors.Add((cell -1));
        }

        return neighbors;
    }
}
