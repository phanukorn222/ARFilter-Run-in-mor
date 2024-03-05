using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class FaceAttachment : MonoBehaviour
{
    public ARFaceManager arFaceManager;
    public GameObject facePrefab; // The PNG image you want to attach to the face

    private Dictionary<TrackableId, GameObject> attachedFaces = new Dictionary<TrackableId, GameObject>();

    void Start()
    {
        if (arFaceManager == null)
        {
            arFaceManager = FindObjectOfType<ARFaceManager>();
        }

        // Subscribe to the ARFaceManager's event for when a face is added
        arFaceManager.facesChanged += OnFacesChanged;
    }

    void OnFacesChanged(ARFacesChangedEventArgs eventArgs)
    {
        SendToFlutter.Send("Faces count = " + eventArgs.added.Count);
        SendToFlutter.Send("requestedMaximumFaceCount = " + arFaceManager.requestedMaximumFaceCount);
        SendToFlutter.Send("supportedFaceCount = " + arFaceManager.supportedFaceCount);
        foreach (var face in eventArgs.added)
        {
            AttachFaceToTrackable(face);
        }

        foreach (var face in eventArgs.updated)
        {
            UpdateAttachedFacePosition(face);
        }

        foreach (var face in eventArgs.removed)
        {
            RemoveAttachedFace(face);
        }
    }

    void AttachFaceToTrackable(ARFace face)
    {
        if (facePrefab == null)
        {
            SendToFlutter.Send("Face prefab is not assigned.");
            Debug.LogError("Face prefab is not assigned.");
            return;
        }

        if (attachedFaces.ContainsKey(face.trackableId))
        {
            RemoveAttachedFace(face);
        }

        GameObject newAttachedFace = Instantiate(facePrefab, face.transform);
        // Adjust position and rotation as needed
        attachedFaces.Add(face.trackableId, newAttachedFace);
    }

    void UpdateAttachedFacePosition(ARFace face)
    {
        if (attachedFaces.TryGetValue(face.trackableId, out GameObject attachedFace))
        {
            // Update position and rotation as needed
            // You can adjust the position based on some dynamic calculations here
        }
    }

    void RemoveAttachedFace(ARFace face)
    {
        if (attachedFaces.TryGetValue(face.trackableId, out GameObject attachedFace))
        {
            Destroy(attachedFace);
            attachedFaces.Remove(face.trackableId);
        }
    }

    // Public method to adjust the position of the attached face prefab
    public void AdjustFacePosition(Vector3 position)
    {
        foreach (var attachedFace in attachedFaces.Values)
        {
            attachedFace.transform.localPosition = position;
        }
    }

    public void SetFaceFilterPrefabs(string data) {
        SendToFlutter.Send("Prefabs/" + data);
        facePrefab = Resources.Load("Prefabs/" + data, typeof(GameObject)) as GameObject;

        if(facePrefab != null) {
            SendToFlutter.Send("Face prefab is assigned.");
            // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    } 

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SendToFlutter.Send("scene_loaded");
    }

    // called when the game is terminated
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
