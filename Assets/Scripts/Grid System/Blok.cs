using UnityEngine;

public class Blok : MonoBehaviour
{
    public int blockType; // Her bloðun bir türü olacak (örneðin renk)

    private void OnMouseDown()
    {
        // Týklanan bloðun ismini konsola yaz
        Debug.Log("Týklanan blok: " + gameObject.name);

        // Bloka týklanýnca eþleþme sistemini çalýþtýr
        GridManager.Instance.MatchBloklar(this);
    }
}