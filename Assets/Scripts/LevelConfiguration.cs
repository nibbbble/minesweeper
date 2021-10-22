using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class LevelConfiguration : ScriptableObject
{
    [Tooltip("Width : Height")]
    public Vector2Int gridSize;
    public int mineCount;
}
