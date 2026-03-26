using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTT.BubbleShooter.Demo
{
    /// <summary>
    /// This class is responsible for rendering a <see cref="ColoredBubble"/> on a given <see cref="BubbleController"/>.
    /// </summary>
    public class ColoredBubbleRenderer : IBubbleRenderer
    {
        /// <summary>
        /// The _defaultSprite field is the sprite used when showing the <see cref="BubbleController"/>.
        /// </summary>
        private readonly Sprite _defaultSprite;

        /// <summary>
        /// The constructor initializes the <see cref="_defaultSprite"/> field value.
        /// </summary>
        /// <param name="defaultSprite">The default sprite to use when showing the <see cref="BubbleController"/>.</param>
        public ColoredBubbleRenderer(Sprite defaultSprite) => _defaultSprite = defaultSprite;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="bubble"><inheritdoc/></param>
        /// <param name="controller"><inheritdoc/></param>
        public void Render(Bubble bubble, BubbleController controller, List<Sprite> sprites, ColorConfig[] colorConfigs)
        {
            ColoredBubble coloredBubble = bubble as ColoredBubble;
            controller.SpriteRenderer.sprite = _defaultSprite;
            controller.SpriteRenderer.color = coloredBubble.Color;
            controller.Text.text = string.Empty;

            for (int i = 0; i < colorConfigs.Length; i++)
            {
                Color color = colorConfigs[i].BubbleColors;
                if (coloredBubble.Color.Equals(color))
                {
                    if (sprites.Count > 0 && i < sprites.Count)
                    {
                        controller.SpriteRenderer.sprite = sprites[i];
                        controller.SpriteRenderer.color = Color.white;
                        break;
                    }
                }
            }
        }
    }
}