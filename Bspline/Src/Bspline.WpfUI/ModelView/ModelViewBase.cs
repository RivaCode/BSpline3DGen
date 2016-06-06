using System.Linq.Expressions;
using Bspline.Core;
using System;
using System.ComponentModel;

namespace Bspline.WpfUI.ModelView
{
    /// <summary>
    /// Abstract model view class to act as notifiable model for WPF
    /// </summary>
    public abstract class ModelViewBase:DisposableObject,INotifyPropertyChanged
    {
        #region INotifyPropertyChanged members

        /// <summary>
        /// <see cref="INotifyPropertyChanged"/>
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        
        #endregion

        #region Protected

        /// <summary>
        /// Notifies WPF that the specific property was changed
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyExpression"></param>
        protected void OnPropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            OnPropertyChanged(ExtractPropertyName(propertyExpression));
        }

        /// <summary>
        /// Extract the string literal from the pass property 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyExpression"></param>
        /// <returns></returns>
        private string ExtractPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            return (((MemberExpression)(propertyExpression.Body)).Member).Name;
        }

        /// <summary>
        /// Call the WPF to re-read the property
        /// </summary>
        /// <param name="propertyName"></param>
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        } 

        #endregion
    }
}
