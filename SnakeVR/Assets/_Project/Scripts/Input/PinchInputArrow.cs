using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gustorvo.SnakeVR.Input
{
    public enum Direction
    {
        Vertical,
        Horizontal,
    }

    public class PinchInputArrow : MonoBehaviour
    {
        [SerializeField] AudioSource audioSource;
        [SerializeField] Sprite filledArrowSprite;
        [SerializeField] SpriteRenderer spriteRenderer;
        [SerializeField] float maxRange = 0.1f;
        [SerializeField] Direction direction;
        private Color spriteColor;
        private Sprite originalSprite;


        private float t;
        private float alpha;
        private bool played;

        private void Awake()
        {
            spriteColor = spriteRenderer.color;
            originalSprite = spriteRenderer.sprite;
        }

        private void Update()
        {
            // adjust alpha based on movement
            float move = direction == Direction.Horizontal ? transform.localPosition.x : transform.localPosition.y;
            t = Mathf.InverseLerp(0.02f, maxRange, Mathf.Abs(move));
            float alpha = Mathf.Lerp(0, 1f, t);
            spriteColor.a = alpha;
            spriteRenderer.color = spriteColor;

            // rotate arrow to look in the direction of the movement
            var rot = Mathf.Sign(move) == 1f ? new Vector3(0, 0, 0) : new Vector3(0, 0, 180);
            transform.localRotation = Quaternion.Euler(rot);

            //replace sprite
            spriteRenderer.sprite = t > 0.999f ? filledArrowSprite : originalSprite;
            
            // play audio
            if (t > 0.999f && !played)
            {
                audioSource.Play();
                played = true;
            }
            if (t < 0.9f && played) played = false;
        }
    }
}