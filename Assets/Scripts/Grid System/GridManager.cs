using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    public int rows = 8;
    public int columns = 8;
    public float blokBoyutu = 1.1f;
    public GameObject[] blokPrefablar;

    private GameObject[,] bloklar;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        bloklar = new GameObject[rows, columns];
        GridOlustur();
    }

    void GridOlustur()
    {
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                Vector2 pos = new Vector2(col * blokBoyutu, row * blokBoyutu);
                int randomIndex = Random.Range(0, blokPrefablar.Length);
                GameObject yeniBlok = Instantiate(blokPrefablar[randomIndex], pos, Quaternion.identity);
                bloklar[row, col] = yeniBlok;

                // Blok tipini belirle
                Blok blokScript = yeniBlok.GetComponent<Blok>();
                blokScript.blockType = randomIndex;
            }
        }
    }

    // E�le�me sistemi
    public void MatchBloklar(Blok startBlok)
    {
        List<Blok> matchedBloklar = new List<Blok>();
        FindMatches(startBlok, matchedBloklar);

        if (matchedBloklar.Count >= 3)
        {
            // E�le�en bloklar� yok et
            foreach (Blok blok in matchedBloklar)
            {
                Vector2 pos = blok.transform.position;
                int row = Mathf.RoundToInt(pos.y / blokBoyutu);
                int col = Mathf.RoundToInt(pos.x / blokBoyutu);

                // Bloklar� yok et
                Destroy(blok.gameObject);
                bloklar[row, col] = null;
            }

            // Bo�luklar� doldur
            FillEmptySpaces();
        }
    }

    // Etraf�ndaki ayn� t�rde bloklar� bulur
    void FindMatches(Blok blok, List<Blok> matchedBloklar)
    {
        if (matchedBloklar.Contains(blok)) return;

        matchedBloklar.Add(blok);

        Blok komsuBlok;

        // Yukar�
        if (blok.transform.position.y + blokBoyutu < rows)
        {
            komsuBlok = GetBlok(blok.transform.position.x, blok.transform.position.y + blokBoyutu);
            if (komsuBlok != null && komsuBlok.blockType == blok.blockType)
                FindMatches(komsuBlok, matchedBloklar);
        }

        // A�a��
        if (blok.transform.position.y - blokBoyutu >= 0)
        {
            komsuBlok = GetBlok(blok.transform.position.x, blok.transform.position.y - blokBoyutu);
            if (komsuBlok != null && komsuBlok.blockType == blok.blockType)
                FindMatches(komsuBlok, matchedBloklar);
        }

        // Sa�a
        if (blok.transform.position.x + blokBoyutu < columns)
        {
            komsuBlok = GetBlok(blok.transform.position.x + blokBoyutu, blok.transform.position.y);
            if (komsuBlok != null && komsuBlok.blockType == blok.blockType)
                FindMatches(komsuBlok, matchedBloklar);
        }

        // Sola
        if (blok.transform.position.x - blokBoyutu >= 0)
        {
            komsuBlok = GetBlok(blok.transform.position.x - blokBoyutu, blok.transform.position.y);
            if (komsuBlok != null && komsuBlok.blockType == blok.blockType)
                FindMatches(komsuBlok, matchedBloklar);
        }
    }

    // Bloklar�n a�a��ya d��mesini sa�la
    void FillEmptySpaces()
    {
        for (int col = 0; col < columns; col++)
        {
            for (int row = 1; row < rows; row++)
            {
                if (bloklar[row, col] == null)
                {
                    // �st�ndeki bloklar� kayd�r
                    for (int rowAbove = row; rowAbove < rows; rowAbove++)
                    {
                        if (bloklar[rowAbove, col] != null)
                        {
                            bloklar[row, col] = bloklar[rowAbove, col];
                            bloklar[rowAbove, col] = null;

                            // Bloku yeni pozisyona ta��
                            bloklar[row, col].transform.position = new Vector2(col * blokBoyutu, row * blokBoyutu);
                            break;
                        }
                    }
                }
            }
        }

        // Yukar�dan yeni blok ekle
        for (int col = 0; col < columns; col++)
        {
            for (int row = rows - 1; row >= 0; row--)
            {
                if (bloklar[row, col] == null)
                {
                    Vector2 pos = new Vector2(col * blokBoyutu, row * blokBoyutu);
                    int randomIndex = Random.Range(0, blokPrefablar.Length);
                    GameObject yeniBlok = Instantiate(blokPrefablar[randomIndex], pos, Quaternion.identity);
                    bloklar[row, col] = yeniBlok;

                    Blok blokScript = yeniBlok.GetComponent<Blok>();
                    blokScript.blockType = randomIndex;
                }
            }
        }
    }

    // Belirtilen pozisyondaki blo�u bulur
    Blok GetBlok(float x, float y)
    {
        foreach (GameObject blokObj in bloklar)
        {
            if (blokObj != null && Mathf.Approximately(blokObj.transform.position.x, x) &&
                Mathf.Approximately(blokObj.transform.position.y, y))
            {
                return blokObj.GetComponent<Blok>();
            }
        }
        return null;
    }

    ///  BlokKaydirVeYenile() bu kod yeni bir sistem 

    IEnumerator BlokKaydirVeYenile()
    {
        bool bloklarHareketEtti = true;

        // A�a�� kayd�rma i�lemi i�in gecikmeyi art�r
        while (bloklarHareketEtti)
        {
            bloklarHareketEtti = false;

            for (int row = 1; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    if (bloklar[row - 1, col] == null && bloklar[row, col] != null)
                    {
                        bloklar[row - 1, col] = bloklar[row, col];
                        bloklar[row, col] = null;

                        bloklar[row - 1, col].transform.position -= new Vector3(0, blokBoyutu, 0);

                        bloklarHareketEtti = true;

                        // Gecikmeyi art�r, daha uzun bekle
                        yield return new WaitForSeconds(0.5f);
                    }
                }
            }
        }

        // Bo� alanlara yeni blok ekleme k�sm�nda da gecikmeyi art�r
        for (int col = 0; col < columns; col++)
        {
            for (int row = rows - 1; row >= 0; row--)
            {
                if (bloklar[row, col] == null)
                {
                    Vector2 pos = new Vector2(col * blokBoyutu, row * blokBoyutu);
                    int randomIndex = Random.Range(0, blokPrefablar.Length);
                    GameObject yeniBlok = Instantiate(blokPrefablar[randomIndex], pos + Vector2.up * rows, Quaternion.identity);
                    bloklar[row, col] = yeniBlok;

                    StartCoroutine(BlokDusur(yeniBlok, pos));

                    // Yeni blok d��me h�z�n� da art�r
                    yield return new WaitForSeconds(0.5f);
                }
            }
        }
    }

    IEnumerator BlokDusur(GameObject blok, Vector2 hedefPos)
    {
        // Bloklar�n a�a�� d��me h�z�n� daha yava� yap
        while (Vector2.Distance(blok.transform.position, hedefPos) > 0.01f)
        {
            blok.transform.position = Vector2.MoveTowards(blok.transform.position, hedefPos, Time.deltaTime * 2); // H�z� daha da d���r
            yield return null;
        }
        blok.transform.position = hedefPos; // Hedefe tam oturt
    }

    // Bloklar yok edildikten sonra �a�r�lacak
    public void BloklariYenile()
    {
        StartCoroutine(BlokKaydirVeYenile());
    }
}