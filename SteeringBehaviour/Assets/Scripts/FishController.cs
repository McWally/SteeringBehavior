using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FishController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float maxSpeed = 5f; // Velocidad máxima del pez
    [SerializeField] private float maxForce = 10f; // Fuerza máxima de dirección
    [SerializeField] private float mass = 1f; // Masa del pez
    [SerializeField] private float wanderRadius = 2f; // Radio de vagar
    [SerializeField] private float wanderDistance = 5f; // Distancia de vagar
    [SerializeField] private float wanderChange = 30f; // Grados de cambio al vagar
    [SerializeField] private float raycastDetectionDistance = 5f; // Distancia de detección de los walls

    [Header("Fish Settings")]
    public bool isBigFish; // Variable pública para determinar si el pez es grande o pequeño
    private Vector2 velocity; // Velocidad actual del pez
    private float wanderAngle; // Ángulo de vagar
    [SerializeField] private Transform target; // Objetivo al que el pez está persiguiendo
    [SerializeField] private Transform hunter; // Cazador del que el pez está huyendo

    ParticleSystem particles;
    

    void Start()
    {
        particles = transform.GetComponentInChildren<ParticleSystem>();
    }

    void Update()
    {
        Vector2 avoidanceForce = CollisionAvoidance();
        // Debug.Log(avoidanceForce);

        var emission = particles.emission;
        // var main = particles.main;

        if (target != null && hunter == null)
        {
            // Si hay un objetivo, persigue el objetivo
            ApplySteering(Seek(target.position) + avoidanceForce);
            maxSpeed = 15;
        }
        else if (hunter != null)
        {
            target = null;
            // Si hay un cazador, huye del cazador
            ApplySteering(Flee(hunter.position) + avoidanceForce);
            maxSpeed = 20;

            // Cambia la cantidad de partículas emitidas en base a la speed del pez
            emission.rateOverTime = maxSpeed -5;    
            // main.startSpeed = 8;
        }
        else
        {
            // Si no hay objetivo ni cazador, vagar
            Wander(avoidanceForce);

            // Reiniciar valores base
            emission.rateOverTime = isBigFish ? 10 : 8;
            maxSpeed = isBigFish ? 10 : 12;
            // main.startSpeed = isBigFish ? 1 : 1.5f;
        }

    }

    // Wander
    private void Wander(Vector2 avoidanceF)
    {
        wanderAngle += Random.Range(-wanderChange, wanderChange);
        Vector2 wanderCenter = (Vector2)transform.position + velocity.normalized * wanderDistance;
        Vector2 targetWander = wanderCenter + new Vector2(Mathf.Cos(wanderAngle * Mathf.Deg2Rad) * wanderRadius, Mathf.Sin(wanderAngle * Mathf.Deg2Rad) * wanderRadius);
        ApplySteering(Seek(targetWander) + avoidanceF);
    }

    // Seek
    private Vector2 Seek(Vector2 targetPosition)
    {
        Vector2 desiredVelocity = (targetPosition - (Vector2)transform.position).normalized * maxSpeed;
        Vector2 steering = desiredVelocity - velocity;
        return Vector2.ClampMagnitude(steering, maxForce);
    }

    // Flee
    private Vector2 Flee(Vector2 targetPosition)
    {
        Vector2 desiredVelocity = ((Vector2)transform.position - targetPosition).normalized * maxSpeed;
        Vector2 steering = desiredVelocity - velocity;
        return Vector2.ClampMagnitude(steering, maxForce);
    }

    private void ApplySteering(Vector2 steeringForce)
    {
        velocity = Vector2.ClampMagnitude(velocity + steeringForce / mass, maxSpeed);
        transform.position += (Vector3)velocity * Time.deltaTime;

        // Rotar el pez para que "mire" a la dirección del movimiento (podria implementar flip de sprite para cuando da la vuelta pero pfff)

        if (velocity != Vector2.zero)
        {
            float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
    }

    // Fuerza de evitación de los walls
    private Vector2 CollisionAvoidance()
    {
        Vector2 steeringForce = Vector2.zero;
        RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position, velocity.normalized, raycastDetectionDistance); // Raycast all para evitar chocar con colliders propios o de otros
        for (int i = 0; i < hit.Length; i++)
        {
            if (hit[i].collider.CompareTag("Wall"))
            {
                // Debug.Log("avoid Wall");


                // Punto de impacto del raycast
                Vector2 hitPoint = hit[i].point;

                // Calcular la dirección opuesta al punto de impacto
                Vector2 avoidanceDirection = ((Vector2)transform.position - hitPoint).normalized;

                // Cambiar la dirección del pez drásticamente en el lado contrario
                Vector2 desiredVelocity = avoidanceDirection * maxSpeed;

                // Forzar un giro completo hacia la dirección opuesta
                velocity = desiredVelocity;
                steeringForce = Vector2.ClampMagnitude(desiredVelocity - velocity, maxForce);

                break;
            }
        }

        return steeringForce;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Detectar otros peces y decidir si son targets o hunters
        if (other.CompareTag("Fish")) // podria hacerlo detectando su script/clase
        {
            FishController otherFish = other.GetComponent<FishController>();
            if (otherFish != null)
            {
                if (isBigFish && !otherFish.isBigFish)
                {
                    target = other.transform;
                }
                else if (!isBigFish && otherFish.isBigFish)
                {
                    hunter = other.transform;
                }
            }
        }

        if (other.CompareTag("Player")) hunter = other.transform; // el player siempre será hunter
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Limpiar objetivo/cazador al salir del rango
        if (other.CompareTag("Fish"))
        {
            FishController otherFish = other.GetComponent<FishController>();
            if (otherFish.isBigFish)
            {
                hunter = null;
            }
            else if (!otherFish.isBigFish)
            {
                target = null;
            }
        }

        if (other.CompareTag("Player"))
        {
            hunter = null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector2 direction = velocity.normalized;
        Vector2 startPoint = transform.position;

        // Dibuja la línea del raycast
        Gizmos.DrawLine(startPoint, startPoint + direction * raycastDetectionDistance);
    }
}