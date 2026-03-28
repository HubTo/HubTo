using FluentMigrator;

namespace HubTo.Infrastructure.Migrations;


[Migration(1)]
public class Init : Migration
{
    public override void Up()
    {
        IfDatabase("SQLite").Execute.Sql("PRAGMA foreign_keys = ON;");

        CreateUsers();
        CreateNamespaces();
        CreateUserNamespaces();
        CreateApiKeys();
        CreateArtifacts();
        CreateArtifactVersions();
        CreatePlugin();
        CreatePluginSettings();
    }

    private void CreateUsers()
    {
        Create.Table("USERS")
            .WithColumn("ID").AsGuid().PrimaryKey()
            .WithColumn("CREATED_BY").AsGuid().NotNullable()
            .WithColumn("CREATED_AT").AsDateTime().NotNullable()
            .WithColumn("UPDATED_BY").AsGuid().NotNullable()
            .WithColumn("UPDATED_AT").AsDateTime().NotNullable()
            .WithColumn("IS_DELETED").AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn("USERNAME").AsString(255).NotNullable().Unique("UQ_USERS_USERNAME")
            .WithColumn("EMAIL").AsString(320).NotNullable().Unique("UQ_USERS_EMAIL")
            .WithColumn("PASSWORD_HASH").AsString(255).NotNullable()
            .WithColumn("IS_ACTIVE").AsBoolean().NotNullable();
    }

    private void CreateNamespaces()
    {
        Create.Table("NAMESPACES")
            .WithColumn("ID").AsGuid().PrimaryKey()
            .WithColumn("CREATED_BY").AsGuid().NotNullable()
            .WithColumn("CREATED_AT").AsDateTime().NotNullable()
            .WithColumn("UPDATED_BY").AsGuid().NotNullable()
            .WithColumn("UPDATED_AT").AsDateTime().NotNullable()
            .WithColumn("IS_DELETED").AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn("NAME").AsString(128).NotNullable().Unique("UQ_NAMESPACE_NAME")
            .WithColumn("SLUG").AsString(128).NotNullable().Unique("UQ_NAMESPACE_SLUG")
            .WithColumn("DESCRIPTION").AsString(1000).Nullable();
    }

    private void CreateUserNamespaces()
    {
        Create.Table("USER_NAMESPACES")
            .WithColumn("ID").AsGuid().PrimaryKey()
            .WithColumn("CREATED_BY").AsGuid().NotNullable()
            .WithColumn("CREATED_AT").AsDateTime().NotNullable()
            .WithColumn("UPDATED_BY").AsGuid().NotNullable()
            .WithColumn("UPDATED_AT").AsDateTime().NotNullable()
            .WithColumn("IS_DELETED").AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn("USER_ID").AsGuid().NotNullable()
                .ForeignKey("FK_USER_NAMESPACE_USER", "USERS", "ID")
            .WithColumn("NAMESPACE_ID").AsGuid().NotNullable()
                .ForeignKey("FK_USER_NAMESPACE_NAMESPACE", "NAMESPACES", "ID")
            .WithColumn("NAMESPACE_ROLE").AsInt32().NotNullable()
            .WithColumn("JOINED_AT").AsDateTime().NotNullable();

        Create.UniqueConstraint("UQ_USER_NAMESPACE")
            .OnTable("USER_NAMESPACES")
            .Columns("USER_ID", "NAMESPACE_ID");
    }

    private void CreateApiKeys()
    {
        Create.Table("API_KEYS")
            .WithColumn("ID").AsGuid().PrimaryKey()
            .WithColumn("CREATED_BY").AsGuid().NotNullable()
            .WithColumn("CREATED_AT").AsDateTime().NotNullable()
            .WithColumn("UPDATED_BY").AsGuid().NotNullable()
            .WithColumn("UPDATED_AT").AsDateTime().NotNullable()
            .WithColumn("IS_DELETED").AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn("LABEL").AsString(255).NotNullable()
            .WithColumn("KEY_HASH").AsString(512).NotNullable()
            .WithColumn("PREFIX").AsString(50).NotNullable()
            .WithColumn("PERMISSION").AsInt32().NotNullable()
            .WithColumn("EXPIRES_AT").AsDateTime().Nullable()
            .WithColumn("LAST_USED_AT").AsDateTime().Nullable()
            .WithColumn("IS_REVOKED").AsBoolean().NotNullable()
            .WithColumn("USER_ID").AsGuid().NotNullable()
                .ForeignKey("FK_APIKEY_USER", "USERS", "ID")
            .WithColumn("NAMESPACE_ID").AsGuid().NotNullable()
                .ForeignKey("FK_APIKEY_NAMESPACE", "NAMESPACES", "ID");

        Create.Index("IX_APIKEY_PREFIX").OnTable("API_KEYS").OnColumn("PREFIX");
    }

    private void CreateArtifacts()
    {
        Create.Table("ARTIFACTS")
            .WithColumn("ID").AsGuid().PrimaryKey()
            .WithColumn("CREATED_BY").AsGuid().NotNullable()
            .WithColumn("CREATED_AT").AsDateTime().NotNullable()
            .WithColumn("UPDATED_BY").AsGuid().NotNullable()
            .WithColumn("UPDATED_AT").AsDateTime().NotNullable()
            .WithColumn("IS_DELETED").AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn("NAME").AsString(255).NotNullable()
            .WithColumn("REGISTRAR_TYPE").AsString(255).NotNullable()
            .WithColumn("NAMESPACE_ID").AsGuid().NotNullable()
                .ForeignKey("FK_ARTIFACT_NAMESPACE", "NAMESPACES", "ID");

        Create.UniqueConstraint("UQ_ARTIFACT")
            .OnTable("ARTIFACTS")
            .Columns("NAME", "NAMESPACE_ID", "REGISTRAR_TYPE");
    }

    private void CreateArtifactVersions()
    {
        Create.Table("ARTIFACT_VERSIONS")
            .WithColumn("ID").AsGuid().PrimaryKey()
            .WithColumn("CREATED_BY").AsGuid().NotNullable()
            .WithColumn("CREATED_AT").AsDateTime().NotNullable()
            .WithColumn("UPDATED_BY").AsGuid().NotNullable()
            .WithColumn("UPDATED_AT").AsDateTime().NotNullable()
            .WithColumn("IS_DELETED").AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn("VERSION").AsString(100).NotNullable()
            .WithColumn("STORAGE_PATH").AsString(1000).NotNullable()
            .WithColumn("DIGEST").AsString(255).Nullable()
            .WithColumn("MEDIA_TYPE").AsString(255).Nullable()
            .WithColumn("SIZE_IN_BYTES").AsInt64().Nullable()
            .WithColumn("IS_LISTED").AsBoolean().NotNullable()
            .WithColumn("PUBLISHED_AT").AsDateTime().NotNullable()
            .WithColumn("ARTIFACT_ID").AsGuid().NotNullable()
                .ForeignKey("FK_ARTIFACT_VERSION_ARTIFACT", "ARTIFACTS", "ID");

        Create.UniqueConstraint("UQ_ARTIFACT_VERSION")
            .OnTable("ARTIFACT_VERSIONS")
            .Columns("ARTIFACT_ID", "VERSION");

        Create.Index("IX_ARTIFACT_VERSION_ARTIFACT")
            .OnTable("ARTIFACT_VERSIONS")
            .OnColumn("ARTIFACT_ID");
    }

    private void CreatePlugin()
    {
        Create.Table("PLUGINS")
            .WithColumn("ID").AsGuid().PrimaryKey()
            .WithColumn("CREATED_BY").AsGuid().NotNullable()
            .WithColumn("CREATED_AT").AsDateTime().NotNullable()
            .WithColumn("UPDATED_BY").AsGuid().NotNullable()
            .WithColumn("UPDATED_AT").AsDateTime().NotNullable()
            .WithColumn("IS_DELETED").AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn("NAME").AsString(255).NotNullable()
            .WithColumn("ASSEMBLY_NAME").AsString(255).NotNullable()
            .WithColumn("PLUGIN_TYPE_VALUE").AsInt32()
            .WithColumn("IS_ENABLED").AsBoolean();

    }

    private void CreatePluginSettings()
    {
        Create.Table("PLUGIN_SETTINGS")
            .WithColumn("ID").AsGuid().PrimaryKey()
            .WithColumn("CREATED_BY").AsGuid().NotNullable()
            .WithColumn("CREATED_AT").AsDateTime().NotNullable()
            .WithColumn("UPDATED_BY").AsGuid().NotNullable()
            .WithColumn("UPDATED_AT").AsDateTime().NotNullable()
            .WithColumn("IS_DELETED").AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn("KEY").AsString(255).NotNullable()
            .WithColumn("VALUE").AsString(512).NotNullable()
            .WithColumn("PLUGIN_ID").AsGuid().NotNullable()
                .ForeignKey("FK_PLUGIN_SETTINGS_PLUGIN", "PLUGINS", "ID");
    }

    public override void Down()
    {
        Delete.Table("PLUGIN_SETTINGS");
        Delete.Table("PLUGINS");
        Delete.Table("ARTIFACT_VERSIONS");
        Delete.Table("ARTIFACTS");
        Delete.Table("API_KEYS");
        Delete.Table("USER_NAMESPACES");
        Delete.Table("NAMESPACES");
        Delete.Table("USERS");
    }
}
