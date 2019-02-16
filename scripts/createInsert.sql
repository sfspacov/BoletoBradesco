USE [master]
GO

/****** Object:  Database [dbBoleto]    Script Date: 08/02/2019 22:51:58 ******/
CREATE DATABASE [dbBoleto]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'dbBoleto', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL11.MSSQLSERVER\MSSQL\DATA\dbBoleto.mdf' , SIZE = 5120KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'dbBoleto_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL11.MSSQLSERVER\MSSQL\DATA\dbBoleto_log.ldf' , SIZE = 2048KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO

ALTER DATABASE [dbBoleto] SET COMPATIBILITY_LEVEL = 110
GO

IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [dbBoleto].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO

ALTER DATABASE [dbBoleto] SET ANSI_NULL_DEFAULT OFF 
GO

ALTER DATABASE [dbBoleto] SET ANSI_NULLS OFF 
GO

ALTER DATABASE [dbBoleto] SET ANSI_PADDING OFF 
GO

ALTER DATABASE [dbBoleto] SET ANSI_WARNINGS OFF 
GO

ALTER DATABASE [dbBoleto] SET ARITHABORT OFF 
GO

ALTER DATABASE [dbBoleto] SET AUTO_CLOSE OFF 
GO

ALTER DATABASE [dbBoleto] SET AUTO_CREATE_STATISTICS ON 
GO

ALTER DATABASE [dbBoleto] SET AUTO_SHRINK OFF 
GO

ALTER DATABASE [dbBoleto] SET AUTO_UPDATE_STATISTICS ON 
GO

ALTER DATABASE [dbBoleto] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO

ALTER DATABASE [dbBoleto] SET CURSOR_DEFAULT  GLOBAL 
GO

ALTER DATABASE [dbBoleto] SET CONCAT_NULL_YIELDS_NULL OFF 
GO

ALTER DATABASE [dbBoleto] SET NUMERIC_ROUNDABORT OFF 
GO

ALTER DATABASE [dbBoleto] SET QUOTED_IDENTIFIER OFF 
GO

ALTER DATABASE [dbBoleto] SET RECURSIVE_TRIGGERS OFF 
GO

ALTER DATABASE [dbBoleto] SET  DISABLE_BROKER 
GO

ALTER DATABASE [dbBoleto] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO

ALTER DATABASE [dbBoleto] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO

ALTER DATABASE [dbBoleto] SET TRUSTWORTHY OFF 
GO

ALTER DATABASE [dbBoleto] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO

ALTER DATABASE [dbBoleto] SET PARAMETERIZATION SIMPLE 
GO

ALTER DATABASE [dbBoleto] SET READ_COMMITTED_SNAPSHOT OFF 
GO

ALTER DATABASE [dbBoleto] SET HONOR_BROKER_PRIORITY OFF 
GO

ALTER DATABASE [dbBoleto] SET RECOVERY FULL 
GO

ALTER DATABASE [dbBoleto] SET  MULTI_USER 
GO

ALTER DATABASE [dbBoleto] SET PAGE_VERIFY CHECKSUM  
GO

ALTER DATABASE [dbBoleto] SET DB_CHAINING OFF 
GO

ALTER DATABASE [dbBoleto] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO

ALTER DATABASE [dbBoleto] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO

ALTER DATABASE [dbBoleto] SET  READ_WRITE 
GO


USE [dbBoleto]
GO

/****** Object:  Table [dbo].[BradescoIntegration]    Script Date: 08/02/2019 22:54:08 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[BradescoIntegrations](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OrderId] [nvarchar](50) NOT NULL,
	[Token] [nvarchar](max) NOT NULL,
	[IsRequest] [bit] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[JsonContent] [nvarchar](max) NOT NULL,
	[UrlBoleto] [nvarchar](max) NULL,
 CONSTRAINT [PK_BradescoIntegrations] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO


INSERT INTO [dbo].[BradescoIntegrations]
VALUES
	(74976085,
	'65af91aa-631b-48f9-a552-29ef33e837ef',
	1,
	'2018-07-05 00:00:00.000',
	'{"boleto":{"beneficiario":"ONG Oncoamigo","carteira":"26","data_emissao":"2019-02-15","data_vencimento":"2019-02-18","instrucoes":{"instrucao_linha_1":"Não receber valor diferente do impresso em Valor documento.","instrucao_linha_10":null,"instrucao_linha_11":null,"instrucao_linha_12":null,"instrucao_linha_2":"­Caro Usuário:","instrucao_linha_3":"Boleto sujeito às normas vigentes de compensação bancária.","instrucao_linha_4":"Doação","instrucao_linha_5":null,"instrucao_linha_6":null,"instrucao_linha_7":null,"instrucao_linha_8":null,"instrucao_linha_9":null},"mensagem_cabecalho":null,"nosso_numero":"74976085000","registro":{"agencia_pagador":null,"aplicar_multa":false,"conta_pagador":null,"controle_participante":"Segurança arquivo remessa","data_limite_concessao_desconto":null,"debito_automatico":false,"endereco_debito_automatico":"2","especie_titulo":"99","primeira_instrucao":"00","rateio_credito":false,"razao_conta_pagador":null,"segunda_instrucao":"00","sequencia_registro":"00001","tipo_inscricao_pagador":"01","tipo_ocorrencia":"01","valor_abatimento":0,"valor_desconto":0,"valor_desconto_bonificacao":0,"valor_iof":0,"valor_juros_mora":0,"valor_percentual_multa":0},"tipo_renderizacao":"2","url_logotipo":null,"valor_titulo":"5000"},"comprador":{"documento":"84941890264","endereco":{"bairro":"Jardim Florestal","cep":"69101627","cidade":"Itacoatiara","complemento":null,"logradouro":"Rua Angelim","numero":"2899","uf":"AM"},"ip":"192.168.41.143","nome":"João da Silva","user_agent":"Google Chrome"},"meio_pagamento":"300","merchant_id":"100006608","pedido":{"descricao":"Doação","numero":"74976085","valor":"5000"},"token_request_confirmacao_pagamento":"65af91aa-631b-48f9-a552-29ef33e837ef"}'
	,null),

	(74976085,
	'65af91aa-631b-48f9-a552-29ef33e837ef',
	0,
	'2018-07-05 00:00:00.000',
	'{"boleto":{"data_atualizacao":"2019-02-15T19:31:54","data_geracao":"2019-02-15T19:17:39","linha_digitavel":"23790001246749760850400123456709178040000005000","linha_digitavel_formatada":"23790.00124  67497.608504  00123.456709  1  78040000005000","token":"QXREQ2tDM2VpMmNyeWpTaVkrYnhLOG51eDZJbGxhZmxzbXEvMWN4OXF6TGJZMnZFNW81eERBPT0.","url_acesso":"https://homolog.meiosdepagamentobradesco.com.br/apiboleto/Bradesco?token=QXREQ2tDM2VpMmNyeWpTaVkrYnhLOG51eDZJbGxhZmxzbXEvMWN4OXF6TGJZMnZFNW81eERBPT0.","valor_titulo":"5000"},"meio_pagamento":"300","merchant_id":"100006608","pedido":{"descricao":"Doação","numero":"74976085","valor":"5000"},"status":{"codigo":"0","detalhes":null,"mensagem":"OPERACAO REALIZADA COM SUCESSO"}}',
	'https://homolog.meiosdepagamentobradesco.com.br/apiboleto/Bradesco?token=QXREQ2tDM2VpMmNyeWpTaVkrYnhLOG51eDZJbGxhZmxzbXEvMWN4OXF6TGJZMnZFNW81eERBPT0.')