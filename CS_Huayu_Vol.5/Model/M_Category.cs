using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
  public  class M_Category
    {
      public M_Category()
      {
      }

        #region

      private int _CategoryID;

      public int CategoryID
      {
          get { return _CategoryID; }
          set { _CategoryID = value; }
      }
      private string _CategoryName;

      public string CategoryName
      {
          get { return _CategoryName; }
          set { _CategoryName = value; }
      }
      private double _CategoryValue;

      public  double CategoryValue
      {
          get { return _CategoryValue; }
          set { _CategoryValue = value; }
      }
        #endregion
    }
}
