using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public class Cell
    {
        public bool visited = false;
        public bool[] status = new bool[4]; // transform into xx binary digits or change 4 into 6
    }

    /*[System.Serializable]
    public class Rule
    {
        public GameObject room;
        public Vector2Int minPosition;
        public Vector2Int maxPosition;

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

    }*/

    public Vector2Int size;
    public Vector2Int startPos = new Vector2Int(0,0);
    public GameObject room;
    //public Rule[] rooms;
    public Vector2 offset; //distance between each room inside the GenerateDungeon

    List<List<Cell>> board = new List<List<Cell>>();

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
                /*var newRoom = Instantiate(room, new Vector3(i * offset.x, 0, -j * offset.y), Quaternion.identity, transform).GetComponent<RoomBehaviour>();
                newRoom.UpdateRoom(board[Mathf.FloorToInt(i+j*size.x)].status);
                newRoom.name += " " + i + "-" + j;*/
                //Cell currentCell = board[(i + j * size.x)];
                Cell currentCell = board[i][j];
                Vector2Int currentPos = new Vector2Int(i,j);
                if (currentCell.visited)
                {
                    /*int randomRoom = -1;
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
                    }*/


                    Vector3 new_pos = new Vector3(i * offset.x, 0, -j * offset.y);
                    print(new_pos);
                    var newRoom = Instantiate(room, new_pos, Quaternion.identity, transform).GetComponent<RoomBehaviour>();
                   // var newRoom = Instantiate(rooms[randomRoom].room, new Vector3(i * offset.x, 0, -j * offset.y), Quaternion.identity, transform).GetComponent<RoomBehaviour>();
                    newRoom.UpdateRoom(currentCell.status);
                    newRoom.name += " " + i + "-" + j;

                }
            }
        }

    }

    void MazeGenerator()
    {
        board = new List<List<Cell>>();

        for (int i = 0; i < size.x; i++)
        {
            List<Cell> board1 = new List<Cell>();
            for (int j = 0; j < size.y; j++)
            {

                    board1.Add(new Cell());
            }
            board.Add(board1);
        }


        Vector2Int currentPos = startPos;

        Stack<Vector2Int> path = new Stack<Vector2Int>();
        Stack<Stack<Vector2Int>> paths = new Stack<Stack<Vector2Int>>();
        int m_nVisitedCells = 1;
        bool getNewPath = true;

        while (m_nVisitedCells < 5000)
        {
            int pos_x = currentPos[0];
            int pos_y = currentPos[1];

            board[pos_x][pos_y].visited = true;

            if (currentPos[0]==size.x-1 && currentPos[1]==size.y-1)
            {
                break;
            }

            //Check the cell's neighbors
            List<Vector2Int> neighbors = CheckNeighbors(currentPos);

            if (neighbors.Count == 0)
            {
                if (path.Count == 0)
                {
                    break;
                }
                else
                {
                    if (getNewPath == true)
                    {
                        paths.Push(path);
                        getNewPath = false; // start backtracking
                    }
                    currentPos = path.Pop();
                }
            }
            else
            {
                getNewPath = true;
                path.Push(currentPos);

                //select random cell from neighbors as the next cell
                // causes of no complete path
                Vector2Int newCell_index = neighbors[Random.Range(0, neighbors.Count)];
                int nx = newCell_index[0];
                int ny = newCell_index[1];
                int cx = currentPos[0];
                int cy = currentPos[1];

                // neighbors check
                if (nx>cx && ny == cy && nx - 1 == cx)
                {//right
                    board[cx][cy].status[2] = true; //status[2]--current cell open at right
                    currentPos = new Vector2Int(nx, ny);
                    board[nx][ny].status[3] = true;

                }
                else if (ny>cy && nx == cx && ny - 1 == cy)
                {
                    //down
                    board[cx][cy].status[1] = true;
                    currentPos = new Vector2Int(nx, ny);
                    board[nx][ny].status[0] = true;
                }
                else if (ny == cy && nx + 1 == cx)
                {
                    //left
                    board[cx][cy].status[3] = true;
                    currentPos = new Vector2Int(nx, ny);
                    board[nx][ny].status[2] = true;
                }
                else {
                    //up
                    board[cx][cy].status[0] = true;
                    currentPos = new Vector2Int(nx, ny);
                    board[nx][ny].status[1] = true;
                }
                m_nVisitedCells++;
            }
            
        }
        PrintListPaths(paths);
        GenerateDungeon();
    }

    public static void PrintListPaths(Stack<Stack<Vector2Int>> s)
    {
        if (s.Count == 0)
        {
            return;
            // Debug.Log("count zero");
        }

        Stack<Vector2Int> x = s.Peek();

        s.Pop();

        PrintStack(x);
    }

    public static void PrintStack(Stack<Vector2Int> s)
    {
        // If stack is empty
        if (s.Count == 0)
            return;

        // Extract top of the stack
        Vector2Int p = s.Peek();

        // Pop the top element
        s.Pop();

        // Print the current top
        // of the stack i.e., x
        Debug.Log("x:" + p[0] + " y:"+ p[1] + ";");

        // Proceed to print
        // remaining stack
        PrintStack(s);

        // Push the element back
        s.Push(p);
    }

    // if cell is not visited, add it to the neighbors[]
    List<Vector2Int> CheckNeighbors(Vector2Int cpos)
    {
        int c_x = cpos[0];
        int c_y = cpos[1];
        List<Vector2Int> neighbors = new List<Vector2Int>();
        //int count = board.Count * board[0].Count;
        
        //check left neighbor
        if (c_x-1>=0 && !board[c_x - 1][c_y].visited)
        {
            neighbors.Add(new Vector2Int(c_x-1, c_y)); //add position of neighbours
        }

        //check right neighbor
        if (c_x + 1 < size.x && !board[c_x + 1][c_y].visited)
        {
            neighbors.Add(new Vector2Int(c_x+1, c_y));
        }

        //check down neighbor
        if ((c_y + 1) < size.y && !board[c_x][c_y+1].visited)
        {
            neighbors.Add(new Vector2Int(c_x, c_y + 1));
        }

        //check up neighbor
        if (c_y - 1>=0 && !board[c_x][c_y - 1].visited)
        {
            neighbors.Add(new Vector2Int(c_x, c_y - 1));
        }
        //print(neighbors.Count);
        return neighbors;
    }
}