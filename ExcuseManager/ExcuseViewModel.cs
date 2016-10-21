using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ExcuseManager
{
    class ExcuseViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        private Excuse _Excuse;
        public Excuse CurrentExcuse {
            get { return _Excuse; }
            private set { throw new NotSupportedException(); }
        }

        private static readonly string ExcuseTextName = "ExcuseText";
        private static readonly string ResultName = "Result";
        private static readonly string UsedName = "Used";
        private static readonly string RatingName = "Rating";
        private static readonly string RatedName = "Rated";
        private static readonly string ReadOnlyName = "ReadOnly";

        public event PropertyChangedEventHandler PropertyChanged;

        private Dictionary<string, string> errors;

        public string StarPoints
        {
            get
            {
                return string.Format("{0}, {1} {2}, {3} {4}, {5} {6}, {7} {8}, {9} {10}, {11} {12}, {13} {14}, {15} {16}, {17} {18}, {19} {0}, {1}",
                    GetStarXPoint(0, 10, 10), GetStarYPoint(0, 10, 10), 
                    GetStarXPoint(36, 5, 10), GetStarYPoint(36, 5, 10),
                    GetStarXPoint(72, 10, 10), GetStarYPoint(72, 10, 10),
                    GetStarXPoint(108, 5, 10), GetStarYPoint(108, 5, 10),
                    GetStarXPoint(144, 10, 10), GetStarYPoint(144, 10, 10),
                    GetStarXPoint(180, 5, 10), GetStarYPoint(180, 5, 10),
                    GetStarXPoint(216, 10, 10), GetStarYPoint(216, 10, 10),
                    GetStarXPoint(252, 5, 10), GetStarYPoint(252, 5, 10),
                    GetStarXPoint(288, 10, 10), GetStarYPoint(288, 10, 10),
                    GetStarXPoint(324, 5, 10), GetStarYPoint(324, 5, 10)
                    );
            }
            private set { throw new NotSupportedException(); }
        }

        private int GetStarXPoint(int angleGrad, int radius, int offset)
        {
            double gradToRad = Math.PI / 180;
            return (int)(Math.Sin(gradToRad * angleGrad) * radius) + offset;
        }

        private int GetStarYPoint(int angleGrad, int radius, int offset)
        {
            double gradToRad = Math.PI / 180;
            return (int)(Math.Cos(gradToRad * angleGrad) * radius) + offset;
        }

        public string ExcuseText
        {
            get { return _Excuse.ExcuseText; }
            set
            {
                try
                {
                    _Excuse.ExcuseText = value;
                    errors.Remove(ExcuseTextName);
                    OnPropertyChanged(ExcuseTextName);
                }
                catch(InvalidValueException e)
                {
                    errors.Add(ExcuseTextName, e.Message);
                }
            }
        }

        public string Result
        {
            get { return _Excuse.Result; }
            set
            {
                try
                {
                    _Excuse.Result = value;
                    errors.Remove(ResultName);
                    OnPropertyChanged(ResultName);
                }
                catch (InvalidValueException e)
                {
                    errors.Add(ResultName, e.Message);
                }
            }
        }

        public DateTime Used
        {
            get { return _Excuse.Used; }
            set
            {
                try
                {
                    _Excuse.Used = value;
                    errors.Remove(UsedName);
                    OnPropertyChanged(UsedName);
                }
                catch (InvalidValueException e)
                {
                    errors.Add(UsedName, e.Message);
                }
            }
        }

        public int Rating
        {
            get { return _Excuse.Rating; }
            set
            {
                try
                {
                    _Excuse.Rating = value;
                    errors.Remove(RatingName);
                    OnPropertyChanged(RatingName);
                    OnPropertyChanged(RatedName);
                }
                catch (InvalidValueException e)
                {
                    errors.Add(RatingName, e.Message);
                }
            }
        }

        public bool Rated {
            get { return _Excuse.Rated; }
            private set { throw new NotSupportedException(); }
        }

        public bool ReadOnly
        {
            get { return _Excuse.ReadOnly; }
            private set { throw new NotSupportedException(); }
        }

        public string Error
        {
            get { return null; }
        }

        public string this[string columnName]
        {
            get
            {
                if (errors.ContainsKey(columnName))
                    return errors[columnName];
                else
                    return null;
            }
        }

        public ExcuseViewModel()
        {
            errors = new Dictionary<string, string>();
        }

        public void SetExcuse(Excuse excuse)
        {
            if(excuse != null)
            {
                _Excuse = excuse;
                FireAllProperties();
            }
            else
            {
                throw new InvalidValueException("Excuse can not be null!");
            }
        }

        private void FireAllProperties()
        {
            OnPropertyChanged(ExcuseTextName);
            OnPropertyChanged(ResultName);
            OnPropertyChanged(RatingName);
            OnPropertyChanged(UsedName);
            OnPropertyChanged(RatingName);
            OnPropertyChanged(RatedName);
            OnPropertyChanged(ReadOnlyName);
        }

        protected internal void OnPropertyChanged(string propertyname)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }
    }
}
