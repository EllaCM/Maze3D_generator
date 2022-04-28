import java.util.Stack;
import java.util.Random;

int OFF_MAX = 300;
int step_size = 50;
int num = 2*OFF_MAX/step_size+1;
CellClass[][][] maze = new CellClass[num][num][num];
void setup(){
  size (1080, 1060, P3D);
  defineCells();
}

void draw(){
  background(0);
  translate(width/2, height/2, -OFF_MAX); //create a box
  rotateX(frameCount*.01); //rotate the entire maze overtime
  rotateY(frameCount*.01);
  rotateZ(frameCount*.01);
  for (int i = 0; i < maze.length; i++) {
    for (int j = 0; j < maze[i].length; j++) {
      for (int k = 0; k < maze[i][j].length; k++) {
        if(maze[i][j][k].visited){
          maze[i][j][k].show();
        }
      }
    }
  }
  
  
}

void defineCells() {
  // define Cells
  for (int i = 0; i < maze.length; i++) {
    for (int j = 0; j < maze[i].length; j++) {
      for (int k = 0; k < maze[i][j].length; k++) {
        int x0 = -OFF_MAX+i*step_size;
        int y0 = -OFF_MAX+j*step_size;
        int z0 = -OFF_MAX+k*step_size;
        // create a Cell        
        maze[i][j][k] = new CellClass(x0,y0,z0);
      }
    }
  } // for
  mazeGenerator();
}

class CellClass {
  // this class represents one Box / Cell
  boolean visited;
  int x;
  int y;
  int z;
  boolean[] status = new boolean[6];

  // constr
  CellClass(int x_, int y_, int z_) {
    visited = false;
    x = x_; 
    y = y_; 
    z = z_;
    for(int i =0; i<status.length; i++){
      status[i] = false;
    }
    
  } // constr

  // substitute door status with diff 3d models
  void show(){
    pushMatrix();
    translate(x,y,z);
    // To make it colorful:
    // fill(colorFromOffset(x),colorFromOffset(y),colorFromOffset(z));
    box(30); //each cube's size
    popMatrix();
  }
  int colorFromOffset(int offset){
    return (int)((offset  + OFF_MAX)/(2.8*OFF_MAX)*255);
  }
}

void mazeGenerator(){
  //PVector v = new PVector(0,0,0);
  int[] start = {0,0,0};
  int[] currentCell = start;
  int m_nVisitedCells = 1;
  Stack<int[]> path = new Stack<int[]>();
  while (m_nVisitedCells < 1000)
        {
            int pos_x = currentCell[0];
            int pos_y = currentCell[1];
            int pos_z = currentCell[2];

            maze[pos_x][pos_y][pos_z].visited = true;

            if (currentCell[0]==num-1 && currentCell[1]==num-1 && currentCell[2] == num - 1)
            {
                break;
            }

            //Check the cell's neighbors
            ArrayList<int[]> neighbors = CheckNeighbors(currentCell);

            if (neighbors.size() == 0)
            {
                if (path.size() == 0)
                {
                    break;
                }
                else
                {
                    /*if (getNewPath == true)
                    {
                        paths.Push(path);
                        getNewPath = false; // start backtracking
                    }*/
                    currentCell = path.pop();
                }
            }
            else
            {
                //getNewPath = true;
                path.push(currentCell);

                //select random cell from neighbors as the next cell
                // causes of no complete path    
                Random random = new Random();
                int[] newCell_index = neighbors.get(random.nextInt(neighbors.size()));
                int nx = newCell_index[0];
                int ny = newCell_index[1];
                int nz = newCell_index[2];
                
                // neighbors check
                if (nx>pos_x && ny == pos_y && nz == pos_z && nx - 1 == pos_x)
                {//east
                    maze[pos_x][pos_y][pos_z].status[2] = true; //status[2]--current cell open at right
                    currentCell = new int[] {nx, ny, nz};
                    maze[nx][ny][nz].status[3] = true;

                }
                else if (ny>pos_y && nx == pos_x && nz == pos_z && ny - 1 == pos_y)
                {
                    //south
                    maze[pos_x][pos_y][pos_z].status[1] = true;
                    currentCell = new int[] {nx, ny, nz};
                    maze[nx][ny][nz].status[3] = true;
                }
                else if (ny == pos_y && nz==pos_z && nx + 1 == pos_x)
                {
                    //west
                    maze[pos_x][pos_y][pos_z].status[3] = true;
                    currentCell = new int[] {nx, ny, nz};
                    maze[nx][ny][nz].status[2] = true;
                }
                else if (nx == pos_x && nz == pos_z && ny + 1 == pos_y){
                    //north
                    maze[pos_x][pos_y][pos_z].status[0] = true;
                    currentCell = new int[] {nx, ny, nz};
                    maze[nx][ny][nz].status[1] = true;
                }
                else if (nx == pos_x && ny == pos_y && nz - 1 == pos_z)
                {
                    //up
                    maze[pos_x][pos_y][pos_z].status[5] = true;
                    currentCell = new int[] {nx, ny, nz};
                    maze[nx][ny][nz].status[4] = true;
                }
                else
                {
                    //down
                    maze[pos_x][pos_y][pos_z].status[4] = true;
                    currentCell = new int[] {nx, ny, nz};
                    maze[nx][ny][nz].status[5] = true;
                }
                m_nVisitedCells++;
            }
            
        }
}

ArrayList<int[]> CheckNeighbors(int[] cpos){
        int c_x = cpos[0];
        int c_y = cpos[1];
        int c_z = cpos[2];
        ArrayList<int[]> neighbors = new ArrayList<int[]>();
        
        //check west neighbor
        if (c_x-1>=0 && !maze[c_x - 1][c_y][c_z].visited)
        {
            neighbors.add(new int[] {c_x-1, c_y, c_z}); //add position of neighbours
        }

        //check east neighbor
        if (c_x + 1 < num && !maze[c_x + 1][c_y][c_z].visited)
        {
            neighbors.add(new int[] {c_x+1, c_y, c_z});
        }

        //check south neighbor
        if ((c_y + 1) < num && !maze[c_x][c_y+1][c_z].visited)
        {
            neighbors.add(new int[] {c_x, c_y + 1, c_z});
        }

        //check north neighbor
        if (c_y - 1>=0 && !maze[c_x][c_y - 1][c_z].visited)
        {
            neighbors.add(new int[] {c_x, c_y - 1, c_z});
        }

        //check up neighbor
        if (c_z + 1 < num && !maze[c_x][c_y][c_z + 1].visited)
        {
            neighbors.add(new int[] {c_x, c_y, c_z + 1});
        }
        //check down neighbor
        if (c_z - 1 >= 0 && !maze[c_x][c_y][c_z - 1].visited)
        {
            neighbors.add(new int[] {c_x, c_y, c_z - 1});
        }
        return neighbors;
}
