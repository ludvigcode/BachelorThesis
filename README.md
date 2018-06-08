# Image Quality-Driven Level of Detail Selection on a Triangle Budget

[![alt tag](https://i2.wp.com/draketuroth.files.wordpress.com/2018/06/lod-selection.png?ssl=1&w=900)](https://vimeo.com/274004621)

This paper proposes an image quality-driven process to choose a LOD combination given a camera position and orientation, and a triangle budget. The metric to assess the quality of the rendered image is the structural similarity (SSIM) index, which is a popular choice in computer science for image comparison for its ability to approximate similarity between images. 

The aim of this thesis is to determine the difference in image quality between a custom level of detail pre-preprocessing approach proposed in this paper, and the level of detail system built in the game engine Unity. This is investigated by implementing a framework in Unity for the proposed level of detail pre-preprocessing approach in this paper and designing representative test scenes to collect all data samples. Once the data is collected, the image quality produced by the proposed level of detail pre-preprocessing approach is compared to Unity's existing level of detail approach using perceptual-based metrics. 

Click the image to view the demonstration on Vimeo.
