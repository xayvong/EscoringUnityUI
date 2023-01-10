using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public ServerController ServerController;

    public TMP_Dropdown DropDown;


    public void SelectScene()
    {
        //var selection = DropDown.value;
        SceneManager.LoadScene(DropDown.value);
    }
}
