using MongoDB.Bson;

namespace TwitterWebApi
{
    public class Handle
    {
        public ObjectId _id { get; set; }

        public string name { get; set; }

        public string localId { get; set; }
    }
}