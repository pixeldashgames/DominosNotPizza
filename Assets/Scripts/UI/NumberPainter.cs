using UnityEngine;
using TMPro;

public class NumberPainter : ValuePainter
{
    [SerializeField] private TMP_Text text3D;
    [SerializeField] private TMP_Text text;

    public override void Paint(int value, bool ui)
    {
        if (ui)
        {
            text.text = value.ToString();
        }
        else
        {
            text3D.text = value.ToString();
        }
    }
}
