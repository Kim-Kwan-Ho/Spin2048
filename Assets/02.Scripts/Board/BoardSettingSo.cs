using UnityEngine;


[CreateAssetMenu(fileName = "BoardSettings", menuName = "Scriptable Objects/Board/BoardSetting")]
public class BoardSettingSo : ScriptableObject
{
    [SerializeField] private float _delayTime;
    public float DelayTime { get { return _delayTime; } }
    [SerializeField] private float _rotateTime;
    public float RotateTime { get { return _rotateTime; } }


    [SerializeField] private int _row;
    public int Row { get { return _row; } }
    [SerializeField] private int _col;
    public int Col { get { return _col; } }
    [SerializeField] private float _offSetX;
    public float OffSetX { get { return _offSetX; } }
    [SerializeField] private float _offSetY;
    public float OffSetY { get { return OffSetY; } }

    [SerializeField] private float _cellOffSet;


    [SerializeField] private int _spawnTileMaxLevel;
    public float CalcXStartPos()
    {
        return -_col / 2.0f + _offSetX;
    }
    public float CalcYStartPos()
    {
        return -_col / 2.0f + _offSetY;
    }

    public int GetTotalTileCount()
    {
        return _row * _col;
    }

    public int GetSpawnTileLevel()
    {
        return Random.Range(0, _spawnTileMaxLevel);
    }

}