using UnityEngine;

public class Movimiento : MonoBehaviour
{
    public float velocidad = 5f;

    void Update()
    {
        float movimientoHorizontal = Input.GetAxis("Horizontal"); // A y D
        float movimientoVertical = Input.GetAxis("Vertical"); // W y S

        Vector3 movimiento = new Vector3(movimientoHorizontal, movimientoVertical, 0) * velocidad * Time.deltaTime;

        transform.Translate(movimiento);
    }
}