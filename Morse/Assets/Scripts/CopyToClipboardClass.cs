using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CopyToClipboardClass : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI myText;

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void StartCopy()
    {
        CopyToClipboard(myText.text);
    }

    public void CopyToClipboard(string s)
    {
        TextEditor te = new TextEditor();
        te.text = s;
        te.SelectAll();
        te.Copy();
    }
}