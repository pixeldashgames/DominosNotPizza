using UnityEngine;

public interface IPiecePainter
{
    public void PaintValue(int value, Transform placeholder, bool ui);
}
