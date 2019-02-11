# Geração de Boletos Bancários do Bradesco usando .Net Framework 4.5
WebApi que faz integração com Servidores do Bradesco para gerar boleto bancário para a carteira 26
<br><h2>CONFIGURAÇÕES:</h2>
<ul>
	<li>
		Rodar o script <b>scripts/createDb-createTable.sql</b> na sua base de dados.
	</li>
	<li>
		No arquivo <b>Boleto.WebApi/Web.config</b> alterar a connectionString <b>Entities</b> para apontar para sua base de dados.
	</li>
	<li>
		No arquivo <b>Boleto.WebApi/Web.config</b> alterar as chaves <b>MerchantId</b> e <b>ChaveSeguranca</b> com os valores fornecidos pelo Bradesco tanto para Homol quanto para Produção.
	</li>
	<li>
		No arquivo <b>Boleto.WebApi/Web.config</b> alterar a chave <b>LogFolder</b> para apontar para um diretório que você possui permissão para escrita no servidor.
		<br><b>Obs:</b> Se você está utilizando o Web App do Azure, futuramente irei fazer uma implementação salvando o Log no Blob.
	</li>
</ul>
<br><h2>RODANDO O SISTEMA:</h2>
Para rodar o sistema basta definir o projeto <b>Boleto.WebApi</b> como <i><b>Set as StartUp Project</b></i>
<br><h2>HOMOLOGAÇÃO</h2>
<img src="https://raw.githubusercontent.com/sfspacov/BoletoBradescoDotNetFramework/master/docs/homologacao.PNG" />
<br><h2>DOCUMENTAÇÃO OFICIAL DO BRADESCO:</h2>
https://github.com/sfspacov/BoletoBradescoDotNetFramework/blob/master/docs/Manual_BoletoBancario.pdf
