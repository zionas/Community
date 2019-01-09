using System;
using System.Net.Sockets;
using Amazon;
using Amazon.DynamoDBv2;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using System.Reflection;
using Amazon.DynamoDBv2.DocumentModel;
using Newtonsoft.Json;
using Amazon.DynamoDBv2.DataModel;
using static System.Net.HttpStatusCode;
using CommunityNetWork.Dal.Interfaces;

namespace CommunityNetWork.Dal
{

    public class DynamoDBConnector : IDynamoDB
    {

        static bool connected = false;
        static AmazonDynamoDBClient _client = null;
        static ProvisionedThroughput provisionedThroughput;


        internal DynamoDBConnector(bool useDynamoDBLocal)
        {
            if (_client != null)
                return;
            try
            {
                _client = GetDynamoDBClient(useDynamoDBLocal);
            }
            catch (Exception e)
            {
                string target = useDynamoDBLocal ? " Local " : "";
                throw e;
            }
            connected = true;
            provisionedThroughput = new ProvisionedThroughput
            {
                ReadCapacityUnits = 1000,
                WriteCapacityUnits = 1000

            };
            Console.WriteLine("connected successfully");

        }


        static AmazonDynamoDBClient GetDynamoDBClient(bool useDynamoDBLocal)
        {
            AmazonDynamoDBClient client;

            if (useDynamoDBLocal)
                client = GetLocalClient();
            else
                client = GetWebClient();

            return client;
        }


        static string GetTableName(Type type)
        {
            return type.Name + "s";
        }
        static AmazonDynamoDBClient GetLocalClient()
        {
            Console.WriteLine("  -- Setting up a DynamoDB-Local client (DynamoDB Local seems to be running)");
            AmazonDynamoDBConfig ddbConfig = new AmazonDynamoDBConfig();
            ddbConfig.ServiceURL = "http://localhost:8000";

            return new AmazonDynamoDBClient(ddbConfig);
        }



        static AmazonDynamoDBClient GetWebClient()
        {
            //var credentials = new BasicAWSCredentials(accessKey, secretKey);
            return new AmazonDynamoDBClient();
        }




        static AttributeDefinition GetAttributeDefinition(PropertyInfo p)
        {
            Type type = p.PropertyType;
            return new AttributeDefinition
            {
                AttributeName = p.Name,
                AttributeType = type == typeof(string) ? "S" : type == typeof(bool) ? "B" : "N"
            };
        }


        static KeyType GetKeyType(PropertyInfo p)
        {
            return p.GetCustomAttribute(typeof(DynamoDBHashKeyAttribute), true) != null ?
                KeyType.HASH
                : p.GetCustomAttribute(typeof(DynamoDBRangeKeyAttribute), true) != null ?
                KeyType.RANGE : null;
        }


        static KeySchemaElement GetSchemaElement(PropertyInfo p)
        {
            KeyType keyType = GetKeyType(p);
            return keyType == null ?
                null
               : new KeySchemaElement
               {
                   AttributeName = p.Name,
                   KeyType = keyType

               };
        }


        CreateTableRequest GetCreateTableRequest(Type type)
        {
            List<AttributeDefinition> attrDets = new List<AttributeDefinition>();
            List<KeySchemaElement> keys = new List<KeySchemaElement>();
            foreach (PropertyInfo p in type.GetProperties())
            {

                KeySchemaElement keySchemaElement = GetSchemaElement(p);
                if (keySchemaElement != null)
                {
                    attrDets.Add(GetAttributeDefinition(p));
                    keys.Add(keySchemaElement);
                }
            }
            return new CreateTableRequest
            {
                TableName = GetTableName(type),
                AttributeDefinitions = attrDets,
                KeySchema = keys,
                ProvisionedThroughput = provisionedThroughput
            };
        }


        public async Task<bool> AddModel(Type type)
        {
            CreateTableRequest request = GetCreateTableRequest(type);
            bool result = default(bool);
            try
            {
                result = await CreateTable(request);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return true;
        }


        public bool RemoveModel(Type type)
        {
            string tableName = GetTableName(type);

            var request = new DeleteTableRequest { TableName = tableName };
            DeleteTableResponse response;
            try
            {
                response = _client.DeleteTable(request);
                if (response.HttpStatusCode == OK)
                    return true;

            }
            catch (ResourceNotFoundException)
            {

            }
            return false;


        }


        static DynamoDBEntry CastToDynamoDbEntryType(object value)
        {

            if (value is char)
                return (char)value;
            else if (value is string)
                return (string)value;
            else if (value is string[])
                return (string[])value;
            else if (value is List<string>)
                return (List<string>)value;
            else if (value is bool)
                return (bool)value;
            else if (value is byte)
                return (byte)value;
            else if (value is byte[])
                return (byte[])value;
            else if (value is DateTime)
                return (DateTime)value;
            else if (value is decimal)
                return (decimal)value;
            else if (value is short)
                return (short)value;
            else if (value is int)
                return (int)value;
            else if (value is long)
                return (long)value;
            else if (value is double)
                return (double)value;
            else if (value is Guid)
                return (Guid)value;
            else return default(DynamoDBEntry);

        }


        static void SetEntry(Document doc, PropertyInfo p, object model)
        {
            object value = p.GetValue(model);
            if (p.GetCustomAttribute(typeof(DynamoDBIgnoreAttribute), true) == null)

                doc[p.Name] = CastToDynamoDbEntryType(value);
        }


        public void Add<T>(T model)
        {
            Type type = typeof(T);
            IAmazonDynamoDB db = _client;
            Document doc = new Document();
            foreach (PropertyInfo p in type.GetProperties())
            {
                SetEntry(doc, p, model);
            }
            Table table = Table.LoadTable(_client, GetTableName(type));
            table.PutItem(doc);

        }


        static GetItemOperationConfig GetModelConfig(Type type, bool constintence)
        {
            var attributesToGet = new List<string>();
            foreach (PropertyInfo p in type.GetProperties())
            {
                if (p.GetCustomAttribute(typeof(DynamoDBIgnoreAttribute), true) == null)
                    attributesToGet.Add(p.Name);
            }
            return new GetItemOperationConfig
            {
                AttributesToGet = attributesToGet,
                ConsistentRead = constintence
            };
        }


        public T Get<T>(Primitive hashKey, bool constintence)
        {
            Type type = typeof(T);

            GetItemOperationConfig config = GetModelConfig(type, constintence);
            Table table = Table.LoadTable(_client, GetTableName(type));
            Document doc = table.GetItem(hashKey, config);
            return JsonConvert.DeserializeObject<T>(doc.ToJson());

        }


        async Task<bool> CreateTable(CreateTableRequest request)
        {
            if (!connected)
                return false;

            try
            {
                bool exists = await TableExists(request.TableName);
                if (exists)
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e + e.Message);
                throw new OperationCanceledException(e.Message + ":cant check if table exist");

            }
            CreateTableResponse response;
            try
            {
                Console.WriteLine("  -- Creat Table {0}...", request.TableName);
                response = await _client.CreateTableAsync(request);
                if (response.HttpStatusCode == OK)
                    Console.WriteLine("     -- Created the \"{0}\" table successfully!", request.TableName);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            if (response.TableDescription.TableStatus == TableStatus.ACTIVE)
                return true;
            return false;

        }



        async Task<bool> TableExists(string tableName)
        {
            DescribeTableResponse descResponse;
            try
            {
                ListTablesResponse tblResponse = await _client.ListTablesAsync();

                if (tblResponse.TableNames.Contains(tableName))
                {
                    Console.WriteLine("     A table named {0} already exists in DynamoDB!", tableName);
                    try
                    {
                        descResponse = await _client.DescribeTableAsync(tableName);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Table description is not available ({0})", ex.Message);
                        throw ex;
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return false;
        }

        public void Dispose()
        {
            _client.Dispose();
            _client = null;

        }


    }
}

