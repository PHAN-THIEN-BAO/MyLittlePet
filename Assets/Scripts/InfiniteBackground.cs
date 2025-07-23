using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Multi-layer infinite scrolling background script with parallax effect.
/// Attach this script to a parent GameObject that will contain all your background layers.
/// </summary>
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
        // Initialize and normalize the scroll direction
        if (scrollDirection != Vector2.zero)
        {
            scrollDirection.Normalize();
        }
        else
        {
            scrollDirection = Vector2.left;
        }
        
        // Initialize each layer
        foreach (BackgroundLayer layer in backgroundLayers)
        {
            if (layer.layerObject == null)
            {
                Debug.LogError("Layer object is missing in one of the background layers!");
                continue;
            }
            
            // Get the sprite renderer component
            SpriteRenderer spriteRenderer = layer.layerObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                Debug.LogError("Sprite Renderer component not found on layer: " + layer.layerObject.name);
                continue;
            }
            
            // Calculate the width of the background sprite
            layer.spriteWidth = spriteRenderer.bounds.size.x;
            
            // Set sprite to repeat for clean tiling (avoid seams)
            spriteRenderer.drawMode = SpriteDrawMode.Tiled;
            
            // Calculate reset position (with a small buffer to prevent gaps)
            if (autoCalculateResetPosition)
            {
                // Add a small buffer (10% of width) to ensure smooth transition
                layer.resetPosition = -layer.spriteWidth * 0.9f;
            }
            else
            {
                layer.resetPosition = -layer.spriteWidth;
            }
            
            // Remember start position
            layer.startPosition = layer.layerObject.transform.position.x;
            
            // Create background copies for this layer
            CreateLayerCopies(layer);
        }
    }
      private void Update()
    {
        foreach (BackgroundLayer layer in backgroundLayers)
        {
            if (layer.instances == null || layer.instances.Length == 0)
                continue;
                
            // Calculate movement amount
            float movement;
            if (useTimeBasedScrolling)
            {
                movement = baseScrollSpeed * layer.speedMultiplier * Time.deltaTime;
            }
            else
            {
                movement = baseScrollSpeed * layer.speedMultiplier * 0.01f;
            }
            
            // Move all copies of this layer
            for (int i = 0; i < layer.instances.Length; i++)
            {
                if (layer.instances[i] == null)
                    continue;
                    
                // Move the background
                layer.instances[i].transform.Translate(scrollDirection.x * movement, scrollDirection.y * movement, 0);
                
                // Check if the background needs to be reset (mainly checking x-position for horizontal scrolling)
                if ((scrollDirection.x < 0 && layer.instances[i].transform.position.x <= layer.resetPosition) ||
                    (scrollDirection.x > 0 && layer.instances[i].transform.position.x >= -layer.resetPosition))
                {
                    // Find the rightmost (or leftmost for right scrolling) background instance
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
                        // Position this instance just after the extreme instance
                        Vector3 newPosition = layer.instances[i].transform.position;
                        newPosition.x = extremePosition + (layer.spriteWidth * (scrollDirection.x < 0 ? 1 : -1));
                        layer.instances[i].transform.position = newPosition;
                    }
                    else
                    {
                        // Fallback to the old method if something went wrong
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
        // We need to ensure we have enough copies to cover the screen plus one extra for smooth scrolling
        int requiredCopies = Mathf.Max(layer.numberOfCopies, 3); // Minimum 3 copies to prevent blank spaces
        
        // Create the array of instances for this layer
        layer.instances = new GameObject[requiredCopies];
        
        // The original GameObject is the first one
        layer.instances[0] = layer.layerObject;
        
        // Create copies
        for (int i = 1; i < requiredCopies; i++)
        {
            // Calculate position for the new background
            Vector3 newPosition = layer.layerObject.transform.position;
            newPosition.x += layer.spriteWidth * i * (scrollDirection.x < 0 ? 1 : -1);
            
            // Create the new background
            layer.instances[i] = Instantiate(
                layer.layerObject, 
                newPosition, 
                Quaternion.identity, 
                layer.layerObject.transform.parent
            );
            
            // Rename the new background
            layer.instances[i].name = layer.layerObject.name + " (" + i + ")";
        }
    }
    
    // Visual gizmo to help with setup in the editor
    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying)
        {
            // Draw scroll direction
            Gizmos.color = Color.blue;
            Vector3 center = transform.position;
            Vector3 direction = new Vector3(scrollDirection.x, scrollDirection.y, 0).normalized;
            Gizmos.DrawLine(center, center + direction * 2);
            
            // Draw an arrow head
            Vector3 arrowPos = center + direction * 2;
            Vector3 right = Quaternion.Euler(0, 0, 45) * -direction * 0.5f;
            Vector3 left = Quaternion.Euler(0, 0, -45) * -direction * 0.5f;
            Gizmos.DrawLine(arrowPos, arrowPos + right);
            Gizmos.DrawLine(arrowPos, arrowPos + left);
            
            // Visualize each layer
            if (backgroundLayers != null)
            {
                foreach (BackgroundLayer layer in backgroundLayers)
                {
                    if (layer.layerObject != null)
                    {
                        SpriteRenderer sr = layer.layerObject.GetComponent<SpriteRenderer>();
                        if (sr != null)
                        {
                            // Different color for each layer based on speed multiplier
                            float hue = Mathf.Clamp01(layer.speedMultiplier / 3f);
                            Gizmos.color = Color.HSVToRGB(hue, 0.7f, 1f);
                            Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, 0.3f);
                            Gizmos.DrawCube(sr.bounds.center, sr.bounds.size);
                            
                            // Show where copies would be
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
