using System;
using System.IO;
using System.Linq;
using System.Threading;
using Bspline.Core;
using Microsoft.Kinect;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;

namespace Bspline.Media.Device.Sound
{
    internal class KinectAudioHelper : DisposableObject
    {
        /// <summary>
        /// Class that provide metode to deal with sound control
        /// </summary>
        #region Field

        private const string START = "go";
        private const string STOP = "stop";
        private const string NEW = "new";
        /// <summary>
        /// This commands we use 'go' to start recorde 'stop' to end recorde and 'new' open new 
        /// edit windoow
        /// </summary>
        #endregion

        #region Property

        internal bool UseVoiceRecognition { get; set; }
        /// <summary>
        /// instance of kinect wraper to communication
        /// </summary>
        public KinectWrapper KinectWrapper { get; set; }
        /// <summary>
        /// <seealso cref="SpeechRecognitionEngine"/>
        /// </summary>
        private SpeechRecognitionEngine Src { get; set; }
        #endregion

        #region Constractor
        /// <summary>
        /// connect to Kinect wraper instance to get sound info
        /// </summary>
        /// <param name="kinectWrapper"></param>
        public KinectAudioHelper( KinectWrapper kinectWrapper )
        {
            KinectWrapper = kinectWrapper;
        }

        #endregion

        #region Public
        /// <summary>
        /// Start command activate after 'go' voice command it update the status 
        /// and informs the system
        /// </summary>
        public void Start()
        {
            if ( CreateSpeechRecognizer() )
            {
                KinectAudioSource audioSource = this.KinectWrapper.Sensor.AudioSource;
                audioSource.BeamAngleMode = BeamAngleMode.Adaptive;
                this.Src.SetInputToAudioStream( audioSource.Start(), new SpeechAudioFormatInfo( EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null ) );
                this.Src.RecognizeAsync( RecognizeMode.Multiple );
            }
        }

        /// <summary>
        /// stop the record by updatiting system status
        /// </summary>

        public void Stop()
        {
            if ( this.Src != null )
            {
                this.KinectWrapper.Sensor.AudioSource.Stop();
                this.Src.RecognizeAsyncCancel();
                this.Src.RecognizeAsyncStop();
            }
        }

        #endregion

        #region Protected
        /// <summary>
        /// <seealso cref="DisposableObject"/>
        /// </summary>
        protected override void DisposeInner()
        {
            if ( this.KinectWrapper != null )
            {
                this.Stop();
                this.KinectWrapper = null;
            }
        }

        #endregion

        #region Private
        /// <summary>
        /// using gramar to indicate the voice information
        /// <seealso cref="SpeechRecognitionEngine"/>
        /// </summary>
        /// <returns>bool if command correct or other voice </returns>
        private bool CreateSpeechRecognizer()
        {
            bool result;
            try
            {
                RecognizerInfo ri = GetKinectRecognizer();
                this.Src = new SpeechRecognitionEngine( ri.Id );
                var grammar = new Choices();
                grammar.Add( KinectAudioHelper.START );
                grammar.Add( KinectAudioHelper.STOP );
                grammar.Add( KinectAudioHelper.NEW );


                var gb = new GrammarBuilder( grammar ) { Culture = ri.Culture };

                // Create the actual Grammar instance, and then load it into the speech recognizer.

                this.Src.LoadGrammar( new Grammar( gb ) );
                this.Src.SpeechRecognized += this.SreSpeechRecognized;
                result = true;
            }
            catch
            {
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Get the command from the Kinect
        /// </summary>
        /// <returns></returns>

        private RecognizerInfo GetKinectRecognizer()
        {
            Func<RecognizerInfo, bool> matchingFunc = r =>
            {
                string value;
                r.AdditionalInfo.TryGetValue( "Kinect", out value );
                return bool.TrueString.Equals( value, StringComparison.InvariantCultureIgnoreCase )
                    && "en-US".Equals( r.Culture.Name, StringComparison.InvariantCultureIgnoreCase );
            };
            return SpeechRecognitionEngine.InstalledRecognizers().Where( matchingFunc ).FirstOrDefault();
        }

        /// <summary>
        /// the voice command controler that start correct respond according to voice command
        /// </summary>
        /// <param name="text"></param>
        /// <param name="confidence"></param>

        private void OnSpeechRecognition( string text, float confidence )
        {
            if ( this.UseVoiceRecognition )
            {
                if ( confidence >= 0.5 )
                {
                    switch ( text.ToLower() )
                    {
                        case KinectAudioHelper.START:
                            this.KinectWrapper.ModelDelegate.OnStartVoiceCommand();
                            break;
                        case KinectAudioHelper.STOP:
                            this.KinectWrapper.ModelDelegate.OnStopVoiceCommand();
                            break;
                        case KinectAudioHelper.NEW:
                            this.KinectWrapper.ModelDelegate.OnNewVoiceCommand();
                            break;
                    }
                }
            }
        }

        #endregion

        #region Events

        private void SreSpeechRecognized( object sender, SpeechRecognizedEventArgs e )
        {
            this.OnSpeechRecognition( e.Result.Text, e.Result.Confidence );
        }

        #endregion
    }
}
