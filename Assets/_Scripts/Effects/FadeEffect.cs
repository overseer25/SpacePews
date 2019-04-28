using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A fade effect that fades the sprite provided to it.
/// </summary>
public class FadeEffect : MonoBehaviour
{
    public float fadeRate;
    public float fadeAmount;
    private SpriteRenderer rend;


    /// <summary>
    /// Initialize the fade effect with a sprite, position, and rotation.
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="pos"></param>
    /// <param name="rotation"></param>
    public void Initialize(Sprite sprite, Vector2 pos, Quaternion rotation, float fadeRate, float fadeAmount, Vector3? scale = null)
    {
        transform.localScale = scale ?? Vector3.one;
        rend = GetComponent<SpriteRenderer>();
        transform.position = pos;
        transform.rotation = rotation;
        this.fadeRate = fadeRate;
        this.fadeAmount = fadeAmount;
        rend.sprite = sprite;
        StartCoroutine(Fade());
    }

    /// <summary>
    /// Fade the sprite.
    /// </summary>
    /// <returns></returns>
    private IEnumerator Fade()
    {
        while(rend.color.a > 0)
        {
            rend.color = new Color(rend.color.r, rend.color.g, rend.color.b, rend.color.a - fadeAmount);
            yield return new WaitForSeconds(fadeRate);
        }
        Destroy(gameObject);
    }
}
