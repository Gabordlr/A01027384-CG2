using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyTransformsCar : MonoBehaviour
{
    [SerializeField] Vector3 displacement;
    [SerializeField] float angle;
    [SerializeField] AXIS rotationAxis;
    [SerializeField] GameObject wheel;
    Mesh mesh;
    Vector3[] baseVertices;
    Vector3[] newVertices;

    Matrix4x4 globalComposite;

    Vector3[] wheelDisplacement;
    private GameObject[] wheelArray = new GameObject[4];

    Mesh[] wheelsMesh = new Mesh[4];
    Vector3[][] wheelsBaseVertices = new Vector3[4][];
    Vector3[][] newWheelVertices = new Vector3[4][]; //vertices nuevos

    // Start is called before the first frame update
    void Start()
    {
        wheelDisplacement = new Vector3[4];
        wheelDisplacement[0] = new Vector3(0.8f, 0.4f, 1.5f);
        wheelDisplacement[1] = new Vector3(0.8f, 0.4f, -1.4f);
        wheelDisplacement[2] = new Vector3(-0.8f, 0.4f, 1.5f);
        wheelDisplacement[3] = new Vector3(-0.8f, 0.4f, -1.4f);

        wheelArray = new GameObject[4];
        for (int i = 0; i < 4; i++)
        {
            wheelArray[i] = Instantiate(wheel);

            wheelsMesh[i] = wheelArray[i].GetComponentInChildren<MeshFilter>().mesh;

            wheelsBaseVertices[i] = wheelsMesh[i].vertices;

            newWheelVertices[i] = new Vector3[wheelsBaseVertices[i].Length];
        }
        //initialize the 4 wheels


        mesh = GetComponentInChildren<MeshFilter>().mesh;
        baseVertices = mesh.vertices;

        newVertices = new Vector3[baseVertices.Length];
    }

    // Update is called once per frame
    void Update()
    {
        float angleRaduis = Mathf.Atan2(displacement.x, displacement.z) * Mathf.Rad2Deg;

        Matrix4x4 rotateDirection = HW_Transforms.RotateMat(-angleRaduis, AXIS.Y);
        //create the matrices
        Matrix4x4 move = HW_Transforms.TranslationMat(displacement.x * Time.time, displacement.y * Time.time, displacement.z * Time.time);

        //combine the matrices
        //operations are executed in backward order
        globalComposite = move * rotateDirection;

        DoTransform();
        for (int i = 0; i < 4; i++)
        {
            DoTransformWheels(i);
        }
    }

    void DoTransformWheels(int wheel_index)
    {
        Matrix4x4 initialMove = HW_Transforms.TranslationMat(wheelDisplacement[wheel_index].x, wheelDisplacement[wheel_index].y, wheelDisplacement[wheel_index].z);
        Matrix4x4 initialRotate = HW_Transforms.RotateMat(90, AXIS.Y);
        Matrix4x4 inititalScale = HW_Transforms.ScaleMat(0.35f, 0.35f, 0.35f);

        Matrix4x4 rotateWheels = HW_Transforms.RotateMat(-90*Time.time, AXIS.X);

        Matrix4x4 composite = globalComposite * initialMove * rotateWheels * initialRotate * inititalScale;

        for (int i = 0; i < newWheelVertices[wheel_index].Length; i++)
        {
            Vector4 temp = new Vector4(wheelsBaseVertices[wheel_index][i].x, wheelsBaseVertices[wheel_index][i].y, wheelsBaseVertices[wheel_index][i].z, 1);
            newWheelVertices[wheel_index][i] = composite * temp;
        }

        wheelsMesh[wheel_index].vertices = newWheelVertices[wheel_index];
        wheelsMesh[wheel_index].RecalculateNormals();
    }

    void DoTransform()
    {        
        for (int i = 0; i < newVertices.Length; i++)
        {
            Vector4 temp = new Vector4(baseVertices[i].x, baseVertices[i].y, baseVertices[i].z, 1);
            newVertices[i] = globalComposite * temp;
        }

        mesh.vertices = newVertices;
    }
}
