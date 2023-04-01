namespace VXEngine.Utility {
    public static class Easing {

        public static float Linear(float value) => value;

        public static float QuadIn(float value) => value * value;
        public static float QuadOut(float value) => 1 - (1 - value) * (1 - value);
        public static float QuadInOut(float value) => (float)(value < 0.5 ? 2 * value * value : 1 - Math.Pow(-2 * value + 2, 2) / 2);

    }
}
