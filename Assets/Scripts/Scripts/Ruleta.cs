using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using TMPro;

[System.Serializable]
public class DatosRuleta
{
    public int monedasGuardadas;
}

public class Ruleta : MonoBehaviour
{
    [SerializeField] int monedas = 100;
    [SerializeField] TMP_Text textoMonedas;
    [SerializeField] TMP_Text textoResultado;
    [SerializeField] Transform ruletaTransform;
    [Range(20f, 40f)]
    [SerializeField] float largoLineaDeReferencia = 1.5f;

    [Header("Curvas de Animación para el giro")]
    [SerializeField] AnimationCurve[] curvasDeGiro;

    private bool girando = false;
    private string rutaArchivo;

    void Awake()
    {
        rutaArchivo = Application.persistentDataPath + "/RuletaData.dat";
    }

    void Start()
    {
        CargarMonedas();
        ActualizarTexto();

        if (textoResultado != null)
        {
            textoResultado.text = "Presioná Girar";
        }
    }

    [ContextMenu("Girar ruleta")]
    public void Spin()
    {
        if (girando) return;

        if (textoResultado != null)
        {
            textoResultado.text = "Girando...";
        }

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

        EvaluarAnguloFinal();

        girando = false;
        GuardarMonedas();
    }

    void EvaluarAnguloFinal()
    {
        float anguloZ = ruletaTransform.eulerAngles.z;

        string color = "";
        int puntos = 0;

        if (anguloZ >= 0f && anguloZ < 60f)
        {
            color = "Rojo";
            puntos = 5;
        }
        else if (anguloZ >= 60f && anguloZ < 120f)
        {
            color = "Naranja";
            puntos = 10;
        }
        else if (anguloZ >= 120f && anguloZ < 180f)
        {
            color = "Amarillo";
            puntos = 20;
        }
        else if (anguloZ >= 180f && anguloZ < 240f)
        {
            color = "Verde";
            puntos = 30;
        }
        else if (anguloZ >= 240f && anguloZ < 300f)
        {
            color = "Azul";
            puntos = 100;
        }
        else
        {
            color = "Violeta";
            puntos = 1;
        }

        monedas += puntos;
        ActualizarTexto();

        string mensaje = $"{color}: +{puntos} monedas (Z: {anguloZ:F0}°)";
        Debug.Log(mensaje + " | Total: " + monedas);

        if (textoResultado != null)
        {
            textoResultado.text = mensaje;
        }
    }

    public void GuardarMonedas()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream archivo = File.Create(rutaArchivo);

        DatosRuleta datos = new DatosRuleta();
        datos.monedasGuardadas = monedas;

        bf.Serialize(archivo, datos);
        archivo.Close();
    }

    public void CargarMonedas()
    {
        if (File.Exists(rutaArchivo))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream archivo = File.Open(rutaArchivo, FileMode.Open);

            DatosRuleta datos = (DatosRuleta)bf.Deserialize(archivo);
            archivo.Close();

            monedas = datos.monedasGuardadas;
        }
    }

    void ActualizarTexto()
    {
        if (textoMonedas != null)
        {
            textoMonedas.text = "Monedas: " + monedas;
        }
    }

    public void QuitGame()
    {
        GuardarMonedas();
        Application.Quit();
        Debug.Log("El juego se ha cerrado.");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(ruletaTransform.position, ruletaTransform.position + ruletaTransform.up * largoLineaDeReferencia);
    }
}