using UnityEngine;

public class Blok : MonoBehaviour
{
    public int blockType; // Her blo�un bir t�r� olacak (�rne�in renk)

    private void OnMouseDown()
    {
        // T�klanan blo�un ismini konsola yaz
        Debug.Log("T�klanan blok: " + gameObject.name);

        // Bloka t�klan�nca e�le�me sistemini �al��t�r
        GridManager.Instance.MatchBloklar(this);
    }
}