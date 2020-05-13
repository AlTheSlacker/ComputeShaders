using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderController : MonoBehaviour
{

    void Start()
    {
        // compute shader files have the extension .compute, but you do not use it to load them
        string computeShaderFile = "BasicComputeShader";

        // ensure you have saved the shader.compute file in an asset folder called "Resources" to allow this type of loading
        ComputeShader basicComputeShader = Resources.Load(computeShaderFile) as ComputeShader;

        // a couple of experimental arrays
        // note we can only send blittable data to the shader
        // https://docs.microsoft.com/en-us/dotnet/framework/interop/blittable-and-non-blittable-types
        // so no user structs / objects, these need to be pre-processed first
        double[] array1 = new double[50];
        double[] array2 = new double[10];

        // the compute buffer is the way of sending data to the compute shader
        ComputeBuffer buffer_array1 = new ComputeBuffer(array1.Length, sizeof(double));
        buffer_array1.SetData(array1);
        // you can index the kernel directly, e.g. basicComputeShader.SetBuffer(0, "cs_array", buffer_array1);
        // but finding names seems less error prone
        basicComputeShader.SetBuffer(basicComputeShader.FindKernel("DoubleIndex"), "cs_array", buffer_array1);

        // send some more data using a new buffer, this is just for demonstration purposes and is not needed here
        ComputeBuffer buffer_array2 = new ComputeBuffer(array2.Length, sizeof(double));
        buffer_array2.SetData(array2);
        basicComputeShader.SetBuffer(basicComputeShader.FindKernel("DoubleIndex"), "ref_array", buffer_array2);

        // run the compute shader
        // dispatch defines the distribution of thread groups X x Y x Z, each cell in this 3 dimensional grid is a thread group
        // the id.x id.y and id.z are scaled by this value:
        // basicComputeShader.Dispatch(basicComputeShader.FindKernel("DoubleIndex"), 2, 1, 1);
        // used with a [numthreads(32,1,1)] compute shader will give an id.x range of 0-63
        basicComputeShader.Dispatch(basicComputeShader.FindKernel("DoubleIndex"), 2, 1, 1);

        // get the data that the shader was intended to modify
        buffer_array1.GetData(array1);

        // check it worked (prints 4)
        for (int i = 0; i < array1.Length; i++)
        {
            print("i(" + i + "): " + array1[i]);
        }

        // release the buffers
        buffer_array1.Release();
        buffer_array2.Release();

    }

}
