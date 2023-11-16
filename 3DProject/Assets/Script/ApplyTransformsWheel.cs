using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ApplyTransformsWheel : MonoBehaviour
{
    [SerializeField] int wheel;
    private Vector3 displacement;
    [SerializeField] AXIS rotationAxis;
    Mesh mesh;
    Vector3[] baseVertices;
    Vector3[] newVertices;
    Vector3[] wheelDisplacement;
    // Start is called before the first frame update
    void Start()
    {
        displacement = GameObject.Find("SceneManager(Defines translations)").GetComponent<GlobalVariables>().displacement;
        //Array of vertices that show the translation for each wheel
        wheelDisplacement = new Vector3[4];
        wheelDisplacement[0] = new Vector3(0.8f, 0.4f, 1.5f);
        wheelDisplacement[1] = new Vector3(0.8f, 0.4f, -1.4f);
        wheelDisplacement[2] = new Vector3(-0.8f, 0.4f, 1.5f);
        wheelDisplacement[3] = new Vector3(-0.8f, 0.4f, -1.4f);

        mesh = GetComponentInChildren<MeshFilter>().mesh;
        baseVertices = mesh.vertices;

        newVertices = new Vector3[baseVertices.Length];
        for (int i = 0; i < baseVertices.Length; i++)
        {
            newVertices[i] = baseVertices[i];
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        DoTransform();
    }

    void DoTransform(){
        //Define initial matrices
        Matrix4x4 move = HW_Transforms.TranslationMat(wheelDisplacement[wheel].x, wheelDisplacement[wheel].y, wheelDisplacement[wheel].z);
        Matrix4x4 rotate = HW_Transforms.RotateMat(90, AXIS.Y);
        Matrix4x4 scaled = HW_Transforms.ScaleMat(0.35f, 0.35f, 0.35f);

        Matrix4x4 rotateWheels = HW_Transforms.RotateMat(-90*Time.time, AXIS.X);
        Matrix4x4 moveWheels = HW_Transforms.TranslationMat(displacement.x *Time.time , displacement.y *Time.time, displacement.z *Time.time);

        Matrix4x4 composite = moveWheels * move * rotateWheels * rotate * scaled;

        for (int i=0; i<newVertices.Length; i++)
        {
            Vector4 temp = new Vector4(baseVertices[i].x, baseVertices[i].y, baseVertices[i].z, 1);

            newVertices[i] = composite * temp;
        }

        mesh.vertices = newVertices;
    }
}