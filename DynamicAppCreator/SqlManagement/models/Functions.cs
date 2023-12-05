namespace DynamicAppCreator.SqlManagement.models
{
    public class CreateFunctionInput
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public IEnumerable<SqlParameterModel> Parameters { get; set; }
        public string Script { get; set; }
    }

    public class SqlParameterModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public object DefaultValue { get; set; }
        public System.Data.SqlDbType DataType { get; set; }
    }
}
