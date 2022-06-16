//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class DungeonGenerator : MonoBehaviour
//{
//    public class Cell
//    {
//        public bool visited = false;
//        public bool[] status = new bool[4];
//    }

//    public Vector2 size;
//    public int startPos = 0;
//    public GameObject room;
//    public Vector2 offset;

//    private List<Cell> board = new List<Cell>();

//    void Start()
//    {
//        MazeGenerator();
//    }


//    void Update()
//    {

//    }

//    void GenerateDungeon()
//    {
//        for (int i = 0; i < size.x; i++)
//        {
//            for(int j = 0; j < size.y; j++)
//            {
//                Debug.Log(board[Mathf.FloorToInt(i + j * size.x)].status);

//                GameObject newRoom = Instantiate(room, new Vector3(i * offset.x, 0, -j * offset.y), Quaternion.identity, transform);
//                newRoom.GetComponent<RoomBehavior>().UpdateRoom(board[Mathf.FloorToInt(i + j * size.x)].status);
//            }
//        }
//    }

//    void MazeGenerator()
//    {
//        board = new List<Cell>();

//        for (int i = 0; i < size.x; i++)
//        {
//            for (int j = 0; j < size.y; j++)
//            {
//                board.Add(new Cell());
//            }
//        }

//        int currentCell = startPos;

//        Stack<int> path = new Stack<int>();

//        int k = 0;

//        while(k < 1000)
//        {
//            k++;

//            board[currentCell].visited = true;

//            List<int> neighbors =  CheckNeighbors(currentCell);

//            if(neighbors.Count == 0)
//            {
//                if(path.Count == 0)
//                {
//                    break;
//                }
//                else
//                {
//                    currentCell = path.Pop();
//                }
//            }
//            else
//            {
//                path.Push(currentCell);

//                int newCell = neighbors[Random.Range(0, neighbors.Count)];

//                if(newCell > currentCell)
//                {
//                    // down or right
//                    if(newCell - 1 == currentCell)
//                    {
//                        board[currentCell].status[2] = true;
//                        currentCell = newCell;
//                        board[currentCell].status[3] = true;
//                    }
//                    else
//                    {
//                        board[currentCell].status[1] = true;
//                        currentCell = newCell;
//                        board[currentCell].status[0] = true;
//                    }
//                }
//                else
//                {
//                    // up or left
//                    if (newCell + 1 == currentCell)
//                    {
//                        board[currentCell].status[3] = true;
//                        currentCell = newCell;
//                        board[currentCell].status[2] = true;
//                    }
//                    else
//                    {
//                        board[currentCell].status[0] = true;
//                        currentCell = newCell;
//                        board[currentCell].status[1] = true;
//                    }
//                }
//            }
//        }

//        GenerateDungeon();
//    }

//    List<int> CheckNeighbors(int cell)
//    {
//        List<int> neighbors = new List<int>();

//        // Check up neighbor
//        if(cell - size.x >= 0 && !board[Mathf.FloorToInt(cell - size.x)].visited)
//        {
//            neighbors.Add(Mathf.FloorToInt(cell - size.x));
//        }

//        // Check down neighbor
//        if (cell + size.x < board.Count && !board[Mathf.FloorToInt(cell + size.x)].visited)
//        {
//            neighbors.Add(Mathf.FloorToInt(cell + size.x));
//        }

//        // Check right neighbor
//        if ((cell + 1) % size.x != 0 && !board[Mathf.FloorToInt(cell + 1)].visited)
//        {
//            neighbors.Add(Mathf.FloorToInt(cell + 1));
//        }

//        // Check left neighbor
//        if (cell % size.x != 0 && !board[Mathf.FloorToInt(cell - 1)].visited)
//        {
//            neighbors.Add(Mathf.FloorToInt(cell - 1));
//        }

//        return neighbors;
//    }
//}


///////


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
        public Vector2 offset;

        public bool obligatory;

        public int ProbabilityOfSpawning(int x, int y)
        {
            // 0 - cannot spawn 1 - can spawn 2 - HAS to spawn

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

    List<Cell> board;

    // Start is called before the first frame update
    void Start()
    {
        MazeGenerator();
    }

    void GenerateDungeon()
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


                   
                    var newRoom = Instantiate(rooms[Random.Range(0,4)].room, new Vector3(i * rooms[randomRoom].offset.x, 0, -j * rooms[randomRoom].offset.y), Quaternion.identity, transform).GetComponent<RoomBehavior>();
                    newRoom.UpdateRoom(currentCell.status);
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

        while (k < 1000)
        {
            k++;

            board[currentCell].visited = true;

            if (currentCell == board.Count - 1)
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
        GenerateDungeon();
    }

    List<int> CheckNeighbors(int cell)
    {
        List<int> neighbors = new List<int>();

        //check up neighbor
        if (cell - size.x >= 0 && !board[(cell - size.x)].visited)
        {
            neighbors.Add((cell - size.x));
        }

        //check down neighbor
        if (cell + size.x < board.Count && !board[(cell + size.x)].visited)
        {
            neighbors.Add((cell + size.x));
        }

        //check right neighbor
        if ((cell + 1) % size.x != 0 && !board[(cell + 1)].visited)
        {
            neighbors.Add((cell + 1));
        }

        //check left neighbor
        if (cell % size.x != 0 && !board[(cell - 1)].visited)
        {
            neighbors.Add((cell - 1));
        }

        return neighbors;
    }
}