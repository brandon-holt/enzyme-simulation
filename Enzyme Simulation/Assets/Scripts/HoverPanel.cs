using UnityEngine;

public class HoverPanel : MonoBehaviour
{
    [SerializeField] private GameObject enzPrefab, subPrefab;

    public void MakeEnzyme(Enzyme e)
    {
        SetPosition();
        Enzyme ce = Instantiate(enzPrefab, transform).GetComponent<Enzyme>();
        ce.transform.localPosition = new Vector2(ce.transform.localPosition.x, 110f);
        ce.SetColor(e.color);
        ce.SetName(e.enzymeName);
    }

    public void MakeSubstrate(Substrate s)
    {
        SetPosition();
        Substrate cs = Instantiate(subPrefab, transform).GetComponent<Substrate>();
        cs.transform.localPosition = new Vector2(cs.transform.localPosition.x, 110f);
        cs.SetName(s.substrateName);
        cs.SetColor(s.colors[0], 0);
        cs.SetColor(s.colors[1], 1);
        cs.SetType(s.YES2);
    }

    private void Update() { SetPosition(); }

    private void SetPosition() { transform.position = new Vector2(Input.mousePosition.x + 1f, Input.mousePosition.y + 1f); }
}
