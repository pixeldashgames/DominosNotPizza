using UnityEngine;
using UnityEngine.InputSystem.Utilities;

public class PointsPainter : ValuePainter
{
    [SerializeField] private GameObject[] preMade3DPaints;
    [SerializeField] private GameObject[] preMadeUIPaints;
    [SerializeField] private GameObject point3DPrefab;
    [SerializeField] private GameObject pointUIPrefab;
    [SerializeField] private Transform pointContainer;
    [SerializeField] private float pointsAreaSize;
    
    public override void Paint(int value, bool ui)
    {
        if(value == 0)
            return;

        var preMadePaints = ui ? preMadeUIPaints : preMade3DPaints;
        
        if (preMadePaints != null && value <= preMadePaints.Length)
        {
            preMadePaints[value - 1].SetActive(true);
            return;
        }
        
        var rows = Mathf.CeilToInt(Mathf.Sqrt(value));
        // points are assumed to be area sized.
        var scaleFraction = pointsAreaSize / rows;
        
        if(!ui)
            for (var i = 0; i < rows; i++)
                for (var e = 0; e < rows; e++)
                {
                    if(value == 0)
                        return;
                    
                    var point = Instantiate(point3DPrefab, pointContainer);

                    point.transform.localPosition = 
                        new Vector3(
                            e * scaleFraction - (pointsAreaSize * 0.5f) + (scaleFraction * 0.5f),
                            i * scaleFraction - (pointsAreaSize * 0.5f) + (scaleFraction * 0.5f),
                            0);
                    point.transform.localScale = new Vector3(scaleFraction, scaleFraction, scaleFraction);
                    value--;
                }
        else
            for (var i = 0; i < rows; i++)
                for (var e = 0; e < rows; e++)
                {
                    if (value == 0)
                        return;

                    var point = Instantiate(pointUIPrefab, pointContainer).GetComponent<RectTransform>();
                    point.anchorMin = new Vector2(e*scaleFraction, i*scaleFraction);
                    point.anchorMax = new Vector2((e + 1) * scaleFraction, (i + 1) * scaleFraction);

                    value--;
                }
    }
}
