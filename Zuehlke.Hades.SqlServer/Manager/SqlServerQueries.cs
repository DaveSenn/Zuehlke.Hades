using System.Collections.Generic;
using System.Data;

namespace Zuehlke.Hades.SqlServer.Manager
{
    public static class SqlServerQueries
    {
        #region CreateTables
        public static readonly List<string> CreateTableQueries = new List<string>
        {
            @"IF object_id('hades_policy', 'U') is null
                CREATE TABLE hades_policy (
	                id varchar(255) NOT NULL PRIMARY KEY,
	                description text NOT NULL,
	                effect bit NOT NULL,
	                conditions text NOT NULL
                )",
            @"IF object_id('hades_policy_subject', 'U') is null
                CREATE TABLE hades_policy_subject (
	                compiled text NOT NULL,
	                template varchar(1023) NOT NULL,
	                policy   varchar(255) NOT NULL,
	                FOREIGN KEY (policy) REFERENCES hades_policy(id) ON DELETE CASCADE
                )",
            @"IF object_id('hades_policy_permission', 'U') is null
                CREATE TABLE hades_policy_permission (
	                compiled text NOT NULL,
	                template varchar(1023) NOT NULL,
	                policy   varchar(255) NOT NULL,
	                FOREIGN KEY (policy) REFERENCES hades_policy(id) ON DELETE CASCADE
                )",
            @"IF object_id('hades_policy_resource', 'U') is null
                CREATE TABLE hades_policy_resource (
	                compiled text NOT NULL,
	                template varchar(1023) NOT NULL,
	                policy   varchar(255) NOT NULL,
	                FOREIGN KEY (policy) REFERENCES hades_policy(id) ON DELETE CASCADE
                )",
            @"IF object_id('hades_subject', 'U') is null
                CREATE TABLE hades_subject (
	                id          varchar(64) NOT NULL PRIMARY KEY,
	                has_regex   bit NOT NULL,
	                compiled 	varchar(511) NOT NULL UNIQUE,
	                template 	varchar(511) NOT NULL UNIQUE
                )",
            @"IF object_id('hades_action', 'U') is null
                CREATE TABLE hades_action (
	                id       varchar(64) NOT NULL PRIMARY KEY,
	                has_regex   bit NOT NULL,
	                compiled varchar(511) NOT NULL UNIQUE,
	                template varchar(511) NOT NULL UNIQUE
                )",
            @"IF object_id('hades_resource', 'U') is null
                CREATE TABLE hades_resource (
	                id       varchar(64) NOT NULL PRIMARY KEY,
	                has_regex   bit NOT NULL,
	                compiled varchar(511) NOT NULL UNIQUE,
	                template varchar(511) NOT NULL UNIQUE
                )",
            @"IF object_id('hades_policy_subject_rel', 'U') is null
                CREATE TABLE hades_policy_subject_rel (
	                policy varchar(255) NOT NULL,
	                subject varchar(64) NOT NULL,
	                PRIMARY KEY (policy, subject),
	                FOREIGN KEY (policy) REFERENCES hades_policy(id) ON DELETE CASCADE,
	                FOREIGN KEY (subject) REFERENCES hades_subject(id) ON DELETE CASCADE
                )",
            @"IF object_id('hades_policy_action_rel', 'U') is null
                CREATE TABLE hades_policy_action_rel (
	                policy varchar(255) NOT NULL,
	                action varchar(64) NOT NULL,
	                PRIMARY KEY (policy, action),
	                FOREIGN KEY (policy) REFERENCES hades_policy(id) ON DELETE CASCADE,
	                FOREIGN KEY (action) REFERENCES hades_action(id) ON DELETE CASCADE
                )",
            @"IF object_id('hades_policy_resource_rel', 'U') is null
                CREATE TABLE hades_policy_resource_rel (
	                policy varchar(255) NOT NULL,
	                resource varchar(64) NOT NULL,
	                PRIMARY KEY (policy, resource),
	                FOREIGN KEY (policy) REFERENCES hades_policy(id) ON DELETE CASCADE,
	                FOREIGN KEY (resource) REFERENCES hades_resource(id) ON DELETE CASCADE
                )"
        };
        public static readonly List<string> CreateIndexQueries = new List<string>
        {
            @"IF NOT EXISTS (SELECT 1 FROM sys.fulltext_catalogs WHERE [name] = 'hades_catalog')
                CREATE FULLTEXT CATALOG hades_catalog AS DEFAULT;",
            @"IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'hades_subject_compiled_idx' AND object_id = OBJECT_ID('hades_subject'))
                CREATE UNIQUE INDEX hades_subject_compiled_idx ON hades_subject(id);",
            @"IF NOT EXISTS(SELECT * FROM sys.fulltext_indexes WHERE object_id = OBJECT_ID('hades_subject'))
                CREATE FULLTEXT INDEX ON hades_subject(compiled)
                KEY INDEX hades_subject_compiled_idx ON hades_catalog;",
            @"IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'hades_action_compiled_idx' AND object_id = OBJECT_ID('hades_action'))
                CREATE UNIQUE INDEX hades_action_compiled_idx ON hades_action(id);",
            @"IF NOT EXISTS(SELECT * FROM sys.fulltext_indexes WHERE object_id = OBJECT_ID('hades_action'))
                CREATE FULLTEXT INDEX ON hades_action(compiled)
                KEY INDEX hades_action_compiled_idx ON hades_catalog;",
            @"IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'hades_resource_compiled_idx' AND object_id = OBJECT_ID('hades_resource'))
                CREATE UNIQUE INDEX hades_resource_compiled_idx ON hades_resource(id);",
            @"IF NOT EXISTS(SELECT * FROM sys.fulltext_indexes WHERE object_id = OBJECT_ID('hades_resource'))
                CREATE FULLTEXT INDEX ON hades_resource(compiled)
                KEY INDEX hades_resource_compiled_idx ON hades_catalog;"
        };
        #endregion
        #region Add
        public static string AddPolicyQuery =
            @"INSERT INTO hades_policy (id, description, effect, conditions)
                SELECT @pid, @pdescription, @peffect, @pconditions WHERE NOT EXISTS (
                    SELECT 1 FROM hades_policy WHERE id = @pid)";
        public static string AddPolicyInsert =
            @"INSERT INTO hades_{0} (id, template, compiled, has_regex)
                SELECT @attrid, @template, @compiled, @hasregex
                WHERE NOT EXISTS (SELECT 1 FROM hades_{0} WHERE id = @attrid)";
        public static string AddPolicyInsertRelation =
            @"INSERT INTO hades_policy_{0}_rel (policy, {0})
                SELECT @pid, @attrid WHERE NOT EXISTS (
                    SELECT 1 FROM hades_policy_{0}_rel
                    WHERE policy = @pid AND {0} = @attrid)";

        #endregion
        #region Delete
        public static string DeletePolicyQuery = "DELETE FROM hades_policy WHERE id = @pid";
        #endregion
        #region Get
        public static string GetAllPoliciesQuery =
            @"SELECT
	            p.id, p.effect, p.conditions, p.description,
	            subject.template as subject, resource.template as resource, action.template as action
            FROM
	            hades_policy as p
            LEFT JOIN hades_policy_subject_rel as rs ON rs.policy = p.id
            LEFT JOIN hades_policy_action_rel as ra ON ra.policy = p.id
            LEFT JOIN hades_policy_resource_rel as rr ON rr.policy = p.id
            LEFT JOIN hades_subject as subject ON rs.subject = subject.id
            LEFT JOIN hades_action as action ON ra.action = action.id
            LEFT JOIN hades_resource as resource ON rr.resource = resource.id";
        public static string GetPolicyByIdQuery = GetAllPoliciesQuery + " WHERE p.Id = @pid";
        public static string GetRequestCandidatesQuery = GetAllPoliciesQuery +
            @" WHERE CONTAINS(subject.compiled, @subject) AND (
                ( subject.has_regex != 1 AND subject.template = @subject )
                OR
                ( subject.has_regex = 1 AND @subject LIKE subject.compiled ))";
        public static string GetRequestCandidatesbyResourceAndSubjectQuery = GetAllPoliciesQuery +
            @" WHERE CONTAINS(subject.compiled, @subject) AND (
                ( subject.has_regex != 1 AND subject.template = @subject )
                OR
                ( subject.has_regex = 1 AND @subject LIKE subject.compiled ))
               AND resource.template = @resource";
        #endregion

        #region Parameters
        public static readonly KeyValuePair<string, SqlDbType> PolicyIdParameter =
            new KeyValuePair<string, SqlDbType>("@pid", SqlDbType.VarChar);
        public static readonly KeyValuePair<string, SqlDbType> PolicyDescriptionParameter =
            new KeyValuePair<string, SqlDbType>("@pdescription", SqlDbType.Text);
        public static readonly KeyValuePair<string, SqlDbType> PolicyEffectParameter =
            new KeyValuePair<string, SqlDbType>("@peffect", SqlDbType.Bit);
        public static readonly KeyValuePair<string, SqlDbType> PolicyConditionsParameter =
            new KeyValuePair<string, SqlDbType>("@pconditions", SqlDbType.Text);
        public static readonly KeyValuePair<string, SqlDbType> AttributeIdParameter =
            new KeyValuePair<string, SqlDbType>("@attrid", SqlDbType.VarChar);
        public static readonly KeyValuePair<string, SqlDbType> AttributeTemplateParameter =
            new KeyValuePair<string, SqlDbType>("@template", SqlDbType.VarChar);
        public static readonly KeyValuePair<string, SqlDbType> AttributeCompiledParameter =
            new KeyValuePair<string, SqlDbType>("@compiled", SqlDbType.VarChar);
        public static readonly KeyValuePair<string, SqlDbType> AttributeHasRegexParameter =
            new KeyValuePair<string, SqlDbType>("@hasregex", SqlDbType.Bit);
        public static readonly KeyValuePair<string, SqlDbType> RequestSubjectParameter =
            new KeyValuePair<string, SqlDbType>("@subject", SqlDbType.VarChar);
        public static readonly KeyValuePair<string, SqlDbType> RequestResourceParameter =
            new KeyValuePair<string, SqlDbType>("@resource", SqlDbType.VarChar);
        #endregion
    }
}
