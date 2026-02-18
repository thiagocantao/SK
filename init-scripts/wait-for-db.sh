#!/bin/bash

# Variáveis passadas por variáveis de ambiente
DB_HOST="${DB_HOST}"
DB_PORT="${DB_PORT}"

# Definir um arquivo de log para armazenar as mensagens
LOG_FILE="/usr/local/bin/wait-for-db.log"

# Função para registrar logs
log() {
  echo "$(date +'%Y-%m-%d %H:%M:%S') - $1" | tee -a $LOG_FILE
}

# Função para esperar o banco de dados
wait_for_db() {
  log "Verificando disponibilidade do banco de dados em $1:$2..."
  until nc -z -v -w30 $1 $2
  do
    log "Banco de dados em $1:$2 não disponível ainda. Esperando..."
    sleep 5
  done
  log "Banco de dados em $1:$2 está disponível!"
}

# Registrar o início da execução do script
log "Iniciando script de espera do banco de dados (wait-for-db.sh)..."

# Chama a função para os bancos de teste e produção com base no ambiente
if [ "$ASPNETCORE_ENVIRONMENT" == "Development" ]; then
  log "Ambiente de desenvolvimento detectado. Aguardando banco de dados de teste."
  wait_for_db $DB_HOST $DB_PORT
elif [ "$ASPNETCORE_ENVIRONMENT" == "Production" ]; then
  log "Ambiente de produção detectado. Aguardando banco de dados de produção."
  wait_for_db $DB_HOST $DB_PORT
else
  log "Variável ASPNETCORE_ENVIRONMENT não definida ou ambiente desconhecido. Saindo..."
  exit 1
fi

# Executa o script de inicialização do banco de dados, se presente
if [ -f "/usr/local/bin/init_db.sh" ]; then
  log "Executando script de inicialização init_db.sh..."
  /bin/bash /usr/local/bin/init_db.sh | tee -a $LOG_FILE
else
  log "Script init_db.sh não encontrado. Pulando etapa de inicialização do banco."
fi

# Registrar a conclusão do script e iniciar a aplicação
log "Iniciando a aplicação .NET Core..."
dotnet SK.Report.dll | tee -a $LOG_FILE
