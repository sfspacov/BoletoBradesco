# Geração de Boletos Bancários do Bradesco usando .Net Framework 4.5.2
WebApi que faz integração com Servidores do Bradesco para gerar boleto bancário para a carteira 26
<br><h2>CONFIGURAÇÕES:</h2>
<ul>
	<li>
		Rodar o script <b>createDb-createTable-insertFakeDate.sql</b> na sua base de dados.
	</li>
	<li>
		Nos arquivos <b>Boleto.WebApi/Web.config</b> e <b>Boleto.Tests/App.config</b> alterar:<br>
		- ConnectionString <b>Entities</b> para apontar para sua base de dados.<br>
		- As chaves <b>MerchantId</b> e <b>ChaveSeguranca</b> com os valores fornecidos pelo Bradesco tanto para Homol quanto para Produção.<br>
		- A chave <b>LogFolder</b> para apontar para um diretório que você possui permissão para escrita no servidor.<br>
		<br><b>Obs:</b> Se você está utilizando o Web App do Azure, futuramente irei fazer uma implementação salvando o Log no Blob.
	</li>
</ul>
<br><h2>RODANDO O SISTEMA:</h2>
Para rodar o sistema basta definir o projeto <b>Boleto.WebApi</b> como <i><b>Set as StartUp Project</b></i><br>
<br><h2>DOCUMENTAÇÃO OFICIAL DO BRADESCO:</h2>
https://github.com/sfspacov/BoletoBradescoDotNetFramework/blob/master/docs/Manual_BoletoBancario.pdf
<br><h2>ENTENDENDO O SISTEMA:</h2>
Após entender claramente a documentação oficial, abra o arquivo <b>\Boleto.Tests\Integrated.cs</b> e coloque um breakpoint em cada um dos métodos (É um teste integrado, ou seja, eles vão consultar a base de dados).<br>Depois debugue-os pressionando o botão F11, para entrar na implementação dos métodos.<br>
A imagem abaixo mostra o arquivo <b>\Boleto.Tests\Integrated.cs</b> com os breakpoints:
<img src="https://raw.githubusercontent.com/sfspacov/BoletoBradescoDotNetFramework/master/docs/testes_integrados.PNG" />
<br><h2>DÚVIDAS E HOMOLOGAÇÃO (print da documentação oficial do Bradesco)</h2>
<img src="https://raw.githubusercontent.com/sfspacov/BoletoBradescoDotNetFramework/master/docs/homologacao.PNG" />
