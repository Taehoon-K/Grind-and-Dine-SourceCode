using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingGif : MonoBehaviour
{
    [SerializeField] List<Sprite> frames;
    [SerializeField] float frameRate = 0.1f; // 초당 프레임
    private Image imageComponent;
    private int currentFrame = 0;

    void Start()
    {
        imageComponent = GetComponent<Image>();
        InvokeRepeating(nameof(UpdateFrame), 0, frameRate);
    }

    void UpdateFrame()
    {
        if (frames.Count == 0) return;
        currentFrame = (currentFrame + 1) % frames.Count;
        imageComponent.sprite = frames[currentFrame];
    }

}
