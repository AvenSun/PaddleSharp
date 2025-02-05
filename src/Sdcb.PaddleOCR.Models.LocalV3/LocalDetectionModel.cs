﻿using Sdcb.PaddleInference;
using Sdcb.PaddleOCR.Models.LocalV3.Details;

namespace Sdcb.PaddleOCR.Models.LocalV3;

/// <summary>
/// This class represents a local detection model used by PaddleOCR to detect text from an image.
/// </summary>
public class LocalDetectionModel : DetectionModel
{
    /// <summary>
    /// Gets the name of this model.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the version of this model.
    /// </summary>
    public ModelVersion Version { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalDetectionModel"/> class with the specified name and version.
    /// </summary>
    /// <param name="name">The name of the model.</param>
    /// <param name="version">The version of the model.</param>
    public LocalDetectionModel(string name, ModelVersion version)
    {
        Name = name;
        Version = version;
    }

    /// <inheritdoc/>
    public override PaddleConfig CreateConfig() => Utils.LoadLocalModel(Name);

    /// <summary>
    /// [New] Original lightweight model, supporting Chinese, English, multilingual text detection(Size: 3.8M)
    /// </summary>
    public static LocalDetectionModel ChineseV3 => new("ch_PP-OCRv3_det", ModelVersion.V3);

    /// <summary>
    /// [New] Original lightweight detection model, supporting English(Size: 3.8M)
    /// </summary>
    public static LocalDetectionModel EnglishV3 => new("en_PP-OCRv3_det", ModelVersion.V3);

    /// <summary>
    /// [New] Original lightweight detection model, supporting multiple languages(Size: 3.8M)
    /// </summary>
    public static LocalDetectionModel MultiLanguageV3 => new("ml_PP-OCRv3_det", ModelVersion.V3);

    /// <summary>
    /// Gets an array of all the available <see cref="LocalDetectionModel"/> objects.
    /// </summary>
    public static LocalDetectionModel[] All => new[]
    {
        ChineseV3,
        EnglishV3,
        MultiLanguageV3,
    };
}
