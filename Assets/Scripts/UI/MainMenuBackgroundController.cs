using UnityEngine;
using UnityEngine.UI;

public class MainMenuBackgroundController : MonoBehaviour
{
    public Transform titleTransform;
    public float maxTitleZoom;
    public float titleBobAngle;
    public float titleBobSpeed;
    public float colorChangeSpeed;
    public Image backgroundImage;
    public float scrollSpeed;
    public float titleZoomSpeed;
    
    private Color _color;
    private Vector2 _scrollDirection;


    private void Awake()
    {
        _color = Random.ColorHSV(alphaMin: 1f, alphaMax: 1f, hueMin: 0f, hueMax: 1f, saturationMin: 0.45f,
            saturationMax: 0.6f, valueMin: 0.8f, valueMax: 1f);
        _scrollDirection = Random.insideUnitCircle;
    }

    private void Update()
    {
        Color.RGBToHSV(_color, out var h, out var s, out var v);
        _color = Color.HSVToRGB(h + colorChangeSpeed * Time.deltaTime, s, v);

        var angle = Mathf.Sin(Time.time * titleBobSpeed) * titleBobAngle;
        titleTransform.localRotation = Quaternion.Euler(0, 0, angle);
        
        titleTransform.localScale = Vector3.Slerp(Vector3.one, Vector3.one * maxTitleZoom,
            Mathf.Sin(Time.time * titleZoomSpeed)/2 + 0.5f);
        
        backgroundImage.color = _color;
        backgroundImage.material.mainTextureOffset += _scrollDirection * (scrollSpeed * Time.deltaTime);
    }
}