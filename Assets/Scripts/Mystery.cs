using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Mystery : MonoBehaviour
{
    GameObject level;
    Collider c;
    
    [System.Serializable]
    public struct RenderFeatureToggle
    {
        public ScriptableRendererFeature feature;
        public bool isEnabled;
    }
    [SerializeField]
    private List<RenderFeatureToggle> renderFeatures = new List<RenderFeatureToggle>();
    [SerializeField]
    private UniversalRenderPipelineAsset pipelineAsset;
    

    // Start is called before the first frame update
    void Start()
    {
        level = GameObject.Find("Level");
        c = gameObject.AddComponent<Collider>();
        c.isTrigger = true;

        Bounds bounds = GetComponent<Collider>().bounds;
        bounds.size *= 0.2f;

        GetComponent<MeshCollider>().convex = true;
        GetComponent<MeshCollider>().isTrigger = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.P))
        {
            renderFeatures[0].feature.SetActive(true);
        }
        else
        {
            renderFeatures[0].feature.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // when player enter mystery field, it shows where enemies are
        // get player from the level
        Debug.Log("MYSTERY");
    }
}
