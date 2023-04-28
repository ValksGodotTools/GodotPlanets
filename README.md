# Godot Planets
Making planets in Godot!

https://user-images.githubusercontent.com/6277739/234764252-c91a1726-1416-41af-9a43-df04e99bb875.mp4

![Untitled](https://user-images.githubusercontent.com/6277739/234762377-65cb1992-0629-4bef-91c2-59e4faf6eea2.png)
![2](https://user-images.githubusercontent.com/6277739/234654822-cf03a839-d6fa-48ef-ba6d-3988e37c51f7.png)
![1](https://user-images.githubusercontent.com/6277739/234654811-2a35c727-ebdc-4a58-8427-4893b93de0dd.png)

## Setup
1. Download and install the [latest Godot 4 C# release](https://godotengine.org/)
2. Clone this repository with all its submodules
```
git clone --recursive https://github.com/ValksGodotTools/Template
```

## Roadmap
- Look into properly calculating the normals instead of just doing `.Normalized()` to each vertex. Instead take the neighboring vertices into account when calculating the normal. But getting the neighboring vertices is going to be a pain on its own... Edit: Seems like SurfaceTool can do this for us, see https://docs.godotengine.org/en/stable/classes/class_surfacetool.html#class-surfacetool-method-generate-normals
- Textures!
- Water!
- Trees!
- Focal point of view shader!
- Split the chunks into further chunks!

## Reminders
- Generalize camera orbit code so you never have to write it again
- The shared vertex count seems to be not correct. Apparently this formula https://math.stackexchange.com/questions/2529679/count-of-vertices-of-a-subdivided-triangle calculates the shared vertex count for a subdivided triangle. It seems to be correct. But when I tested it with higher numbers the numbers just didn't add up. I'm assuming my code is just wack. I need someone to help me redo the Chunk.cs code from scratch to make sure we are using the correct number of vertices. Also checking to see if not rendering extra triangles by accident. Stuff like that. Maybe we can make the code more performant at the same time.

## Contributing
Currently looking for programmers to peer review my code.

[Projects Coding Style](https://github.com/Valks-Games/sankari/wiki/Code-Style)

If you have any questions, talk to me over Discord (`va#9904`)
