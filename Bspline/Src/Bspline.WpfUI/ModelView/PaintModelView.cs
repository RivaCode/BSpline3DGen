using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Bspline.Core;
using Bspline.Core.AlgorithmResults;
using Bspline.Core.DataPacket;
using Bspline.Core.Interfaces;
using Bspline.WpfUI.Commands;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace Bspline.WpfUI.ModelView
{

    /// <summary>
    /// Derived model vide to handle the paint area of application
    /// </summary>
    public class PaintModelView : ModelViewColleague
    {
        #region Fields

        /// <summary>
        /// field to hold info if recording is on
        /// </summary>
        private bool _isRecording;

        #endregion

        #region Properties

        #region View Binded

        /// <summary>
        /// Property to create string literal from received hands coordinates
        /// </summary>
        public string CoordinateText
        {
            get
            {
                return this.ShowCoordinate
                    ? string.Format("X:{0};Y:{1}", (int) this.RightHandLeft, (int) this.RightHandTop)
                    : string.Empty;
            }
        }


        #region Commands


        /// <summary>
        /// Property to hold the record command <see cref="ICommand"/>
        /// </summary>
        public ICommand RecordingUpdate
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    this.UpdateUi();
                    try
                    {
                        if (!this.IsRecording)
                        {
                            this.Mediator.NotifyRecordingDone();
                        }
                    }
                    catch (Exception)
                    {
                        this.PaintingDelegate.OnSurfaceReady(null);
                    }

                }, obj => this.IsMediaStreaming);
            }
        }

        #endregion

        #endregion

        /// <summary>
        /// Property to read if the application in record mode
        /// </summary>
        private bool IsRecording
        {
            get { return _isRecording; }
            set
            {
                _isRecording = value;
                this.Mediator.StartRecording(_isRecording);
            }
        }
        
        /// <summary>
        /// Property to hold the painting area width
        /// </summary>
        public double PaintWidth { get; set; }


        /// <summary>
        /// Property to hold the painting area height
        /// </summary>
        public double PaintHeight { get; set; }


        /// <summary>
        /// Property to hold the application window to delegate to
        /// </summary>
        private IPaintingDelegate PaintingDelegate { get; set; }

        /// <summary>
        /// Property to show coordinate
        /// </summary>
        private bool ShowCoordinate { get; set; }

        /// <summary>
        /// Property to hold right hand left position for ellipse
        /// </summary>
        private double RightHandLeft { get; set; }


        /// <summary>
        /// Property to hold right hand top position for ellispe
        /// </summary>
        private double RightHandTop { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of derived model vide to handle the paint area of application
        /// </summary>
        /// <param name="mediator">mediator</param>
        /// <param name="paintingDelegate">window to delegate to</param>
        public PaintModelView(ModelViewMediator mediator, IPaintingDelegate paintingDelegate)
            : base( mediator )
        {
            PaintingDelegate = paintingDelegate;
        }

        #endregion

        #region Internal

        /// <summary>
        /// <see cref="ModelViewColleague"/>
        /// </summary>
        /// <param name="motionDataPacket"></param>
        internal override void UpdateTrackingStream(MotionDataPacket motionDataPacket)
        {
            this.PaintingDelegate.OnGestureMove(motionDataPacket.RightPosition.Position);

            this.RightHandLeft = motionDataPacket.RightPosition.Position.X;
            this.RightHandTop = motionDataPacket.RightPosition.Position.Y;
            this.OnPropertyChanged(() => this.CoordinateText); 
        }


        /// <summary>
        /// <see cref="ModelViewColleague"/>
        /// </summary>
        internal override void NotifyRecordRequest()
        {
            this.PostToUiThread(() =>
            {
                if (RecordingUpdate.CanExecute(null))
                {
                    RecordingUpdate.Execute(null);
                }
            });
        }


        /// <summary>
        /// <see cref="ModelViewColleague"/>
        /// </summary>
        internal override void NotifyNewRequest()
        {
            this.PostToUiThread(() => this.PaintingDelegate.OnSurfaceClear());
        }


        /// <summary>
        /// <see cref="ModelViewColleague"/>
        /// </summary>
        internal override int[] GetDrawingDimension()
        {
            return new[] {(int) this.PaintWidth, (int) this.PaintHeight - 15};
        }


        /// <summary>
        /// <see cref="ModelViewColleague"/>
        /// </summary>
        internal override void NotifyMediaWorkStatusChanged()
        {
            //no need to update UI from this MV when media status changes 
        }

        internal override void UpdateScreenPaint(AlgResult result)
        {
            this.PaintingDelegate.OnSurfaceReady(result);
        }


        /// <summary>
        /// <see cref="ModelViewColleague"/>
        /// </summary>
        internal override void NotifyDrawingDimensionChanged(double width, double height)
        {
            this.PaintWidth = width;
            this.PaintHeight = height;
        }


        /// <summary>
        /// <see cref="ModelViewColleague"/>
        /// </summary>
        internal override void NotifySettingsChanged()
        {
            this.ShowCoordinate = this.Mediator.AppSettings.ShowCoordinates;
            this.OnPropertyChanged(()=>this.CoordinateText);
        }

        #endregion

        #region Protected


        /// <summary>
        /// <see cref="DisposableObject"/>
        /// </summary>
        protected override void DisposeInner()
        {
            this.PaintingDelegate = null;
            base.DisposeInner();
        }


        /// <summary>
        /// <see cref="ModelViewColleague"/>
        /// </summary>
        protected override void PrepareUiBindedProperties()
        {
            if (this.IsMediaStreaming && !this.IsRecording)
            {
                this.IsRecording = true;
                this.IconHeight = 70;
                this.IconWidth = 50;
                this.ImageSource = "stop";
            }
            else
            {
                this.IsRecording = false;
                this.IconWidth = 100;
                this.IconHeight = 100;
                this.ImageSource = "record";
            }

            this.IconMargin=new Thickness(2,0,0,0);
            base.PrepareUiBindedProperties();
        }

        #endregion

    }
}
