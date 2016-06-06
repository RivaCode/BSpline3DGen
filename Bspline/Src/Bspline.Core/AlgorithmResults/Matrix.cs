using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bspline.Core.Location;

namespace Bspline.Core.AlgorithmResults
{
    /// <summary>
    /// Class that build matrix from list 
    /// matrix that we use to save data
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public class Matrix<TValue> : IEnumerable<KeyValuePair<int, List<TValue>>>
    {
        /// <summary>
        /// matrix can be from any type of info
        /// </summary>
        private Dictionary<int, List<TValue>> ValueMatrix { get; set; }
        /// <summary>
        /// return number of colums
        /// </summary>
        public int Columns
        {
            get { return this[0].Count; }
        }
        /// <summary>
        /// return number of rows
        /// </summary>
        public int Rows
        {
            get { return this.ValueMatrix.Keys.Count; }
        }
        /// <summary>
        /// return new matrix extension
        /// </summary>
        public Matrix()
        {
            ValueMatrix = new Dictionary<int, List<TValue>>();
        }
        /// <summary>
        /// build matrix to whanted size
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public TValue this[int x, int y]
        {
            get { return this.ValueMatrix[x][y]; }
            set { this.ValueMatrix[x][y] = value; }
        }
        /// <summary>
        /// generate list for matrix that comnatin TValue
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public List<TValue> this[int x]
        {
            get
            {
                if ( !this.ValueMatrix.ContainsKey( x ) )
                {
                    this.ValueMatrix[x] = new List<TValue>();
                }
                return this.ValueMatrix[x];
            }
        }
        /// <summary>
        /// find the value in index
        /// </summary>
        /// <param name="predicat"></param>
        /// <returns></returns>
        public TValue Find( Func<TValue, bool> predicat )
        {
            TValue point = ValueMatrix.Values.SelectMany( points => points ).FirstOrDefault( predicat );
            return point;
        }
        /// <summary>
        /// <seealso cref="IEnumerator"/>
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<int, List<TValue>>> GetEnumerator()
        {
            return this.ValueMatrix.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
