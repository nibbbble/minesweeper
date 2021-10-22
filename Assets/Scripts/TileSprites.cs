using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class TileSprites : ScriptableObject
{
    public Sprite hiddenTile;
    public Sprite mineTile;
    public Sprite emptyTile;
    public Sprite[] numberedTiles;
    public Sprite flaggedTile;
    public Sprite mineGameOverTile;
    public Sprite mineWrongFlagTile;
}
