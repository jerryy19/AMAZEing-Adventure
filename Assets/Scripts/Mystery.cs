using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Mystery : MonoBehaviour
{
    GameObject level;
    Collider c;

    private List<ScriptableRendererFeature> renderFeatures; 
    [SerializeField]
    private UniversalRenderPipelineAsset pipelineAsset;
    

    // Start is called before the first frame update
    void Start()
    {
        level = GameObject.Find("Level");
        // c = gameObject.AddComponent<Collider>();
        // c.isTrigger = true;

        pipelineAsset = (UniversalRenderPipelineAsset)AssetDatabase.LoadAssetAtPath("Assets/URP Asset.asset", typeof(UniversalRenderPipelineAsset));
        ScriptableRenderer renderer = pipelineAsset.GetRenderer(0);
        var property = typeof(ScriptableRenderer).GetProperty("rendererFeatures", BindingFlags.NonPublic | BindingFlags.Instance);
        renderFeatures = property.GetValue(renderer) as List<ScriptableRendererFeature>;
        renderFeatures[0].SetActive(false);

        Bounds bounds = GetComponent<Collider>().bounds;
        bounds.size *= 0.2f;

        GetComponent<MeshCollider>().convex = true;
        GetComponent<MeshCollider>().isTrigger = true;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        // when player enter mystery field, it shows where enemies are
        // get player from the level
        Debug.Log("MYSTERY");
        if (other.name == "BigVegas(Clone)")
            renderFeatures[0].SetActive(true);
    }
    private void OnTriggerExit(Collider other)
    {
        renderFeatures[0].SetActive(false);
    }
}
