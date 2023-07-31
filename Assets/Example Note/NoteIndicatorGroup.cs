using TMPro;
using UnityEngine;

public class NoteIndicatorGroup : MonoBehaviour
{
    public GameObject prefab;
    public TextMeshProUGUI text;

    void Start()
    {
        for (var i = 0; i < 128; i++)
        {
            var go = Instantiate<GameObject>(prefab);
            go.transform.position = new Vector3(i % 12, i / 12, 0);
            go.GetComponent<NoteIndicator>().noteNumber = i;

#if UNITY_ANDROID && !UNITY_EDITOR
            text.text = "ANDROID";
#endif
        }
    }
}
