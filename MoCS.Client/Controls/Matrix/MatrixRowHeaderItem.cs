using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoCS.Client.Controls.Matrix
{
    /// <summary>
    /// Represents a header shown in the leftmost slot in a row of a matrix.
    /// </summary>
    public class MatrixRowHeaderItem : MatrixItemBase
    {
        public MatrixRowHeaderItem(object rowHeader)
        {
            this.RowHeader = rowHeader;
        }

        public object RowHeader { get; private set; }
    }
}
