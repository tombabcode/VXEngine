using Microsoft.Xna.Framework;

namespace VXEngine.Utility {
    public static class Transition {

        public static Color Color(Color source, Color target, float t) {
            if (t < 0) return source;
            if (t > 1) return target;
            return new Color((byte)((target.R - source.R) * t + source.R), (byte)((target.G - source.G) * t + source.G), (byte)((target.B - source.B) * t + source.B), (byte)((target.A - source.A) * t + source.A));
        }

    }
}
