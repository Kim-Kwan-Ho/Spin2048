using UnityEngine;


[CreateAssetMenu(fileName = "TileSettings", menuName = "Scriptable Objects/Tile/TileSetting")]
public class TileSettingSo : ScriptableObject
{
    [SerializeField] private float _moveDuration;
    public float MoveDuration { get { return _moveDuration; } }

    [SerializeField] private int[] _value;
    public int GetValue(int step)
    {
        return step >= _value.Length ? _value[^1] : _value[step];
    }

    [SerializeField] private Color[] _bgColor;
    public Color GetBgColor(int step)
    {
        return step >= _bgColor.Length ? _bgColor[^1] : _bgColor[step];
    }

    [SerializeField] private Color[] _textColor;
    public Color GetTextColor(int step)
    {
        return step >= _textColor.Length ? _textColor[^1] : _textColor[step];
    }


}
