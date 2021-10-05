using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mouseCursor : MonoBehaviour
{
    private Cursor cursor;

    // Start is called before the first frame update
    void Start()
    {
        cursor = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Cursor>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = cursorPos;
    }
}
