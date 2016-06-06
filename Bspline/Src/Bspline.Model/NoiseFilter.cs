using System.CodeDom.Compiler;
using System.Linq;
using Bspline.Core;
using Bspline.Core.DataPacket;
using Bspline.Core.Interfaces;
using Bspline.Core.Location;
using Bspline.Model.Interfaces;
using System.Collections.Generic;
using System.IO;

namespace Bspline.Model
{
    /// <summary>
    /// Purpose of class is to filter mass of data coming from media device
    /// </summary>
    internal class NoiseFilter:DisposableObject
    {
        #region Properties

        /// <summary>
        /// Property to which this object will delegate calss
        /// </summary>
        private ILogicDelegate LogicDelegate { get; set; }
        
        /// <summary>
        /// Property to handle the amound of points to filter
        /// changable by user
        /// </summary>
        internal int NumberOfPointsToFilter { get; set; }

        private bool CanSaveHand
        {
            get
            {
                return this.RightHands.Count == this.NumberOfPointsToFilter;
            }
        }

        /// <summary>
        /// Property to save all filtered points
        /// </summary>
        private List<IPosition> RightHands { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of class is to filter mass of data coming from media device
        /// </summary>
        /// <param name="logicDelegate">object to delegate calls to it</param>
        public NoiseFilter(ILogicDelegate logicDelegate)
        {
            this.LogicDelegate = logicDelegate;
            this.NumberOfPointsToFilter = 3;
            RightHands = new List<IPosition>();
        }

        
        #endregion

        #region Public

        /// <summary>
        /// Filters the massive amount of <see cref="Bspline.Core.DataPacket.MotionDataPacket"/> coming from <see cref="Bspline.Media.MediaWrapperBase"/>
        /// </summary>
        /// <param name="dataPacket"></param>
        public void Filter(MotionDataPacket dataPacket)
        {
    
            if (this.CanSaveHand)
            {
                IPosition avgPosition = this.RightHands
                    .Select(hand =>
                    {
                        hand.Position.X = (int) this.RightHands.Average(obj => obj.Position.X);
                        hand.Position.Y = (int) this.RightHands.Average(obj => obj.Position.Y);

                        return hand;
                    }).First();

                this.LogicDelegate.OnMotionDataPacketFiltered(new MotionDataPacket(null, avgPosition));
                this.Reset();
            }
            else
            {
                this.RightHands.Add(dataPacket.RightPosition);
            }
        }

        /// <summary>
        /// Clears all collected points
        /// </summary>
        public void Reset()
        {
            this.RightHands.Clear();
        }
        #endregion

        #region Protected

        /// <summary>
        /// <see cref="Bspline.Core.DisposableObject"/>
        /// </summary>
        protected override void DisposeInner()
        {

            if (this.RightHands != null)
            {

                this.RightHands.Clear();
                this.RightHands = null;
            }
            this.LogicDelegate = null;
        }

        #endregion
    }
}
