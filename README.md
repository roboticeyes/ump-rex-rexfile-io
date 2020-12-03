# ump-rex-rexfile-io
Unity Package for REXfile input and output

## Installation
To install use `Add package from git URL...` function in the Unity Package Manager.
The package is inside the path `Assets/package` therefore the git URL should look like this:

https://github.com/roboticeyes/upm-rex-rexfile-io.git?path=/Assets/package

To select a specific version add `#version` to the url
e.g.: https://github.com/roboticeyes/upm-rex-rexfile-io.git?path=/Assets/package#v1.0.17

## Usage
An example Scene can be found under `Assets/Example`

### Convert .rex to GameObjects
This code starts an conversion from .rex to GameObjects
``` C#
byte[] data = File.ReadAllBytes ("rexFile");
RexConverter.Instance.ConvertFromRex (data, (success, loadedObjects) =>
{
    if (!success)
    {
        Debug.Log ("ConvertFromRex failed.");
        return;
    }

    List<MeshFilter> meshFilters = new List<MeshFilter> ();
    foreach (var item in loadedObjects.Meshes)
    {
        item.gameObject.SetActive (true);
        meshFilters.Add (item.GetComponentInChildren<MeshFilter>());
    }
});
```

### Convert Meshes to .rex
``` C#
MeshFilter[] meshesToWrite;
...
byte[] rexFile = RexConverter.Instance.GenerateRexFile (meshesToWrite);
```

### What is REX?
- REX is an Augmented Reality platform. [Find out more about REX](https://github.com/roboticeyes/openrex)
- REX data format is our own data format to store REX-relevant information efficiently. [Have a look at the specification](https://github.com/roboticeyes/openrex/blob/master/doc/rex-spec-v1.md)
