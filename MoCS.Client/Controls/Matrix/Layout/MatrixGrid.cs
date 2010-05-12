using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace MoCS.Client.Controls.Matrix.Layout
{
    /// <summary>
    /// A Grid panel that creates its own rows and columns based on the
    /// Grid.Row and Grid.Column values assigned to its children.
    /// </summary>
    class MatrixGrid : Grid
    {
        #region Constructor

        public MatrixGrid()
        {
            _childToMonitorMap = new Dictionary<DependencyObject, MatrixGridChildMonitor>();
            _converter = new MatrixGridChildConverter(this);
        }

        #endregion // Constructor

        #region OnVisualChildrenChanged

        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);

            if (visualAdded != null)
                this.StartMonitoringChildElement(visualAdded);
            else
                this.StopMonitoringChildElement(visualRemoved);
        }

        #endregion // OnVisualChildrenChanged

        #region Inspect Row/Column Index

        internal void InspectRowIndex(int index)
        {
            // Delay the call that adds rows in case the RowDefinitions 
            // collection is currently read-only due to a layout pass.
            base.Dispatcher.BeginInvoke(new Action(delegate
            {
                while (base.RowDefinitions.Count - 1 < index)
                {
                    base.RowDefinitions.Add(new RowDefinition());

                    // Make the column headers just tall 
                    // enough to display their content.
                    if (base.RowDefinitions.Count == 1)
                        base.RowDefinitions[0].Height = new GridLength(1, GridUnitType.Auto);
                }
            }));
        }

        internal void InspectColumnIndex(int index)
        {
            // Delay the call that adds rows in case the ColumnDefinitions 
            // collection is currently read-only due to a layout pass.
            base.Dispatcher.BeginInvoke(new Action(delegate
            {
                while (base.ColumnDefinitions.Count - 1 < index)
                {
                    base.ColumnDefinitions.Add(new ColumnDefinition());

                    // Make the row headers just wide 
                    // enough to display their content.
                    if (base.ColumnDefinitions.Count == 1)
                        base.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Auto);
                }
            }));
        }

        #endregion // Inspect Row/Column Index

        #region Private Helpers

        Binding CreateMonitorBinding(DependencyObject childElement, DependencyProperty property)
        {
            return new Binding
            {
                Converter = _converter,
                ConverterParameter = property,
                Mode = BindingMode.OneWay,
                Path = new PropertyPath(property),
                Source = childElement
            };
        }

        void StartMonitoringChildElement(DependencyObject childElement)
        {
            // Create a MatrixGridChildMonitor in order to detect
            // changes made to the Grid.Row and Grid.Column attached 
            // properties on the new child element.

            MatrixGridChildMonitor monitor = new MatrixGridChildMonitor();

            BindingOperations.SetBinding(
                monitor,
                MatrixGridChildMonitor.GridRowProperty,
                this.CreateMonitorBinding(childElement, Grid.RowProperty));

            BindingOperations.SetBinding(
                monitor,
                MatrixGridChildMonitor.GridColumnProperty,
                this.CreateMonitorBinding(childElement, Grid.ColumnProperty));

            _childToMonitorMap.Add(childElement, monitor);
        }

        void StopMonitoringChildElement(DependencyObject childElement)
        {
            // Remove the MatrixGridChildMonitor from the map 
            // and clear its bindings, which effectively kills 
            // the monitor and releases it from memory.

            if (_childToMonitorMap.ContainsKey(childElement))
            {
                MatrixGridChildMonitor monitor = _childToMonitorMap[childElement];
                BindingOperations.ClearAllBindings(monitor);
                _childToMonitorMap.Remove(childElement);
            }
        }

        #endregion // Private Helpers

        #region Fields

        readonly Dictionary<DependencyObject, MatrixGridChildMonitor> _childToMonitorMap;
        readonly MatrixGridChildConverter _converter;

        #endregion // Fields
    }
}
