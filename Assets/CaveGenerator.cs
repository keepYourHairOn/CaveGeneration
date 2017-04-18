using UnityEngine;
using System.Collections;

public class CaveGenerator : MonoBehaviour {

    /// <summary>
    ///  Set of cell types.
    /// </summary>
    private enum CellType { Rock = 1, Floor = 2, Wall = 3};

    /// <summary>
    /// Width of the map.
    /// </summary>
    protected int width = 150;
    /// <summary>
    /// Height of the map.
    /// </summary>
    protected int height = 150;
    /// <summary>
    /// Color scheme of the map.
    /// </summary>
    protected Color32[] colors;
    /// <summary>
    /// Texture set to apply for the grid.
    /// </summary>
    protected Texture2D texture;
    /// <summary>
    /// Number of iterations of the CA.
    /// </summary>
    protected int N = 8;
    /// <summary>
    /// Percentage of rocks.
    /// </summary>
    protected double R = 0.5;
    /// <summary>
    /// Neighborhood value threshold.
    /// </summary>
    protected int T = 5;
    /// <summary>
    /// Grids resolution.
    /// </summary>
    protected int gridX = 50;
    protected int gridY = 50;
    /// <summary>
    /// Central grid of the map.
    /// </summary>
    protected Cell[,] baseGrid;
    /// <summary>
    /// Cave map.
    /// </summary>
    protected Cell[,] map;

    /// <summary>
    /// Cells of the grid.
    /// </summary>
    private class Cell
    {
        int type;
        int group;
        int neighbourVal;

        public int Type
        {
            get { return type; }
            set { type = value; }
        }

        public int Group
        {
            get { return group; }
            set { group = value; }
        }

        public int NeighbourVal
        {
            get { return neighbourVal; }
            set { neighbourVal = value; }
        }

    }

    /// <summary>
    /// Method for inititalization of Unity object
    /// </summary>
	void Start () {

        texture = new Texture2D(width, height);
        GetComponent<Renderer>().material.mainTexture = texture;
        colors = new Color32[width * height];


        baseGrid = CreateBaseGrid();
        map = new Cell[width, height];
        CreateCaveMap();
        map = AddWalls(map);
        //map = baseGrid;
        Draw();

        System.IO.StreamWriter file = new System.IO.StreamWriter("D:\\Education\\4course\\PCG\\speed.txt");
        file.WriteLine("Speed of Cave Generation algorithm: " + (Time.realtimeSinceStartup * 1000));

        file.Close();

        texture.SetPixels32(colors);
        texture.Apply();

	}

    /// <summary>
    /// Update object.
    /// </summary>
	void Update () {
	
	}



    /// <summary>
    /// Creates Base grid of the cave map.
    /// </summary>
    /// <returns>Array with cells of the floor type.</returns>
    protected Cell[,] CreateBaseGrid()
    {
        Cell[,] grid = new Cell[gridX, gridY];
        for (int i = 0; i < gridX; i++)
        {
            for (int j = 0; j < gridY; j++)
            {
                grid[i, j] = new Cell();
                grid[i, j].Type = (int)CellType.Floor;
            }
        }

        grid = distributeRocks(grid);
        grid = CellularAutomata(grid);

        return grid;
    }

    /// <summary>
    /// Randomply distribute rocks along the grid.
    /// </summary>
    /// <param name="grid">Array with cells.</param>
    /// <returns>Array of cells with added rocks.</returns>
    protected Cell[,] distributeRocks(Cell[,] grid)
    {
        int maxRocks = (int)(grid.GetLength(0) * grid.GetLength(1) * R);
        int count = 0;
        int x = 0;
        int y = 0;

        while (count < maxRocks)
        {
            x = Random.Range(0, grid.GetLength(0) - 1);
            y = Random.Range(0, grid.GetLength(1) - 1);
            if (grid[x, y].Type != 1)
            {
                grid[x, y].Type = (int)CellType.Rock;
                count++;
            }
        }

        return grid;
    }

    /// <summary>
    /// Iterated cellular automata algorithm.
    /// Examins relationship of floor and rock cells.
    /// Assign neighbourhood value to each cell.
    /// </summary>
    /// <param name="grid">Array with cells.</param>
    /// <returns>Array of cells with neighbourhood values.</returns> 
    protected Cell[,] CellularAutomata(Cell[,] grid)
    {
        for (int i = 0; i < N; i++)
        {
            for (int j = 0; j < grid.GetLength(0); j++)
            {
                for (int k = 0; k < grid.GetLength(1); k++)
                {
                    int neighbourhoovValue = NeighborhoodValue(grid, j, k, 1);
                    grid[j, k].NeighbourVal = neighbourhoovValue;
                    if (neighbourhoovValue >= T)
                    {
                        grid[j, k].Type = (int)CellType.Rock;
                    }
                    else
                    {
                        grid[j, k].Type = (int)CellType.Floor;
                    }

                }
            }
        }
        

        return grid;
    }

    /// <summary>
    /// Calculates neighbourhood value of the cell.
    /// </summary>
    /// <param name="grid">Array of the cells.</param>
    /// <param name="x">Coordinate of the cell on the x axis.</param>
    /// <param name="y">Coordinate of the cell on the y axis.</param>
    /// <param name="r">Range to check on the axis.</param>
    /// <returns>Number of neighbor rocks.</returns> 
    protected int NeighborhoodValue(Cell[,] grid, int x, int y, int r)
    {
        int startX = x - r;
        int startY = y - r;
        int endX = x + (r);
        int endY = y + (r);

        int iX = startX;
        int iY = startY;

        int rockCounter = 0;

        for (iY = startY; iY <= endY; iY++)
        {
            for (iX = startX; iX <= endX; iX++)
            {
                if (!(iX == x && iY == y))
                {
                    if (IsOutOfBounds(iX, iY, grid.GetLength(0), grid.GetLength(1)) == false)
                    {
                        if (grid[iX, iY].Type == 1)
                        {
                            rockCounter += 1;
                        }
                    }
                }
            }
        }
        return rockCounter;
    }


    /// <summary>
    /// Checks wheather index is out of bounda of the array.
    /// </summary>
    /// <param name="x">Index of the cell on x axis.</param>
    /// <param name="y">Index of the cell on y axis.</param>
    /// <param name="gridWidth">Maximal value of x axis.</param>
    /// <param name="gridHeight">Maximal value of the y axis.</param>
    /// <returns>Returns array formed from 8 grids.</returns>
    protected bool IsOutOfBounds(int x, int y, int gridWidth, int gridHeight)
    {
        if (x < 0 || y < 0)
        {
            return true;
        }
        else if (x > gridWidth - 1 || y > gridHeight - 1)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Creates cave map of defined width and height.
    /// </summary>
    /// <returns>Returns array formed from 8 grids.</returns>
    protected Cell[,] CreateCaveMap()
    {
        Cell cell;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if ((i < gridX || i >= (gridX * 2)) && (j < gridY || j >= (gridY * 2)))
                {
                    
                    cell = new Cell();
                    cell.Type = (int)CellType.Floor;
                    map[i, j] = cell;
                    
                }
                else
                {
                    int iX = i - gridX;
                    int iY = j - gridY;
                    if (IsOutOfBounds(iX, iY, baseGrid.GetLength(0), baseGrid.GetLength(1)) == false)
                    {
                        cell = baseGrid[i - gridX, j - gridY];
                        map[i, j] = cell;
                    }
                    else
                    {
                        cell = new Cell();
                        cell.Type = (int)CellType.Floor;
                        map[i, j] = cell;
                    }
                }
            }
        }

        map = distributeRocks(map);

        return map;
    }

    /// <summary>
    /// Add walls to the grid.
    /// </summary>
    /// <param name="grid">Array of cells.</param>
    /// <returns>Array of cells with walls added.</returns>
    protected Cell[,] AddWalls(Cell[,] grid)
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Cell cell = grid[i, j];
                // If cell is a rock and has at least 1 floor neighbour.
                if (cell.NeighbourVal < 8 && cell.Type == 1)
                {
                    grid[i, j].Type = (int)CellType.Wall;
                }
            }
        }

        return grid;
    }

    /// <summary>
    /// Creates Base grid of the cave map.
    /// </summary>
    /// <param name="basic">Array of the initial heights.</param>
    /// <param name="octave">Number of noizes to glue up together.</param>
    /// <returns>Array with cells of the floor type.</returns>
 
 

    /// <summary>
    /// Assign a color to cocrete point on the base of it's type.
    /// </summary>
    /// <param name="type">Type of the cell.</param>
    /// <returns>A color based on the cell's type.</returns>
    public Color GetColor(int type)
    {
        if (type == 1)
        {
            // Rock type.
            return Color.black;
        }
        else if (type == 2)
        {
            // Floor type.
            return Color.white;

        }
        else
        {
            // Wall type.
            return Color.gray;

        }
    }

    /// <summary>
    /// Forms a colors map of the cave.
    /// </summary>
    public void Draw()
    {
        int colorsNumber = 0;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                colors[colorsNumber] = GetColor(map[i, j].Type);
                colorsNumber++;
            }
        }

    }
}

