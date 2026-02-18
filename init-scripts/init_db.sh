#!/bin/bash

# Vari�veis passadas por vari�veis de ambiente
DB_NAME="${POSTGRES_DB}"
DB_APP_USER="${DB_APP_USER}"
DB_APP_USER_PASSWORD="${DB_APP_USER_PASSWORD}"
POSTGRES_USER="${POSTGRES_USER}"
DB_ORIGINAL_HOST="${DB_ORIGINAL_HOST}"
DB_PORT="${DB_PORT}"
DB_HOST="${DB_HOST}"
DB_SCHEMAS="${DB_SCHEMAS}"
SK_REPORT_ADMIN_USER="${SK_REPORT_ADMIN_USER}"
SK_REPORT_ADMIN_PASSWORD="${SK_REPORT_ADMIN_PASSWORD}"

# Espera o banco de dados estar pronto
/usr/local/bin/wait-for-it.sh $DB_HOST:$DB_PORT --timeout=60 --strict -- echo "PostgreSQL est� pronto, iniciando configura��o."

# Definir a senha como vari�vel de ambiente
export PGPASSWORD=$DB_APP_USER_PASSWORD

# Verifica se o usu�rio j� existe
echo "Verificando se o usu�rio $DB_APP_USER j� existe no banco de dados..."
USER_EXISTS=$(psql -U $POSTGRES_USER -h $DB_HOST -p $DB_PORT -tAc "SELECT 1 FROM pg_roles WHERE rolname='${DB_APP_USER}'")

# Se o usu�rio n�o existir, cria ele com permiss�es de SUPERUSER
if [ "$USER_EXISTS" != "1" ]; then
    echo "Verificando e instalando extensões necessárias..."
    psql -U $POSTGRES_USER -h $DB_HOST -p $DB_PORT -c "CREATE EXTENSION IF NOT EXISTS pg_cron WITH SCHEMA pg_catalog;"

    psql -U $POSTGRES_USER -h $DB_HOST -p $DB_PORT -d $DB_NAME <<EOF
-- Script SQL para cria��o e configura��o de roles

-- Criar a role azure_pg_admin (n�o pode fazer login e pertence ao grupo pg_monitor)
DO \$\$ BEGIN
    CREATE ROLE azure_pg_admin NOLOGIN;
EXCEPTION
    WHEN duplicate_object THEN RAISE NOTICE 'Role azure_pg_admin already exists, skipping creation.';
END \$\$;
GRANT pg_monitor TO azure_pg_admin;

-- Criar a role azuresu (superuser com permiss�es adicionais)
DO \$\$ BEGIN
    CREATE ROLE azuresu SUPERUSER CREATEROLE CREATEDB REPLICATION BYPASSRLS;
EXCEPTION
    WHEN duplicate_object THEN RAISE NOTICE 'Role azuresu already exists, skipping creation.';
END \$\$;

-- Criar a role replication (replica��o)
DO \$\$ BEGIN
    CREATE ROLE replication REPLICATION;
EXCEPTION
    WHEN duplicate_object THEN RAISE NOTICE 'Role replication already exists, skipping creation.';
END \$\$;

-- Criar a role $SK_REPORT_ADMIN_USER com permiss�es espec�ficas e como membro de azure_pg_admin
DO \$\$ BEGIN
    CREATE ROLE "$SK_REPORT_ADMIN_USER" CREATEROLE CREATEDB;
EXCEPTION
    WHEN duplicate_object THEN RAISE NOTICE 'Role $SK_REPORT_ADMIN_USER already exists, skipping creation.';
END \$\$;
GRANT pg_read_all_settings, pg_read_all_stats, pg_stat_scan_tables, azure_pg_admin TO "$SK_REPORT_ADMIN_USER";
GRANT CONNECT ON DATABASE "$DB_NAME" TO "$SK_REPORT_ADMIN_USER";

-- Criar a role $DB_APP_USER com permiss�es de leitura e escrita
DO \$\$ BEGIN
    CREATE USER "$DB_APP_USER";
EXCEPTION
    WHEN duplicate_object THEN RAISE NOTICE 'Role "$DB_APP_USER" already exists, skipping creation.';
END \$\$;
GRANT pg_read_all_data, pg_write_all_data TO "$DB_APP_USER";
GRANT ALL PRIVILEGES ON DATABASE "$DB_NAME" TO "$DB_APP_USER";

EOF
    # Atualizando senhas
    psql -U $POSTGRES_USER -h $DB_HOST -p $DB_PORT -c "ALTER ROLE \"$SK_REPORT_ADMIN_USER\"  WITH LOGIN PASSWORD '$SK_REPORT_ADMIN_PASSWORD';"
    psql -U $POSTGRES_USER -h $DB_HOST -p $DB_PORT -c "ALTER ROLE \"$DB_APP_USER\" PASSWORD '$DB_APP_USER_PASSWORD';"

    IFS=$'\n'
    for SCHEMA_ENTRY in $DB_SCHEMAS; do
        USER_NAME=$(echo "$SCHEMA_ENTRY" | cut -d':' -f1)
        KEY_WORD=$(echo "$SCHEMA_ENTRY" | cut -d':' -f2)    

        echo "Criando usu�rio $USER_NAME"
        psql -U $POSTGRES_USER -h $DB_HOST -p $DB_PORT -c "CREATE USER \"$USER_NAME\" LOGIN PASSWORD '$KEY_WORD';"
    done

    # Executar o pg_dump com a senha j� definida na vari�vel
    echo "Gerando dump do banco de dados $DB_NAME do host original $DB_ORIGINAL_HOST..."
    pg_dump -h $DB_ORIGINAL_HOST -U $DB_APP_USER -F c -b -v \
      --no-security-labels \
      --exclude-schema='cron' \
      --exclude-schema='azure' \
      --exclude-schema='pgaadauth' \
      -f db_dump_file.dump $DB_NAME

    # Restaura o banco de dados no host atual
    echo "Restaurando o banco de dados $DB_NAME no host $DB_HOST..."
    pg_restore -h $DB_HOST -U $POSTGRES_USER -p $DB_PORT -d $DB_NAME -v db_dump_file.dump

    # Criar roles espec�ficas para idiomas com privil�gios de acesso aos Schemas espec�ficos
    echo "Criar roles espec�ficas para idiomas com privil�gios de acesso aos Schemas espec�ficos"
    for SCHEMA_ENTRY in $DB_SCHEMAS; do
        USER_NAME=$(echo "$SCHEMA_ENTRY" | cut -d':' -f1)
        SCHEMA_NAME=${USER_NAME:4}  # Remover o prefixo 'usr_' para obter o nome do schema

        echo "Concedendo acesso de leitura ao usu�rio $USER_NAME no SCHEMA $SCHEMA_NAME"
        psql -U $POSTGRES_USER -h $DB_HOST -p $DB_PORT -c "GRANT CONNECT ON DATABASE \"$DB_NAME\" TO \"$USER_NAME\";"
        psql -U $POSTGRES_USER -h $DB_HOST -p $DB_PORT -c "GRANT USAGE ON SCHEMA $SCHEMA_NAME TO \"$USER_NAME\";"
        psql -U $POSTGRES_USER -h $DB_HOST -p $DB_PORT -c "GRANT SELECT ON ALL TABLES IN SCHEMA $SCHEMA_NAME TO \"$USER_NAME\";"
        psql -U $POSTGRES_USER -h $DB_HOST -p $DB_PORT -c "ALTER DEFAULT PRIVILEGES IN SCHEMA $SCHEMA_NAME GRANT SELECT ON TABLES TO \"$USER_NAME\";"
        
        psql -U $POSTGRES_USER -h $DB_HOST -p $DB_PORT -c "GRANT USAGE ON SCHEMA $SCHEMA_NAME TO \"$DB_APP_USER\";"
        psql -U $POSTGRES_USER -h $DB_HOST -p $DB_PORT -c "GRANT SELECT ON ALL TABLES IN SCHEMA $SCHEMA_NAME TO \"$DB_APP_USER\";"
        psql -U $POSTGRES_USER -h $DB_HOST -p $DB_PORT -c "ALTER DEFAULT PRIVILEGES IN SCHEMA $SCHEMA_NAME GRANT SELECT ON TABLES TO \"$DB_APP_USER\";"
    done

    echo "Dump e restaura��o conclu�dos com sucesso."
fi

# Limpar a vari�vel de ambiente ap�s o uso
unset PGPASSWORD