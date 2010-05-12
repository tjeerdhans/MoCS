using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoCS.Client.Controls.Matrix
{
    public class MatrixCellItem : MatrixItemBase
    {
        public MatrixCellItem(object value)
        {
            this.Value = value;
        }

        public object Value { get; private set; }
    }
}
