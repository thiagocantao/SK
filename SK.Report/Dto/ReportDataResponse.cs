namespace SK.Report.Dto
{
    public class ReportDataResponse
    {
        private const int StatusOkCode = 200;

        public string Status { get; set; } = null!;

        public ResponseData Response { get; set; } = null!;

        public bool StatusOk { get => Response.StatusID == StatusOkCode; }

        public class ResponseData
        {
            public string UserID { get; set; } = null!;
            public string WorkspaceID { get; set; } = null!;
            public int StatusID { get; set; }
            public string Language { get; set; } = null!;
            public string StatusTitle { get; set; } = null!;
            public string ObjectID { get; set; } = null!;
            public string ObjectType { get; set; } = null!;
            public string token { get; set; } = null!;
            public string user_id { get; set; } = null!;
            public int expires { get; set; }
        }

    }
}
