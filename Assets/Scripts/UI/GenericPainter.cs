using UnityEngine;

[CreateAssetMenu(menuName = "Domino/Painters/Generic Painter")]
public class GenericPainter : ScriptableObject, IPiecePainter
{
    [SerializeField] ValuePainter valuePainterPrefab;

    public void PaintValue(int value, Transform placeholder, bool ui)
    {
        var painter = Instantiate(valuePainterPrefab, placeholder);
        painter.Paint(value, ui);
    }
}
