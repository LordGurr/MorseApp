using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Menu : MonoBehaviour
{
    public GameObject PrevMenu;
    [SerializeField] public UnityEvent onLoad;

    public void StepForward(GameObject nextMenu)
    {
        nextMenu.SetActive(true);
        nextMenu.GetComponent<Menu>().onLoad.Invoke();
        nextMenu.GetComponent<Menu>().PrevMenu = gameObject;
        gameObject.SetActive(false);
    }

    public void StepForwardButKeepBack(GameObject nextMenu)
    {
        nextMenu.SetActive(true);
        nextMenu.GetComponent<Menu>().onLoad.Invoke();
        nextMenu.GetComponent<Menu>().PrevMenu = PrevMenu;
        gameObject.SetActive(false);
    }

    public void StepForwardButDontSetBack(GameObject nextMenu)
    {
        nextMenu.SetActive(true);
        nextMenu.GetComponent<Menu>().onLoad.Invoke();
        //nextMenu.GetComponent<Menu>().PrevMenu;
        gameObject.SetActive(false);
    }

    public void StepBack()
    {
        if (PrevMenu != null)
        {
            PrevMenu.SetActive(true);
            PrevMenu.GetComponent<Menu>().onLoad.Invoke();
            gameObject.SetActive(false);
        }
    }

    public void Exit()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            Debug.Log("Cancelled pressed");
            if (PrevMenu != null)
            {
                StepBack();
            }
        }
    }
}