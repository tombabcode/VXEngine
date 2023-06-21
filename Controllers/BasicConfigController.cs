using Microsoft.Xna.Framework;
using System.Text;

namespace VXEngine.Controllers {
    public class BasicConfigController {

        public bool IsWindowResponsive { get; protected set; } = true;
        public bool IsPixelart { get; protected set; } = false;

        protected int _defaultWindowWidth = 800;
        protected int _defaultWindowHeight = 600;
        protected bool _defaultWindowFullscreen = false;
        protected bool _defaultWindowVSync = true;
        protected float _defaultVolumeMaster = 1.0f;
        protected float _defaultVolumeMusic = 1.0f;
        protected float _defaultVolumeSound = 1.0f;

        public int WindowWidth { get; protected set; }
        public int WindowHeight { get; protected set; }
        public bool WindowFullscreen { get; protected set; }
        public bool WindowVSync { get; protected set; }

        public float VolumeMaster { get; protected set; }
        public float VolumeMusic { get; protected set; }
        public float VolumeSound { get; protected set; }

        public string ConfigFilePath { get; protected set; } = "";
        public string ConfigFileName { get; protected set; } = "config";
        public string ConfigFileExtension { get; protected set; } = "cfg";

        public float ViewWidth { get; private set; }
        public float ViewHeight { get; private set; }
        public float ViewScaleX { get; private set; }
        public float ViewScaleY { get; private set; }
        public float ViewOffsetX { get; private set; }
        public float ViewOffsetY { get; private set; }

        public int ViewStaticWidth { get; protected set; } = 1920;
        public int ViewStaticHeight { get; protected set; } = 1080;

        public Random Random { get; protected set; }

        public BasicConfigController( ) {
            Random = new Random( );
        }

        /// <summary>
        /// Load configuration file
        /// </summary>
        public virtual void Load( ) {
            string path = Path.Combine(ConfigFilePath, ConfigFileName + '.' + ConfigFileExtension);

            // Config file exists
            if (File.Exists(path)) {
                using (StreamReader sr = new StreamReader(path, Encoding.UTF8)) {
                    string line = "";
                    while ((line = sr.ReadLine( )) != null)
                        LoadConfigProperty(line);
                    sr.Close( );
                }
            } else {
                WindowWidth = _defaultWindowWidth;
                WindowHeight = _defaultWindowHeight;
                WindowFullscreen = _defaultWindowFullscreen;
                WindowVSync = _defaultWindowVSync;
                VolumeMaster = _defaultVolumeMaster;
                VolumeMusic = _defaultVolumeMusic;
                VolumeSound = _defaultVolumeSound;

                Save( );
            }

            Validate( );
            RecalculateView( );
        }

        /// <summary>
        /// Update or create configuration file
        /// </summary>
        public virtual void Save( ) {
            string path = Path.Combine(ConfigFilePath, ConfigFileName + '.' + ConfigFileExtension);

            using (StreamWriter sw = new StreamWriter(path, false, Encoding.UTF8)) {
                sw.WriteLine("WINDOW_WIDTH=" + WindowWidth);
                sw.WriteLine("WINDOW_HEIGHT=" + WindowHeight);
                sw.WriteLine("WINDOW_FULLSCREEN=" + WindowFullscreen);
                sw.WriteLine("WINDOW_VSYNC=" + WindowVSync);
                sw.WriteLine("VOLUME_MASTER=" + VolumeMaster);
                sw.WriteLine("VOLUME_MUSIC=" + VolumeMusic);
                sw.WriteLine("VOLUME_SOUND=" + VolumeSound);
                List<string> custom = SaveCustomConfigProperty( );
                custom.ForEach(line => sw.WriteLine(line));
                sw.Close( );
            }

            Validate( );
        }

        /// <summary>
        /// Load single config line and parse it
        /// </summary>
        protected virtual void LoadConfigProperty(string line) {
            string[] separated = line.Split('=');

            if (separated.Length != 2) { return; }

            string key = separated[0].ToLower( );
            string value = separated[1];

            switch (key) {
                case "window_width": WindowWidth = int.TryParse(value, out int newWindowWidth) ? newWindowWidth : _defaultWindowWidth; break;
                case "window_height": WindowHeight = int.TryParse(value, out int newWindowHeight) ? newWindowHeight : _defaultWindowHeight; break;
                case "window_fullscreen": WindowFullscreen = bool.TryParse(value, out bool newWindowFullscreen) ? newWindowFullscreen : _defaultWindowFullscreen; break;
                case "window_vsync": WindowVSync = bool.TryParse(value, out bool newWindowVSync) ? newWindowVSync : _defaultWindowVSync; break;
                case "volume_master": VolumeMaster = float.TryParse(value, out float newVolumeMaster) ? newVolumeMaster : _defaultVolumeMaster; break;
                case "volume_music": VolumeMusic = float.TryParse(value, out float newVolumeMusic) ? newVolumeMusic : _defaultVolumeMusic; break;
                case "volume_sound": VolumeSound = float.TryParse(value, out float newVolumeSound) ? newVolumeSound : _defaultVolumeSound; break;
                default: LoadCustomConfigProperty(key, value); break;
            }
        }

        /// <summary>
        /// Validate configuration properties
        /// </summary>
        protected virtual void Validate( ) {
            if (VolumeMaster < 0) VolumeMaster = 0;
            if (VolumeMaster > 1) VolumeMaster = 1;
            if (VolumeMusic < 0) VolumeMusic = 0;
            if (VolumeMusic > 1) VolumeMusic = 1;
            if (VolumeSound < 0) VolumeSound = 0;
            if (VolumeSound > 1) VolumeSound = 1;
            if (WindowWidth < 800) WindowWidth = 800;
            if (WindowHeight < 600) WindowHeight = 600;
        }

        /// <summary>
        /// Updte view dimensions
        /// </summary>
        public virtual void RecalculateView( ) {
            // Static view
            if (!IsWindowResponsive) {
                // Calculate view scale
                ViewScaleX = (float)WindowWidth / ViewStaticWidth;
                ViewScaleY = (float)WindowHeight / ViewStaticHeight;

                // Make view either horizontal or vertical
                if (ViewScaleX >= ViewScaleY)
                    ViewScaleX = ViewScaleY;
                else
                    ViewScaleY = ViewScaleX;

                // Set view dimensions
                ViewWidth = ViewStaticWidth;
                ViewHeight = ViewStaticHeight;

                // Set offset
                ViewOffsetX = (int)Math.Floor(WindowWidth * .5f - ViewWidth * ViewScaleX * .5f);
                ViewOffsetY = (int)Math.Floor(WindowHeight * .5f - ViewHeight * ViewScaleY * .5f);

            // Responsive view
            } else {
                ViewScaleX = 1;
                ViewScaleY = 1;
                ViewWidth = WindowWidth;
                ViewHeight = WindowHeight;
                ViewOffsetX = 0;
                ViewOffsetY = 0;
            }
        }

        /// <summary>
        /// Update window
        /// </summary>
        public virtual void ApplyWindowChanges(GraphicsDeviceManager manager) {
            manager.PreferredBackBufferWidth = WindowWidth;
            manager.PreferredBackBufferHeight = WindowHeight;
            manager.IsFullScreen = WindowFullscreen;
            manager.SynchronizeWithVerticalRetrace = WindowVSync;
            manager.ApplyChanges( );
        }

        /// <summary>
        /// Loads custom config property during loading configuration
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        protected virtual void LoadCustomConfigProperty(string key, string value) { }

        /// <summary>
        /// Add custom config property to the saved file
        /// </summary>
        /// <returns></returns>
        protected virtual List<string> SaveCustomConfigProperty( ) { return new List<string>( ); }

    }
}
