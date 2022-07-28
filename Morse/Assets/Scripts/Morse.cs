using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using TMPro;

public class Morse : MonoBehaviour
{
    private string[] Alfabet = new string[]
    {
        "A",
        "B",
        "C",
        "D",
        "E",
        "F",
        "G",
        "H",
        "I",
        "J",
        "K",
        "L",
        "M",
        "N",
        "O",
        "P",
        "Q",
        "R",
        "S",
        "T",
        "U",
        "V",
        "W",
        "X",
        "Y",
        "Z",
        //Svenska karaktärer
        "Å",
        "Ä",
        "Ö",
        //Nummer
        "0",
        "1",
        "2",
        "3",
        "4",
        "5",
        "6",
        "7",
        "8",
        "9",
        //Special karaktärer
        /*".",
        "-",
        "(",
        ")",
        "@",
        @"""",
        "%",
        "'",
        ";",
        ":",*/
    };

    private string[] MorseMotsvarighet = new string[]
    {
        ".-",
        "-...",
        "-.-.",
        "-..",
        ".",
        "..-.",
        "--.",
        "....",
        "..",
        ".---",
        "-.-",
        ".-..",
        "--",
        "-.",
        "---",
        ".--.",
        "--.-",
        ".-.",
        "...",
        "-",
        "..-",
        "...-",
        ".--",
        "-..-",
        "-.--",
        "--..",
        //Svenska karaktärer
        ".--.-",
        ".-.-",
        "---.",
        //Nummer
        "-----",
        ".----",
        "..---",
        "...--",
        "....-",
        ".....",
        "-....",
        "--...",
        "---..",
        "----.",
        //Special karaktärer
        /*".-.-.-", // .
        "-....-", // -
        "-.--.",  // (
        "-..-.-", // )
        ".--.-.", // @
        ".-..-.", // "
        ".--..", // %
        ".----.", // '
        "-.-.-.", // ;
        "---...", // :*/
    };

    private string ConvertToMorse(string input)
    {
        string temp = input.Replace('.', '_');
        for (int i = 0; i < Alfabet.Length; i++)
        {
            temp = temp.ToLower().Replace(Alfabet[i].ToLower(), MorseMotsvarighet[i] + "/");
        }
        temp = temp.Replace("/ ", "|");
        temp = temp.Replace(" ", "|");
        if (temp[temp.Length - 1] == '/')
        {
            temp = temp.Remove(temp.Length - 1);
        }
        return temp;
    }

    private string ConvertToAlfabet(string input)
    {
        string temp = string.Empty;
        input = input.Replace("|", "/|/");
        input = input.Replace("_", "/_/");

        string[] alfabet = input.Split('/');
        for (int i = 0; i < alfabet.Length; i++)
        {
            int index = MorseMotsvarighet.ToList().FindIndex(o => o == alfabet[i].Trim());
            if (index > -1)
            {
                alfabet[i] = Alfabet[index].ToLower();
            }
        }
        for (int i = 0; i < alfabet.Length; i++)
        {
            //temp = temp.Replace(MorseMotsvarighet[i], Alfabet[i].ToLower());
            temp += alfabet[i];
        }
        temp = temp.Replace('_', '.');
        temp = temp.Replace('|', ' ');
        return temp;
    }

    private string[] ord;
    private string nuvarandeOrd;

    [SerializeField] private TextMeshProUGUI textMesh;
    [SerializeField] private TextMeshProUGUI morse;

    private int currentLetter = 0;

    private string nuvarandeOrdMorse;
    private bool[] gissadeKorrektBokstav;

    // Start is called before the first frame update
    private void Start()
    {
        ord = File.ReadAllLines("Assets/WordList/new-swedish-character-list.txt");
        nuvarandeOrd = ord[Random.Range(0, ord.Length)];
        textMesh.text = nuvarandeOrd;
        morse.text = string.Empty;
        nuvarandeOrdMorse = ConvertToMorse(nuvarandeOrd);
        gissadeKorrektBokstav = new bool[nuvarandeOrd.Length];
        SetWordColour();
    }

    public void DotPressed()
    {
        morse.text += ".";
        UpdateWord();
    }

    private void UpdateWord()
    {
        if (ConvertToMorse(nuvarandeOrd[currentLetter].ToString())[morse.text.Length - 1] != morse.text[morse.text.Length - 1])
        {
            //nuvarandeOrd = ord[Random.Range(0, ord.Length)];
            //textMesh.text = nuvarandeOrd;
            morse.text = string.Empty;
            gissadeKorrektBokstav[currentLetter] = false;
            currentLetter++;
            SetWordColour();
        }
        else if (ConvertToMorse(nuvarandeOrd[currentLetter].ToString()) == morse.text)
        {
            morse.text = string.Empty;
            gissadeKorrektBokstav[currentLetter] = true;
            currentLetter++;
            SetWordColour();
        }

        if (currentLetter >= nuvarandeOrd.Length)
        {
            nuvarandeOrd = ord[Random.Range(0, ord.Length)];
            textMesh.text = nuvarandeOrd;
            morse.text = string.Empty;
            nuvarandeOrdMorse = ConvertToMorse(nuvarandeOrd);
            gissadeKorrektBokstav = new bool[nuvarandeOrd.Length];
            currentLetter = 0;
            SetWordColour();
        }
    }

    private void SetWordColour()
    {
        textMesh.text = string.Empty;
        for (int i = 0; i < nuvarandeOrd.Length; i++)
        {
            if (currentLetter <= i)
            {
                textMesh.text += "<color=yellow>" + nuvarandeOrd[i] + "</color>";
            }
            else if (gissadeKorrektBokstav[i])
            {
                textMesh.text += "<color=green>" + nuvarandeOrd[i] + "</color>";
            }
            else
            {
                textMesh.text += "<color=red>" + nuvarandeOrd[i] + "</color>";
            }
        }
    }

    public void DashPressed()
    {
        morse.text += "-";
        UpdateWord();
    }

    // Update is called once per frame
    private void Update()
    {
    }
}