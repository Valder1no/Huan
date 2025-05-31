using UnityEngine;

public class AlignInGrid : MonoBehaviour
{
    public int columns = 3;
    public float spacing = 1f;

    void Start()
    {
        AlignObjects();
    }

    void AlignObjects()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            int row = i / columns;
            int col = i % columns;
            child.localPosition = new Vector3(col * spacing, 0, row * spacing);
        }
    }
}
