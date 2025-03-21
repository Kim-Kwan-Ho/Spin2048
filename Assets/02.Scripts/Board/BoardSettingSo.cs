using UnityEngine;


[CreateAssetMenu(fileName = "BoardSettings", menuName = "Scriptable Objects/Board/BoardSetting")]
public class BoardSettingSo : ScriptableObject
{

    [Header("Delay Times")]
    [SerializeField] private float _delayTime;
    public float DelayTime { get { return _delayTime; } }
    [SerializeField] private float _rotateTime;
    public float RotateTime { get { return _rotateTime; } }
    [SerializeField] private float _spawnDelayTime;
    public float SpawnDelayTime { get { return _spawnDelayTime; } }


    [Header("Board Size")]
    [SerializeField] private int _row;
    public int Row { get { return _row; } }
    [SerializeField] private int _col;
    public int Col { get { return _col; } }

    [Header("Spawn Pos")]
    [SerializeField] private float _offSetX;
    [SerializeField] private float _offSetY;

    [SerializeField] private float _cellOffSet;
    public float CellOffSet { get { return _cellOffSet; } }

    [SerializeField] private float _backgroundPadding;
    public float BackGroundPadding { get { return _backgroundPadding; } }


    [Header("Spawn Settings")]
    [SerializeField] private int _spawnTileMaxLevel;
    public int GetTotalTileCount()
    {
        return _row * _col;
    }

    public int GetSpawnTileLevel()
    {
        return Random.Range(0, _spawnTileMaxLevel+1);
    }

}