using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public class Cell
    {
        public bool visited = false;
        public bool[] status = new bool[6]; // transform into xx binary digits or change 4 into 6
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

    public Vector3Int size;
    public Vector3Int startPos = new Vector3Int(0, 0, 0);
    public GameObject room;
    //public Rule[] rooms;
    public Vector3 offset; //distance between each room inside the GenerateDungeon

    List<List<List<Cell>>> board = new List<List<List<Cell>>>();

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
                for (int k = 0; k < size.z; k++)
                {
                    /*var newRoom = Instantiate(room, new Vector3(i * offset.x, 0, -j * offset.y), Quaternion.identity, transform).GetComponent<RoomBehaviour>();
                    newRoom.UpdateRoom(board[Mathf.FloorToInt(i+j*size.x)].status);
                    newRoom.name += " " + i + "-" + j;*/
                    //Cell currentCell = board[(i + j * size.x)];
                    Cell currentCell = board[i][j][k];
                    Vector3Int currentPos = new Vector3Int(i, j, k);
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

                        Vector3 new_pos = new Vector3(i * offset.x, k * offset.z, -j * offset.y);
                        //print(new_pos);
                        var newRoom = Instantiate(room, new_pos, Quaternion.identity, transform).GetComponent<RoomBehaviour>();
                        newRoom.UpdateRoom(currentCell.status);
                        newRoom.name += " " + i + "-" + j + "-" + k;

                    }
                }
            }
        }

    }

    void MazeGenerator()
    {
        board = new List<List<List<Cell>>>();

        for (int i = 0; i < size.x; i++)
        {
            List<List<Cell>> board1 = new List<List<Cell>>();
            for (int j = 0; j < size.y; j++)
            {
                List<Cell> board2 = new List<Cell>();
                for (int k = 0; k < size.z; k++)
                {

                    board2.Add(new Cell());
                }
                board1.Add(board2);
            }
            board.Add(board1);
        }


        Vector3Int currentPos = startPos;

        Stack<Vector3Int> path = new Stack<Vector3Int>();
        Stack<Stack<Vector3Int>> paths = new Stack<Stack<Vector3Int>>();
        int m_nVisitedCells = 1;
        bool getNewPath = true;

        while (m_nVisitedCells < size.x*size.y*size.z)
        {
            int pos_x = currentPos[0];
            int pos_y = currentPos[1];
            int pos_z = currentPos[2];

            board[pos_x][pos_y][pos_z].visited = true;

            //Check the cell's neighbors
            List<Vector3Int> neighbors = CheckNeighbors(currentPos);

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
                Vector3Int newCell_index = neighbors[Random.Range(0, neighbors.Count)];
                int nx = newCell_index[0];
                int ny = newCell_index[1];
                int nz = newCell_index[2];
                int cx = currentPos[0];
                int cy = currentPos[1];
                int cz = currentPos[2];

                // neighbors check
                if (nx > cx && ny == cy && nz == cz && nx - 1 == cx)
                {//east
                    board[cx][cy][cz].status[2] = true; //status[2]--current cell open at right
                    currentPos = new Vector3Int(nx, ny, nz);
                    board[nx][ny][nz].status[3] = true;

                }
                else if (ny > cy && nx == cx && nz == cz && ny - 1 == cy)
                {
                    //south
                    board[cx][cy][nz].status[1] = true;
                    currentPos = new Vector3Int(nx, ny, nz);
                    board[nx][ny][nz].status[0] = true;
                }
                else if (ny == cy && nz == cz && nx + 1 == cx)
                {
                    //west
                    board[cx][cy][cz].status[3] = true;
                    currentPos = new Vector3Int(nx, ny, nz);
                    board[nx][ny][nz].status[2] = true;
                }
                else if (nx == cx && nz == cz && ny + 1 == cy)
                {
                    //north
                    board[cx][cy][cz].status[0] = true;
                    currentPos = new Vector3Int(nx, ny, nz);
                    board[nx][ny][nz].status[1] = true;
                }
                else if (nx == cx && ny == cy && nz - 1 == cz)
                {
                    //up
                    board[cx][cy][cz].status[5] = true;
                    currentPos = new Vector3Int(nx, ny, nz);
                    board[nx][ny][nz].status[4] = true;
                }
                else
                {
                    //down
                    board[cx][cy][cz].status[4] = true;
                    currentPos = new Vector3Int(nx, ny, nz);
                    board[nx][ny][nz].status[5] = true;
                }
                m_nVisitedCells++;
            }

        }
        PrintListPaths(paths);
        GenerateDungeon();
    }

    public static void PrintListPaths(Stack<Stack<Vector3Int>> s)
    {
        if (s.Count == 0)
        {
            return;
            // Debug.Log("count zero");
        }

        Stack<Vector3Int> x = s.Peek();

        s.Pop();

        PrintStack(x);
    }

    public static void PrintStack(Stack<Vector3Int> s)
    {
        // If stack is empty
        if (s.Count == 0)
            return;

        // Extract top of the stack
        Vector3Int p = s.Peek();

        // Pop the top element
        s.Pop();

        // Print the current top
        // of the stack i.e., x
        Debug.Log("x:" + p[0] + " y:" + p[1] + " z:" + p[2] + ";");

        // Proceed to print
        // remaining stack
        PrintStack(s);

        // Push the element back
        // s.Push(p);
    }

    // if cell is not visited, add it to the neighbors[]
    List<Vector3Int> CheckNeighbors(Vector3Int cpos)
    {
        int c_x = cpos[0];
        int c_y = cpos[1];
        int c_z = cpos[2];
        List<Vector3Int> neighbors = new List<Vector3Int>();
        //int count = board.Count * board[0].Count;

        //check west neighbor
        if (c_x - 1 >= 0 && !board[c_x - 1][c_y][c_z].visited)
        {
            neighbors.Add(new Vector3Int(c_x - 1, c_y, c_z)); //add position of neighbours
        }

        //check east neighbor
        if (c_x + 1 < size.x && !board[c_x + 1][c_y][c_z].visited)
        {
            neighbors.Add(new Vector3Int(c_x + 1, c_y, c_z));
        }

        //check south neighbor
        if ((c_y + 1) < size.y && !board[c_x][c_y + 1][c_z].visited)
        {
            neighbors.Add(new Vector3Int(c_x, c_y + 1, c_z));
        }

        //check north neighbor
        if (c_y - 1 >= 0 && !board[c_x][c_y - 1][c_z].visited)
        {
            neighbors.Add(new Vector3Int(c_x, c_y - 1, c_z));
        }

        //check up neighbor
        if (c_z + 1 < size.z && !board[c_x][c_y][c_z + 1].visited)
        {
            neighbors.Add(new Vector3Int(c_x, c_y, c_z + 1));
        }
        //check down neighbor
        if (c_z - 1 >= 0 && !board[c_x][c_y][c_z - 1].visited)
        {
            neighbors.Add(new Vector3Int(c_x, c_y, c_z - 1));
        }
        return neighbors;
    }
}