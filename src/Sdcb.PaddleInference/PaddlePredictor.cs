﻿using Sdcb.PaddleInference.Native;
using System;
using System.Text;

namespace Sdcb.PaddleInference;

/// <summary>
/// Wraps Paddle C Library's PaddlePredictor interface.
/// </summary>
public class PaddlePredictor : IDisposable
{
    private IntPtr _ptr;

    /// <summary>
    /// Initializes a new instance of the <see cref="PaddlePredictor"/> class.
    /// </summary>
    /// <param name="predictorPointer">Predictor pointer.</param>
    /// <exception cref="ArgumentNullException">Thrown when predictorPointer is null.</exception>
    public PaddlePredictor(IntPtr predictorPointer)
    {
        if (predictorPointer == IntPtr.Zero)
        {
            throw new ArgumentNullException(nameof(predictorPointer));
        }
        _ptr = predictorPointer;
    }

    /// <summary>
    /// Creates and returns a new instance of the <see cref="PaddlePredictor"/> class that is a copy of the current instance.
    /// </summary>
    /// <returns>A new instance of <see cref="PaddlePredictor"/> object that is a copy of the current instance.</returns>
    public PaddlePredictor Clone() => new(PaddleNative.PD_PredictorClone(_ptr));

    /// <summary>
    /// Gets the input tensor names of this predictor.
    /// </summary>
    public string[] InputNames
    {
        get
        {
            using PaddleNative.PdStringArrayWrapper wrapper = new() { ptr = PaddleNative.PD_PredictorGetInputNames(_ptr) };
            return wrapper.ToArray();
        }
    }

    /// <summary>
    /// Gets the output tensor names of this predictor.
    /// </summary>
    public string[] OutputNames
    {
        get
        {
            using PaddleNative.PdStringArrayWrapper wrapper = new() { ptr = PaddleNative.PD_PredictorGetOutputNames(_ptr) };
            return wrapper.ToArray();
        }
    }

    /// <summary>
    /// Gets the input tensor by name.
    /// </summary>
    /// <param name="name">Name of the input tensor to get.</param>
    /// <returns>An instance of <see cref="PaddleTensor"/> representing the input tensor.</returns>
    public unsafe PaddleTensor GetInputTensor(string name)
    {
        byte[] nameBytes = Encoding.UTF8.GetBytes(name);
        fixed (byte* ptr = nameBytes)
        {
            return new PaddleTensor(PaddleNative.PD_PredictorGetInputHandle(_ptr, (IntPtr)ptr));
        }
    }

    /// <summary>
    /// Gets the output tensor by name.
    /// </summary>
    /// <param name="name">Name of the output tensor to get.</param>
    /// <returns>An instance of <see cref="PaddleTensor"/> representing the output tensor.</returns>
    public unsafe PaddleTensor GetOutputTensor(string name)
    {
        byte[] nameBytes = Encoding.UTF8.GetBytes(name);
        fixed (byte* ptr = nameBytes)
        {
            return new PaddleTensor(PaddleNative.PD_PredictorGetOutputHandle(_ptr, (IntPtr)ptr));
        }
    }

    /// <summary>
    /// Gets the number of input tensors of this predictor.
    /// </summary>
    public long InputSize => PaddleNative.PD_PredictorGetInputNum(_ptr);

    /// <summary>
    /// Gets the number of output tensors of this predictor.
    /// </summary>
    public long OutputSize => PaddleNative.PD_PredictorGetOutputNum(_ptr);

    /// <summary>
    /// Runs the prediction with input data and generates model output.
    /// </summary>
    /// <returns>true if prediction runs successfully; false otherwise.</returns>
    public bool Run() => PaddleNative.PD_PredictorRun(_ptr) != 0;

    /// <summary>
    /// Frees the unmanaged resources used by the <see cref="PaddlePredictor"/> class.
    /// </summary>
    public void Dispose()
    {
        if (_ptr != IntPtr.Zero)
        {
            PaddleNative.PD_PredictorDestroy(_ptr);
            _ptr = IntPtr.Zero;
        }
    }
}
