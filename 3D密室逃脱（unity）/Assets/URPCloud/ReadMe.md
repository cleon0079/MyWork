# Unity Cloud Render Pipeline Setup Tool

## Introduction
This Unity package provides a simple and efficient tool for setting up cloud rendering in projects that use the Universal Render Pipeline (URP). With a custom `Cloud Render Pipeline` and `CloudContainer`, you can easily add realistic cloud rendering to your scenes using our tool.

The package includes an editor window that allows you to create and remove the cloud rendering feature and the necessary GameObject in your scenes with just a few clicks.

## Features
- **Automatic Setup**: Easily add or remove cloud rendering to your scene using the built-in tool.
- **URP Support**: Fully compatible with the Universal Render Pipeline.
- **Customizable Clouds**: Modify the cloud properties directly from the inspector or through code.
- **Sample Scene**: Demonstrates the setup and usage of the cloud rendering system.

## Installation
1. Download the package from the Unity Asset Store.
2. Open your Unity project.
3. Import the `UPRCloud` file.
4. After importing, the `Cloud Render Pipeline Setup Tool` will be available in the Unity Editor.

## How to Use
1. **Open the Tool**:
   - In the Unity Editor, go to **Tools -> Setup Cloud Render Pipeline**. This will open the custom editor window for managing the cloud rendering feature.

2. **Creating Cloud Rendering**:
   - In the editor window, click on the **Create** button.
   - This will automatically create a `CloudContainer` GameObject in your scene and add the `Cloud Render Pipeline` feature to your current URP.
   - Once created, run the game onec then you can modify cloud properties directly in the inspector for the `CloudContainer`.

3. **Removing Cloud Rendering**:
   - If you want to remove the cloud rendering feature and the associated `CloudContainer`, simply click the **Remove** button in the editor window.
   - This will remove both the `Cloud Render Pipeline` feature from URP and the `CloudContainer` from the scene.

## Requirements
- Unity version **2021.1** or higher.
- Universal Render Pipeline (URP) must be installed and active in the project.

## Sample Scene
- You can find a sample scene in `Assets/URPCloud/Scene/`. This scene demonstrates how to set up the cloud rendering system.
- Open the sample scene to see how the clouds behave and to customize the properties for your own project.

## Customization
- After adding the `Cloud Render Pipeline` and `CloudContainer`:
  - Adjust the **Cloud Settings** from the inspector on the `CloudContainer` GameObject.
  - Customize properties such as cloud density, wind speed, and more to fit your scene's atmosphere.