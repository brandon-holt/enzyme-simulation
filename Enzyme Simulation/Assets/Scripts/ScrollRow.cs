using UnityEngine;
using UnityEngine.UI;

public class ScrollRow : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private Transform content;
    [SerializeField] private Button add;
    public int maxQuantity;

    private void Awake()
    {
        add.onClick.AddListener(AddOne);

        AddOne();

        if (!add.interactable) { add.transform.SetParent(transform); }
    }

    public void AddOne()
    {
        Instantiate(prefab, content);

        add.transform.SetAsLastSibling();
    }

    private void Update()
    {
        MaintainQuantity();
    }

    private void MaintainQuantity()
    {
        if (maxQuantity == -1) { return; }
        else if (content.childCount <= maxQuantity)
        {
            add.gameObject.SetActive(true);
        }
        else
        {
            add.gameObject.SetActive(false);
        }
    }
}
