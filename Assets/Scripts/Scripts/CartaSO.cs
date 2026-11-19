using UnityEngine;

[CreateAssetMenu(fileName = "NuevaCarta", menuName = "ColeccionCartas/Carta", order = 0)]
public class CartaSO : ScriptableObject
{
    public string nombreCarta;
    [TextArea] public string descripcion;
    public int poder;
}