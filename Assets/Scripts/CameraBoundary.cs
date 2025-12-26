using UnityEngine;

public class CameraBoundary : MonoBehaviour
{
    public Transform target; // O jogador que a câmera segue
    public float minX;       // Limite Esquerdo (World Coordinate)
    public float maxX;       // Limite Direito (World Coordinate)
    public float minY;       // Limite Inferior (World Coordinate)
    public float maxY;       // Limite Superior (World Coordinate)

    private float camHalfWidth;
    private float camHalfHeight;

    void Start()
    {
        // Calcula a metade da largura e altura da câmera ortográfica
        Camera cam = Camera.main;
        camHalfHeight = cam.orthographicSize;
        camHalfWidth = cam.aspect * camHalfHeight;
    }

    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 targetPosition = new Vector3(target.position.x, target.position.y, transform.position.z);

            // Calcula a posição de parada X, levando em conta o tamanho da câmera
            float clampedX = Mathf.Clamp(
                targetPosition.x, 
                minX + camHalfWidth, 
                maxX - camHalfWidth
            );

            // Calcula a posição de parada Y, levando em conta o tamanho da câmera
            float clampedY = Mathf.Clamp(
                targetPosition.y, 
                minY + camHalfHeight, 
                maxY - camHalfHeight
            );

            // Define a nova posição da câmera
            transform.position = new Vector3(clampedX, clampedY, transform.position.z);
        }
    }
}