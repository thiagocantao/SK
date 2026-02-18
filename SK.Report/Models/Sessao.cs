namespace SK.Report.Models
{
    public class Sessao
    {
        public Sessao(string ip, string workspaceID, string editMode, string userID, string objectID, string objectType, string language)
        {
            Ip = ip;
            WorkspaceID = workspaceID;
            EditMode = editMode;
            UserID = userID;
            ObjectID = objectID;
            ObjectType = objectType;
            Language = language;
        }

        public string Ip { get; set; }
        public string WorkspaceID { get; set; }
        public string EditMode { get; set; }
        public string UserID { get; set; }
        public string ObjectID { get; set; }
        public string ObjectType { get; set; }
        public string Language { get; set; }
    }
}
