/*

    there are arrays of ints that are generated
        - the array is multidimensional, so it can have a width and height
        - https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/arrays/multidimensional-arrays
    
    the mainGrid is for storing mine positions
        - 0 means no mine, 1 means mine
    the mineNumberGrid is for storing the number of mines around a position on the board
    the tileObjectGrid is just for connecting a position on the mainGrid to a tile GameObject

    firstly, the tile GameObjects are instantiated
        - tin order according to gridSize
        - the tileObjectGrid gets a reference to each tile
    this is for ensuring later on that the player won't click on a tile with a mine on their first go 
    
    once a tile is clicked, this script gets called to actually generate the mines
        - store the position of the player's clicked tile for later

    =============================

    GenerateMines()
        - create list of all Vector2Int positions possible with current gridSize
        - remove position of player's clicked tile and every position surrounding it
        
        - choose position randomly from list
        - set value of int in mainGrid in that position to 1
        - remove that position from the list after choosing it
        - continue n many times, n = mineCount

    GenerateMineNumbers()
        - use int (mineNumber) for counting mines found
        - iterate through each int (current int) in mainGrid
        - iterate through each int surrounding the current int
            - basically: 
                take the xpos of the current int -1, 
                ypos of the current int +1,
                then add/minus respectively
                - this makes more sense if you look at the code itself, lol
            - if a mine is found (int == 1), increase mineNumber
        - edit mineNumberGrid with mineNumber at current int's position
        - reset mineNumber
        - continue n many times, n = tileCount

    AssignTileValues()
        - go through each tile GameObject in tileObjectGrid
        - assign each tile their respective value
    
    =============================

    EmptyTileClick()
        - when you click on a tile with no value, every tile around it would be revealed
        - this repeats when the new revealed tile also has no value
        - i don't know if this is the greatest idea for doing it

        - called by Click() from tile if it had no value
        - get player's clicked tile's position from Click()
        - iterate through each tile GameObject surrounding the current tile
        - call tile's Click() function
            - it'll call EmptyTileClick() again automatically, it works!

    GameOver()
        - does the same thing as above, except for every unclicked tile

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    [SerializeField]
    StateManager stateManager;

    int[,] mainGrid;
    // copies default state of grid
    int[,] mineNumberGrid;
    GameObject[,] tileObjectGrid;

    [SerializeField]
    GameObject tileObject;
    [SerializeField]
    Vector2 startingPosition;
    Vector2 tilePosition;
    [SerializeField]
    Canvas tileCanvas;
    [SerializeField]
    LevelConfiguration levelConfig;
    int mineCount;
    Vector2Int gridSize;
    float tileSize;

    public void Initialize() {
        // destroy every tile instance
        // i don't know if this is the greatest way of doing this, lol
        foreach (Transform t in tileCanvas.transform) {
            Tile tile = t.GetComponent<Tile>();
            if (tile) {
                tile.DestroySelf();
            }
        }
        
        mineCount = levelConfig.mineCount;
        gridSize = levelConfig.gridSize;

        mainGrid = new int[gridSize.x, gridSize.y];
        mineNumberGrid = mainGrid.Clone() as int[,];
        tileObjectGrid = new GameObject[gridSize.x, gridSize.y];
        
        // i was going to use this for dynamically changing the size of tiles
        // when i started out with only a square grid (ex. 8x8)
        // but i changed to rectangular grids instead
        // idk how to use this anymore
        tileSize = 1f;
        
        Reset(mainGrid);
        GenerateTileGrid();
    }

    public int GetTileCount() {
        int result = gridSize.x * gridSize.y;
        return result;
    }

    public int GetMineCount() {
        return mineCount;
    }

    // resets all ints to be 0
    void Reset(int[,] array) {
        int x, y;
        for (y = 0; y < gridSize.y; y++) {
            for (x = 0; x < gridSize.x; x++) {
                array[x, y] = 0;
            }
        }
    }

    void GenerateTileGrid() {
        int x, y;
        float newX = 0, newY = 0;

        for (y = 0; y < gridSize.y; y++) {
            newX = 0;
            for (x = 0; x < gridSize.x; x++) {
                GameObject tile = GenerateTile(
                    startingPosition.x + newX, 
                    startingPosition.y - newY,
                    new Vector2Int(x, y)
                );
                
                tileObjectGrid[x, y] = tile;

                newX += tileSize; 
            }
            newY += tileSize;
        }
        
    }

    // generate tile at position with value
    GameObject GenerateTile(float x, float y, Vector2Int gridPosition) {
        Vector3 newPosition = new Vector3(x, y, 0);

        GameObject tile = Instantiate(tileObject, newPosition, Quaternion.identity);
        tile.transform.SetParent(tileCanvas.transform, false);
        tile.transform.localScale = new Vector3(tileSize, tileSize, 1);

        Tile _tile = tile.GetComponent<Tile>();
        if (_tile) {
            _tile.SetManagers(this, stateManager);
            _tile.SetGridPosition(gridPosition);
        }

        return tile;
    }

    public void GameStart(Vector2Int gridPosition) {
        GenerateMines(gridPosition);
    }

    void GenerateMines(Vector2Int gridPosition) {
        int x, y;

        // https://stackoverflow.com/questions/3578456/whats-the-algorithm-behind-minesweeper-generation
        // i'll admit that this wasn't my first idea, lol
        List<Vector2Int> gridChoices = new List<Vector2Int>();
        for (y = 0; y < gridSize.y; y++) {
            for (x = 0; x < gridSize.x; x++) {
                gridChoices.Add(new Vector2Int(x, y));
            }
        }

        // removes position of player's clicked tile
        // and the positions of tiles surrounding that tile
        int gx = gridPosition.x;
        int gy = gridPosition.y;
        for (y = 1; y >= -1; y--) {
            for (x = -1; x <= 1; x++) {
                if (gx + x < 0 || gx + x > mainGrid.GetLength(0) &&
                    gy - y < 0 || gy - y > mainGrid.GetLength(1)
                ) continue;
                
                gridChoices.Remove(new Vector2Int(
                    gx + x,
                    gy - y
                ));
            }
        }

        for (int i = 0; i < mineCount; i++) {
            int choice = Random.Range(0, gridChoices.Count);

            Vector2Int chosen = gridChoices[choice];
            gridChoices.Remove(gridChoices[choice]);

            mainGrid[chosen.x, chosen.y] = 1;
        }

        GenerateMineNumbers();
    }

    // this part looks like a mess
    // but it works
    void GenerateMineNumbers() {
        int x, y, mineNumber;
        int _x, _y;

        for (y = 0; y < gridSize.y; y++) {
            for (x = 0; x < gridSize.x; x++) {
                mineNumber = 0;
                if (mainGrid[x, y] == 1) continue;
                
                for (_y = 1; _y > -2; _y--) {
                    for (_x = -1; _x < 2; _x++) {
                        if (
                            (x + _x >= 0 && x + _x < mainGrid.GetLength(0)) &&
                            (y - _y >= 0 && y - _y < mainGrid.GetLength(1))
                        ) {
                            if (_x == 0 && _y == 0) continue;

                            if (mainGrid[x + _x, y - _y] == 1) {
                                mineNumber++;
                            }
                        }
                    }
                }
                mineNumberGrid[x, y] = mineNumber;
            }
        }
        AssignTileValues();
    }

    void AssignTileValues() {
        int x, y;
        for (y = 0; y < gridSize.y; y++) {
            for (x = 0; x < gridSize.x; x++) {
                Tile tile = tileObjectGrid[x, y].GetComponent<Tile>();
                if (tile) {
                    if (mainGrid[x, y] == 1) {
                        tile.SetMine();
                    } else if (mainGrid[x, y] == 0) {
                        tile.SetEmpty(mineNumberGrid[x, y]);
                    }
                }
            }
        }
    }

    public void EmptyTileClick(Vector2Int gridPosition) {
        int x, y;
        int gx = gridPosition.x;
        int gy = gridPosition.y;
        for (y = 1; y >= -1; y--) {
            for (x = -1; x <= 1; x++) {
                if (
                    (gx + x >= 0 && gx + x < mainGrid.GetLength(0)) &&
                    (gy - y >= 0 && gy - y < mainGrid.GetLength(1))
                ) {
                    if (x == 0 && y == 0) continue;

                    Tile tile = tileObjectGrid[gx + x, gy - y].GetComponent<Tile>();
                    if (tile) {
                        if (!tile.ClickedBool())
                            tile.Click();
                    }
                }
            }
        }
    }

    public void GameOver(Vector2Int gridPosition) {
        int x, y;
        for (y = 0; y < gridSize.y; y++) {
            for (x = 0; x < gridSize.x; x++) {
                if (x == gridPosition.x && y == gridPosition.y) continue;
                
                GameObject tile = tileObjectGrid[x, y];
                Tile _tile = tile.GetComponent<Tile>();
                if (_tile && !_tile.ClickedBool()) {
                    _tile.GameOver();
                }
            }
        }
    }
}
