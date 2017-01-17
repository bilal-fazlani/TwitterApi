using MongoDB.Bson;

namespace TwitterWebApi.Models
{
    public class Handle
    {
        public ObjectId _id { get; set; }

        public string name { get; set; }

        public string localId { get; set; }
    }
}