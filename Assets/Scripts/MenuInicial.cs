using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuInicial : MonoBehaviour
{
    public static string nextLevel;
    public void Jugar()
    {
        SceneManager.LoadScene("Level_1");
    }
    public void Menu()
    {
        SceneManager.LoadScene("Menu_Inicio");
    }
    public void Salir()
    {
        Debug.Log("Salir");
        Application.Quit();
    }
    public static void LevelLoad(string nombre)
    {
        nextLevel = nombre;
        SceneManager.LoadScene(nextLevel);

    }

}
