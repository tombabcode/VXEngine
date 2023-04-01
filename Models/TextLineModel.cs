namespace VXEngine.Models {
    public class TextLineModel {

        public List<string> Words { get; set; }
        public bool JustifyLine { get; set; }
        public float JustifyOffset { get; set; }
        public string Output { get; private set; }

        public TextLineModel(List<string> words) {
            Words = words;
            Output = "";
            Words.ForEach(word => Output += word + " ");
            Output = Output.TrimEnd( );
        }

    }
}
