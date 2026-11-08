using System.Collections;
using UnityEngine;
using TMPro;

public class Ruleta : MonoBehaviour
{
    [SerializeField] int monedas = 100;
    [SerializeField] TMP_Text textoMonedas;
    [SerializeField] Transform ruletaTransform;

    [Header("Curvas de Animación para el giro")]
    [SerializeField] AnimationCurve[] curvasDeGiro; 

    private bool girando = false;

    void Start()
    {
        ActualizarTexto();
    }

    
    public void Spin()
    {
        if (girando) return;

        
        int resultado = Random.Range(0, 6);

        
        switch (resultado)
        {
            case 0:
                monedas += 100;
                break;
            case 1:
                monedas -= 50;
                break;
            case 2:
                monedas *= 2;
                break;
            case 3:
                monedas /= 2;
                break;
            case 4:
                monedas = 0;
                break;
            case 5:
                monedas += 10;
                break;
        }

        
        monedas = Mathf.Max(monedas, 0);
        Debug.Log("Salió el caso: " + resultado);
        ActualizarTexto();

        
        StartCoroutine(GirarRutina());
    }

    IEnumerator GirarRutina()
    {
        girando = true;

        float zActual = ruletaTransform.eulerAngles.z;
        float zFinal = zActual - (360f * 5) - Random.Range(0f, 360f);

        float duracion = 3f;
        float tiempoTranscurrido = 0f;

        
        AnimationCurve curvaSeleccionada = curvasDeGiro[Random.Range(0, curvasDeGiro.Length)];

        while (tiempoTranscurrido < duracion)
        {
            tiempoTranscurrido += Time.deltaTime;
            float t = tiempoTranscurrido / duracion;

            
            float valorCurva = curvaSeleccionada.Evaluate(t);

            float zRotacion = Mathf.Lerp(zActual, zFinal, valorCurva);
            ruletaTransform.eulerAngles = new Vector3(0, 0, zRotacion);

            yield return null;
        }

        ruletaTransform.eulerAngles = new Vector3(0, 0, zFinal);
        girando = false;
    }

    void ActualizarTexto()
    {
        textoMonedas.text = "Monedas: " + monedas;
    }

    
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("El juego se ha cerrado."); 
    }
}