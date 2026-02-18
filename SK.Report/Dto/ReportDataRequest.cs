using Newtonsoft.Json;

namespace SK.Report.Dto
{
    public class ReportDataRequest
    {
        [JsonProperty("sktoken")]
        public string SkToken { get; }
        public string User { get; }
        public string Password { get; }
        public string ObjectID { get; }
        public string ObjectType { get; }

        public ReportDataRequest(string sktoken, string user, string password, string objectID, string objectType)
        {
            SkToken = sktoken;
            User = user;
            Password = password;
            ObjectID = objectID;
            ObjectType = objectType;
        }

        public override bool Equals(object? obj)
        {
            return obj is ReportDataRequest other &&
                   SkToken == other.SkToken &&
                   User == other.User &&
                   Password == other.Password &&
                   ObjectID == other.ObjectID &&
                   ObjectType == other.ObjectType;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(SkToken, User, Password, ObjectID, ObjectType);
        }
    }
}
