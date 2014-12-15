using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
   public class M_CountryCode
    {
       public M_CountryCode()
       {
       }

       #region
       private int _id;
       private string _ChinesName;
       private string _EnglishName;
       private int _parentID;
       private int _levels;
       #endregion


       #region
       public int Id
       {
           get { return _id; }
           set { _id = value; }
       }
       public string ChinesName
       {
           get { return _ChinesName; }
           set { _ChinesName = value; }
       }
       public string EnglishName
       {
           get { return _EnglishName; }
           set { _EnglishName = value; }
       }
       public int ParentID
       {
           get { return _parentID; }
           set { _parentID = value; }
       }
       public int Levels
       {
           get { return _levels; }
           set { _levels = value; }
       }
       #endregion
    }

    
}
