using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Zuehlke.Hades.Interfaces;
using Zuehlke.Hades.SqlServer.Matcher;

namespace Zuehlke.Hades.SqlServer.Manager
{
    /// <summary>
    /// Manages the policies for the access control in a SQL Server (T-SQL) database
    /// </summary>
    public class SqlServerManager : IAclManager
    {
        private readonly string _connectionString;
        private const string MarsConnectionStringParameter = "multipleactiveresultsets";

        /// <summary>
        /// Initializes a new instance of <see cref="SqlServerManager"/> with the given connection string
        /// </summary>
        /// <param name="connectionString">Connection string for the database connection</param>
        public SqlServerManager(string connectionString)
        {
            if (connectionString.ToLower().Contains($"{MarsConnectionStringParameter}=false;"))
                throw new ArgumentException("The database connection string must allow multiple active result sets.");
            _connectionString = connectionString;
        }

        /// <summary>
        /// Handles the matching of attributes
        /// </summary>
        public IMatcher Matcher { get; } = new SqlServerRegexMatcher();

        /// <summary>
        /// Adds a policy asynchronously with the information provided as a <see cref="Zuehlke.Hades.PolicyCreationRequest"/>.
        /// </summary>
        /// <param name="policyCreationRequest">Information to create the new <see cref="Zuehlke.Hades.Policy"/> with</param>
        /// <returns>The newly created <see cref="Zuehlke.Hades.Policy"/></returns>
        public async Task<Policy> AddPolicyAsync(PolicyCreationRequest policyCreationRequest)
        {
            using (var con = new SqlConnection(_connectionString))
            {
                await con.OpenAsync();
                using (var transaction = con.BeginTransaction())
                {
                    var guid = Guid.NewGuid().ToString();
                    var policy = new Policy(policyCreationRequest) { Id = guid };
                    await AddPolicyAsync(policy, con, transaction);
                    transaction.Commit();
                    return policy;
                }
            }
        }

        /// <summary>
        /// Deletes a policy by its id asynchronously
        /// </summary>
        /// <param name="id">The id of the policy</param>
        /// <returns>true on sucess / false if unsuccessful</returns>
        public async Task<bool> DeletePolicyAsync(string id)
        {
            using(var con = new SqlConnection(_connectionString))
            {
                await con.OpenAsync();
                using (var transaction = con.BeginTransaction())
                {
                    var result = await DeletePolicyAsync(id, con, transaction);
                    transaction.Commit();
                    return result;
                }
            }
        }

        /// <summary>
        /// Get all policies asynchronously
        /// </summary>
        /// <returns>A list of all policies</returns>
        public async Task<List<Policy>> GetAllPoliciesAsync()
        {
            using (var con = new SqlConnection(_connectionString))
            {
                await con.OpenAsync();
                var command = new SqlCommand(SqlServerQueries.GetAllPoliciesQuery, con);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    return await GetPoliciesFromRowsAsync(reader);
                }

            }
        }

        /// <summary>
        /// Get a specific policy by its id asynchronously
        /// </summary>
        /// <param name="id">The id of the policy</param>
        /// <returns>The <see cref="Zuehlke.Hades.Policy"/> with the given id</returns>
        /// <exception cref="KeyNotFoundException">If there is no policy with the given id</exception>
        public async Task<Policy> GetPolicyByIdAsync(string id)
        {
            using (var con = new SqlConnection(_connectionString))
            {
                await con.OpenAsync();

                var command = new SqlCommand(SqlServerQueries.GetPolicyByIdQuery, con);
                command.Parameters.Add(SqlServerQueries.PolicyIdParameter.Key,
                    SqlServerQueries.PolicyIdParameter.Value).Value = id;

                using (var reader = await command.ExecuteReaderAsync())
                {
                    var policies = await GetPoliciesFromRowsAsync(reader);
                    if(policies.Count > 0)
                    {
                        return policies.First();
                    }
                    throw new KeyNotFoundException();
                }
                    
            }
        }

        /// <summary>
        /// Get all policies that might be applicable for the given request asynchronously
        /// </summary>
        /// <param name="request">The access request</param>
        /// <returns>A list of policies that might be applicable</returns>
        public async Task<List<Policy>> GetRequestCandidatesAsync(AccessRequest request)
        {
            using (var con = new SqlConnection(_connectionString))
            {
                await con.OpenAsync();
                SqlCommand command;

                if (string.IsNullOrEmpty(request.Resource))
                {
                    command = new SqlCommand(SqlServerQueries.GetRequestCandidatesQuery, con);
                    command.Parameters.Add(SqlServerQueries.RequestSubjectParameter.Key,
                        SqlServerQueries.RequestSubjectParameter.Value).Value = request.Subject;
                }
                else
                {
                    command = new SqlCommand(SqlServerQueries.GetRequestCandidatesbyResourceAndSubjectQuery, con);
                    command.Parameters.Add(SqlServerQueries.RequestSubjectParameter.Key,
                        SqlServerQueries.RequestSubjectParameter.Value).Value = request.Subject;
                    command.Parameters.Add(SqlServerQueries.RequestResourceParameter.Key,
                        SqlServerQueries.RequestResourceParameter.Value).Value = request.Resource;
                }
                using (var reader = await command.ExecuteReaderAsync())
                {
                    return await GetPoliciesFromRowsAsync(reader);
                }
            }
        }

        /// <summary>
        /// Updates an existing policy (with the provided id) to the given policy information asynchronously
        /// </summary>
        /// <param name="policy">Updated policy</param>
        /// <returns>The updated <see cref="Zuehlke.Hades.Policy"/> (should be the same as the one that was passed into the method)</returns>
        /// <exception cref="KeyNotFoundException">If there is no policy with the id provided in the passed policy</exception>
        public async Task<Policy> UpdatePolicyAsync(Policy policy)
        {
            using (var con = new SqlConnection(_connectionString))
            {
                await con.OpenAsync();
                using (var transaction = con.BeginTransaction())
                {
                    if (await DeletePolicyAsync(policy.Id, con, transaction))
                    {
                        await AddPolicyAsync(policy, con, transaction);
                        transaction.Commit();
                        return policy;
                    }
                    transaction.Rollback();
                    throw new KeyNotFoundException();
                }
            }
        }
        
        /// <summary>
        /// Creates the database schema/tables that are nessecary asynchronously.
        /// </summary>
        /// <returns></returns>
        public async Task CreateDatabaseSchemaAsync()
        {
            using (var con = new SqlConnection(_connectionString))
            {
                await con.OpenAsync();
                using (var transaction = con.BeginTransaction())
                {
                    foreach (var query in SqlServerQueries.CreateTableQueries)
                    {
                        using (var command = new SqlCommand(query, con, transaction))
                        {
                            try
                            {
                                await command.ExecuteNonQueryAsync();
                            }
                            catch (SqlException)
                            {
                                transaction.Rollback();
                                throw;
                            }
                        }
                    }
                    transaction.Commit();
                }
                foreach (var query in SqlServerQueries.CreateIndexQueries)
                {
                    using (var command = new SqlCommand(query, con))
                    {
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
        }

        /// <summary>
        /// Converts a <see cref="Zuehlke.Hades.RequestEffect"/> to the corresponding bit represented trough an integer
        /// </summary>
        /// <param name="effect">The <see cref="Zuehlke.Hades.RequestEffect"/> enum to convert from</param>
        /// <returns>The bit - deny:0, allow:1</returns>
        private int EffectToBit(RequestEffect effect)
        {
            return effect == RequestEffect.Allow ? 1 : 0;
        }

        /// <summary>
        /// Converts the effect bit to the corresponding <see cref="Zuehlke.Hades.RequestEffect"/> enum 
        /// </summary>
        /// <param name="bit">Bit to convert from (0:deny, 1:allow)</param>
        /// <returns>The corresponding <see cref="Zuehlke.Hades.RequestEffect"/></returns>
        private RequestEffect BitToEffect(int bit)
        {
            return bit == 1 ? RequestEffect.Allow : RequestEffect.Deny;
        }

        /// <summary>
        /// Deletes a policy asynchronously
        /// </summary>
        /// <param name="id">The id of the policy that should be deleted</param>
        /// <param name="con">The sql connection that should be used</param>
        /// <param name="transaction">The sql transaction that should be used</param>
        /// <returns>true on success / false if unsuccessful</returns>
        private async Task<bool> DeletePolicyAsync(string id, SqlConnection con, SqlTransaction transaction)
        {
            var command = new SqlCommand(SqlServerQueries.DeletePolicyQuery, con, transaction);
            command.Parameters.Add(SqlServerQueries.PolicyIdParameter.Key,
                SqlServerQueries.PolicyIdParameter.Value).Value = id;
            try
            {
                return (await command.ExecuteNonQueryAsync() > 0);
            }
            catch (SqlException)
            {
                transaction.Rollback();
                throw;
            }
        }

        /// <summary>
        /// Adds a new policy to the SQL database 
        /// </summary>
        /// <param name="policy">The <see cref="Zuehlke.Hades.Policy"/> that should be added</param>
        /// <param name="con">The sql connection that should be used</param>
        /// <param name="transaction">The sql transaction that should be used</param>
        /// <returns></returns>
        private async Task AddPolicyAsync(Policy policy, SqlConnection con, SqlTransaction transaction)
        {
            using (var command = new SqlCommand(SqlServerQueries.AddPolicyQuery, con, transaction))
            {
                command.Parameters.Add(SqlServerQueries.PolicyIdParameter.Key,
                    SqlServerQueries.PolicyIdParameter.Value).Value = policy.Id;
                command.Parameters.Add(SqlServerQueries.PolicyDescriptionParameter.Key,
                    SqlServerQueries.PolicyDescriptionParameter.Value).Value = policy.Description;
                command.Parameters.Add(SqlServerQueries.PolicyEffectParameter.Key,
                    SqlServerQueries.PolicyEffectParameter.Value).Value = EffectToBit(policy.Effect);
                var conditionsJson = string.Empty;
                if(policy.Conditions != null && policy.Conditions.Count > 0)
                {
                    conditionsJson = JsonConvert.SerializeObject(policy.Conditions, Formatting.Indented, new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Objects,
                        TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
                    });
                }
                command.Parameters.Add(SqlServerQueries.PolicyConditionsParameter.Key,
                    SqlServerQueries.PolicyConditionsParameter.Value).Value = conditionsJson;
                try
                {
                    await command.ExecuteNonQueryAsync();
                }
                catch (SqlException)
                {
                    transaction.Rollback();
                    throw;
                }
                var relations = new List<KeyValuePair<string, List<string>>>
                {
                    new KeyValuePair<string, List<string>>("action", policy.Actions),
                    new KeyValuePair<string, List<string>>("resource", policy.Resources),
                    new KeyValuePair<string, List<string>>("subject", policy.Subjects)
                };
                foreach (var rel in relations)
                {
                    foreach (var template in rel.Value)
                    {
                        var id = Sha256Hash(template);
                        using (var attrCommand = new SqlCommand(string.Format(SqlServerQueries.AddPolicyInsert, rel.Key), con, transaction))
                        {
                            attrCommand.Parameters.Add(SqlServerQueries.AttributeIdParameter.Key,
                                SqlServerQueries.AttributeIdParameter.Value).Value = id;
                            attrCommand.Parameters.Add(SqlServerQueries.AttributeTemplateParameter.Key,
                                SqlServerQueries.AttributeTemplateParameter.Value).Value = template;
                            attrCommand.Parameters.Add(SqlServerQueries.AttributeCompiledParameter.Key,
                                SqlServerQueries.AttributeCompiledParameter.Value).Value = template;
                            attrCommand.Parameters.Add(SqlServerQueries.AttributeHasRegexParameter.Key,
                                SqlServerQueries.AttributeHasRegexParameter.Value).Value = ((SqlServerRegexMatcher) Matcher).IsRegexLikePattern(template);

                            try
                            {
                                await attrCommand.ExecuteNonQueryAsync();
                            }
                            catch (SqlException)
                            {
                                transaction.Rollback();
                                throw;
                            }
                            using (var relCommand = new SqlCommand(string.Format(SqlServerQueries.AddPolicyInsertRelation, rel.Key), con, transaction))
                            {
                                relCommand.Parameters.Add(SqlServerQueries.PolicyIdParameter.Key,
                                    SqlServerQueries.PolicyIdParameter.Value).Value = policy.Id;
                                relCommand.Parameters.Add(SqlServerQueries.AttributeIdParameter.Key,
                                    SqlServerQueries.AttributeIdParameter.Value).Value = id;
                                try
                                {
                                    await relCommand.ExecuteNonQueryAsync();
                                }
                                catch (SqlException)
                                {
                                    transaction.Rollback();
                                    throw;
                                }
                            }
                        }
                    }
                }
            }

        }

        /// <summary>
        /// Reads the policies from the provided <see cref="System.Data.SqlClient.SqlDataReader"/> into a <see cref="List{Policy}"/>
        /// </summary>
        /// <param name="reader">The <see cref="System.Data.SqlClient.SqlDataReader"/> to read from</param>
        /// <returns>List of policies</returns>
        private async Task<List<Policy>> GetPoliciesFromRowsAsync(SqlDataReader reader)
        {
            var policies = new Dictionary<string, Policy>();
            //read all rows of the joined result view
            while (await reader.ReadAsync())
            {
                var id = reader["id"].ToString();
                var action = reader["action"].ToString();
                var resource = reader["resource"].ToString();
                var subject = reader["subject"].ToString();
                //if the policy is already in our list, we just need to append the property lists
                if (policies.ContainsKey(id))
                {
                    policies[id].Actions.Add(action);
                    policies[id].Subjects.Add(subject);
                    policies[id].Resources.Add(resource);
                }
                else {
                    policies.Add(id, new Policy
                    {
                        Id = id,
                        Description = reader["description"].ToString(),
                        Effect = BitToEffect(Convert.ToInt32(reader["effect"])),
                        Conditions = ConditionsFromJson(reader["conditions"].ToString()),
                        Actions = new List<string> { action },
                        Subjects = new List<string> { subject },
                        Resources = new List<string> { resource }
                    });
                }
            }
            foreach(var policy in policies.Values)
            {
                policy.Actions = policy.Actions.Distinct().ToList();
                policy.Subjects = policy.Subjects.Distinct().ToList();
                policy.Resources = policy.Resources.Distinct().ToList();
            }
            return policies.Select(p => p.Value).ToList();
        }

        private List<ICondition> ConditionsFromJson(string conditionJson)
        {
            if(!string.IsNullOrWhiteSpace(conditionJson))
            {
                return JsonConvert.DeserializeObject<List<ICondition>>(conditionJson, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Objects
                });
            }
            return null;
        }
        private static string Sha256Hash(string value)
        {
            using (var hash = SHA256.Create())
            {
                return string.Concat(hash
                  .ComputeHash(Encoding.UTF8.GetBytes(value))
                  .Select(item => item.ToString("x2")));
            }
        }
    }
}
