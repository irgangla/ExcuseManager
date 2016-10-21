using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ExcuseManager
{
    [XmlRoot("Excuses")]
    public class ExcuseList
    {
        [XmlIgnore]
        public string Filepath { get; private set; }
        [XmlArray("ExcuseList")]
        [XmlArrayItem("Excuse")]
        public List<Excuse> Excuses { get; private set; }

        public ExcuseList()
        {
            Excuses = new List<Excuse>();
        }

        public ExcuseList(string file) : this()
        {
            Filepath = file;
        }

        public void Save()
        {
            if(Filepath == null)
            {
                return;
            }

            XmlSerializer serializer = new XmlSerializer(typeof(ExcuseList));
            FileStream file = new FileStream(Filepath, File.Exists(Filepath)?FileMode.Truncate:FileMode.Create);
            serializer.Serialize(file, this);
        }

        public static ExcuseList Load(string filepath)
        {
            if (filepath == null)
            {
                return null;
            }

            XmlSerializer serializer = new XmlSerializer(typeof(ExcuseList));
            FileStream file = new FileStream(filepath, FileMode.OpenOrCreate);
            ExcuseList list = null;
            try
            {
                list = (ExcuseList)serializer.Deserialize(file);
                list.Filepath = filepath;
            }
            catch(Exception)
            {
                //Null return as not successful signal.
            }

            return list;
        }

        public static ExcuseList Create(string filepath)
        {
            if (filepath == null)
            {
                return null;
            }
            
            ExcuseList list = null;

            try
            {
                list = new ExcuseList(filepath);
                list.Save();
            }
            catch (Exception)
            {
                //Null return as not successful signal.
                list = null;
            }

            return list;
        }
    }

    public class Excuse
    {
        public static readonly Excuse DefaultExcuse = new Excuse(true);

        private string _ExcuseText;
        [XmlElement("Text")]
        public string ExcuseText
        {
            get { return _ExcuseText; }
            set
            {
                if(!ReadOnly)
                {
                    this._ExcuseText = value;
                }
                else
                {
                    throw new InvalidValueException("Excuse is readonly!");
                }
                    
            }
        }
        
        private string _Result;
        [XmlElement("Note")]
        public string Result
        {
            get { return _Result; }
            set
            {
                if(!ReadOnly)
                {
                    this._Result = value;
                }
                else
                {
                    throw new InvalidValueException("Excuse is readonly!");
                }

            }
        }

        [XmlElement("Used")]
        private DateTime _Used;
        public DateTime Used
        {
            get { return _Used; }
            set
            {
                if (!ReadOnly)
                {
                    if (value > DateTime.Now)
                    {
                        throw new InvalidValueException("Used date can not be in future!");
                    }
                    else
                    {
                        this._Used = value;
                    }
                }
                else
                {
                    throw new InvalidValueException("Excuse is readonly!");
                }
            }
        }

        private int _Rating;
        [XmlElement("Rating")]
        public int Rating
        {
            get { return _Rating; }
            set {
                if (!ReadOnly)
                {
                    if (value >= 0 && value <= 5)
                    {
                        this._Rating = value;
                        Rated = true;
                    }
                    else
                    {
                        throw new InvalidValueException("Rating value is out of range!");
                    }
                }
                else
                {
                    throw new InvalidValueException("Excuse is readonly!");
                }
            }
        }

        [XmlAttribute("Rated")]
        public bool Rated;
        [XmlIgnore]
        public bool ReadOnly { get; private set; }
        
        public Excuse()
        {
            Used = DateTime.Now;
            Rating = 0;
            Rated = false;
            ReadOnly = false;
        }

        public Excuse(bool readOnly) : this()
        {
            ExcuseText = "[No Excuse Selected]";
            Result = "[No Excuse Selected]";
            Rating = 1;
            ReadOnly = readOnly;
            Rated = false;
        }
    }
}
