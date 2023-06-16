using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VXEngine.Controllers;

namespace VXEngine.Utility;

public static class TextureUtility {

    public static Texture2D GetPart(BasicContentController content, Texture2D originalImage, int x, int y, int width, int height) {
        if (originalImage == null) throw new ArgumentNullException("Texture cannot be null");
        if (x < 0) x = 0;
        if (y < 0) y = 0;
        if (x > width) throw new ArgumentOutOfRangeException("Position X of the texture's part is greater than texture itself");
        if (y > height) throw new ArgumentOutOfRangeException("Position Y of the texture's part is greater than texture itself");
        if (width <= 0) throw new ArgumentOutOfRangeException("Width of the texture's part cannot be less or equal to 0");
        if (height <= 0) throw new ArgumentOutOfRangeException("Width of the texture's part cannot be less or equal to 0");
        if (x + width > originalImage.Width) throw new ArgumentOutOfRangeException("Width of the texture's part cannot be greater than texture itself");
        if (y + height > originalImage.Height) throw new ArgumentOutOfRangeException("Height of the texture's part cannot be greater than texture itself");

        // Create texture part
        Texture2D part = new Texture2D(content.Device, width, height);
        Color[] partData = new Color[width * height];

        // Get original data info
        Color[] originalData = new Color[originalImage.Width * originalImage.Height];
        originalImage.GetData(originalData);

        // Fill it
        for (int iy = 0; iy < height; iy++)
            for (int ix = 0; ix < width; ix++)
                partData[iy * width + ix] = originalData[(iy + y) * originalImage.Width + (ix + x)];

        part.SetData(partData);
        return part;
    }

}
