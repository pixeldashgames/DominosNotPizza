using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBackgroundController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer backgroundRenderer;
    
    void Start()
    {
        backgroundRenderer.color = Random.ColorHSV(alphaMin: 1f, alphaMax: 1f, hueMin: 0f, hueMax: 1f, saturationMin: 0.45f,
            saturationMax: 0.6f, valueMin: 0.8f, valueMax: 1f);
    }
}
