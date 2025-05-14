using Unity.Netcode;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class BuildSystem : NetworkBehaviour
{

    bool isBuilding = false;

    PlayerData playerData;

    GameObject selectedBlock;

    public GameObject wall;

    GameObject cam;

    public float gridSizeX = 1f;
    public float gridSizeY = 1f;

    public LayerMask layerMask;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerData = GetComponent<PlayerData>();
        cam = playerData.GetCameraGameObject();
    }

    // Update is called once per frame
    void Update()
    {

        if(!IsOwner) return;

        if(Input.GetKeyDown(KeyCode.B))
        {
            ChangeBuildMode(!isBuilding);
        }

        if(!isBuilding) return;

        if(!selectedBlock)
        {
            selectedBlock = Instantiate(wall);
            Collider col = selectedBlock.GetComponent<Collider>();
            if(col)
            {
                col.enabled = false;
            }
        }

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
                    //snaps block to the top of other one
                    float targetY = hit.collider.bounds.max.y;
                    float snappedY = Mathf.Round(targetY / gridSizeY) * gridSizeY;
                    selectedBlock.transform.position = new Vector3(x, snappedY + blockHeight / 2f, z);                    
                }     
            }
        }

        if(Input.GetKeyDown(KeyCode.R) && selectedBlock)
        {
            selectedBlock.transform.localEulerAngles += new Vector3(0,90,0);
        }

        if(Input.GetButtonDown("Fire1") && selectedBlock)
        {
            Vector3 blockPos = selectedBlock.transform.position;
            Quaternion blockRot = selectedBlock.transform.rotation;
            string cleanedName = selectedBlock.name.Replace("(Clone)", "").Trim();
            ServerBuildManager.Instance.BuildServerRpc(cleanedName, blockPos.x, blockPos.y, blockPos.z,blockRot.x, blockRot.y, blockRot.z, blockRot.w);
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
