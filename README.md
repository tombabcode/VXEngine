# VXEngine
WIP - MonoGame extension for making 2D games, that includes advanced nesting, primitives and additional functions for better code management

# NOTE THAT THIS IS [WIP] STATE AND IT'S NOT READY FOR USAGE

### Nesting

Objects can have parent and childrens. Transforming parent affects its children.

```C#
objectA = new Box(...);
objectB = new Box(...);

objectB.SetParent(objectA);

objectA.SetAngleInDegrees(45);
objectB.SetAngleInDegrees(15);

// Output:
// object A rotation is 45 degrees
// object B rotation is 60 degrees
```

### Primitives

You can use primitives such as Box, Circle, Line or Sprite to easily create different objects

```C#
// Create red box, 200 pixels in width, 200 pixels in height, at the point of (100, 100)
box = new Box(...)
box.SetColorFill(Color.Red)
   .SetPosition(100, 100)
   .SetSize(200, 200)

// Create yellow circle with radius 50, at the center of the scene
circle = new Circle(...)
circle.SetColorFill(Color.Yellow)
      .SetRadius(50)
      .SetParent(scene)
      .SetPositionAsPercent(.5f, .5f)
      .SetAlign(.5f, .5f);

// Create sprite at the left-bottom corner of the scene
texture = content.Texture;
sprite = new Sprite(texture);
sprite.SetParent(scene)
      .SetPositionAsPercent(.0f, 1.0f)
      .SetAlign(.0f, 1.0f);
```
