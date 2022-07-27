using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
        "Å",
        "Ä",
        "Ö",
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
        ".",
        "-",
        "(",
        ")",
        "@",
        @"""",
        "%",
        "'",
        ";",
        ":",
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
        ".-.-.-", // .
        "-....-", // -
        "-.--.",  // (
        "-..-.-", // )
        ".--.-.", // @
        ".-..-.", // "
        ".--..", // %
        ".----.", // '
        "-.-.-.", // ;
                "---...", // :
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

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }
}