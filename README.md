# Night Optimization Kit 🌟

Optimize your Unity projects with the Night Optimization Kit. This package contains a set of powerful tools to enhance your game's performance and efficiency.

# 🧪 *What's new? 9/19/24*

### 🧪 1) Call Activator: <br>
&emsp;&emsp;&emsp; Easily call frame-based methods less to improve performance!<br>
```csharp
CallActivator helloWorldCallActivator = new CallActivator(activateTime: 10);
void Update()
{
    // Log 'hello world' frameRate / activateTime times per second 
    // (with activateTime set to 10) 
    helloWorldCallActivator.Request(() => Debug.Log("Hello world!"));
}
```

### Extra Examples of Call Activator:

```csharp
CallActivator networkActivator = new CallActivator(activateTime: 10);

void Update()
{
    // Every 10 frames, perform a network operation in a secondary thread.
    networkActivator.RequestSec(() =>
    {
        // Secondary thread: Send data over the network (e.g., an API call)
        SendDataToServer("Player position updated.");
        Debug.Log("Data sent to server.");
    });
}

void SendDataToServer(string data)
{
    // Simulating a network operation.
    // In reality, this could be an async operation.
}
```

```csharp
CallActivator raycastActivator = new CallActivator(activateTime: 10);
Vector3 targetPosition = new Vector3(0, 0, 0); // Define your target position

void Update()
{
    // Every 10 frames, perform a Raycast on the main thread and process the result in a secondary thread.
    raycastActivator.Request(() =>
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Main thread: Raycast hit an object.
            return hit.collider.gameObject.transform.position; // Return the position of the hit object
        }
        return Vector3.zero; // Return zero vector if nothing is hit
    },
    hitPosition =>
    {
        // Secondary thread: Check the distance to the target position.
        if (hitPosition != Vector3.zero)
        {
            float distance = Vector3.Distance(hitPosition, targetPosition);
            Debug.Log("Hit object position: " + hitPosition + ", Distance to target: " + distance);
        }
        else
        {
            Debug.Log("No object hit.");
        }
    });
}

```
### 🧪 2) Instance checks: <br>
&emsp;&emsp;&emsp; In previous version, there was no way to check if the component has an instance without creating a new instance. <br>
&emsp;&emsp;&emsp; Now we have .HasInstance() method!<br>
### 🧪 3) New singleton types: <br>
&emsp;&emsp;&emsp; - *ForceSingletonDontDestroy\<T>*<br>
&emsp;&emsp;&emsp; - *ProtectedSingletonDontDestroy\<T>*<br>
&emsp;&emsp;&emsp; - *ForceProtectedSingletonDontDestroy\<T>*<br>
<br>

## Features

- **NightJob System**: Boost performance with parallel job scheduling.
- **Nightcull Dynamic Culling**: Optimize scenes using dynamic culling techniques.
- **DotTrail V2**: DotTrail is now for parallel programming!
- **NightPool**: Manage object pooling for better memory usage.

<br>

## Installation

To use the Night Optimization Kit, simply download the package and import it into your Unity project.

Enjoy optimizing your Unity games with the Night Optimization Kit!

<br><br><br>