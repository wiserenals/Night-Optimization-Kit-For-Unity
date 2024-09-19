# Night Optimization Kit 🌟

Optimize your Unity projects with the Night Optimization Kit. This package contains a set of powerful tools to enhance your game's performance and efficiency.


# 🧪 *What's new? 9/20/24*

### 🧪 1) SchedulableBehaviour Class: <br>
Manages method execution order and timing to optimize performance and update frequency.
Methods can be called in a specific sequence using DiscreteUpdateAttribute.
Supports customizable update intervals using EnumerableDiscreteUpdate for better frame management.


### *Why Use SchedulableBehaviour?*
In game development, optimizing performance and managing updates efficiently is crucial. The Update method in Unity runs every frame, which can lead to performance issues if all updates are processed simultaneously. Using SchedulableBehaviour and DiscreteUpdateAttribute allows you to:
<br>
#### *a) Prioritize Critical Updates:*
Ensure that high-priority updates occur before less critical ones, reducing potential frame rate drops.
<br>
#### *b) Reduce Update Frequency:*
Optimize performance by controlling how often certain updates are executed.
<br>
#### *c) Balance Load:*
Distribute computationally intensive tasks across multiple frames to avoid spikes in resource usage.

### EnumerableDiscreteUpdate Example:
```csharp
    using UnityEngine;
    
    public class TestCode : SchedulableBehaviour
    {
        private float timer;
        private const float interval = 1f;
        
        //This example has some issues but is just given to help you understand how to use it.
        protected override IEnumerator EnumerableDiscreteUpdate()
        {
            timer += Time.deltaTime;
    
            if (timer >= interval)
            {
                Camera.main.backgroundColor = Color.blue;
                timer = 0f;
            }
            
            yield return 1; // wait 1 frame to continue
            
            Vector3 position = transform.position;
            position.x += Mathf.Sin(Time.time) * 0.1f;
            transform.position = position;
            
            yield return 1;
    
            float rotationSpeed = 50f;
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    
            yield return 1;
            
            GameObject uiElement = GameObject.Find("UIElement");
            
            if (uiElement != null)
            {
                yield return 1;
                
                RectTransform rectTransform = uiElement.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(Screen.width / 2, Screen.height / 2);
            }
        }
    }

```

### EnumerableDiscreteUpdate Example:
```csharp
using UnityEngine;

public class CharacterController : SchedulableBehaviour
{
    private Vector3 moveDirection;
    private Animator animator;
    private bool isInteracting;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    [DiscreteUpdate(1)] // Update every => currentFrame % 3 == 0
    void UpdateMovement()
    {
        float moveInput = Input.GetAxis("Horizontal");
        moveDirection = new Vector3(moveInput, 0, 0) * Time.deltaTime * 5f;

        transform.position += moveDirection;

        animator.SetFloat("Speed", Mathf.Abs(moveInput));
    }

    [DiscreteUpdate(2)] // Update every => currentFrame % 3 == 1
    void UpdateAnimation()
    {
        if (animator != null)
        {
            float speed = animator.GetFloat("Speed");
            animator.SetFloat("AnimationSpeed", speed);
        }
    }

    [DiscreteUpdate(3)] // Update every => currentFrame % 3 == 2
    void UpdateInteractions()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            isInteracting = !isInteracting;
            Debug.Log($"Interaction: {isInteracting}");
        }
        if (isInteracting)
        {
            Debug.Log("The character interacts with an object.");
        }
    }
}
```

### 🧪 2) Implemented all singleton types for SchedulableBehaviour <br>
### 🧪 3) "lacks await..." warning fixed. <br>

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