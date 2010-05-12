using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MoCS.Client.Controls.Matrix
{
    /// <summary>
    /// Base class for a matrix of data to be displayed by MatrixControl.
    /// </summary>
    /// <typeparam name="TRow">The type of object used in the row headers.</typeparam>
    /// <typeparam name="TColumn">The type of object used in the column headers.</typeparam>
    public abstract class MatrixBase<TRow, TColumn>
    {
        #region Constructor

        protected MatrixBase()
        {
        }

        #endregion // Constructor

        #region Abstract Methods

        /// <summary>
        /// Child classes must override this method to return a listing of
        /// all entities that are to be displayed in the matrix column headers.
        /// </summary>
        protected abstract IEnumerable<TColumn> GetColumnHeaderValues();

        /// <summary>
        /// Child classes must override this method to return a listing of
        /// all entities that are to be displayed in the matrix row headers.
        /// </summary>
        protected abstract IEnumerable<TRow> GetRowHeaderValues();

        /// <summary>
        /// Child classes must override this method to provide the value of
        /// each cell in the matrix, where a 'cell' is defined as the 
        /// intersection of a column header value and a row header value.
        /// </summary>
        /// <param name="rowHeaderValue">The value of the cell's row header.</param>
        /// <param name="columnHeaderValue">The value of the cell's column header.</param>
        protected abstract object GetCellValue(TRow rowHeaderValue, TColumn columnHeaderValue);

        #endregion // Abstract Methods

        #region MatrixItems

        /// <summary>
        /// Returns a read-only collection of all cells in the matrix.
        /// </summary>
        public ReadOnlyCollection<MatrixItemBase> MatrixItems
        {
            get
            {
                if (_matrixItems == null)
                {
                    _matrixItems = new ReadOnlyCollection<MatrixItemBase>(this.BuildMatrix());
                }
                return _matrixItems;
            }
        }

        #endregion // MatrixItems

        #region Matrix Construction

        List<MatrixItemBase> BuildMatrix()
        {
            List<MatrixItemBase> matrixItems = new List<MatrixItemBase>();

            // Get the column and row header values from the child class.
            List<TColumn> columnHeaderValues = this.GetColumnHeaderValues().ToList();
            List<TRow> rowHeaderValues = this.GetRowHeaderValues().ToList();

            this.CreateEmptyHeader(matrixItems);
            this.CreateColumnHeaders(matrixItems, columnHeaderValues);
            this.CreateRowHeaders(matrixItems, rowHeaderValues);
            this.CreateCells(matrixItems, rowHeaderValues, columnHeaderValues);

            return matrixItems;
        }

        void CreateEmptyHeader(List<MatrixItemBase> matrixItems)
        {
            // Insert a blank item in the top left corner.
            matrixItems.Add(new MatrixEmptyHeaderItem
            {
                GridRow = 0,
                GridColumn = 0
            });
        }

        void CreateColumnHeaders(List<MatrixItemBase> matrixItems, List<TColumn> columnHeaderValues)
        {
            // Insert the column header items in the first row.
            for (int column = 1; column <= columnHeaderValues.Count; ++column)
            {
                matrixItems.Add(new MatrixColumnHeaderItem(columnHeaderValues[column - 1])
                {
                    GridRow = 0,
                    GridColumn = column
                });
            }
        }

        void CreateRowHeaders(List<MatrixItemBase> matrixItems, List<TRow> rowHeaderValues)
        {
            // Insert the row headers items in the first slot 
            // of each row after the column header row.
            for (int row = 1; row <= rowHeaderValues.Count; ++row)
            {
                matrixItems.Add(new MatrixRowHeaderItem(rowHeaderValues[row - 1])
                {
                    GridRow = row,
                    GridColumn = 0
                });
            }
        }

        void CreateCells(List<MatrixItemBase> matrixItems, List<TRow> rowHeaderValues, List<TColumn> columnHeaderValues)
        {
            // Insert a cell item for each row/column intersection.
            for (int row = 1; row <= rowHeaderValues.Count; ++row)
            {
                TRow rowHeaderValue = rowHeaderValues[row - 1];

                for (int column = 1; column <= columnHeaderValues.Count; ++column)
                {
                    // Ask the child class for the cell's value.
                    object cellValue = this.GetCellValue(rowHeaderValue, columnHeaderValues[column - 1]);
                    matrixItems.Add(new MatrixCellItem(cellValue)
                    {
                        GridRow = row,
                        GridColumn = column
                    });
                }
            }
        }

        #endregion // Matrix Construction

        #region Fields

        ReadOnlyCollection<MatrixItemBase> _matrixItems;

        #endregion // Fields
    }
}