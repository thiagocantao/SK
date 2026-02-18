# Projeto SKReport

Este repositório contém a aplicação **SKReport**, desenvolvida utilizando **ASP.NET Core 6** e integrada com um banco de dados **PostgreSQL**. O pipeline de deploy está configurado utilizando **Github Actions**, com a aplicação e o banco de dados sendo executados em **containers Docker** em uma VPS Linux.

## Tecnologias Utilizadas

### 1. ASP.NET Core 6
- **Descrição**: A aplicação principal foi desenvolvida utilizando o framework **ASP.NET Core 6**. Esse framework oferece suporte para construção de aplicações web modernas, seguras e escaláveis.
- **Uso**: A aplicação **SKReport** roda em containers Docker e usa o Kestrel como servidor web, com configuração para reverse proxy utilizando **Nginx**.

### 2. PostgreSQL
- **Descrição**: O banco de dados utilizado é o **PostgreSQL**, versão 14.13, que é um sistema de gerenciamento de banco de dados relacional open-source robusto e eficiente.
- **Uso**: O banco de dados está configurado para rodar em um container Docker e se comunica com a aplicação **SKReport** via string de conexão dinâmica, com autenticação sensível armazenada como variáveis de ambiente.

### 3. Docker
- **Descrição**: A aplicação e suas dependências são executadas dentro de containers **Docker**.
- **Uso**:
  - Um **Dockerfile** define a imagem da aplicação **SKReport**.
  - O **docker-compose.yml** gerencia os serviços da aplicação e do banco de dados, além de configurar volumes e networks.
  - Os containers são atualizados durante o deploy via **Github Actions**.

### 4. Github Actions
- **Descrição**: **Github Actions** é usado para automatizar o pipeline de integração e deployment contínuo (CI/CD) do projeto.
- **Uso**: 
  - O arquivo `deploy.yml` define os gatilhos para o pipeline. 
  - O deploy é disparado automaticamente quando:
    - Um commit é enviado para as branches `main` ou `develop`.
    - Uma nova **tag** é criada na branch `main`.
    - Um **pull request** é concluído na branch `main`.
    - Uma nova **release** é publicada no repositório.
  - As variáveis sensíveis, como as credenciais do banco de dados, são armazenadas como **Github Secrets** e passadas como variáveis de ambiente durante o deploy.

### 5. Nginx
- **Descrição**: O **Nginx** é utilizado como **reverse proxy** para redirecionar as requisições HTTP para a aplicação ASP.NET Core, que está rodando no **Kestrel**.
- **Uso**: Configurado para servir a aplicação da porta 80 da VPS, repassando as requisições para o container da aplicação que expõe a porta 5000.

### 7. Docker Hub
- **Descrição**: As imagens da aplicação **SKReport** são publicadas no **Docker Hub** para facilitar o versionamento e deployment dos containers.
- **Uso**: O processo de build e push das imagens é automatizado através do pipeline no **Github Actions**.

## Estrutura do Projeto

- **ASP.NET Core**: Código principal da aplicação web.
- **Dockerfile**: Define a construção da imagem da aplicação.
- **docker-compose.yml**: Gerencia os serviços e containers, incluindo o banco de dados PostgreSQL.
- **deploy.yml**: Pipeline de deploy automatizado utilizando Github Actions.
- **appsettings.json**: Arquivo de configuração contendo a string de conexão com o banco de dados (substituída dinamicamente via pipeline).

## Como Executar o Projeto

### 1. Pré-requisitos
- **Docker** e **docker-compose** instalados.
- Acesso a um servidor com Nginx configurado para reverse proxy.
- Configuração de variáveis de ambiente para acessar o banco de dados.

### 2. Subindo a Aplicação
```bash
docker-compose up -d
