using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceBaboon
{
    public class PlayerFlash : MonoBehaviour
    {
        Color originColor;
        public void FlashStart(SpriteRenderer sprite, Color flashColor)
        {
            originColor = sprite.color;
            sprite.color = flashColor;
        }

        public void FlashEnd(SpriteRenderer sprite, Color originalColor)
        {
            //sprite.color = Color.Lerp(originColor, sprite.color, 0.2f);
            sprite.color = originalColor;
        }
    }
}
