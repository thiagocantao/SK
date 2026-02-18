#!/bin/bash

PG_CONF_DIR="/var/lib/postgresql/data"
# Definir um arquivo de log para armazenar as mensagens
LOG_FILE="$PG_CONF_DIR/init-db-config.log"

# Fun��o para registrar logs
log() {
    echo "$(date +'%Y-%m-%d %H:%M:%S') - $1" | tee -a $LOG_FILE
}

log "Verificando disponibilidade do banco de dados em localhost:5432 ..."
# Verificar se o banco de dados já está inicializado
if [ -f "$PG_CONF_DIR/PG_VERSION" ]; then
    log "Banco de dados já inicializado, aplicando configurações."
    # Adicionar a linha ao pg_hba.conf se ainda não existir
    if ! grep -qxF 'host all all 0.0.0.0/0 md5' $PG_CONF_DIR/pg_hba.conf; then
        echo 'host all all 0.0.0.0/0 md5' >>$PG_CONF_DIR/pg_hba.conf
        log "'pg_hba.conf' configurado com sucesso."
    fi
    # Adicionar a linha ao postgresql.conf se ainda não existir
    if ! grep -qxF 'host all all 0.0.0.0/0 md5' $PG_CONF_DIR/postgresql.conf; then
        echo "shared_preload_libraries = 'pg_cron'" >>$PG_CONF_DIR/postgresql.conf
        echo "cron.database_name = 'postgres'" >>$PG_CONF_DIR/postgresql.conf
        log "'postgresql.conf' configurado com sucesso."
    fi
else
    log "Banco de dados não inicializado. Saindo do script de configuração."
fi
