using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoCS.Client.Controls.Matrix
{
    /// <summary>
    /// Represents a header shown in the topmost slot in a column of a matrix.
    /// </summary>
    public class MatrixColumnHeaderItem : MatrixItemBase
    {
        public MatrixColumnHeaderItem(object columnHeader)
        {
            this.ColumnHeader = columnHeader;
        }

        public object ColumnHeader { get; private set; }
    }
}
