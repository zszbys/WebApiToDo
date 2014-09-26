using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace ToDoApi.Models
{
    [BsonIgnoreExtraElements]
    public class ToDo
    {
        //http://stackoverflow.com/a/7982411
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("task")]
        [BsonRequired]
        [Required]
        public string Task { get; set; }

        [BsonElement("comment")]
        public string Comment { get; set; }

        [BsonElement("done")]
        public bool Done { get; set; }

        // Set datesaved to now if not supplied
        private DateTime? _dateSaved;
        [BsonElement("dateSaved")]
        public DateTime? DateSaved
        {
            get { return _dateSaved; }
            set
            {
                if (value == null || value == DateTime.MinValue)
                {
                    _dateSaved = DateTime.Now;
                }
                else
                {
                    _dateSaved = value;
                }
            }
        }
    }
}