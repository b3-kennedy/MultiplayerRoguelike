using Unity.Netcode;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class BuildSystem : NetworkBehaviour
{

    bool isBuilding = false;

    bool isDestroyMode = false;

    PlayerData playerData;

    GameObject selectedBlock;

    public GameObject block;

    public GameObject wall;
    public GameObject floor;
    public GameObject ladder;

    GameObject cam;

    public float gridSizeX = 1f;
    public float gridSizeY = 1f;

    public LayerMask layerMask;

    GameObject previouslyHighlightedBlock;
    Material originalMaterial;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerData = GetComponent<PlayerData>();
        cam = playerData.GetCameraGameObject();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Destroy(selectedBlock);
            block = wall;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Destroy(selectedBlock);
            block = floor;
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Destroy(selectedBlock);
            block = ladder;
        }

        if (!IsOwner) return;

        PlayerInput();

        if(!isBuilding) return;

        PlaceHolder();

        if(!isDestroyMode) return;

        DestroyMode();

    }

    void DestroyMode()
    {
        if (selectedBlock)
        {
            Destroy(selectedBlock);
        }

        GameObject blockToDestroy = null;

        // Perform the raycast
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, 100, layerMask))
        {
            if (hit.collider.CompareTag("BuildingBlock"))
            {
                GameObject hitBlock = hit.collider.gameObject;

                // If we're looking at a new block, revert the previous one
                if (previouslyHighlightedBlock && hitBlock != previouslyHighlightedBlock)
                {
                    // Restore previous material
                    previouslyHighlightedBlock.GetComponent<MeshRenderer>().material = originalMaterial;
                    previouslyHighlightedBlock = null;
                    originalMaterial = null;
                }

                // Only highlight if not already highlighted
                if (hitBlock != previouslyHighlightedBlock)
                {
                    previouslyHighlightedBlock = hitBlock;
                    originalMaterial = new Material(hitBlock.GetComponent<MeshRenderer>().material); // Copy original

                    ChangeMaterial(hitBlock, 0.75f, Color.red); // Apply highlight
                }

                blockToDestroy = hitBlock;
            }
            else
            {
                //if hit something but not a building block
                if (previouslyHighlightedBlock)
                {
                    previouslyHighlightedBlock.GetComponent<MeshRenderer>().material = originalMaterial;
                    previouslyHighlightedBlock = null;
                    originalMaterial = null;
                }
            }
        }
        else
        {
            // If we are not hitting anything, reset the previous highlight
            if (previouslyHighlightedBlock)
            {
                previouslyHighlightedBlock.GetComponent<MeshRenderer>().material = originalMaterial;
                previouslyHighlightedBlock = null;
                originalMaterial = null;
            }
        }

        // Handle destruction input
        if (Input.GetButtonDown("Fire1") && blockToDestroy)
        {
            ServerBuildManager.Instance.DestroyServerRpc(blockToDestroy.GetComponent<NetworkObject>().NetworkObjectId);
            ServerManager.Instance.BuildNavmeshServerRpc();
        }
    }

    void PlayerInput()
    {
        if(Input.GetKeyDown(KeyCode.B))
        {
            ChangeBuildMode(!isBuilding);
        }

        if(!isBuilding) return;

        if(Input.GetButtonDown("Fire3"))
        {
            ChangeDestroyMode(!isDestroyMode);
        }
    }

    void ChangeDestroyMode(bool value)
    {
        isDestroyMode = value;
    }

    public void ChangeMaterial(GameObject block, float alpha, Color? newColor = null)
    {
        // Clone the material
        Material selectedBlockMat = new Material(block.GetComponent<MeshRenderer>().material);
        block.GetComponent<MeshRenderer>().material = selectedBlockMat;

        // Set the surface type to Transparent
        selectedBlockMat.SetFloat("_Surface", 1f); // 1 = Transparent
        selectedBlockMat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        selectedBlockMat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

        // Set blend and depth options for transparency
        selectedBlockMat.SetInt("_ZWrite", 0);
        selectedBlockMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        selectedBlockMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        selectedBlockMat.SetOverrideTag("RenderType", "Transparent");

        // Get the current color (from _BaseColor or fallback)
        Color finalColor;
        if (selectedBlockMat.HasProperty("_BaseColor"))
        {
            finalColor = selectedBlockMat.GetColor("_BaseColor");
        }
        else
        {
            finalColor = selectedBlockMat.color;
        }

        // If a new color is provided, update RGB (preserve current if null)
        if (newColor.HasValue)
        {
            finalColor.r = newColor.Value.r;
            finalColor.g = newColor.Value.g;
            finalColor.b = newColor.Value.b;
        }

        // Always update alpha
        finalColor.a = Mathf.Clamp01(alpha);

        // Apply final color
        if (selectedBlockMat.HasProperty("_BaseColor"))
        {
            selectedBlockMat.SetColor("_BaseColor", finalColor);
        }
        else
        {
            selectedBlockMat.color = finalColor;
        }
    }

    void PlaceHolder()
    {
        if(!selectedBlock && !isDestroyMode)
        {
            selectedBlock = Instantiate(block);

            ChangeMaterial(selectedBlock, 0.75f);
            
            Collider col = selectedBlock.GetComponent<Collider>();
            if(col)
            {
                col.enabled = false;
            }
        }

        if(!selectedBlock) return;

        if(Physics.Raycast(cam.transform.position, cam.transform.forward,out RaycastHit hit,100,layerMask))
        {
            if(hit.collider)
            {
                // Snap the point to the nearest grid point
                float x = Mathf.Round(hit.point.x / gridSizeX) * gridSizeX;
                float y = Mathf.Round(hit.point.y / gridSizeY) * gridSizeY;
                float z = Mathf.Round(hit.point.z / gridSizeX) * gridSizeX;

                float blockHeight = selectedBlock.transform.localScale.y;
                Vector3 snappedPosition = new Vector3(x, y, z); 

                //normal is (0,1,0) when looking at floor
                if(hit.normal == Vector3.up)
                {
                    selectedBlock.transform.position = snappedPosition + new Vector3(0, blockHeight/2, 0); 
                }
                else
                {
                    if (selectedBlock.GetComponent<Ladder>())
                    {
                        // Center on the block being looked at
                        Vector3 blockCenter = hit.collider.bounds.center;

                        // Push the ladder forward slightly so it sits against the face
                        Vector3 faceOffset = hit.normal * (selectedBlock.transform.localScale.z / 2f);

                        selectedBlock.transform.position = blockCenter + faceOffset;
                        selectedBlock.transform.rotation = Quaternion.LookRotation(-hit.normal); // face away from wall
                    }
                    else if (selectedBlock.GetComponent<Floor>())
                    {
                        // Snap to the top of the block being looked at
                        float targetY = hit.collider.bounds.max.y;
                        float snappedY = Mathf.Round(targetY / gridSizeY) * gridSizeY;

                        Vector3 offset = hit.normal * (selectedBlock.transform.localScale.z / 2f);

                        Vector3 snappedPos = new Vector3(x, snappedY + blockHeight / 2f, z) + new Vector3(offset.x, 0, offset.z);

                        selectedBlock.transform.position = snappedPos;
                    }
                    else
                    {
                        //snaps block to the top of other one
                        float targetY = hit.collider.bounds.max.y;
                        float snappedY = Mathf.Round(targetY / gridSizeY) * gridSizeY;
                        selectedBlock.transform.position = new Vector3(x, snappedY + blockHeight / 2f, z);
                    }
                }     
            }
        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            selectedBlock.transform.localEulerAngles += new Vector3(0,90,0);
        }

        if (Input.GetButtonDown("Fire1"))
        {
            Vector3 blockPos = selectedBlock.transform.position;
            Quaternion blockRot = selectedBlock.transform.rotation;
            string cleanedName = StringManager.RemoveCloneString(selectedBlock.name);
            ServerBuildManager.Instance.BuildServerRpc(cleanedName, blockPos.x, blockPos.y, blockPos.z, blockRot.x, blockRot.y, blockRot.z, blockRot.w);
            ServerManager.Instance.BuildNavmeshServerRpc();
        }
    }

    public void ChangeBuildMode(bool value)
    {
        isBuilding = value;
        if(value)
        {
            playerData.GetGunParent().gameObject.SetActive(false);
        }
        else
        {   
            if(selectedBlock)
            {
                Destroy(selectedBlock);
            }
            playerData.GetGunParent().gameObject.SetActive(true);       
        }
    }
}
