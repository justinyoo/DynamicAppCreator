using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using System.Reflection;
using DAC.core.models;
using DynamicAppCreator.Data;

namespace DynamicAppCreator.SqlManagement.DataProcessing
{
    public class ContextBuilder
    {
        private readonly ApplicationDbContext dbContext;

        public ContextBuilder(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public class DetailsModel
        {
            public SqlTables sqlTables { get; set; }
            public SqlDatabases sqlDatabases { get; set; }
            public SqlServers sqlServers { get; set; }

            public string AssemblyName()
            {
                return "RuntimeModels" + sqlServers.Id + "d" + sqlDatabases.Id;
            }
            public string TableName()
            {

                return $"{sqlTables.Name}_V{sqlTables.Version}";
            }

        }
        private DetailsModel getDetails(long TableID)
        {
            var query = from table in dbContext.Set<SqlTables>()
                        join database in dbContext.Set<SqlDatabases>() on table.Database equals database.Id
                        join server in dbContext.Set<SqlServers>()
                            on database.ServerID equals server.Id
                        where table.Id == TableID
                        select new DetailsModel { sqlTables = table, sqlDatabases = database, sqlServers = server };

            return query.FirstOrDefault();
        }

        private Dictionary<string, EntityManagerItem> _dataSources { get; set; } = new Dictionary<string, EntityManagerItem>();
        public EntityManagerItem GetAssembly(DetailsModel request)
        {
            if (!_dataSources.ContainsKey(request.AssemblyName()))
            {
                AssemblyName assemblyName = new AssemblyName()
                {
                    Name = request.AssemblyName()
                };

                var nab = AppDomain.CreateDomain("");

                EntityManagerItem emi = new EntityManagerItem();

                emi.Assembly = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndCollect);
                emi.ModuleBuilder = emi.Assembly.DefineDynamicModule(assemblyName.Name);
                _dataSources.Add(request.AssemblyName(), emi);


                emi.DbContextBuilder = emi.ModuleBuilder.DefineType("DynamicDbContext", TypeAttributes.Public |
                TypeAttributes.Class |
                TypeAttributes.AutoClass |
                TypeAttributes.AnsiClass |
                TypeAttributes.BeforeFieldInit |
                TypeAttributes.AutoLayout, typeof(DbContext));
                emi.DbContext = BuildDynamicDbContextType(emi, request);
                return emi;
            }
            else
            {
                return _dataSources[request.AssemblyName()];
            }
        }

        private Type BuildDynamicDbContextType(EntityManagerItem emi, DetailsModel request)
        {
            var baseConstructor = typeof(DbContext).GetConstructor(BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Instance, null, new Type[] { typeof(DbContextOptions) }, null);

            //Set up the default constructor to call base
            var defaultConstructor = emi.DbContextBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new Type[0]);
            var defaultConstructorIl = defaultConstructor.GetILGenerator();
            defaultConstructorIl.Emit(OpCodes.Ldarg_0); // push "this";
            defaultConstructorIl.Emit(OpCodes.Call, baseConstructor);
            //The compiler always adds two nops
            defaultConstructorIl.Emit(OpCodes.Nop);
            defaultConstructorIl.Emit(OpCodes.Nop);
            defaultConstructorIl.Emit(OpCodes.Ret);

            //Set up the construct that passes the DB Context Options - change this section here if you're calling other overloads to DB context
            var dbContextConstructor = emi.DbContextBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new Type[] { typeof(DbContextOptions) });

            var dbContextConstructorIl = dbContextConstructor.GetILGenerator();

            dbContextConstructorIl.Emit(OpCodes.Ldarg_0); // push "this";
            dbContextConstructorIl.Emit(OpCodes.Ldarg_1); // push the 1st parameter
            dbContextConstructorIl.Emit(OpCodes.Call, baseConstructor);
            //The compiler always adds two nops
            dbContextConstructorIl.Emit(OpCodes.Nop);
            dbContextConstructorIl.Emit(OpCodes.Nop);
            dbContextConstructorIl.Emit(OpCodes.Ret);

            emi.GeneratedTables.ForEach(t =>
            {
                addTableToContext(emi, t, request);
            });

            return emi.DbContextBuilder.CreateType();
        }

        public Type CreateModelClass(DetailsModel request)
        {

            if (_dataSources[request.AssemblyName()].ModuleBuilder.Assembly.DefinedTypes.Any(x => x.Name == request.TableName()))
            {
                return _dataSources[request.AssemblyName()].ModuleBuilder.Assembly.GetTypes().FirstOrDefault(x => x.Name == request.TableName());
            }
            TypeBuilder classBuilder = _dataSources[request.AssemblyName()].ModuleBuilder.DefineType(request.TableName(), TypeAttributes.Public |
                TypeAttributes.Class |
                TypeAttributes.AutoClass |
                TypeAttributes.AnsiClass |
                TypeAttributes.BeforeFieldInit |
                TypeAttributes.AutoLayout);

            var tableNameAttr = typeof(System.ComponentModel.DataAnnotations.Schema.TableAttribute).GetConstructor(new Type[] { typeof(string) });
            var tableNameBuilder = new CustomAttributeBuilder(tableNameAttr, new object[] { request.sqlTables.Name });
            classBuilder.SetCustomAttribute(tableNameBuilder);
            request.sqlTables.Columns().ToList().ForEach(col =>
            {

                FieldBuilder columnFieldBuilder = classBuilder.DefineField($"_{col.Name}", DataType.GetSystemType(col.DataType), FieldAttributes.Private);
                PropertyBuilder columnPropBuilder = classBuilder.DefineProperty(col.Name, PropertyAttributes.HasDefault, DataType.GetSystemType(col.DataType), null);
                MethodAttributes getSetAttr = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;
                MethodBuilder columnPropGetMethod = classBuilder.DefineMethod($"get_{col.Name}", getSetAttr, DataType.GetSystemType(col.DataType), Type.EmptyTypes);

                ILGenerator columnGetIL = columnPropGetMethod.GetILGenerator();
                columnGetIL.Emit(OpCodes.Ldarg_0);
                columnGetIL.Emit(OpCodes.Ldfld, columnFieldBuilder);
                columnGetIL.Emit(OpCodes.Ret);


                MethodBuilder columnPropSetMethod = classBuilder.DefineMethod($"set_{col.Name}", getSetAttr, null, new Type[] { DataType.GetSystemType(col.DataType) });
                ILGenerator columnSetIL = columnPropSetMethod.GetILGenerator();
                columnSetIL.Emit(OpCodes.Ldarg_0);
                columnSetIL.Emit(OpCodes.Ldarg_1);
                columnSetIL.Emit(OpCodes.Stfld, columnFieldBuilder);
                columnSetIL.Emit(OpCodes.Ret);


                columnPropBuilder.SetGetMethod(columnPropGetMethod);
                columnPropBuilder.SetSetMethod(columnPropSetMethod);

                //if (col.Properties.isPrimary)
                //{
                //    var keyAttr = typeof(KeyAttribute).GetConstructor(new Type[] { });
                //    var keyBuilder = new CustomAttributeBuilder(keyAttr, new object[] { });
                //    columnPropBuilder.SetCustomAttribute(keyBuilder);
                //}

                //if (col != null && col.Size != null && col.Size > -1)
                //{
                //    var maxLengthAttr = typeof(MaxLengthAttribute).GetConstructor(new Type[] { typeof(int) });
                //    var maxLengthBuilder = new CustomAttributeBuilder(maxLengthAttr, new object[] { col.Size });
                //    columnPropBuilder.SetCustomAttribute(maxLengthBuilder);
                //}

                //foreach (var prop in col.Validations)
                //{
                //    if (prop.ValidationRule != null && prop.ValidationRule != "")
                //    {
                //        var keyAttr = typeof(RegularExpressionAttribute).GetConstructor(new Type[] { typeof(string), typeof(string) });
                //        var keyBuilder = new CustomAttributeBuilder(keyAttr, new object[] { prop.ValidationRule, prop.ErrorMessage });
                //        columnPropBuilder.SetCustomAttribute(keyBuilder);
                //    }
                //}


            });

            return classBuilder.CreateType();
        }

        private void addTableToContext(EntityManagerItem emi, Type modelType, DetailsModel request)
        {

            if (emi.DbContext.GetProperty(request.TableName()) == null)
            {
                var shortName = request.TableName();

                //Create the generic DbSet
                Type dbSetType = typeof(DbSet<>);


                //Backing Field
                FieldBuilder fieldBuilder = emi.DbContextBuilder.DefineField(shortName, dbSetType.MakeGenericType(modelType), FieldAttributes.Private);
                //Property Builder
                PropertyBuilder propertyBuilder = emi.DbContextBuilder.DefineProperty(shortName, PropertyAttributes.HasDefault, dbSetType.MakeGenericType(modelType), null);
                //Getter
                MethodAttributes getSetAttr = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;

                MethodBuilder getPropBuilder = emi.DbContextBuilder.DefineMethod($"get_{shortName}", getSetAttr, typeof(string), Type.EmptyTypes);

                ILGenerator getIL = getPropBuilder.GetILGenerator();

                getIL.Emit(OpCodes.Ldarg_0);
                getIL.Emit(OpCodes.Ldfld, fieldBuilder);
                getIL.Emit(OpCodes.Ret);

                //Setter
                MethodBuilder setPropBuilder =
                    emi.DbContextBuilder.DefineMethod($"set_{shortName}",
                        getSetAttr,
                        null,
                        new Type[] { typeof(string) });

                ILGenerator setIL = setPropBuilder.GetILGenerator();

                setIL.Emit(OpCodes.Ldarg_0);
                setIL.Emit(OpCodes.Ldarg_1);
                setIL.Emit(OpCodes.Stfld, fieldBuilder);
                setIL.Emit(OpCodes.Ret);

                //Assign them
                propertyBuilder.SetGetMethod(getPropBuilder);
                propertyBuilder.SetSetMethod(setPropBuilder);
                emi.DbContext = BuildDynamicDbContextType(emi, request);
            }

        }

        public object GetDbSet(long TableID)
        {

            var details = getDetails(TableID);
            var asm = GetAssembly(details);
            var dbSet = CreateModelClass(details);

            //addTableToContext(asm, dbSet, details);
            return dbSet.Assembly.CreateInstance(details.TableName());
            //GetDbContext(details.sqlServers.Id, details.sqlDatabases.Id);
            //details.sqlTables.Columns();
        }
    }
}
