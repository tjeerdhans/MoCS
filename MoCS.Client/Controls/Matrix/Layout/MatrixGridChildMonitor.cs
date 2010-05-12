using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace MoCS.Client.Controls.Matrix.Layout
{
    /// <summary>
    /// Exposes two dependency properties which are bound to in
    /// order to know when the visual children of a MatrixGrid are
    /// given new values for the Grid.Row and Grid.Column properties.
    /// </summary>
    class MatrixGridChildMonitor : DependencyObject
    {
        #region GridRow

        public int GridRow
        {
            get { return (int)GetValue(GridRowProperty); }
            set { SetValue(GridRowProperty, value); }
        }

        public static readonly DependencyProperty GridRowProperty =
            DependencyProperty.Register(
            "GridRow",
            typeof(int),
            typeof(MatrixGridChildMonitor),
            new UIPropertyMetadata(0));

        #endregion // GridRow

        #region GridColumn

        public int GridColumn
        {
            get { return (int)GetValue(GridColumnProperty); }
            set { SetValue(GridColumnProperty, value); }
        }

        public static readonly DependencyProperty GridColumnProperty =
            DependencyProperty.Register(
            "GridColumn",
            typeof(int),
            typeof(MatrixGridChildMonitor),
            new UIPropertyMetadata(0));

        #endregion // GridColumn
    }
}
