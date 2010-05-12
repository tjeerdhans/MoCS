using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Controls;
using System.Globalization;

namespace MoCS.Client.Controls.Matrix.Layout
{
    /// <summary>
    /// The value converter applied to bindings against properties of 
    /// MatrixGridChildMonitor that informs a MatrixGrid of the Grid.Row
    /// and Grid.Column settings applied to its child elements.
    /// </summary>
    class MatrixGridChildConverter : IValueConverter
    {
        #region Constructor

        public MatrixGridChildConverter(MatrixGrid matrixGrid)
        {
            _matrixGrid = matrixGrid;
        }

        #endregion // Constructor

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int)
            {
                int index = (int)value;

                if (parameter == Grid.RowProperty)
                    _matrixGrid.InspectRowIndex(index);
                else
                    _matrixGrid.InspectColumnIndex(index);
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion // IValueConverter Members

        #region Fields

        readonly MatrixGrid _matrixGrid;

        #endregion // Fields
    }
}
