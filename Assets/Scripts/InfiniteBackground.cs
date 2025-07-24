using UnityEngine;
using System.Collections.Generic;

public class InfiniteBackground : MonoBehaviour
{    [System.Serializable]
    public class BackgroundLayer
    {
        [Tooltip("The GameObject containing the sprite for this layer")]
        public GameObject layerObject;
        
        [Tooltip("Scrolling speed multiplier for this layer (1 = base speed, <1 = slower, >1 = faster)")]
        public float speedMultiplier = 1f;
        
        [Tooltip("Number of copies for this layer (minimum 3 recommended)")]
        [Range(3, 10)]
        public int numberOfCopies = 3;
        
        [HideInInspector]
        public GameObject[] instances;
        
        [HideInInspector]
        public float spriteWidth;
        
        [HideInInspector]
        public float resetPosition;
        
        [HideInInspector]
        public float startPosition;
    }
    
    [Header("Base Scroll Settings")]
    [Tooltip("Base scrolling speed applied to all layers")]
    public float baseScrollSpeed = 5f;
    
    [Tooltip("Direction of scrolling")]
    public Vector2 scrollDirection = Vector2.left;
    
    [Header("Background Layers")]
    [Tooltip("Define each background layer from back to front")]
    public List<BackgroundLayer> backgroundLayers = new List<BackgroundLayer>();
    
    [Header("Optional Settings")]
    [Tooltip("If true, will scroll based on time. If false, will scroll based on frames.")]
    public bool useTimeBasedScrolling = true;
    
    [Tooltip("If true, will automatically calculate the reset position based on sprite renderer size")]
    public bool autoCalculateResetPosition = true;
      private void Start()
    {
        if (scrollDirection != Vector2.zero)
        {
            scrollDirection.Normalize();
        }
        else
        {
            scrollDirection = Vector2.left;
        }
        
        foreach (BackgroundLayer layer in backgroundLayers)
        {
            if (layer.layerObject == null)
            {
                Debug.LogError("Layer object is missing in one of the background layers!");
                continue;
            }
            
            SpriteRenderer spriteRenderer = layer.layerObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                Debug.LogError("Sprite Renderer component not found on layer: " + layer.layerObject.name);
                continue;
            }
            
            layer.spriteWidth = spriteRenderer.bounds.size.x;
            
            spriteRenderer.drawMode = SpriteDrawMode.Tiled;
            
            if (autoCalculateResetPosition)
            {
                layer.resetPosition = -layer.spriteWidth * 0.9f;
            }
            else
            {
                layer.resetPosition = -layer.spriteWidth;
            }
            
            layer.startPosition = layer.layerObject.transform.position.x;
            
            CreateLayerCopies(layer);
        }
    }
      private void Update()
    {
        foreach (BackgroundLayer layer in backgroundLayers)
        {
            if (layer.instances == null || layer.instances.Length == 0)
                continue;
                
            float movement;
            if (useTimeBasedScrolling)
            {
                movement = baseScrollSpeed * layer.speedMultiplier * Time.deltaTime;
            }
            else
            {
                movement = baseScrollSpeed * layer.speedMultiplier * 0.01f;
            }
            
            for (int i = 0; i < layer.instances.Length; i++)
            {
                if (layer.instances[i] == null)
                    continue;
                    
                layer.instances[i].transform.Translate(scrollDirection.x * movement, scrollDirection.y * movement, 0);
                
                if ((scrollDirection.x < 0 && layer.instances[i].transform.position.x <= layer.resetPosition) ||
                    (scrollDirection.x > 0 && layer.instances[i].transform.position.x >= -layer.resetPosition))
                {
                    float extremePosition = scrollDirection.x < 0 ? float.MinValue : float.MaxValue;
                    int extremeIndex = -1;
                    
                    for (int j = 0; j < layer.instances.Length; j++)
                    {
                        if (layer.instances[j] == null) continue;
                        
                        float xPos = layer.instances[j].transform.position.x;
                        if ((scrollDirection.x < 0 && xPos > extremePosition) ||
                            (scrollDirection.x > 0 && xPos < extremePosition))
                        {
                            extremePosition = xPos;
                            extremeIndex = j;
                        }
                    }
                    
                    if (extremeIndex >= 0)
                    {
                        Vector3 newPosition = layer.instances[i].transform.position;
                        newPosition.x = extremePosition + (layer.spriteWidth * (scrollDirection.x < 0 ? 1 : -1));
                        layer.instances[i].transform.position = newPosition;
                    }
                    else
                    {
                        float newPositionX = layer.startPosition + (layer.spriteWidth * layer.instances.Length * (scrollDirection.x < 0 ? 1 : -1));
                        layer.instances[i].transform.position = new Vector3(
                            newPositionX, 
                            layer.instances[i].transform.position.y, 
                            layer.instances[i].transform.position.z
                        );
                    }
                }
            }
        }
    }
      private void CreateLayerCopies(BackgroundLayer layer)
    {
        int requiredCopies = Mathf.Max(layer.numberOfCopies, 3);
        
        layer.instances = new GameObject[requiredCopies];
        
        layer.instances[0] = layer.layerObject;
        
        for (int i = 1; i < requiredCopies; i++)
        {
            Vector3 newPosition = layer.layerObject.transform.position;
            newPosition.x += layer.spriteWidth * i * (scrollDirection.x < 0 ? 1 : -1);
            
            layer.instances[i] = Instantiate(
                layer.layerObject, 
                newPosition, 
                Quaternion.identity, 
                layer.layerObject.transform.parent
            );
            
            layer.instances[i].name = layer.layerObject.name + " (" + i + ")";
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying)
        {
            Gizmos.color = Color.blue;
            Vector3 center = transform.position;
            Vector3 direction = new Vector3(scrollDirection.x, scrollDirection.y, 0).normalized;
            Gizmos.DrawLine(center, center + direction * 2);
            
            Vector3 arrowPos = center + direction * 2;
            Vector3 right = Quaternion.Euler(0, 0, 45) * -direction * 0.5f;
            Vector3 left = Quaternion.Euler(0, 0, -45) * -direction * 0.5f;
            Gizmos.DrawLine(arrowPos, arrowPos + right);
            Gizmos.DrawLine(arrowPos, arrowPos + left);
            
            if (backgroundLayers != null)
            {
                foreach (BackgroundLayer layer in backgroundLayers)
                {
                    if (layer.layerObject != null)
                    {
                        SpriteRenderer sr = layer.layerObject.GetComponent<SpriteRenderer>();
                        if (sr != null)
                        {
                            float hue = Mathf.Clamp01(layer.speedMultiplier / 3f);
                            Gizmos.color = Color.HSVToRGB(hue, 0.7f, 1f);
                            Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, 0.3f);
                            Gizmos.DrawCube(sr.bounds.center, sr.bounds.size);
                            
                            for (int i = 1; i < layer.numberOfCopies; i++)
                            {
                                Vector3 copyPos = sr.bounds.center;
                                copyPos.x += sr.bounds.size.x * i * (scrollDirection.x < 0 ? 1 : -1);
                                Gizmos.DrawCube(copyPos, sr.bounds.size);
                            }
                        }
                    }
                }
            }
        }
    }
}