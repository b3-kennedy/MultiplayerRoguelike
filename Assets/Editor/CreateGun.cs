using UnityEngine;
using UnityEditor;

public class CreateGun : EditorWindow
{
    private GameObject childToAssign;

    [MenuItem("Tools/Create Gun")]
    public static void ShowWindow()
    {
        GetWindow<CreateGun>("Create Gun");
    }

    void OnGUI()
    {
        GUILayout.Label("Custom Object Creator", EditorStyles.boldLabel);
        
        childToAssign = (GameObject)EditorGUILayout.ObjectField("Child GameObject", childToAssign, typeof(GameObject), true);

        if (GUILayout.Button("Create Parent Object"))
        {
            CreateObject();
        }
    }

    private void CreateObject()
    {
        // Create the parent GameObject
        GameObject parent = new GameObject("CustomObject");
        Undo.RegisterCreatedObjectUndo(parent, "Create Custom Object");

        // Add required components
        parent.AddComponent<Animator>();
        var collider = parent.AddComponent<BoxCollider>();
        parent.AddComponent<Rigidbody>();
        parent.layer = 6;
        collider.size = new Vector3(0.03f, 0.27f, 0.77f);

        // Assign child if provided
        if (childToAssign != null)
        {
            GameObject childInstance = Instantiate(childToAssign);
            childInstance.name = childToAssign.name; // Optional: keep original name
            childInstance.transform.SetParent(parent.transform);
            childInstance.transform.localPosition = Vector3.zero;
            childInstance.layer = 6;
        }

        Selection.activeGameObject = parent;
    }
}

