using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Utils
{
    // ----- Player -----
    private static GameObject player;

    /** <summary> 
    * Função pra pegar o jogador independente da cena, caso exista.
    * Também atualiza a referência caso o jogador seja recriado.
    * </summary> **/
    public static GameObject GetPlayer()
    {
        if (player.IsUnityNull() || player == null) player = Object.FindAnyObjectByType<PlayerInput>().gameObject;
        return player;
    }

    // ----- Visual Effects -----
    public static Coroutine BlinkRed(SpriteRenderer spriteRenderer, MonoBehaviour mono)
    {
        return mono.StartCoroutine(BlinkRed(spriteRenderer));
    }

    private static IEnumerator BlinkRed(SpriteRenderer spriteRenderer)
    {
        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = originalColor;
    }
}