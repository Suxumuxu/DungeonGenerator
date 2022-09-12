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

    //added, do we want all our grid to be utilized
    public bool useWholeGrid;

    List<Cell> board = new List<Cell>();


    List<bool> visitedChecks = new List<bool>();

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
                
                //go through the cells of our board, column 0, column 1 and column 2
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

                    var newRoom = Instantiate(rooms[randomRoom].room, new Vector3(i * offset.x, 0, -j * offset.y), Quaternion.identity, transform).GetComponent<RoomBehaviour>();
                    newRoom.UpdateRoom(currentCell.status);
                    newRoom.name += " " + i + "-" + j;

                }
            }
        }

    }


    void MazeGenerator()
    {
        int useWholeGridCheck = 0;
        // create the board here
        // board = new List<Cell>();
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                board.Add(new Cell());
                visitedChecks.Add(false);
            }
        }

        //for (int p = 0; p < visitedChecks.Count; p++)
        //{
        //    Debug.Log(visitedChecks[p]);
        //}

        //Debug.Log(countBools(visitedChecks,false));

        //Debug.Log(board.Count);

        int currentCell = startPos;

        Stack<int> path = new Stack<int>();

        //custom list that contains the whole sequence of rooms visited by our DFS algorithm
        List<int> wholePath = new List<int>();

        int k = 0;

        while (k < 150)
        {
            k++;

            //mark current cell object as visited 
            board[currentCell].visited = true;

            wholePath.Add(currentCell);

            //if useWholeGrid is checked 
            if (useWholeGrid==false)
            {
                useWholeGridCheck = 1;
            }
            else
            {
                useWholeGridCheck = 0;
            }
            
            //if current cell is pointing at the last Cell of our board
            //Break!
            if (currentCell == board.Count-useWholeGridCheck)

            {
                break;
            }

            //Define a list called neighbors that contains the number of neighboring cells available
            //Check the cell's neighbors
            List<int> neighbors = CheckNeighbors(currentCell);


            //Debug.Log("i:" + currentCell + "n:" + neighbors.Count) ;


            //if no neighbors are returned
            if (neighbors.Count == 0)
            {
                //and path doesnt have elements
                if (path.Count == 0)
                {
                    break;
                }
                //if path has elements, go back to previous cell
                else
                {
                    currentCell = path.Pop();
                }
            }
            //if we have at least a neighbor
            else
            {
                path.Push(currentCell);

                //pick a random neighbor from the available neighbors returned from the list
                int newCell = neighbors[Random.Range(0, neighbors.Count)];


                // we check now that
                // the exit door of the currentCell will be open == 1 
                // and the entrance door of the newCell also set to == 1
                // + and we assign here as current current cell our new cell 
                
                // right or down
                if (newCell > currentCell)
                {
                    //right
                    if (newCell - 1 == currentCell)
                    {
                        board[currentCell].status[2] = true;
                        currentCell = newCell;
                        board[currentCell].status[3] = true;
                    }
                    //down
                    else
                    {
                        board[currentCell].status[1] = true;
                        currentCell = newCell;
                        board[currentCell].status[0] = true;
                    }
                }
                //up or left
                else
                {
                    //left
                    if (newCell + 1 == currentCell)
                    {
                        board[currentCell].status[3] = true;
                        currentCell = newCell;
                        board[currentCell].status[2] = true;
                    }
                    //up
                    else
                    {
                        board[currentCell].status[0] = true;
                        currentCell = newCell;
                        board[currentCell].status[1] = true;
                    }
                }

            }

            visitedChecks[currentCell] = true;

            Debug.Log(countBools(visitedChecks,true));

        }


        //for (int p = 0; p<wholePath.Count;p++)
        //{
        //Debug.Log(wholePath[p]);
        //}

        //Debug.Log(k);

        string str = "";

        for (int p = 0; p< visitedChecks.Count;p++)
        {
            str += visitedChecks[p] + ",";
            //Debug.Log(visitedChecks[p]);
        }

        Debug.Log(str);





        GenerateDungeon();
    }

    List<int> CheckNeighbors(int cell)
    {
        List<int> neighbors = new List<int>();

        //check up neighbor, if index is greater than size.x we are not on the first row && check the cell above us if is visited
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

    public static int countBools(List<bool> array, bool flag)
    {
        int value = 0;

        for (int i = 0; i < array.Count; i++)
        {
            if (array[i] == flag) 
                
                value++;
        }

        return value;
    }

}
