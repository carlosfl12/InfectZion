using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class History : MonoBehaviour
{
    public float minAlpha = 0.1f; // Value mínimo of alpha
    public float maxAlpha = 0.4f; // Value máximo of alpha
    public float speed = 1f; // flicker speed

    private float currentAlpha; // Value current of alpha
    private float targetAlpha; // Value target of alpha

    public Button buttonSaltar;


    string words = "Después de embarcarse en una peligrosa expedición de pesca en la Antártida, Jake Lake, un pescador de Ushuaia, comienza a sentirse gravemente enfermo después de comer una especie exótica de pescado. Los médicos no pueden determinar qué le está pasando y su condición empeora rápidamente, con alucinaciones y comportamientos extraños. Finalmente descubren que el pescado estaba contaminado con una peligrosa bacteria congelada durante millones de años en el hielo antártico, liberada por el cambio climático. La infección se extendió rápidamente y comenzó a convertir a las personas en zombies mientras los científicos luchan por encontrar una cura. ¿Podrás ayudar en este mundo post-apocalíptico y encontrar una manera de detener la infección? ";



    public Text texto;
    // Start is called before the first frame update
    void Start()
    {
        string levelLoad = MenuInicial.nextLevel;
        StartCoroutine(Timer());
        StartCoroutine(LoadLevel("Level_1"));

        // Sets the initial value of alpha
        currentAlpha = minAlpha;
        targetAlpha = maxAlpha;


        // Desactiva el botón y oculta el botón
        buttonSaltar.gameObject.SetActive(false);


    }
    void Update()
    {

        // Interpolates the value of alpha between minAlpha and maxAlpha
        currentAlpha = Mathf.Lerp(currentAlpha, targetAlpha, Time.deltaTime * speed);

        // If the value of alpha has reached the target value, change the target.
        if (Mathf.Abs(currentAlpha - targetAlpha) < 0.01f)
        {
            targetAlpha = (targetAlpha == minAlpha) ? maxAlpha : minAlpha;
        }

        // Applies the value of alpha to the colour of the image.
        GetComponent<Image>().color = new Color(0.39f, 0.3f, 0.3f, currentAlpha);
    }



    //function to write text letter by letter every X seconds
    IEnumerator Timer()
    {
        foreach (char character in words)
        {
            texto.text += character;
            yield return new WaitForSeconds(0.08f);
        }
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene("Level_1");
    }
    IEnumerator LoadLevel(string level)
    {
        yield return new WaitForSeconds(3f);
        //we load the scene asynchronously
        AsyncOperation op = SceneManager.LoadSceneAsync(level);
        //We prevent it from being activated when the scene is ready.
        op.allowSceneActivation = false;

        while (!op.isDone)
        {
            //confirm scene loading is OK
            if (op.progress >= 0.9f)
            {
                buttonSaltar.gameObject.SetActive(true);
            }
            yield return null;

        }
    }
}
